using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingController : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    private DirectionalContainer.Cardinal<ProximalSquare> potentialMovements; 

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;

    [SerializeField] private LayerMask gripLayer;

    private Grip.Square currentConnectedSquare;

    private bool isLeaning = false;
    private Vector2 leaningDirection;
    public Vector2 LeaningDirection
    {
        get
        {
            return leaningDirection;
        }
    }

    public bool ConnectIfPossible(bool rightHandIsConnecting)
    {
        if(rightHandIsConnecting)
        {
            Grip rightHand = GripChecker.CheckAreaForGrip(rightHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(rightHand != null)
            {
                Grip.Square foundSquare = GripChecker.PopulateGripSquare(rightHand, Vector2.left, Vector2.down, gripLayer);
                if (foundSquare.GripCount >= 2)
                {
                    MoveToSquare(foundSquare);

                    return true;
                }
            }
        }
        else
        {
            Grip leftHand = GripChecker.CheckAreaForGrip(leftHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(leftHand != null)
            {
                Grip.Square foundSquare = GripChecker.PopulateGripSquare(leftHand, Vector2.right, Vector2.down, gripLayer);
                if(foundSquare.GripCount >= 2)
                {
                    MoveToSquare(foundSquare);
                    return true;
                }
            }
        }

        return false;
    }

    private void Update()
    {
        if (!isSetUp || !isControllingMovement)
        {
            return;
        }

        UpdateLeaningStatus();

        if(isLeaning && playerActions.GripLeft.WasPressed)
        {
            MoveInLeaningDirectionIfPossible(false);
        }
        else if(isLeaning && playerActions.GripRight.WasPressed)
        {
            MoveInLeaningDirectionIfPossible(true);
        }
    }

    private void FindPotentialMovements(Grip.Square currentSquare)
    {

    }

    private void UpdateLeaningStatus()
    {
        Vector2 cardinalInput = new Vector2(playerActions.Move.X, playerActions.Move.Y).ClosestCardinalDirection();
        cardinalInput.Normalize();

        if (cardinalInput == Vector2.up)
        {
            isLeaning = true;
            leaningDirection = Vector2.up;
        }
        else if (cardinalInput == Vector2.right)
        {
            isLeaning = true;
            leaningDirection = Vector2.right;
        }
        else if (cardinalInput == Vector2.down)
        {
            isLeaning = true;
            leaningDirection = Vector2.down;
        }
        else if (cardinalInput == Vector2.left)
        {
            isLeaning = true;
            leaningDirection = Vector2.left;
        }
        else
        {
            isLeaning = false;
            leaningDirection = Vector2.zero;
        }
    }

    private void MoveInLeaningDirectionIfPossible(bool rightGripWasChosen)
    {
        Vector2 direction = leaningDirection;
        Grip.Square newGripSquare = GripChecker.FindConnectedGripSquareInDirection(currentConnectedSquare, direction, gripLayer);

        if (direction == Vector2.up)
        {
            //is there a handhold in the chosen direction?
            if (rightGripWasChosen)
            {
                if(newGripSquare.upperRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if (newGripSquare.upperLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.right)
        {
            //Edge case for movement back and forth across a single column of Grips
            if (newGripSquare.lowerLeft != null && newGripSquare.upperLeft != null && !newGripSquare.HasRightSide) 
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if (newGripSquare.lowerRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
                else if (newGripSquare.lowerLeft != null && newGripSquare.upperLeft != null && newGripSquare.upperRight == null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.upperRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.down)
        {
            //Edge case for movement down when there are no footholds
            if (newGripSquare.upperLeft != null && newGripSquare.upperRight != null && !newGripSquare.HasBottomSide)
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if(newGripSquare.lowerRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.lowerLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.left)
        {
            //Edge case for movement down when there are no footholds
            if (newGripSquare.upperRight != null && newGripSquare.lowerRight != null && !newGripSquare.HasLeftSide)
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if(newGripSquare.upperLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.lowerLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
    }

    private void MoveToSquare(Grip.Square newSquare)
    {
        newSquare.DebugSquare();
        transform.position = newSquare.Center;
        currentConnectedSquare = newSquare;
    }

    public override void LoseControl()
    {
        base.LoseControl();
        isLeaning = false;
        leaningDirection = Vector2.zero;
    }

    public class ProximalSquare
    {
        public Grip.Square square;
        public bool isJumpNecessary;
    }

}
