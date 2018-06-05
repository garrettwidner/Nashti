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
    [SerializeField] private float climbMoveSpeed = 7f;
    [SerializeField] private bool showDebug = true;

    [SerializeField] private MoveEvent OnMoveStarted;
    [SerializeField] private MoveEvent OnMoveEnded;
    [SerializeField] private MoveEvent OnJumpConnectionHappened;

    private int minimumGripsForJumpSquare = 3;
    private bool isMoving = false;
    private float t = 0.0f;
    private Vector2 lerpStart;
    private Vector2 lerpEnd;

    private Grip.Square currentConnectedSquare;
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
        potentialMoves = new CardinalContainer<PotentialMove>();
        ResetPotentialMoves();
    }
    
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

                    if(OnJumpConnectionHappened != null)
                    {
                        Move connection = new Move(foundSquare, false);
                        OnJumpConnectionHappened.Invoke(connection);
                    }
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

                    if (OnJumpConnectionHappened != null)
                    {
                        Move connection = new Move(foundSquare, true);
                        OnJumpConnectionHappened.Invoke(connection);
                    }
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

    private void FindPotentialMoves()
    {
        potentialMoves.up = FindPotentialMoveInDirection(Vector2.up);
        potentialMoves.right = FindPotentialMoveInDirection(Vector2.right);
        potentialMoves.down = FindPotentialMoveInDirection(Vector2.down);
        potentialMoves.left = FindPotentialMoveInDirection(Vector2.left);
    }

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

    private void MakeMoveIfPossible(bool rightGripWasChosen)
    {
        Vector2 direction = leaningDirection;
        Move nextMove = new Move(potentialMoves.Vector2ToObject(leaningDirection), !rightGripWasChosen);
        if(nextMove.NewSquareWasFound)
        {
            Grip selectedGrip = nextMove.ConnectingGrip;

            if(selectedGrip != null && !selectedGrip.IsEmpty)
            {
                if(OnMoveStarted != null)
                {
                    OnMoveStarted.Invoke(nextMove);
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
        FindPotentialMoves();
        transform.position = lerpEnd;

        if(OnMoveEnded != null)
        {
            OnMoveEnded.Invoke(previousMove);
        }
    }

    private void MoveToSquareImmediate(Grip.Square newSquare)
    {
        //print("Jump to square");
        //newSquare.DebugSquare();
        transform.position = newSquare.Center;
        currentConnectedSquare = newSquare;

        FindPotentialMoves();
    }

    public override void LoseMovementControl()
    {
        base.LoseMovementControl();
        isLeaning = false;
        leaningDirection = Vector2.zero;
    }

    public class PotentialMove
    {
        protected Grip.Square newSquare;
        protected bool isJumpNecessary;
        protected Vector2 direction;

        public PotentialMove()
        {
            newSquare = new Grip.Square();
            isJumpNecessary = false;
            direction = Vector2.zero;
        }

        public PotentialMove(Grip.Square nextSquare, bool jumpIsNecessary, Vector2 moveDirection)
        {
            newSquare = nextSquare;
            isJumpNecessary = jumpIsNecessary;
            direction = moveDirection;
        }

        public PotentialMove(Grip.Square nextSquare, bool jumpIsNecessary)
        {
            newSquare = nextSquare;
            isJumpNecessary = jumpIsNecessary;
            direction = Vector2.up;
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

        public Vector2 Direction
        {
            get
            {
                return direction;
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
            direction = move.Direction;
        }

        public Move(Grip.Square nextSquare, bool jumpIsNecessary, Vector2 moveDirection, bool leftGripUsed) : base(nextSquare, jumpIsNecessary, moveDirection)
        {
            connectedThroughLeftGrip = leftGripUsed;
        }

        public Move(Grip.Square nextSquare, bool leftGripUsed)
        {
            newSquare = nextSquare;
            isJumpNecessary = false;
            connectedThroughLeftGrip = leftGripUsed;
            direction = Vector2.up;
        }
        
        public Grip ConnectingGrip
        {
            get
            {
                if (connectedThroughLeftGrip)
                {
                    return newSquare.FindLeftSelectingGripGivenDirection(Direction);
                }
                return newSquare.FindRightSelectingGripGivenDirection(Direction);
            }
        }

        public Grip NonConnectingGrip
        {
            get
            {
                if (connectedThroughLeftGrip)
                {
                    return newSquare.FindRightSelectingGripGivenDirection(Direction);
                }
                return newSquare.FindLeftSelectingGripGivenDirection(Direction);
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

[System.Serializable] 
public class MoveEvent: UnityEvent<PlayerClimbingController.Move>
{

}
