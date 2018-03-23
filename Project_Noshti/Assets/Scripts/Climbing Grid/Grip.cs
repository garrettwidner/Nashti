﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{
    public const float WIDTH_BETWEEN_GRIPS = .5f;
    public const float GRIP_WIDTH = .1f;
    public const float HALF_GRIP_WIDTH = .05f;

    [SerializeField]
    [Range(1,10)]
    private int quality;
    [SerializeField]
    private Type type;
    [SerializeField]
    private BoxCollider2D boxCollider;

    public enum Type
    {
        Backwall,
        Sidewall,
        Ladder
    };

    public struct Square
    {
        public Grip upperLeft;
        public Grip upperRight;
        public Grip lowerLeft;
        public Grip lowerRight;

        public bool IsNull
        {
            get
            {
                if(upperLeft == null && upperRight == null && lowerLeft == null && lowerRight == null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasRightSide
        {
            get
            {
                if(upperRight != null || lowerRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasLeftSide
        {
            get
            {
                if(upperLeft != null || lowerLeft != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasUpperSide
        {
            get
            {
                if(upperLeft != null || upperRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasBottomSide
        {
            get
            {
                if(lowerLeft != null || lowerRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public int GripCount
        {
            get
            {
                int count = 0;
                if(upperLeft != null)
                {
                    count++;
                }
                if(upperRight != null)
                {
                    count++;
                }
                if(lowerLeft != null)
                {
                    count++;
                }
                if(lowerRight != null)
                {
                    count++;
                }
                return count;
            }
        }

        public Vector2 Center
        {
            get
            {
                float centerOffset = WIDTH_BETWEEN_GRIPS / 2;
                if(upperLeft != null)
                {
                    return (Vector2)upperLeft.transform.position + new Vector2(centerOffset, -centerOffset);
                }
                if (upperRight != null)
                {
                    return (Vector2)upperRight.transform.position + new Vector2(-centerOffset, -centerOffset);
                }
                if (lowerLeft != null)
                {
                    return (Vector2)lowerLeft.transform.position + new Vector2(centerOffset, centerOffset);
                }
                if (lowerRight != null)
                {
                    return (Vector2)lowerRight.transform.position + new Vector2(-centerOffset, centerOffset);
                }

                return Vector2.zero;
            }
        }

        public void DebugSquare()
        {
            float sideLength = 0.1f;
            float visibleTime = 1f;
            if (upperLeft != null)
            {
                SuperDebugger.DrawPlusAtPoint(upperLeft.transform.position, Color.yellow, sideLength, visibleTime);
            }
            if (upperRight != null)
            {
                SuperDebugger.DrawPlusAtPoint(upperRight.transform.position, Color.magenta, sideLength, visibleTime);
            }
            if (lowerLeft != null)
            {
                SuperDebugger.DrawPlusAtPoint(lowerLeft.transform.position, Color.cyan, sideLength, visibleTime);
            }
            if (lowerRight != null)
            {
                SuperDebugger.DrawPlusAtPoint(lowerRight.transform.position, Color.white, sideLength, visibleTime);
            }
        }

        public Square(Grip upLeft, Grip upRight, Grip lowLeft, Grip lowRight)
        {
            upperLeft = upLeft;
            upperRight = upRight;
            lowerLeft = lowLeft;
            lowerRight = lowRight;
        }
    }
}
