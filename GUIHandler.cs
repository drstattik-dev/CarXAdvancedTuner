using System;
using UnityEngine;

public class GUIHandler : MonoBehaviour
{
public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

public static void DrawString(Vector2 position, string label, Color color, int fontSize, bool centered = true)
{
    GUI.color = color;
    StringStyle.fontSize = fontSize;
    StringStyle.normal.textColor = color;
    var content = new GUIContent(label);
    var size = StringStyle.CalcSize(content);
    var upperLeft = centered ? position - size / 2f : position;
    GUI.Label(new Rect(upperLeft, size), content, StringStyle);
}

public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
{
    GUI.depth = 0;
    var rad2deg = 360 / (Math.PI * 2);
    Vector2 d = end - start;
    float a = (float)rad2deg * Mathf.Atan(d.y / d.x);

    if (d.x < 0)
        a += 180;

    int width2 = (int)Mathf.Ceil(width / 2);

    GUIUtility.RotateAroundPivot(a, start);
    GUI.color = color;
    GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), Texture2D.whiteTexture, ScaleMode.StretchToFill);
    GUIUtility.RotateAroundPivot(-a, start);
}

public static void DrawBox(Vector2 position, Vector2 size, Color color, bool centered = true)
{
    var upperleft = centered ? position - size / 2f : position;
    GUI.color = color;
    GUI.DrawTexture(new Rect(position, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
}

public static void DrawBoxOutline(Vector2 Point, float width, float height, Color color)
{
    DrawLine(Point, new Vector2(Point.x + width, Point.y), color, 2);
    DrawLine(Point, new Vector2(Point.x, Point.y + height), color, 2);
    DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x + width, Point.y), color, 2);
    DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x, Point.y + height), color, 2);
}
}