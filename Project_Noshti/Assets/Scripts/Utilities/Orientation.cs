using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orientation
{
    public enum Direction
    {
        Up,
        UpRight,
        Right,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft
    };

    public static Vector2 DirectionToVector2(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.UpRight:
                return (Vector2.up + Vector2.right).normalized;
            case Direction.Right:
                return Vector2.right;
            case Direction.DownRight:
                return (Vector2.down + Vector2.right).normalized;
            case Direction.Down:
                return Vector2.down;
            case Direction.DownLeft:
                return (Vector2.down + Vector2.left).normalized;
            case Direction.Left:
                return Vector2.left;
            default:
                return (Vector2.up + Vector2.left).normalized;
        }
    }
}

[System.Serializable]
public class CardinalContainer<T> : Orientation
{
    public T up;
    public T right;
    public T down;
    public T left;
}

[System.Serializable]
public class OrdinalContainer<T> : Orientation
{
    public T upRight;
    public T downRight;
    public T downLeft;
    public T upLeft;
}


