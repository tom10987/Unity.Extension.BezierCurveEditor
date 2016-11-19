
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BezierCurve))]
public class BezierCurveDrawer : PropertyDrawer
{
  public override float GetPropertyHeight(SerializedProperty property,
                                          GUIContent label)
  {
    return CustomStyles.lineHeight * 4f;
  }


  SerializedObject _object = null;
  BezierEditor _editor = null;

  void PropertySetup(SerializedProperty property)
  {
    if (_object != null) { return; }

    _object = new SerializedObject(property.objectReferenceValue);
    _editor = new BezierEditor(_object);
  }


  public override void OnGUI(Rect position,
                             SerializedProperty property,
                             GUIContent label)
  {
    position.height = EditorGUIUtility.singleLineHeight;

    EditorGUI.PropertyField(position, property);
    if (property.objectReferenceValue == null) { return; }

    PropertySetup(property);
    _object.Update();

    position.y += CustomStyles.lineHeight;
    EditorGUI.PropertyField(position, _editor.resolution);

    position.y += CustomStyles.lineHeight;
    EditorGUI.PropertyField(position, _editor.color);

    position.y += CustomStyles.lineHeight;
    EditorGUI.PropertyField(position, _editor.close);

    BezierEditor.display = CustomStyles.DrawToggle(BezierEditor.header,
                                                   BezierEditor.display,
                                                   BuiltinStyle.toggleModule);
    if (BezierEditor.display) { _editor.orderList.DoLayoutList(); }

    if (GUI.changed) { _object.ApplyModifiedProperties(); }
  }
}
