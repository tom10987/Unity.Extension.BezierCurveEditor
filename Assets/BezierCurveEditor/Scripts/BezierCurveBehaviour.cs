
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BezierCurveBehaviour : MonoBehaviour
{
  [SerializeField]
  BezierCurve _curve = null;

  public BezierCurve curve { get { return _curve; } }
}
