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

            if (!foundGrip.IsNull)
            {
                return foundGrip;
            }
        }

        return null;
    }

    /// <summary>
    /// Checkdistance is measured in number of concurrent spaces to check for grips. InitialSpacesToSkip is subtracted from the start of this.
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
        Grip foundGrip = null;
        spacesSearchedBeforeGripFound = 0;

        if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return foundGrip;
        }
            
        for(int i = 1 + initialSpacesToSkip; i < spacesToCheck; i++)
        {
           Vector2 checkPosition = (Vector2)startGrip.transform.position + (direction * WIDTH_BETWEEN_GRIPS * i);
           foundGrip = CheckLocationForGrip(checkPosition, HALF_GRIP_WIDTH, gripLayer);
            if (foundGrip != null && !foundGrip.IsNull)
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
        /// Creates a Grip.Square with the given grips
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
        /// Creates a Grip.Square given a starting Grip and the directions the square continues in
        /// </summary>
        /// <param name="startGrip"></param>
        /// <param name="hDirection"></param>
        /// <param name="vDirection"></param>
        /// <param name="gripLayer"></param>
        public Square(Grip startGrip, Vector2 hDirection, Vector2 vDirection, LayerMask gripLayer)
        {
            upperLeft = null;
            upperRight = null;
            lowerLeft = null;
            lowerRight = null;

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
                    upperRight = startGrip;
                    if (hCollider != null)
                    {
                        upperLeft = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        lowerRight = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        lowerLeft = dCollider.GetComponent<Grip>();
                    }
                }
                //Starting grip is on the bottom
                else
                {
                    lowerRight = startGrip;
                    if (hCollider != null)
                    {
                        lowerLeft = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        upperRight = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        upperLeft = dCollider.GetComponent<Grip>();
                    }
                }
            }
            //Starting grip is on the left
            else
            {
                //Starting grip is on the top
                if (verticalGripLocation.y < startGrip.transform.position.y)
                {
                    upperLeft = startGrip;
                    if (hCollider != null)
                    {
                        upperRight = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        lowerLeft = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        lowerRight = dCollider.GetComponent<Grip>();
                    }
                }
                //Starting grip is on the bottom
                else
                {
                    lowerLeft = startGrip;
                    if (hCollider != null)
                    {
                        lowerRight = hCollider.GetComponent<Grip>();
                    }
                    if (vCollider != null)
                    {
                        upperLeft = vCollider.GetComponent<Grip>();
                    }
                    if (dCollider != null)
                    {
                        upperRight = dCollider.GetComponent<Grip>();
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
                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
                return;
            }
            if (!grip1.IsInSameSquareAs(grip2))
            {
                Debug.LogWarning("Grips given for constructor are not in same square");
                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
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
                    lowerRight = rightMost;
                    lowerLeft = (grip1 == rightMost) ? grip2 : grip1;
                    upperRight = FindAdjacentTo(lowerRight, Vector2.up, gripLayer);
                    upperLeft = FindAdjacentTo(lowerLeft, Vector2.up, gripLayer);
                }
                else
                {
                    upperRight = rightMost;
                    upperLeft = (grip1 == rightMost) ? grip2 : grip1;
                    lowerRight = FindAdjacentTo(upperRight, Vector2.down, gripLayer);
                    lowerLeft = FindAdjacentTo(upperLeft, Vector2.down, gripLayer);
                }
            }
            else if(verticallyInline)
            {
                if(movingDirection == Vector2.right)
                {
                    upperLeft = topMost;
                    lowerLeft = (grip1 == topMost) ? grip2 : grip1;
                    upperRight = FindAdjacentTo(upperLeft, Vector2.right, gripLayer);
                    lowerRight = FindAdjacentTo(lowerLeft, Vector2.right, gripLayer);
                }
                else
                {
                    upperRight = topMost;
                    lowerRight = (grip1 == topMost) ? grip2 : grip1;
                    upperLeft = FindAdjacentTo(upperRight, Vector2.left, gripLayer);
                    lowerLeft = FindAdjacentTo(lowerRight, Vector2.left, gripLayer);
                }
            }
            //Diagonal
            else
            {
                if(topMost == rightMost)
                {
                    upperRight = topMost;
                    lowerLeft = (grip1 == topMost) ? grip2 : grip1;
                    upperLeft = FindAdjacentTo(upperRight, Vector2.left, gripLayer);
                    lowerRight = FindAdjacentTo(lowerLeft, Vector2.right, gripLayer);
                }
                else
                {
                    upperLeft = topMost;
                    lowerRight = (grip1 == topMost) ? grip2 : grip1;
                    upperRight = FindAdjacentTo(upperLeft, Vector2.right, gripLayer);
                    lowerLeft = FindAdjacentTo(lowerRight, Vector2.left, gripLayer);
                }
            }
        }

        public Square FindAdjacentSquareInDirection(Vector2 direction, LayerMask gripLayer)
        {
            direction.Normalize();
            if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
            {
                Debug.LogWarning("Given direction must be a cardinal direction");
                return new Square();
            }

            if (direction == Vector2.up)
            {
                if (upperLeft != null)
                {
                    return new Square(upperLeft, Vector2.right, Vector2.up, gripLayer);
                }
                else if (upperRight != null)
                {
                    return new Square(upperRight, Vector2.left, Vector2.up, gripLayer);
                }
            }
            else if (direction == Vector2.right)
            {
                if (upperRight != null)
                {
                    return new Square(upperRight, Vector2.right, Vector2.down, gripLayer);
                }
                else if (lowerRight != null)
                {
                    return new Square(lowerRight, Vector2.right, Vector2.up, gripLayer);
                }
            }
            else if (direction == Vector2.down)
            {
                if (lowerRight != null)
                {
                    return new Square(lowerRight, Vector2.left, Vector2.down, gripLayer);
                }
                else if (lowerLeft != null)
                {
                    return new Square(lowerLeft, Vector2.right, Vector2.down, gripLayer);
                }
            }
            else if (direction == Vector2.left)
            {
                if (lowerLeft != null)
                {
                    return new Square(lowerLeft, Vector2.left, Vector2.up, gripLayer);
                }
                else if (upperLeft != null)
                {
                    return new Square(upperLeft, Vector2.left, Vector2.down, gripLayer);
                }
            }

            return new Square();
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
                return upperLeft;
            }
            else if(direction == Vector2.right)
            {
                return upperRight;
            }
            else if(direction == Vector2.down)
            {
                return lowerLeft;
            }
            else
            {
                return lowerLeft;
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
                return upperRight;
            }
            else if (direction == Vector2.right)
            {
                return lowerRight;
            }
            else if (direction == Vector2.down)
            {
                return lowerRight;
            }
            else
            {
                return upperLeft;
            }

        }

        
        public Square FindFirstSquareInDirection(Vector2 direction, LayerMask gripLayer, int spacesToCheck, int minimumGripsInFoundSquare)
        {

            Grip leftStart = FindLeftSelectingSquareGivenDirection(direction);
            Grip rightStart = FindRightSelectingSquareGivenDirection(direction);

            int spacesBeforeLeftFound;
            int spacesBeforeRightFound;

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

            return CheckHandholdsAndIterate(direction, leftFound, rightFound, gripLayer, spacesToCheck, spacesBeforeLeftFound, spacesBeforeRightFound, minimumGripsInFoundSquare);
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
                        if (leftFound)
                        {
                            SuperDebugger.DrawPlus(leftFound.transform.position, Color.red, HALF_GRIP_WIDTH, .5f);
                        }
                        */

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
                        if (rightFound)
                        {
                            SuperDebugger.DrawPlus(rightFound.transform.position, Color.green, HALF_GRIP_WIDTH, .5f);
                        }
                        */

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

        int numberCalled = 0;
        int maxNumberOfCalls = 30;

        /*
        private Square IterateThroughSquaresInDirection(Vector2 direction, LayerMask gripLayer, int spacesToCheck, Grip leftStart, Grip rightStart, ref int spacesBeforeLeftFound, ref int spacesBeforeRightFound)
        {
            Grip leftFound = FindInDirection(leftStart, direction, gripLayer, spacesToCheck, out spacesBeforeLeftFound);
            Grip rightFound = FindInDirection(rightStart, direction, gripLayer, spacesToCheck, out spacesBeforeRightFound);

            numberCalled++;
            print("IterateThroughSquares called " + numberCalled + " times.");
            if(numberCalled > maxNumberOfCalls)
            {
                numberCalled = 0;
                return new Square();
            }

            if (leftFound != null && rightFound != null && !leftFound.IsNull && !rightFound.IsNull)
            {
                if (leftFound.IsInSameSquareAs(rightFound))
                {
                    return new Square(leftFound, rightFound, direction, gripLayer);
                }
                //Grips are not in same square; continue iterating 
                //Left found first, iterate along left and continue checking
                else if (spacesBeforeLeftFound < spacesBeforeRightFound)
                {
                    int spacesToSkip = spacesBeforeLeftFound;
                    leftFound = FindInDirection(leftStart, direction, gripLayer, spacesToCheck, out spacesBeforeLeftFound, spacesToSkip);
                    if(leftFound != null)
                    {
                        SuperDebugger.DrawBoxAtPoint(leftFound.transform.position, HALF_GRIP_WIDTH, Color.white, 1.2f);
                        return IterateThroughSquaresInDirection(direction, gripLayer, spacesToCheck, leftStart, rightStart, ref spacesBeforeLeftFound, ref spacesBeforeRightFound);
                    }
                    else
                    {
                        return new Square();
                    }
                }
                //Right found first, iterate along right and continue checking
                else
                {
                    int spacesToSkip = spacesBeforeRightFound;
                    rightFound = FindInDirection(rightStart, direction, gripLayer, spacesToCheck, out spacesBeforeRightFound, spacesToSkip);
                    if(rightFound != null)
                    {
                        SuperDebugger.DrawBoxAtPoint(rightFound.transform.position, HALF_GRIP_WIDTH, Color.white, 1.2f);
                        return IterateThroughSquaresInDirection(direction, gripLayer, spacesToCheck, leftStart, rightStart, ref spacesBeforeLeftFound, ref spacesBeforeRightFound);
                    }
                    else
                    {
                        return new Square();
                    }
                }
            }
            else
            {
                //Can't connect with less than 3; return null square.
                return new Square();
            }
        }

    */

        public Square()
        {
            upperLeft = null;
            upperRight = null;
            lowerLeft = null;
            lowerRight = null;
        }
    }
}
