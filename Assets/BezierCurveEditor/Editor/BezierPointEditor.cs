
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurveBehaviour))]
public class BezierPointEditor : Editor
{
  BezierCurveBehaviour _bezier = null;

  void OnEnable() { _bezier = target as BezierCurveBehaviour; }


  void OnSceneGUI()
  {
    serializedObject.Update();

    var curve = _bezier.curve;
    var point = curve[curve.selected];
    point.position = DrawPoint(point.position, curve.name, curve.selected);

    if (point.handleType != BezierPoint.Handle.None)
    {
      Handles.color = Color.cyan;
      point.globalA = DrawHandle(point.globalA);
      point.globalB = DrawHandle(point.globalB);
    }

    Handles.color = Color.yellow;
    Handles.DrawLine(point.position, point.globalA);
    Handles.DrawLine(point.position, point.globalB);

    if (GUI.changed)
    {
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(curve);
      Repaint();
    }
  }

  static Vector3 DrawPoint(Vector3 position, string name, int selected)
  {
    var offset = Vector3.up * HandleUtility.GetHandleSize(position) * 0.5f;
    Handles.Label(position + offset, name + ": index " + selected);

    Handles.color = Color.green;
    var size = HandleUtility.GetHandleSize(position) * 0.15f;
    position = Handles.FreeMoveHandle(position, Quaternion.identity, size,
                                      Vector3.zero, Handles.CubeCap);
    return position;
  }

  static Vector3 DrawHandle(Vector3 handle)
  {
    var size = HandleUtility.GetHandleSize(handle) * 0.15f;
    return Handles.FreeMoveHandle(handle, Quaternion.identity, size,
                                  Vector3.zero, Handles.SphereCap);
  }


  [DrawGizmo(GizmoType.InSelectionHierarchy)]
  static void DrawGizmos(BezierCurveBehaviour bezier, GizmoType gizmoType)
  {
    if (bezier.curve == null) { return; }

    int last = bezier.curve.count - 1;
    if (last < 1) { return; }

    Gizmos.color = bezier.curve.color;

    for (int i = 0; i < last; ++i)
    {
      DrawCurve(bezier.curve[i], bezier.curve[i + 1], bezier.curve.resolution);
    }

    if (!bezier.curve.closed) { return; }
    DrawCurve(bezier.curve[last], bezier.curve[0], bezier.curve.resolution);
  }

  static void DrawCurve(BezierPoint p1, BezierPoint p2, int division)
  {
    float resolution = division;

    Vector3 last = p1.position;
    Vector3 current = Vector3.zero;

    for (int i = 1, limit = division + 1; i < limit; ++i)
    {
      current = BezierCurve.GetPoint(p1, p2, i / resolution);
      Gizmos.DrawLine(last, current);
      last = current;
    }
  }
}
