
using UnityEngine;
using System.Collections.Generic;

public class BezierCurve : ScriptableObject
{
#if UNITY_EDITOR
  [UnityEditor.MenuItem("Custom Assets/Create Bezier Asset")]
  static void CreateAsset()
  {
    var asset = CreateInstance<BezierCurve>();
    asset.AddPointAt(Vector3.zero);
    UnityEditor.ProjectWindowUtil.CreateAsset(asset, "BezierCurve.asset");
  }

  /// <summary> エディター専用：使用しないでください </summary>
  public int selected { get; set; }
#endif

  [SerializeField, Range(8, 64)]
  int _resolution = 16;
  public int resolution { get { return _resolution; } }


  [SerializeField]
  Color _lineColor = Color.white;
  public Color color { get { return _lineColor; } }


  [SerializeField]
  bool _closeSpline = false;
  public bool closed { get { return _closeSpline; } }


  bool _dirty = true;

  [SerializeField]
  List<BezierPoint> _points = new List<BezierPoint>();

  public BezierPoint this[int index] { get { return _points[index]; } }
  public BezierPoint[] points { get { return _points.ToArray(); } }

  /// <summary> 曲線の管理下にある、制御点の数を返す </summary>
  public int count { get { return _points.Count; } }


  public void AddPoint(BezierPoint point)
  {
    _points.Add(point);
    _dirty = true;
  }

  public void AddPointAt(Vector3 position)
  {
    var point = new BezierPoint();

    point.handleType = BezierPoint.Handle.Connected;
    point.position = position;

    AddPoint(point);
  }

  public bool RemovePoint(BezierPoint point)
  {
    _dirty = _points.Remove(point);
    return _dirty;
  }


  float _length = 0f;
  public float length { get { return !_dirty ? _length : UpdateLength(); } }

  float UpdateLength()
  {
    _length = 0f;

    int last = count - 1;

    for (int i = 0; i < last; ++i)
    {
      _length += ApproximateLength(_points[i], _points[i + 1], _resolution);
    }

    if (_closeSpline)
    {
      _length += ApproximateLength(_points[last], _points[0], _resolution);
    }

    _dirty = false;

    return _length;
  }


  public Vector3 GetPointAt(float percent)
  {
    int last = count - 1;
    if (percent <= 0f) { return _points[0].position; }
    if (percent >= 1f) { return _points[last].position; }

    float total = 0f;
    float curve = 0f;

    BezierPoint p1 = null;
    BezierPoint p2 = null;

    for (int i = 0; i < last; ++i)
    {
      curve = ApproximateLength(_points[i], _points[i + 1], _resolution) / length;

      if ((total + curve) > percent)
      {
        p1 = _points[i];
        p2 = _points[i + 1];
        break;
      }

      total += curve;
    }

    if (closed && p1 == null)
    {
      p1 = _points[last];
      p2 = _points[0];
    }

    percent -= total;

    return GetPoint(p1, p2, percent / curve);
  }


  public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float percent)
  {
    bool changed = (p2.handleA != Vector3.zero);

    return p1.handleB != Vector3.zero
      ? changed
        ? CubicCurvePoint(p1.position, p1.globalB, p2.globalA, p2.position, percent)
        : QuadraticCurvePoint(p1.position, p1.globalB, p2.position, percent)
      : changed
        ? QuadraticCurvePoint(p1.position, p2.globalA, p2.position, percent)
        : LinearPoint(p1.position, p2.position, percent);
  }

  public static Vector3 CubicCurvePoint(Vector3 p1, Vector3 p2,
                                        Vector3 p3, Vector3 p4,
                                        float percent)
  {
    float t1 = Mathf.Clamp01(percent);
    float t2 = 1 - t1;

    p1 = p1 * Mathf.Pow(t2, 3);
    p2 = p2 * Mathf.Pow(t2, 2) * 3 * t1;
    p3 = p3 * Mathf.Pow(t1, 2) * 3 * t2;
    p4 = p4 * Mathf.Pow(t1, 3);

    return p1 + p2 + p3 + p4;
  }

  public static Vector3 QuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3,
                                            float percent)
  {
    float t1 = Mathf.Clamp01(percent);
    float t2 = 1 - t1;

    p1 = p1 * Mathf.Pow(t2, 2);
    p2 = p2 * (2 * t1 * t2);
    p3 = p3 * Mathf.Pow(t1, 2);

    return p1 + p2 + p3;
  }

  public static Vector3 LinearPoint(Vector3 p1, Vector3 p2, float percent)
  {
    return p1 + ((p2 - p1) * Mathf.Clamp01(percent));
  }


  public static Vector3 GetPoint(float percent, params Vector3[] points)
  {
    percent = Mathf.Clamp01(percent);

    int order = points.Length - 1;
    Vector3 point = Vector3.zero;

    for (int i = 0, length = points.Length; i < length; ++i)
    {
      int binomial = BinomialCoefficient(i, order);
      float pow1 = Mathf.Pow(percent, order - i);
      float pow2 = Mathf.Pow(1 - percent, i);

      point += points[length - i - 1] * (binomial * pow1 * pow2);
    }

    return point;
  }


  public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int division)
  {
    float resolution = division;
    float total = 0f;

    Vector3 last = p1.position;
    Vector3 current = Vector3.zero;

    for (int i = 0, limit = division + 1; i < limit; ++i)
    {
      current = GetPoint(p1, p2, i / resolution);
      total += (current - last).magnitude;
      last = current;
    }

    return total;
  }


  static int BinomialCoefficient(int i, int n)
  {
    return Factoral(n) / (Factoral(i) * Factoral(n - i));
  }

  static int Factoral(int i)
  {
    if (i == 0) { return 1; }

    int total = 1;

    while ((i - 1) >= 0)
    {
      total *= i;
      --i;
    }

    return total;
  }
}
