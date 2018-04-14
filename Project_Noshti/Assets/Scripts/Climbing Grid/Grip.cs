using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{
    public const float WIDTH_BETWEEN_GRIPS = .25f;
    public const float GRIP_WIDTH = .05f;
    public const float HALF_GRIP_WIDTH = .025f;

    [SerializeField] [Range(1,10)] private int quality;
    [SerializeField] private Type type;
    [SerializeField] private BoxCollider2D boxCollider;

    public enum Type
    {
        Square,
        Peg,
        Ladder
    };

    public bool IsEmpty
    {
        get
        {
            return boxCollider == null;
        }
    }

    public bool IsNotEmpty
    {
        get
        {
            return !IsEmpty;
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

        bool xIsInSquare = IsPositionDifferenceWithinBoundsOfSquare(xDifference);
        bool yIsInSquare = IsPositionDifferenceWithinBoundsOfSquare(yDifference);

        if (xIsInSquare == true && yIsInSquare == true)
        {
            return true;
        }
        return false;
    }

    public static Grip CheckLocationForGrip(Vector2 location, float checkRadius, LayerMask gripLayer)
    {
        Collider2D[] foundColliders = Physics2D.OverlapCircleAll(location, checkRadius, gripLayer);
        if (foundColliders != null)
        {
            Collider2D closest = null;

            //Find closest collider with a GripPoint component
            foreach (Collider2D collider in foundColliders)
            {
                Grip foundGrip = collider.GetComponent<Grip>();
                if (foundGrip != null)
                {
                    if (closest == null)
                    {
                        closest = collider;
                    }
                    else
                    {
                        closest = FindClosestCollider(location, collider, closest);
                    }
                }
                else
                {
                    Debug.LogWarning("Object " + collider.name + " on Grip layer missing GripPoint component. Omitted from CheckProximity() results");
                }
            }

            if (closest != null)
            {
                return closest.GetComponent<Grip>();
            }
        }

        return null;
    }

    private static Collider2D FindClosestCollider(Vector2 start, Collider2D collider1, Collider2D collider2)
    {
        float distanceTo1 = Vector2.Distance(start, collider1.transform.position);
        float distanceTo2 = Vector2.Distance(start, collider2.transform.position);

        if (distanceTo1 < distanceTo2)
        {
            return collider1;
        }

        return collider2;
    }

    private bool IsPositionDifferenceWithinBoundsOfSquare(float positionalDifference)
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

    /// <summary>
    /// Returns null grip if none is found
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Grip FindAdjacentTo(Grip startGrip, Vector2 direction, LayerMask gripLayer)
    {
        direction.Normalize();
        if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return null;
        }

        Vector2 checkLocation = (Vector2)startGrip.transform.position + (direction * WIDTH_BETWEEN_GRIPS);
        Vector2 colliderSizeOffset = new Vector2(Grip.HALF_GRIP_WIDTH, Grip.HALF_GRIP_WIDTH);

        Collider2D foundCollider = Physics2D.OverlapArea(checkLocation + colliderSizeOffset, checkLocation - colliderSizeOffset, gripLayer);
        if(foundCollider != null)
        {
            Grip foundGrip = foundCollider.GetComponent<Grip>();

            if (!foundGrip.IsEmpty)
            {
                return foundGrip;
            }
        }

        return null;
    }

    /// <summary>
    /// Checkdistance is measured in number of concurrent spaces to check for  InitialSpacesToSkip is subtracted from the start of this.
    /// For example, spacesToCheck = 5 and initialSpacesToSkip = 1 would start checking at the second space in the direction.
    /// spacesBeforeFound returns 
    /// </summary>
    /// <param name="startGrip"></param>
    /// <param name="direction"></param>
    /// <param name="gripLayer"></param>
    /// <param name="spacesToCheck"></param>
    /// <param name="initialSpacesToSkip"></param>
    /// <returns></returns>
    public static Grip FindInDirection(Grip startGrip, Vector2 direction, LayerMask gripLayer, int spacesToCheck, out int spacesSearchedBeforeGripFound, int initialSpacesToSkip = 0)
    {
        return FindInDirection(startGrip.transform.position, direction, gripLayer, spacesToCheck, out spacesSearchedBeforeGripFound);
    }

    public static Grip FindInDirection(Vector2 startPosition, Vector2 direction, LayerMask gripLayer, int spacesToCheck, out int spacesSearchedBeforeGripFound, int initialSpacesToSkip = 0)
    {
        Grip foundGrip = null;
        spacesSearchedBeforeGripFound = 0;

        if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return foundGrip;
        }

        for (int i = 1 + initialSpacesToSkip; i < spacesToCheck; i++)
        {
            Vector2 checkPosition = startPosition + (direction * WIDTH_BETWEEN_GRIPS * i);
            foundGrip = CheckLocationForGrip(checkPosition, HALF_GRIP_WIDTH, gripLayer);
            if (foundGrip != null && !foundGrip.IsEmpty)
            {
                spacesSearchedBeforeGripFound = i;
                break;
            }
        }

        return foundGrip;
    }

    /// <summary>
    /// Returns the first Grip found if moving in movingDirection. Returns null Grip if neither grip is closer.
    /// </summary>
    /// <param name="movingDirection"></param>
    /// <param name="grip1"></param>
    /// <param name="grip2"></param>
    /// <returns></returns>
    public static Grip DetermineClosestGripInMovingDirection(Vector2 movingDirection, Grip grip1, Grip grip2)
    {
        movingDirection.Normalize();
        if (movingDirection != Vector2.left && movingDirection != Vector2.right && movingDirection != Vector2.up && movingDirection != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return null;
        }

        if (movingDirection == Vector2.up)
        {
            if (grip1.IsHorizontallyInLineWith(grip2))
            {
                return null;
            }
            else
            {
                return (grip1.transform.position.y < grip2.transform.position.y) ? grip1 : grip2;
            }
        }
        else if (movingDirection == Vector2.right)
        {
            if (grip1.IsVerticallyInLineWith(grip2))
            {
                return null;
            }
            else
            {
                return (grip1.transform.position.x < grip2.transform.position.y) ? grip1 : grip2;
            }
        }
        else if (movingDirection == Vector2.down)
        {
            if (grip1.IsHorizontallyInLineWith(grip2))
            {
                return null;
            }
            else
            {
                return (grip1.transform.position.y > grip2.transform.position.y) ? grip1 : grip2;
            }
        }
        else
        {
            if (grip1.IsVerticallyInLineWith(grip2))
            {
                return null;
            }
            else
            {
                return (grip1.transform.position.x > grip2.transform.position.y) ? grip1 : grip2;
            }
        }
    }

    [System.Serializable]
    public class Square : OrdinalContainer<Grip>
    {
        public bool IsEmpty
        {
            get
            {
                if(upLeft == null && upRight == null && downLeft == null && downRight == null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsNotEmpty
        {
            get
            {
                return !IsEmpty;
            }
        }

        public bool HasGripOnRightSide
        {
            get
            {
                if (upRight != null || downRight != null)
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
                if (upRight != null && downRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasGripOnLeftSide
        {
            get
            {
                if (upLeft != null || downLeft != null)
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
                if (upLeft != null && downLeft != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasGripOnTopSide
        {
            get
            {
                if (upLeft != null || upRight != null)
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
                if (upLeft != null && upRight != null)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasGripOnBottomSide
        {
            get
            {
                if (downLeft != null || downRight != null)
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
                if (downLeft != null && downRight != null)
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
                if(upLeft != null)
                {
                    count++;
                }
                if(upRight != null)
                {
                    count++;
                }
                if(downLeft != null)
                {
                    count++;
                }
                if(downRight != null)
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
                if(upLeft != null)
                {
                    return (Vector2)upLeft.transform.position + new Vector2(centerOffset, -centerOffset);
                }
                if (upRight != null)
                {
                    return (Vector2)upRight.transform.position + new Vector2(-centerOffset, -centerOffset);
                }
                if (downLeft != null)
                {
                    return (Vector2)downLeft.transform.position + new Vector2(centerOffset, centerOffset);
                }
                if (downRight != null)
                {
                    return (Vector2)downRight.transform.position + new Vector2(-centerOffset, centerOffset);
                }

                return Vector2.zero;
            }
        }

        public Vector2 GetPositionOfGrip(Orientation.Direction gripDirectionFromCenter)
        {
            switch (gripDirectionFromCenter)
            {
                case Orientation.Direction.UpRight:
                    return Center + (Vector2.up * WIDTH_BETWEEN_GRIPS * .5f) + (Vector2.right * WIDTH_BETWEEN_GRIPS * .5f);
                case Orientation.Direction.DownRight:
                    return Center + (Vector2.down * WIDTH_BETWEEN_GRIPS * .5f) + (Vector2.right * WIDTH_BETWEEN_GRIPS * .5f);
                case Orientation.Direction.DownLeft:
                    return Center + (Vector2.down * WIDTH_BETWEEN_GRIPS * .5f) + (Vector2.left * WIDTH_BETWEEN_GRIPS * .5f);
                case Orientation.Direction.UpLeft:
                    return Center + (Vector2.up * WIDTH_BETWEEN_GRIPS * .5f) + (Vector2.left * WIDTH_BETWEEN_GRIPS * .5f);
                default:
                    Debug.LogWarning("Grips are only found in ordinal directions (NW, SW, NE, SE)");
                    return Vector2.zero;
            }
        }

        public void DebugSquare()
        {
            float sideLength = 0.1f;
            float visibleTime = 1f;
            if (upLeft != null)
            {
                SuperDebugger.DrawBoxAtPoint(upLeft.transform.position, sideLength, Color.yellow, visibleTime);
            }
            if (upRight != null)
            {
                SuperDebugger.DrawBoxAtPoint(upRight.transform.position, sideLength, Color.magenta,  visibleTime);
            }
            if (downLeft != null)
            {
                SuperDebugger.DrawBoxAtPoint(downLeft.transform.position, sideLength, Color.cyan, visibleTime);
            }
            if (downRight != null)
            {
                SuperDebugger.DrawBoxAtPoint(downRight.transform.position, sideLength, Color.white, visibleTime);
            }
        }

        /// <summary>
        /// Creates a Grip.Square with the given grips
        /// </summary>
        /// <param name="upLeft"></param>
        /// <param name="upRight"></param>
        /// <param name="lowLeft"></param>
        /// <param name="lowRight"></param>
        public Square(Grip upLeft, Grip upRight, Grip lowLeft, Grip lowRight)
        {
            upLeft = upLeft;
            upRight = upRight;
            downLeft = lowLeft;
            downRight = lowRight;
        }

        /// <summary>
        /// Creates a Grip.Square given a starting Grip and the directions the square continues in
        /// </summary>
        /// <param name="startGrip"></param>
        /// <param name="hDirection"></param>
        /// <param name="vDirection"></param>
        /// <param name="gripLayer"></param>
        public Square(Grip startGrip, Vector2 hDirection, Vector2 vDirection, LayerMask gripLayer)
        {
            upLeft = null;
            upRight = null;
            downLeft = null;
            downRight = null;

            hDirection.Normalize();
            vDirection.Normalize();
           
            if (hDirection != Vector2.left && hDirection != Vector2.right)
            {
                Debug.LogWarning("hDirection must be either left or right");
                return;
            }
            if (vDirection != Vector2.up && vDirection != Vector2.down)
            {
                Debug.LogWarning("vDirection must be either down or up");
                return;
            }

            Vector2 hOffset = hDirection * Grip.WIDTH_BETWEEN_GRIPS;
            Vector2 vOffset = vDirection * Grip.WIDTH_BETWEEN_GRIPS;

            Vector2 horizontalGripLocation = (Vector2)startGrip.transform.position + hOffset;
            Vector2 verticalGripLocation = (Vector2)startGrip.transform.position + vOffset;
            Vector2 diagonalGripLocation = (Vector2)startGrip.transform.position + hOffset + vOffset;

            Vector2 colliderSizeOffset = new Vector2(Grip.HALF_GRIP_WIDTH, Grip.HALF_GRIP_WIDTH);

            Collider2D hCollider = Physics2D.OverlapArea(horizontalGripLocation + colliderSizeOffset, horizontalGripLocation - colliderSizeOffset, gripLayer);
            Collider2D vCollider = Physics2D.OverlapArea(verticalGripLocation + colliderSizeOffset, verticalGripLocation - colliderSizeOffset, gripLayer);
            Collider2D dCollider = Physics2D.OverlapArea(diagonalGripLocation + colliderSizeOffset, diagonalGripLocation - colliderSizeOffset, gripLayer);

            //Starting grip is on the right
            if (horizontalGripLocation.x < startGrip.transform.position.x)
            {
                //Starting grip is on the top
                if (verticalGripLocation.y < startGrip.transform.position.y)
                {
                    upRight = startGrip;
                    if (hCollider != null)
                    {
                        upLeft = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        downRight = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        downLeft = dCollider.GetComponent<Grip>();
                    }
                }
                //Starting grip is on the bottom
                else
                {
                    downRight = startGrip;
                    if (hCollider != null)
                    {
                        downLeft = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        upRight = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        upLeft = dCollider.GetComponent<Grip>();
                    }
                }
            }
            //Starting grip is on the left
            else
            {
                //Starting grip is on the top
                if (verticalGripLocation.y < startGrip.transform.position.y)
                {
                    upLeft = startGrip;
                    if (hCollider != null)
                    {
                        upRight = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        downLeft = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        downRight = dCollider.GetComponent<Grip>();
                    }
                }
                //Starting grip is on the bottom
                else
                {
                    downLeft = startGrip;
                    if (hCollider != null)
                    {
                        downRight = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        upLeft = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        upRight = dCollider.GetComponent<Grip>();
                    }
                }
            }
        }


        /// <summary>
        /// Creates a Grip.Square if given two grips at the 'bottom' of the square, with the bottom being the first grips encountered 
        /// when travelling along the movingDirection
        /// </summary>
        /// <param name="grip1"></param>
        /// <param name="grip2"></param>
        public Square(Grip grip1, Grip grip2, Vector2 movingDirection, LayerMask gripLayer)
        {
            movingDirection.Normalize();
            if (movingDirection != Vector2.left && movingDirection != Vector2.right && movingDirection != Vector2.up && movingDirection != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                upLeft = null;
                upRight = null;
                downLeft = null;
                downRight = null;
                return;
            }
            if (!grip1.IsInSameSquareAs(grip2))
            {
                Debug.LogWarning("Grips given for constructor are not in same square");
                upLeft = null;
                upRight = null;
                downLeft = null;
                downRight = null;
                return;
            }

            bool horizontallyInline = grip1.IsHorizontallyInLineWith(grip2);
            bool verticallyInline = grip1.IsVerticallyInLineWith(grip2);
            Grip topMost = null;
            Grip rightMost = null;

            if (!horizontallyInline)
            {
                topMost = (grip1.transform.position.y > grip2.transform.position.y) ? grip1 : grip2;
            }
            if (!verticallyInline)
            {
                rightMost = (grip1.transform.position.x > grip2.transform.position.x) ? grip1 : grip2;
            }

            if(horizontallyInline)
            {
                if(movingDirection == Vector2.up)
                {
                    downRight = rightMost;
                    downLeft = (grip1 == rightMost) ? grip2 : grip1;
                    upRight = FindAdjacentTo(downRight, Vector2.up, gripLayer);
                    upLeft = FindAdjacentTo(downLeft, Vector2.up, gripLayer);
                }
                else
                {
                    upRight = rightMost;
                    upLeft = (grip1 == rightMost) ? grip2 : grip1;
                    downRight = FindAdjacentTo(upRight, Vector2.down, gripLayer);
                    downLeft = FindAdjacentTo(upLeft, Vector2.down, gripLayer);
                }
            }
            else if(verticallyInline)
            {
                if(movingDirection == Vector2.right)
                {
                    upLeft = topMost;
                    downLeft = (grip1 == topMost) ? grip2 : grip1;
                    upRight = FindAdjacentTo(upLeft, Vector2.right, gripLayer);
                    downRight = FindAdjacentTo(downLeft, Vector2.right, gripLayer);
                }
                else
                {
                    upRight = topMost;
                    downRight = (grip1 == topMost) ? grip2 : grip1;
                    upLeft = FindAdjacentTo(upRight, Vector2.left, gripLayer);
                    downLeft = FindAdjacentTo(downRight, Vector2.left, gripLayer);
                }
            }
            //Diagonal
            else
            {
                if(topMost == rightMost)
                {
                    upRight = topMost;
                    downLeft = (grip1 == topMost) ? grip2 : grip1;
                    upLeft = FindAdjacentTo(upRight, Vector2.left, gripLayer);
                    downRight = FindAdjacentTo(downLeft, Vector2.right, gripLayer);
                }
                else
                {
                    upLeft = topMost;
                    downRight = (grip1 == topMost) ? grip2 : grip1;
                    upRight = FindAdjacentTo(upLeft, Vector2.right, gripLayer);
                    downLeft = FindAdjacentTo(downRight, Vector2.left, gripLayer);
                }
            }
        }

        /// <summary>
        /// Returns the directly adjacent square, including half of the current square. 
        /// Returns an empty square if grip count requirements not met.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="gripLayer"></param>
        /// <param name="minimumGripCount"></param>
        /// <returns></returns>
        public Square FindAdjacentSquareInDirection(Vector2 direction, LayerMask gripLayer, int minimumGripCount)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return new Square();
            }

            Square foundSquare = new Square();

            if (direction == Vector2.up)
            {
                if (upLeft != null)
                {
                    foundSquare = new Square(upLeft, Vector2.right, Vector2.up, gripLayer);
                }
                else if (upRight != null)
                {
                    foundSquare = new Square(upRight, Vector2.left, Vector2.up, gripLayer);
                }
            }
            else if (direction == Vector2.right)
            {
                if (upRight != null)
                {
                    foundSquare = new Square(upRight, Vector2.right, Vector2.down, gripLayer);
                }
                else if (downRight != null)
                {
                    foundSquare = new Square(downRight, Vector2.right, Vector2.up, gripLayer);
                }
            }
            else if (direction == Vector2.down)
            {
                if (downRight != null)
                {
                    foundSquare = new Square(downRight, Vector2.left, Vector2.down, gripLayer);
                }
                else if (downLeft != null)
                {
                    foundSquare = new Square(downLeft, Vector2.right, Vector2.down, gripLayer);
                }
            }
            else
            {
                if (downLeft != null)
                {
                    foundSquare = new Square(downLeft, Vector2.left, Vector2.up, gripLayer);
                }
                else if (upLeft != null)
                {
                    foundSquare = new Square(upLeft, Vector2.left, Vector2.down, gripLayer);
                }
            }

            return foundSquare.GripCount >= minimumGripCount ? foundSquare : new Square();
        }

        public Grip FindLeftSelectingSquareGivenDirection(Vector2 direction)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return null;
            }

            if(direction == Vector2.up)
            {
                return upLeft;
            }
            else if(direction == Vector2.right)
            {
                return upRight;
            }
            else if(direction == Vector2.down)
            {
                return downLeft;
            }
            else
            {
                return downLeft;
            }
        }

        public Vector2 FindLeftSelectingPointGivenDirection(Vector2 direction)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return Vector2.zero;
            }

            if (direction == Vector2.up)
            {
                return GetPositionOfGrip(Orientation.Direction.UpLeft);
            }
            else if (direction == Vector2.right)
            {
                return GetPositionOfGrip(Orientation.Direction.UpRight);
            }
            else if (direction == Vector2.down)
            {
                return GetPositionOfGrip(Orientation.Direction.DownLeft);
            }
            else
            {
                return GetPositionOfGrip(Orientation.Direction.DownLeft);
            }
        }


        public Grip FindRightSelectingSquareGivenDirection(Vector2 direction)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return null;
            }

            if (direction == Vector2.up)
            {
                return upRight;
            }
            else if (direction == Vector2.right)
            {
                return downRight;
            }
            else if (direction == Vector2.down)
            {
                return downRight;
            }
            else
            {
                return upLeft;
            }
        }

        public Vector2 FindRightSelectingPointGivenDirection(Vector2 direction)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return Vector2.zero;
            }

            if (direction == Vector2.up)
            {
                return GetPositionOfGrip(Orientation.Direction.UpRight);
            }
            else if (direction == Vector2.right)
            {
                return GetPositionOfGrip(Orientation.Direction.DownRight);
            }
            else if (direction == Vector2.down)
            {
                return GetPositionOfGrip(Orientation.Direction.DownRight);
            }
            else
            {
                return GetPositionOfGrip(Orientation.Direction.UpLeft);
            }
        }



        public Square FindFirstSquareInDirection(Vector2 direction, LayerMask gripLayer, int spacesToCheck, int minimumGrips)
        {
            int spacesBeforeLeftFound = 0;
            int spacesBeforeRightFound = 0;

            Vector2 leftStart = FindLeftSelectingPointGivenDirection(direction);
            Vector2 rightStart = FindRightSelectingPointGivenDirection(direction);

            Grip leftFound = FindInDirection(leftStart, direction, gripLayer, spacesToCheck, out spacesBeforeLeftFound);
            Grip rightFound = FindInDirection(rightStart, direction, gripLayer, spacesToCheck, out spacesBeforeRightFound);

            if (leftFound)
            {
                SuperDebugger.DrawX(leftFound.transform.position, HALF_GRIP_WIDTH, Color.red, 1f);
            }
            if(rightFound)
            {
                SuperDebugger.DrawX(rightFound.transform.position, HALF_GRIP_WIDTH, Color.green, 1f);
            }

            return CheckHandholdsAndIterate(direction, leftFound, rightFound, gripLayer, spacesToCheck, spacesBeforeLeftFound, spacesBeforeRightFound, minimumGrips);
        }

        private Square CheckHandholdsAndIterate(Vector2 direction, Grip leftFound, Grip rightFound, LayerMask gripLayer, int spacesToCheck, int totalLeftSpacesSearched, int totalRightSpacesSearched, int minimumGripsInSquare)
        {
            //print("--------------------");
            //print("Spaces before left found: " + totalLeftSpacesSearched);
            //print("Spaces before right found: " + totalRightSpacesSearched);

            if (totalRightSpacesSearched > spacesToCheck || totalLeftSpacesSearched > spacesToCheck)
            {
                print("RETURNING: Grips not found within " + (spacesToCheck) + " spaces.");
                return new Square();
            }

            if (leftFound && rightFound)
            {
                if (leftFound.IsInSameSquareAs(rightFound) && new Square(leftFound, rightFound, direction, gripLayer).GripCount >= minimumGripsInSquare)
                {
                    //TODO: Currently returns any two grips in a square; need to specify what types of squares can be connected to.
                    return new Square(leftFound, rightFound, direction, gripLayer);
                }
                else
                {
                    //left closest; iterate from left
                    if (Vector2.Distance(leftFound.transform.position, Center) < Vector2.Distance(rightFound.transform.position, Center))
                    {
                        int leftSpacesSearchedThisIteration;
                        int leftSpacesLeftToCheck = spacesToCheck + 1 - totalLeftSpacesSearched;

                        leftFound = FindInDirection(leftFound, direction, gripLayer, leftSpacesLeftToCheck, out leftSpacesSearchedThisIteration);
                        totalLeftSpacesSearched += leftSpacesSearchedThisIteration;

                        /*
                        //Debugging Code
                        print("Left grip is closest to start; iterating on left.");
                        print("Left spaces left to check: " + leftSpacesLeftToCheck);
                        print("Total left spaces searched CHANGED: " + totalLeftSpacesSearched);
                        */

                        if (leftFound)
                        {
                            SuperDebugger.DrawPlus(leftFound.transform.position, Color.red, HALF_GRIP_WIDTH, .5f);
                        }

                        return CheckHandholdsAndIterate(direction, leftFound, rightFound, gripLayer, spacesToCheck, totalLeftSpacesSearched, totalRightSpacesSearched, minimumGripsInSquare);
                    }
                    //right closest; iterate from right
                    else
                    {
                        int rightSpacesSearchedThisIteration;
                        int rightSpacesLeftToCheck = spacesToCheck + 1 - totalRightSpacesSearched;

                        rightFound = FindInDirection(rightFound, direction, gripLayer, rightSpacesLeftToCheck, out rightSpacesSearchedThisIteration);
                        totalRightSpacesSearched += rightSpacesSearchedThisIteration;

                        /*
                        //Debugging code
                        print("Right grip is closest to start; iterating on right");
                        print("Right spaces left to check: " + rightSpacesLeftToCheck);
                        print("Total right spaces searched CHANGED: " + totalRightSpacesSearched);
                        */

                        if (rightFound)
                        {
                            SuperDebugger.DrawPlus(rightFound.transform.position, Color.green, HALF_GRIP_WIDTH, .5f);
                        }

                        return CheckHandholdsAndIterate(direction, leftFound, rightFound, gripLayer, spacesToCheck, totalLeftSpacesSearched, totalRightSpacesSearched, minimumGripsInSquare);
                    }
                }
            }
            else
            {
                //No grip found in specified bounds.
                if(leftFound == null)
                {
                    print("RETURNING: Left grip not found within " + (spacesToCheck) + " spaces.");
                }
                else
                {
                    print("RETURNING: Right grip not found within " + (spacesToCheck) + " spaces.");
                }
                return new Square();
            }

        }

        public Square()
        {
            upRight = null;
            downRight = null;
            downLeft = null;
            upLeft = null;
        }
    }
}
