
using UnityEngine;

[System.Serializable]
public class BezierPoint
{
  public enum Handle
  {
    /// <summary> 別のハンドルに影響を与える </summary>
    Connected,
    /// <summary> 別のハンドルの影響を受けない </summary>
    Broken,
    /// <summary> ハンドルを持たない </summary>
    None,
  }

  /// <summary> ハンドルを更新したときの挙動 </summary>
  public Handle handleType = Handle.None;

  /// <summary> 制御点自体の座標 </summary>
  public Vector3 position = Vector3.zero;


  // readonly なフィールドは、エディター拡張の使用時にプロパティとして取得できない
  [SerializeField]
  Vector3[] _handles = new Vector3[] { Vector3.zero, Vector3.zero, };

  /// <summary> ハンドルのローカル座標を添え字アクセスで操作 </summary>
  public Vector3 this[int index]
  {
    get { return _handles[index]; }
    set
    {
      if (_handles[index] == value) { return; }
      _handles[index] = value;

      switch (handleType)
      {
        case Handle.None: handleType = Handle.Broken; break;
        case Handle.Connected: _handles[(index ^ 1)] = -value; break;
        default: break;
      }
    }
  }


  public Vector3 handleA { get { return this[0]; } set { this[0] = value; } }
  public Vector3 handleB { get { return this[1]; } set { this[1] = value; } }

  public Vector3 globalA
  {
    get { return position + handleA; }
    set { handleA = value - position; }
  }

  public Vector3 globalB
  {
    get { return position + handleB; }
    set { handleB = value - position; }
  }
}
