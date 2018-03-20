using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingController : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;

    [SerializeField] private LayerMask gripLayer;

    private Grip.Square currentConnectedSquare;

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

        if (playerActions.Up.WasPressed)
        {
            MoveInDirection(Vector2.up);
        }
        else if (playerActions.Right.WasPressed)
        {
            MoveInDirection(Vector2.right);
        }
        else if (playerActions.Down.WasPressed)
        {
            MoveInDirection(Vector2.down);
        }
        else if (playerActions.Left.WasPressed)
        {
            MoveInDirection(Vector2.left);
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        if(direction == Vector2.up)
        {
            //Technically this must be true for upwards direction- remove this in later version?
            if(currentConnectedSquare.HasUpperSide)
            {
                Grip.Square newGripSquare = GripChecker.FindGripSquareInDirection(currentConnectedSquare, direction, gripLayer);
                if(newGripSquare.HasUpperSide)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.right)
        {
            if(currentConnectedSquare.HasRightSide)
            {
                Grip.Square newGripSquare = GripChecker.FindGripSquareInDirection(currentConnectedSquare, direction, gripLayer);
                if(newGripSquare.HasRightSide)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.down)
        {
            if (currentConnectedSquare.HasLowerSide)
            {
                Grip.Square newGripSquare = GripChecker.FindGripSquareInDirection(currentConnectedSquare, direction, gripLayer);
                if (newGripSquare.HasLowerSide)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.left)
        {
            if (currentConnectedSquare.HasLeftSide)
            {
                Grip.Square newGripSquare = GripChecker.FindGripSquareInDirection(currentConnectedSquare, direction, gripLayer);
                if (newGripSquare.HasLeftSide)
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

}
