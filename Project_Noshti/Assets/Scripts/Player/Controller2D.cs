using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider2D))]
public class Controller2D : RaycastController 
{
    private float maxClimbingAngle = 80.0f;
    private float maxDescendAngle = 75.0f;
    private float minVelocityToBeConsideredMoving = 0.001f;

    private Vector2 actualVelocity;
    public Vector2 ActualVelocity
    {
        get
        {
            return actualVelocity;
        }
    }


    public CollisionInfo collisions;
    [HideInInspector]public Vector2 playerInput;

    public int FacingDirection
    {
        get
        {
            return collisions.facingDirection;
        }
    }

    public bool IsMovingHorizontally
    {
        get
        {
            return Mathf.Abs(actualVelocity.x) > minVelocityToBeConsideredMoving;
        }
    }

    public bool IsMovingVertically
    {
        get
        {
            return Mathf.Abs(actualVelocity.y) > minVelocityToBeConsideredMoving;
        }
    }

    public bool IsGrounded
    {
        get
        {
            return collisions.below;
        }
    }

    public override void Start()
    {
        base.Start();
        collisions.facingDirection = 1;
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        Move(velocity, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
    {
        collisions.Reset();
        UpdateRaycastOrigins();

        collisions.velocityOld = velocity;
        playerInput = input;

        if (velocity.x != 0)
        {
            collisions.facingDirection = (int)Mathf.Sign(velocity.x);
        }

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        actualVelocity = velocity;
        transform.Translate(velocity);

        if (standingOnPlatform == true)
        {
            collisions.below = true;
        }
    }

    private void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.facingDirection;
        float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

        if (Mathf.Abs(velocity.x) < SkinWidth)
        {
            rayLength = 2 * SkinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);


            if (hit)
            {
                //skip this raycasthit in order to not get stuck inside boxes
                if (hit.distance == 0)
                {
                    continue;
                }
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbingAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbingAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    //Fixing horizontal collision jittering while on slope
                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }

                
            }
        }
    }

    private void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 || hit.distance == 0 )
                    {
                        continue;
                    }
                    if (collisions.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 1f);
                        continue;
                    }
                }
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1 ? true : false;
                collisions.above = directionY == 1 ? true : false;
            }
        }

        //Deals with getting stuck when changing from one slope grade to another
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            //if it's a slope that we can walk on
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                //if the player's moving down the slope
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    //if it's far enough above the slope that it hasn't connected yet
                    if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    private void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);


    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    private void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public int facingDirection;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            climbingSlope = false;
            descendingSlope = false;
            above = below = false;
            left = right = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
