
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class BezierEditor
{
  public BezierCurve curve { get; private set; }

  public SerializedProperty resolution { get; private set; }
  public SerializedProperty color { get; private set; }
  public SerializedProperty close { get; private set; }

  public ReorderableList orderList { get; private set; }

  public static bool display { get; set; }
  public static string header { get { return display ? "Close" : "Open"; } }


  public BezierEditor(SerializedObject serializedObject)
  {
    curve = serializedObject.targetObject as BezierCurve;

    resolution = serializedObject.FindProperty("_resolution");
    color = serializedObject.FindProperty("_lineColor");
    close = serializedObject.FindProperty("_closeSpline");

    var points = serializedObject.FindProperty("_points");
    orderList = new ReorderableList(serializedObject, points);

    orderList.headerHeight = 0f;
    orderList.footerHeight = 20f;
    orderList.elementHeight = CustomStyles.lineHeight * 4f + 4f;

    orderList.drawElementCallback = (rect, index, active, focused) =>
    {
      rect.height -= 2f;

      bool repaint = (Event.current.type == EventType.Repaint);
      if (repaint) { GUI.skin.box.Draw(rect, false, active, active, false); }

      rect.x += 2f;
      rect.y += 2f;
      rect.width -= 4f;
      rect.height += 2f;

      var point = points.GetArrayElementAtIndex(index);
      EditorGUI.PropertyField(rect, point);
    };

    orderList.onSelectCallback = (list) =>
    {
      curve.selected = list.index;
      SceneView.RepaintAll();
    };

    orderList.onAddCallback = (list) =>
    {
      curve.AddPointAt(Vector3.zero);
      list.index = curve.count - 1;
      EditorUtility.SetDirty(curve);
    };
  }
}
