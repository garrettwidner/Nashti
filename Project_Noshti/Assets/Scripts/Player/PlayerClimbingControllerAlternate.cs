using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingControllerAlternate : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;

    [SerializeField] private LayerMask gripLayer;

    private Grip.Square currentConnectedSquare;

    private bool isLeaning = false;
    private Vector2 leaningDirection;

    public bool ConnectIfPossible(bool rightHandIsConnecting)
    {
        if(rightHandIsConnecting)
        {
            Grip rightHand = GripChecker.CheckProximity(rightHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
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
            Grip leftHand = GripChecker.CheckProximity(leftHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
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

        if(isLeaning && playerActions.WasPressedGripVector != Vector2.zero)
        {
            Grip.Square newGripSquare = GripChecker.FindGripSquareInDirection(currentConnectedSquare, leaningDirection, gripLayer);

            if (leaningDirection == Vector2.up)
            {
                if(playerActions.HorizontalAxisGripWasPressed)
                {
                    MoveUpIfPossible(newGripSquare);
                }
            }
            else if(leaningDirection == Vector2.right)
            {
                if(playerActions.VerticalAxisGripWasPressed)
                {
                    MoveRightIfPossible(newGripSquare);
                }
            }
            else if(leaningDirection == Vector2.down)
            {
                if (playerActions.HorizontalAxisGripWasPressed)
                {
                    MoveDownIfPossible(newGripSquare);
                }
            }
            else if (leaningDirection == Vector2.left)
            {
                if (playerActions.VerticalAxisGripWasPressed)
                {
                    MoveLeftIfPossible(newGripSquare);
                }
            }
        }
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

    private void MoveUpIfPossible(Grip.Square newGripSquare)
    {
        if(playerActions.GripLeft.WasPressed)
        {
            if(newGripSquare.upperLeft != null)
            {
                MoveToSquare(newGripSquare);
            }
        }
        else if(playerActions.GripRight.WasPressed)
        {
            if(newGripSquare.upperRight != null)
            {
                MoveToSquare(newGripSquare);
            }
        }
    }

    private void MoveRightIfPossible(Grip.Square newGripSquare)
    {
        if(playerActions.GripUp.WasPressed)
        {
            if(newGripSquare.upperRight != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if(newGripSquare.upperLeft != null && newGripSquare.lowerLeft != null && !newGripSquare.HasRightSide)
            {
                MoveToSquare(newGripSquare);
            }
        }
        else if(playerActions.GripDown.WasPressed)
        {
            if(newGripSquare.lowerRight != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if (newGripSquare.upperLeft != null && newGripSquare.lowerLeft != null && !newGripSquare.HasRightSide)
            {
                MoveToSquare(newGripSquare);
            }
        }
    }

    private void MoveDownIfPossible(Grip.Square newGripSquare)
    {
        if(playerActions.GripLeft.WasPressed)
        {
            if(newGripSquare.lowerLeft != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if(newGripSquare.upperLeft != null && newGripSquare.upperRight != null && !newGripSquare.HasBottomSide)
            {
                MoveToSquare(newGripSquare);
            }
        }
        else if(playerActions.GripRight.WasPressed)
        {
            if(newGripSquare.lowerRight != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if (newGripSquare.upperLeft != null && newGripSquare.upperRight != null && !newGripSquare.HasBottomSide)
            {
                MoveToSquare(newGripSquare);
            }
        }
    }

    private void MoveLeftIfPossible(Grip.Square newGripSquare)
    {
        if(playerActions.GripUp.WasPressed)
        {
            if(newGripSquare.upperLeft != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if(newGripSquare. upperRight != null && newGripSquare.lowerRight != null && !newGripSquare.HasLeftSide)
            {
                MoveToSquare(newGripSquare);
            }
        }
        else if(playerActions.GripDown.WasPressed)
        {
            if (newGripSquare.lowerLeft != null)
            {
                MoveToSquare(newGripSquare);
            }
            else if (newGripSquare.upperRight != null && newGripSquare.lowerRight != null && !newGripSquare.HasLeftSide)
            {
                MoveToSquare(newGripSquare);
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

}
