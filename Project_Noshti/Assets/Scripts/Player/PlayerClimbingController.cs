﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingController : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    private CardinalContainer<Movement> potentialMovements;
    public CardinalContainer<Movement> GetPotentialMovements
    {
        get
        {
            return potentialMovements;
        }
    }

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private int jumpDistance = 5;

    private bool isConnectingAfterJump = false;

    [SerializeField] private LayerMask gripLayer;
    [SerializeField] private bool showDebug = true;

    private int minimumGripsForJumpSquare = 3;

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

    private void Awake()
    {
        potentialMovements = new CardinalContainer<Movement>();
        ResetPotentialMovements();
    }
    private void ResetPotentialMovements()
    {
        potentialMovements.up = null;
        potentialMovements.right = null;
        potentialMovements.down = null;
        potentialMovements.left = null;
    }

    public bool ConnectAtHandsIfPossible(bool rightHandIsConnecting)
    {
        isConnectingAfterJump = true;

        if (rightHandIsConnecting)
        {
            Grip rightHand = Grip.CheckLocationForGrip(rightHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(rightHand != null)
            {
                Grip.Square foundSquare = new Grip.Square(rightHand, Vector2.left, Vector2.down, gripLayer);
                if (foundSquare.GripCount >= 2)
                {
                    MoveToSquare(foundSquare);

                    return true;
                }
            }
        }
        else
        {
            Grip leftHand = Grip.CheckLocationForGrip(leftHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(leftHand != null)
            {
                Grip.Square foundSquare = new Grip.Square(leftHand, Vector2.right, Vector2.down, gripLayer);
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

        if(isLeaning && (playerActions.GripLeft.WasReleased || playerActions.GripRight.WasReleased))
        {
            if (isConnectingAfterJump)
            {
                isConnectingAfterJump = false;
                return;
            }
            else if (playerActions.GripLeft.WasReleased)
            {
                MoveInLeaningDirectionIfPossible(false);
            }
            else if(playerActions.GripRight.WasReleased)
            {
                MoveInLeaningDirectionIfPossible(true);
            }
        }

        /*
        else if(isLeaning && playerActions.Jump.WasReleased)
        {
            Movement potentialMovement = potentialMovements.Vector2ToObject(leaningDirection);
            if (potentialMovement.foundSquare && potentialMovement.isJumpNecessary)
            {
                MoveToSquare(potentialMovement.square);
            }
        }
        */
    }

    private void FindPotentialMovements()
    {
        //print("FindPotentialMovements called");
        ResetPotentialMovements();

        potentialMovements.up = FindProximalSquareInDirection(Vector2.up);
        potentialMovements.right = FindProximalSquareInDirection(Vector2.right);
        potentialMovements.down = FindProximalSquareInDirection(Vector2.down);
        potentialMovements.left = FindProximalSquareInDirection(Vector2.left);

        if(showDebug)
        {
            //print("------------");
            
            if (potentialMovements.up.foundSquare)
            {
                //print("Up square debugged at point: " + potentialMovements.up.square.Center);
                potentialMovements.up.square.DebugSquare();
                //SuperDebugger.DrawBoxAtPoint(potentialMovements.up.square.Center, Grip.WIDTH_BETWEEN_GRIPS, Color.green, 2f);
                //SuperDebugger.DrawPlus(potentialMovements.up.square.Center, Grip.WIDTH_BETWEEN_GRIPS, Color.green, 2f);

            }

            if (potentialMovements.right.foundSquare)
            {
                //print("Right square debugged");
                potentialMovements.right.square.DebugSquare();
            }
            if (potentialMovements.down.foundSquare)
            {
                //print("Down square debugged");
                potentialMovements.down.square.DebugSquare();
            }
            if (potentialMovements.left.foundSquare)
            {
                //print("Left square debugged");
                potentialMovements.left.square.DebugSquare();
            }
            
        }
    }

    private Movement FindProximalSquareInDirection(Vector2 direction)
    {
        Movement pSquare = new Movement();
        pSquare.isJumpNecessary = false;
        pSquare.foundSquare = false;

        pSquare.square = currentConnectedSquare.FindAdjacentSquareInDirection(direction, gripLayer, 2);

        if (pSquare.square.IsEmpty || pSquare.square.SideIsEmpty(direction)) 
        {
            pSquare.square = currentConnectedSquare.FindFirstSquareInDirection(direction, gripLayer, jumpDistance, minimumGripsForJumpSquare);
            if(!pSquare.square.IsEmpty)
            {
                pSquare.isJumpNecessary = true;
                pSquare.foundSquare = true;
            }
        }
        else
        {
            pSquare.foundSquare = true;
        }
        return pSquare;
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
        Movement nextMovement = potentialMovements.Vector2ToObject(leaningDirection);
        if(nextMovement.foundSquare)
        {
            Grip selectedGrip;
            selectedGrip = rightGripWasChosen ? nextMovement.square.FindRightSelectingGripGivenDirection(direction)
                                                  : nextMovement.square.FindLeftSelectingGripGivenDirection(direction);
            if (selectedGrip != null && !selectedGrip.IsEmpty)
            {
                MoveToSquare(nextMovement.square);
            }
        }
    }

    /*
    private void MoveInLeaningDirectionIfPossible(bool rightGripWasChosen)
    {
        Vector2 direction = leaningDirection;
        Grip.Square newGripSquare = currentConnectedSquare.FindAdjacentSquareInDirection(leaningDirection, gripLayer, 2);

        if (direction == Vector2.up)
        {
            //is there a handhold in the chosen direction?
            if (rightGripWasChosen)
            {
                if(newGripSquare.upRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if (newGripSquare.upLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.right)
        {
            //Edge case for movement back and forth across a single column of Grips
            if (newGripSquare.downLeft != null && newGripSquare.upLeft != null && !newGripSquare.HasGripOnRightSide) 
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if (newGripSquare.downRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
                else if (newGripSquare.downLeft != null && newGripSquare.upLeft != null && newGripSquare.upRight == null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.upRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.down)
        {
            //Edge case for movement down when there are no footholds
            if (newGripSquare.upLeft != null && newGripSquare.upRight != null && !newGripSquare.HasGripOnBottomSide)
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if(newGripSquare.downRight != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.downLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
        else if(direction == Vector2.left)
        {
            //Edge case for movement down when there are no footholds
            if (newGripSquare.upRight != null && newGripSquare.downRight != null && !newGripSquare.HasGripOnLeftSide)
            {
                MoveToSquare(newGripSquare);
            }
            else if (rightGripWasChosen)
            {
                if(newGripSquare.upLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
            else
            {
                if(newGripSquare.downLeft != null)
                {
                    MoveToSquare(newGripSquare);
                }
            }
        }
    }
    */

    private void MoveToSquare(Grip.Square newSquare)
    {
        //newSquare.DebugSquare();
        transform.position = newSquare.Center;
        currentConnectedSquare = newSquare;

        FindPotentialMovements();
    }

    public override void LoseMovementControl()
    {
        base.LoseMovementControl();
        isLeaning = false;
        leaningDirection = Vector2.zero;
    }

    public class Movement
    {
        public Grip.Square square;
        public bool isJumpNecessary;
        public bool foundSquare;
    }

}
