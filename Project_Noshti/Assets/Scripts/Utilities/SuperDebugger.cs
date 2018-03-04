using UnityEngine;
using System.Collections;

public static class SuperDebugger
{
    public static void DrawPlusAtPoint(Vector2 point, Color color, float sideLength = 1, float timeVisible = 0.01f)
    {
        Debug.DrawLine(point, point + Vector2.up * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.right * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.down * sideLength, color, timeVisible);
        Debug.DrawLine(point, point + Vector2.left * sideLength, color, timeVisible);
    }

}
