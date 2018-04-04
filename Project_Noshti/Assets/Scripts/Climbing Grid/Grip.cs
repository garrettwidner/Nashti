using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{
    public const float WIDTH_BETWEEN_GRIPS = .25f;
    public const float GRIP_WIDTH = .05f;
    public const float HALF_GRIP_WIDTH = .025f;

    [SerializeField]
    [Range(1,10)]
    private int quality;
    [SerializeField]
    private Type type;
    [SerializeField]
    private BoxCollider2D boxCollider = null;

    public enum Type
    {
        Backwall,
        Sidewall,
        Ladder
    };

    public bool IsNull
    {
        get
        {
            return boxCollider == null;
        }
    }

    public bool IsHorizontallyInLineWith(Grip grip)
    {
        float yDifference = transform.position.y - grip.transform.position.y;
        if(Mathf.Abs(yDifference) >= HALF_GRIP_WIDTH)
        {
            return false;
        }
        return true;
    }

    public bool IsVerticallyInLineWith(Grip grip)
    {
        float xDifference = transform.position.x - grip.transform.position.x;
        if (Mathf.Abs(xDifference) >= HALF_GRIP_WIDTH)
        {
            return false;
        }
        return true;
    }

    public bool IsInSameSquareAs(Grip grip)
    {
        float xDifference = transform.position.x - grip.transform.position.x;
        float yDifference = transform.position.y - grip.transform.position.y;

        bool xIsInSquare = IsDifferenceWithinSquareBounds(xDifference);
        bool yIsInSquare = IsDifferenceWithinSquareBounds(yDifference);

        if (xIsInSquare == true && yIsInSquare == true)
        {
            return true;
        }
        return false;
    }

    private bool IsDifferenceWithinSquareBounds(float positionalDifference)
    {
        float minLeeway = HALF_GRIP_WIDTH;
        float maxLeeway = WIDTH_BETWEEN_GRIPS + HALF_GRIP_WIDTH;

        if (Mathf.Abs(positionalDifference) > minLeeway)
        {

            if (Mathf.Abs(positionalDifference) < maxLeeway)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public class Square
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

        public bool HasFullRightSide
        {
            get
            {
                if (upperRight != null && lowerRight != null)
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

        public bool HasFullLeftSide
        {
            get
            {
                if (upperLeft != null && lowerLeft != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasTopSide
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

        public bool HasFullTopSide
        {
            get
            {
                if (upperLeft != null && upperRight != null)
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

        public bool HasFullBottomSide
        {
            get
            {
                if (lowerLeft != null && lowerRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsConnectible
        {
            get
            {
                return GripCount >= 2;
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
                SuperDebugger.DrawPlus(upperLeft.transform.position, Color.yellow, sideLength, visibleTime);
            }
            if (upperRight != null)
            {
                SuperDebugger.DrawPlus(upperRight.transform.position, Color.magenta, sideLength, visibleTime);
            }
            if (lowerLeft != null)
            {
                SuperDebugger.DrawPlus(lowerLeft.transform.position, Color.cyan, sideLength, visibleTime);
            }
            if (lowerRight != null)
            {
                SuperDebugger.DrawPlus(lowerRight.transform.position, Color.white, sideLength, visibleTime);
            }
        }

        /// <summary>
        /// Creates a Grip.Square with the given 
        /// </summary>
        /// <param name="upLeft"></param>
        /// <param name="upRight"></param>
        /// <param name="lowLeft"></param>
        /// <param name="lowRight"></param>
        public Square(Grip upLeft, Grip upRight, Grip lowLeft, Grip lowRight)
        {
            upperLeft = upLeft;
            upperRight = upRight;
            lowerLeft = lowLeft;
            lowerRight = lowRight;
        }

        /// <summary>
        /// Populates a grip square if given two grips from the square.
        /// </summary>
        /// <param name="grip1"></param>
        /// <param name="grip2"></param>
        public Square(Grip grip1, Grip grip2, Vector2 precedingDirection)
        {
            precedingDirection.Normalize();
            if (precedingDirection != Vector2.left && precedingDirection != Vector2.right && precedingDirection != Vector2.up && precedingDirection != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
                return;
            }

            bool isHorizontallyInline = grip1.IsHorizontallyInLineWith(grip2);
            bool isVerticallyInline = grip1.IsVerticallyInLineWith(grip2);

            if(precedingDirection == Vector2.left || precedingDirection == Vector2.right)
            {

            }
            else if(precedingDirection == Vector2.up || precedingDirection == Vector2.down)
            {

            }

        }
        

        public Square()
        {
            upperLeft = null;
            upperRight = null;
            lowerLeft = null;
            lowerRight = null;
        }
    }
}
