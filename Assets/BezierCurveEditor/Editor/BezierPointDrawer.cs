
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BezierPoint))]
public class BezierPointDrawer : PropertyDrawer
{
  public override float GetPropertyHeight(SerializedProperty property,
                                          GUIContent label)
  {
    return CustomStyles.lineHeight * 4f;
  }


  public override void OnGUI(Rect position,
                             SerializedProperty property,
                             GUIContent label)
  {
    using (new EditorGUI.PropertyScope(position, label, property))
    {
      var handleType = property.FindPropertyRelative("handleType");
      var handleBody = property.FindPropertyRelative("position");
      var handles = property.FindPropertyRelative("_handles");

      position.height = EditorGUIUtility.singleLineHeight;

      GUI.enabled = true;
      handleType.intValue = (int)UpdateHandleType(handleType, position);
      EditorGUI.indentLevel++;

      position.y += CustomStyles.lineHeight;
      var value = handleBody.vector3Value;
      handleBody.vector3Value = EditorGUI.Vector3Field(position, "Position", value);
      EditorGUI.indentLevel++;

      // ハンドルの状態を GUI 操作に反映
      GUI.enabled = handleType.intValue != (int)BezierPoint.Handle.None;
      {
        // ハンドルの状態が操作に影響するため、あらかじめ判定しておく
        bool connected = handleType.intValue == (int)BezierPoint.Handle.Connected;

        position.y += CustomStyles.lineHeight;
        UpdateField(handles, position, 0, connected);

        position.y += CustomStyles.lineHeight;
        UpdateField(handles, position, 1, connected);
      }
      EditorGUI.indentLevel = 0;

      // ハンドルの状態が無効なら、ハンドルの値も無効化する
      if (!GUI.enabled) { SetHandles(handles, Vector3.zero); }
    }
  }


  static BezierPoint.Handle UpdateHandleType(SerializedProperty handleType,
                                             Rect position)
  {
    var value = (BezierPoint.Handle)handleType.intValue;
    return (BezierPoint.Handle)EditorGUI.EnumPopup(position, "Handle Type", value);
  }


  static void UpdateField(SerializedProperty handles,
                          Rect position,
                          int index,
                          bool connected)
  {
    string label = (index == 0 ? "Handle A" : "Handle B");
    var handle = handles.GetArrayElementAtIndex(index);
    var result = EditorGUI.Vector3Field(position, label, handle.vector3Value);

    var changed = (result != handle.vector3Value);
    if (changed) { handle.vector3Value = result; }

    // ハンドルの状態が Handle.Connected なら、別のハンドルにも値を反映する
    if (!connected | !changed) { return; }
    handles.GetArrayElementAtIndex(index ^ 1).vector3Value = -result;
  }


  static void SetHandles(SerializedProperty handles, Vector3 value)
  {
    handles.GetArrayElementAtIndex(0).vector3Value = value;
    handles.GetArrayElementAtIndex(1).vector3Value = value;
  }
}
