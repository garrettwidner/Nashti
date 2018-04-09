using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripSquareDebugger : MonoBehaviour
{
    public LayerMask gripLayer;

    [Header("Is In Same Square Check (Press I)")]
    public Grip grip1;
    public Grip grip2;

    [Header("Single Grip Bidirectional Constructor Check (Press O)")]
    public Grip sGrip;
    public Direction verticalDirection;
    public Direction horizontalDirection;

    private Vector2 vDirection;
    private Vector2 hDirection;

    [Header("Dual Grip Monodirectional Constructor Check (Press P)")]
    public Grip dualGrip1;
    public Grip dualGrip2;
    public Direction movingDirection;

    private Vector2 mDirection;

    [Header("Directional Adjacency Check (Press Arrow Keys)")]
    public Grip startGrip;

    [Header("Square In Direction Check (Press Arrow Key and U)")]
    public Grip.Square startSquare;
    public int maxSpacesToCheck;

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    };
    private void UpdateVectorsFromDirections()
    {
        hDirection = Vector2FromDirection(horizontalDirection);
        vDirection = Vector2FromDirection(verticalDirection);
        mDirection = Vector2FromDirection(movingDirection);
    }

    private Vector2 Vector2FromDirection(Direction direction)
    {
        switch(direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Right:
                return Vector2.right;
            case Direction.Down:
                return Vector2.down;
            default:
                return Vector2.left;
        }
    }

    private void Update()
    {
        UpdateVectorsFromDirections();

        if(Input.GetKeyDown(KeyCode.I))
        {
            CheckIfGripsInSameSquare();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            CheckSingleGripBidirectionalConstructor();
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            CheckDualGripMonodirectionalConstructor();
        }

        /*
        else if(!Input.GetKey(KeyCode.U))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CheckDirectionalAdjacency(Vector2.up);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CheckDirectionalAdjacency(Vector2.right);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CheckDirectionalAdjacency(Vector2.down);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CheckDirectionalAdjacency(Vector2.left);
            }
        }
        */
        
        else if(Input.GetKey(KeyCode.U))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                FindGripInDirection(startSquare, Vector2.up, maxSpacesToCheck);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                FindGripInDirection(startSquare, Vector2.right, maxSpacesToCheck);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                FindGripInDirection(startSquare, Vector2.down, maxSpacesToCheck);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                FindGripInDirection(startSquare, Vector2.left, maxSpacesToCheck);
            }
        }
        
    }

    private void CheckIfGripsInSameSquare()
    {
        print("Grips are in same grip square: " + grip1.IsInSameSquareAs(grip2));
        SuperDebugger.DrawPlus(grip1.transform.position, Color.green, .2f, 3);
        SuperDebugger.DrawPlus(grip2.transform.position, Color.red, .2f, 3);
    }

    private void CheckSingleGripBidirectionalConstructor()
    {
        Grip.Square newSquare = new Grip.Square(sGrip, hDirection, vDirection, gripLayer);
        SuperDebugger.DrawBoxAtPoint(sGrip.transform.position, Grip.HALF_GRIP_WIDTH, Color.green, 2f);
        if(!newSquare.IsNull)
        {
            newSquare.DebugSquare();
        }
        else
        {
            print("New Square created through -Bidirectional- constructor is null");
        }
    }

    private void CheckDualGripMonodirectionalConstructor()
    {
        Grip.Square newSquare = new Grip.Square(dualGrip1, dualGrip2, mDirection, gripLayer);
        SuperDebugger.DrawBoxAtPoint(dualGrip1.transform.position, Grip.GRIP_WIDTH, Color.magenta, 2f);
        SuperDebugger.DrawBoxAtPoint(dualGrip2.transform.position, Grip.GRIP_WIDTH, Color.magenta, 2f);

        if (!newSquare.IsNull)
        {
            newSquare.DebugSquare();
        }
        else
        {
            print("New Square created through -Dual Grip- constructor is null");
        }
    }

    private void CheckDirectionalAdjacency(Vector2 direction)
    {
        Grip adjacentGrip = Grip.FindAdjacentTo(startGrip, direction, gripLayer);
        if (!adjacentGrip.IsNull)
        {
            SuperDebugger.DrawX(adjacentGrip.transform.position, Grip.GRIP_WIDTH, Color.cyan, 1f);
        }
        else
        {
            print("No adjacent grip found in direction: " + direction);
        }
    }

    private void FindGripInDirection(Grip.Square startSquare, Vector2 cDirection, int maxSpacesToCheck)
    {
        Grip.Square foundSquare = startSquare.FindFirstSquareInDirection(cDirection, gripLayer, maxSpacesToCheck);
        if(!foundSquare.IsNull)
        {
            foundSquare.DebugSquare();
            print("Square found in direction : " + cDirection);
        }
        else
        {
            print("Square -NOT- found in direction : " + cDirection);
        }
    }


}
