using UnityEngine;
using System.Collections;

public static class SuperDebugger
{
    public static void DrawPlus(Vector2 point, Color color, float sideLength = 1, float timeVisible = 0.01f)
    {
        Debug.DrawLine(point, point + Vector2.up * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.right * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.down * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.left * sideLength, color, timeVisible);
    }

    public static void DrawBox(Vector2 min, Vector2 max, Color color, float timeVisible = 0.01f)
    {
        Vector2 upperLeft = new Vector2(min.x, max.y);
        Vector2 lowerRight = new Vector2(max.x, min.y);

        Debug.DrawLine(min, upperLeft, color, timeVisible);
        Debug.DrawLine(upperLeft, max, color, timeVisible);
        Debug.DrawLine(max, lowerRight, color, timeVisible);
        Debug.DrawLine(lowerRight, min, color, timeVisible);
    }

    public static void DrawX(Vector2 point, float halfWidth, Color color, float timeVisible = 0.01f)
    {
        Debug.DrawLine(new Vector2(point.x + halfWidth, point.y + halfWidth), point, color, timeVisible);
        Debug.DrawLine(new Vector2(point.x - halfWidth, point.y + halfWidth), point, color, timeVisible);
        Debug.DrawLine(new Vector2(point.x + halfWidth, point.y - halfWidth), point, color, timeVisible);
        Debug.DrawLine(new Vector2(point.x - halfWidth, point.y - halfWidth), point, color, timeVisible);
    }

    public static void DrawBoxAtPoint(Vector2 point, float halfWidth, Color color, float timeVisible = 0.01f)
    {
        Vector2 upperRight = new Vector2(point.x + halfWidth, point.y + halfWidth);
        Vector2 lowerRight = new Vector2(point.x + halfWidth, point.y - halfWidth);
        Vector3 lowerLeft = new Vector2(point.x - halfWidth, point.y - halfWidth);
        Vector2 upperLeft = new Vector2(point.x - halfWidth, point.y + halfWidth);

        Debug.DrawLine(upperRight, lowerRight, color, timeVisible);
        Debug.DrawLine(lowerRight, lowerLeft, color, timeVisible);
        Debug.DrawLine(lowerLeft, upperLeft, color, timeVisible);
        Debug.DrawLine(upperLeft, upperRight, color, timeVisible);
    }

}
