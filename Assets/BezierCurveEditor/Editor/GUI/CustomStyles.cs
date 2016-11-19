
using UnityEditor;
using UnityEngine;

public static class CustomStyles
{
  static readonly float _height = EditorGUIUtility.singleLineHeight + 2f;
  public static float lineHeight { get { return _height; } }


  public static void DrawHeader(string title, GUIStyle style)
  {
    var rect = GUILayoutUtility.GetRect(0f, 24f, style);
    GUI.Box(rect, title, style);
  }

  public static bool DrawToggle(string title, bool display, GUIStyle style)
  {
    var header = GUILayoutUtility.GetRect(0f, 24f, style);
    GUI.Box(header, title, style);

    var toggle = new Rect(header)
    {
      x = header.x + 4f,
      y = header.y + 2f,
      width = 14f,
      height = 14f,
    };


    if (Event.current.IsRepaint())
    {
      EditorStyles.foldout.Draw(toggle, false, false, display, false);
    }

    if (Event.current.MouseDown(header))
    {
      display = !display;
      Event.current.Use();
    }

    return display;
  }


  public static bool IsRepaint(this Event @event)
  {
    return @event.type == EventType.Repaint;
  }

  public static bool MouseDown(this Event @event, Rect area)
  {
    var isMouseDown = (@event.type == EventType.MouseDown);
    return isMouseDown && area.Contains(@event.mousePosition);
  }
}
