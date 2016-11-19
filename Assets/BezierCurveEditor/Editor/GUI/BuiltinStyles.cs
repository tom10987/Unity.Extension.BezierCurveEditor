
using UnityEditor;
using UnityEngine;

public static class BuiltinStyle
{
  public static GUIStyle headerModule { get; private set; }
  public static GUIStyle toggleModule { get; private set; }

  static BuiltinStyle()
  {
    headerModule = new GUIStyle("ShurikenModuleTitle");
    headerModule.font = EditorStyles.label.font;
    headerModule.fixedHeight = 24f;
    headerModule.contentOffset = new Vector2(6f, -3f);

    toggleModule = new GUIStyle(headerModule);
    toggleModule.border = new RectOffset(15, 7, 4, 4);
    toggleModule.contentOffset = new Vector2(21f, -3f);
  }
}
