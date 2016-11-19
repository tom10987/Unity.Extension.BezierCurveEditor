
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
  BezierEditor _editor = null;

  void OnEnable() { _editor = _editor ?? new BezierEditor(serializedObject); }


  public override void OnInspectorGUI()
  {
    serializedObject.Update();

    EditorGUILayout.PropertyField(_editor.resolution);
    EditorGUILayout.PropertyField(_editor.color);
    EditorGUILayout.PropertyField(_editor.close);

    BezierEditor.display = CustomStyles.DrawToggle(BezierEditor.header,
                                                   BezierEditor.display,
                                                   BuiltinStyle.toggleModule);
    if (BezierEditor.display) { _editor.orderList.DoLayoutList(); }

    if (GUI.changed)
    {
      serializedObject.ApplyModifiedProperties();
      EditorUtility.SetDirty(_editor.curve);
      Repaint();
    }
  }
}
