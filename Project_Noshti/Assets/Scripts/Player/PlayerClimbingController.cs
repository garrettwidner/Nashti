using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerClimbingController : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    /*
    private CardinalContainer<Movement> potentialMovements;
    public CardinalContainer<Movement> GetPotentialMovements
    {
        get
        {
            return potentialMovements;
        }
    }
    */

    private CardinalContainer<PotentialMove> potentialMoves;
    public CardinalContainer<PotentialMove> GetPotentialMoves
    {
        get
        {
            return potentialMoves;
        }
    }

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;
    [SerializeField] private bool canJump = true;
    [SerializeField] private int jumpDistance = 5;

    private bool isConnectingAfterJump = false;
    public bool IsConnectingAfterJump
    {
        get
        {
            return isConnectingAfterJump;
        }
    }

    [SerializeField] private LayerMask gripLayer;

    [SerializeField] private MovementEvent OnMovementStarted;
    [SerializeField] private MovementEvent OnMoveEnded;

    [SerializeField] private bool showDebug = true;

    private int minimumGripsForJumpSquare = 3;

    private bool isMoving = false;
    [SerializeField] private float climbMoveSpeed = 4.5f;
    private float t = 0.0f;
    private Vector2 lerpStart;
    private Vector2 lerpEnd;

    private Grip.Square currentConnectedSquare;
    //private Movement previousMovement;
    private Move previousMove;

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
        //potentialMovements = new CardinalContainer<Movement>();
        potentialMoves = new CardinalContainer<PotentialMove>();
        //ResetPotentialMovements();
        ResetPotentialMoves();
    }

    /*
    private void ResetPotentialMovements()
    {
        potentialMovements.up = null;
        potentialMovements.right = null;
        potentialMovements.down = null;
        potentialMovements.left = null;
    }
    */
    
    private void ResetPotentialMoves()
    {
        potentialMoves.up = null;
        potentialMoves.right = null;
        potentialMoves.down = null;
        potentialMoves.left = null;
    }

    public bool ConnectAtHandsIfPossible(bool rightHandIsConnecting)
    {
        //Makes it so that first move after jump doesn't happen
        //isConnectingAfterJump = true;

        if (rightHandIsConnecting)
        {
            Grip rightHand = Grip.CheckLocationForGrip(rightHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(rightHand != null)
            {
                Grip.Square foundSquare = new Grip.Square(rightHand, Vector2.left, Vector2.down, gripLayer);
                if (foundSquare.GripCount >= 2)
                {
                    MoveToSquareImmediate(foundSquare);
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
                    MoveToSquareImmediate(foundSquare);
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

        if(!isMoving)
        {
            UpdateLeaningStatus();

            if (playerActions.GripLeft.WasPressed || playerActions.GripRight.WasPressed)
            {
                if (isConnectingAfterJump)
                {
                    isConnectingAfterJump = false;
                    return;
                }
                else
                {
                    if (playerActions.GripLeft.WasPressed && isLeaning)
                    {
                        //MoveInLeaningDirectionIfPossible(false);
                        MakeMoveIfPossible(false);
                    }
                    else if (playerActions.GripRight.WasPressed && isLeaning)
                    {
                        //MoveInLeaningDirectionIfPossible(true);
                        MakeMoveIfPossible(true);
                    }
                }
            }
        }
        else
        {
            transform.position = Vector2.Lerp(lerpStart, lerpEnd, t);
            t += Time.deltaTime * climbMoveSpeed;

            if(t >= 1)
            {
                EndMovement();
            }
        }
        
    }

    /*
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
            
            if (potentialMovements.up.newSquareWasFound)
            {
                //print("Up square debugged at point: " + potentialMovements.up.square.Center);
                potentialMovements.up.newSquare.DebugSquare();
            }

            if (potentialMovements.right.newSquareWasFound)
            {
                //print("Right square debugged");
                potentialMovements.right.newSquare.DebugSquare();
            }
            if (potentialMovements.down.newSquareWasFound)
            {
                //print("Down square debugged");
                potentialMovements.down.newSquare.DebugSquare();
            }
            if (potentialMovements.left.newSquareWasFound)
            {
                //print("Left square debugged");
                potentialMovements.left.newSquare.DebugSquare();
            }
            
        }
    }
    */

    private void FindPotentialMoves()
    {
        potentialMoves.up = FindPotentialMoveInDirection(Vector2.up);
        potentialMoves.right = FindPotentialMoveInDirection(Vector2.right);
        potentialMoves.down = FindPotentialMoveInDirection(Vector2.down);
        potentialMoves.left = FindPotentialMoveInDirection(Vector2.left);
    }

    /*
    private Movement FindProximalSquareInDirection(Vector2 direction)
    {
        Movement pSquare = new Movement();
        pSquare.isJumpNecessary = false;
        pSquare.newSquareWasFound = false;

        pSquare.newSquare = currentConnectedSquare.FindAdjacentSquareInDirection(direction, gripLayer, 2);

        if (pSquare.newSquare.IsEmpty || pSquare.newSquare.SideIsEmpty(direction)) 
        {
            pSquare.newSquare = currentConnectedSquare.FindFirstSquareInDirection(direction, gripLayer, jumpDistance, minimumGripsForJumpSquare);
            if(!pSquare.newSquare.IsEmpty)
            {
                pSquare.isJumpNecessary = true;
                pSquare.newSquareWasFound = true;
            }
        }
        else
        {
            pSquare.newSquareWasFound = true;
        }
        return pSquare;
    }
    */

    private PotentialMove FindPotentialMoveInDirection(Vector2 direction)
    {
        Grip.Square newSquare = currentConnectedSquare.FindAdjacentSquareInDirection(direction, gripLayer, 2);
        bool isJumpNecessary = false;

        if(newSquare.IsEmpty || newSquare.SideIsEmpty(direction))
        {
            newSquare = currentConnectedSquare.FindFirstSquareInDirection(direction, gripLayer, jumpDistance, minimumGripsForJumpSquare);
            if(!newSquare.IsEmpty)
            {
                isJumpNecessary = true;
            }
        }

        return new PotentialMove(newSquare, isJumpNecessary, direction);
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

    /*
    private void MoveInLeaningDirectionIfPossible(bool rightGripWasChosen)
    {
        Vector2 direction = leaningDirection;
        Movement nextMovement = potentialMovements.Vector2ToObject(leaningDirection);
        if(nextMovement.newSquareWasFound)
        {
            Grip selectedGrip;
            selectedGrip = rightGripWasChosen ? nextMovement.newSquare.FindRightSelectingGripGivenDirection(direction)
                                                  : nextMovement.newSquare.FindLeftSelectingGripGivenDirection(direction);
            if (selectedGrip != null && !selectedGrip.IsEmpty)
            {
                if(OnMovementStarted != null)
                {
                    OnMovementStarted.Invoke(rightGripWasChosen, nextMovement.isJumpNecessary, leaningDirection, selectedGrip);
                }

                previousMovement = nextMovement;
                MoveToSquare(nextMovement.newSquare);
            }
        }
    }
    */

    private void MakeMoveIfPossible(bool rightGripWasChosen)
    {
        Vector2 direction = leaningDirection;
        Move nextMove = new Move(potentialMoves.Vector2ToObject(leaningDirection), !rightGripWasChosen);
        if(nextMove.NewSquareWasFound)
        {
            Grip selectedGrip = nextMove.ConnectingGrip;

            if(selectedGrip != null && !selectedGrip.IsEmpty)
            {
                if(OnMovementStarted != null)
                {
                    OnMovementStarted.Invoke(rightGripWasChosen, nextMove.IsJumpNecessary, leaningDirection, selectedGrip);
                }

                previousMove = nextMove;
                MoveToSquare(nextMove.NewSquare);
            }
        }
    }

    private void MoveToSquare(Grip.Square newSquare)
    {
        //print("Lerp to square");
        lerpStart = transform.position;
        lerpEnd = newSquare.Center;
        t = 0.0f;
        isMoving = true;

        currentConnectedSquare = newSquare;
    }

    private void EndMovement()
    {
        isMoving = false;
        //FindPotentialMovements();
        FindPotentialMoves();
        transform.position = lerpEnd;

        if(OnMoveEnded != null)
        {
            //OnMoveEnded()
        }
    }

    private void MoveToSquareImmediate(Grip.Square newSquare)
    {
        //print("Jump to square");
        //newSquare.DebugSquare();
        transform.position = newSquare.Center;
        currentConnectedSquare = newSquare;

        //FindPotentialMovements();
        FindPotentialMoves();
    }

    public override void LoseMovementControl()
    {
        base.LoseMovementControl();
        isLeaning = false;
        leaningDirection = Vector2.zero;
    }

    public class Movement
    {
        public Grip.Square newSquare;
        public bool isJumpNecessary;
        public bool newSquareWasFound;
        public bool connectedThroughLeftGrip;
        //The ideas in GripConnection below can be 
        //inferred from the information contained
        //within this class already. Create a set
        //of properties to access these instead of
        //adding the overhead necessary to 
        //provide them all.

        //newSquareWasFound can be inferred as well, so
        //only 3 of the pieces of data need to be provided.
    }

    public class PotentialMove
    {
        protected Grip.Square newSquare;
        protected bool isJumpNecessary;
        protected Vector2 movementDirection;

        public PotentialMove()
        {
            newSquare = new Grip.Square();
            isJumpNecessary = false;
            movementDirection = Vector2.zero;
        }

        public PotentialMove(Grip.Square nextSquare, bool jumpIsNecessary, Vector2 moveDirection)
        {
            newSquare = nextSquare;
            isJumpNecessary = jumpIsNecessary;
            movementDirection = moveDirection;
        }

        public Grip.Square NewSquare
        {
            get
            {
                return newSquare;
            }
        }

        public bool IsJumpNecessary
        {
            get
            {
                return isJumpNecessary;
            }
        }

        public bool NewSquareWasFound
        {
            get
            {
                return !newSquare.IsEmpty;
            }
        }

        public Vector2 MovementDirection
        {
            get
            {
                return movementDirection;
            }
        }
    }

    public class Move : PotentialMove
    {
        protected bool connectedThroughLeftGrip;

        public Move(PotentialMove move, bool leftGripUsed) 
        {
            newSquare = move.NewSquare;
            isJumpNecessary = move.IsJumpNecessary;
            connectedThroughLeftGrip = leftGripUsed;
            movementDirection = move.MovementDirection;
        }

        public Move(Grip.Square nextSquare, bool jumpIsNecessary, Vector2 moveDirection, bool leftGripUsed) : base(nextSquare, jumpIsNecessary, moveDirection)
        {
            connectedThroughLeftGrip = leftGripUsed;
        }
        
        public Grip ConnectingGrip
        {
            get
            {
                if (connectedThroughLeftGrip)
                {
                    return newSquare.FindLeftSelectingGripGivenDirection(MovementDirection);
                }
                return newSquare.FindRightSelectingGripGivenDirection(MovementDirection);
            }
        }

        public Grip NonConnectingGrip
        {
            get
            {
                if (connectedThroughLeftGrip)
                {
                    return newSquare.FindRightSelectingGripGivenDirection(MovementDirection);
                }
                return newSquare.FindLeftSelectingGripGivenDirection(MovementDirection);
            }
        }

        public bool ConnectedThroughRightGrip
        {
            get
            {
                return !connectedThroughLeftGrip;
            }
        }

        public bool ConnectedThroughLeftGrip
        {
            get
            {
                return connectedThroughLeftGrip;
            }
        }
    }
}

//Needs to communicate whether the rightmost grip was chosen, 
//                     whether a jump was necessary, 
//                     the direction, and 
//                     the connecting grip itself.
[System.Serializable]
public class MovementEvent: UnityEvent<bool, bool, Vector2, Grip>
{

}
