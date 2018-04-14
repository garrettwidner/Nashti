using System;
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
        UpLeft,
        Center
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

    public static Direction Vector2ToDirection(Vector2 v)
    {
        v.Normalize();

        if (v == Vector2.up)
        {
            return Direction.Up;
        }
        else if(v == (Vector2.up + Vector2.right).normalized)
        {
            return Direction.UpRight;
        }
        else if(v == Vector2.right)
        {
            return Direction.Right;
        }
        else if(v == (Vector2.right + Vector2.down).normalized)
        {
            return Direction.DownRight;
        }
        else if(v == Vector2.down)
        {
            return Direction.Down;
        }
        else if(v == (Vector2.down + Vector2.left).normalized)
        {
            return Direction.DownLeft;
        }
        else if(v == Vector2.left)
        {
            return Direction.Left;
        }
        else if(v == (Vector2.left + Vector2.up).normalized)
        {
            return Direction.UpLeft;
        }

        Debug.LogWarning("Passed in Vector2 must be a cardinal or ordinal direction");
        return Direction.Center;
    }
}

[System.Serializable]
public class CardinalContainer<T> : Orientation
{
    public T up;
    public T right;
    public T down;
    public T left;

    public T Vector2ToObject(Vector2 v)
    {
        Direction direction = Vector2ToDirection(v);

        return DirectionToObject(direction);
    }

    public T DirectionToObject(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return up;
            case Direction.Right:
                return right;
            case Direction.Down:
                return down;
            case Direction.Left:
                return left;
            default:
                Debug.LogWarning("Cardinal Containers only contian objects in cardinal directions. Returning up.");
                return up;
        }
    }
}

[System.Serializable]
public class OrdinalContainer<T> : Orientation
{
    public T upRight;
    public T downRight;
    public T downLeft;
    public T upLeft;

    public T Vector2ToObject(Vector2 v)
    {
        Direction direction = Vector2ToDirection(v);

        return DirectionToObject(direction);
    }

    public T DirectionToObject(Direction direction)
    {
        switch (direction)
        {
            case Direction.UpRight:
                return upRight;
            case Direction.DownRight:
                return downRight;
            case Direction.DownLeft:
                return downLeft;
            case Direction.UpLeft:
                return upLeft;
            default:
                Debug.LogWarning("Ordinal Containers only contian objects in ordinal directions. Returning upRight.");
                return upRight;

        }
    }
}


