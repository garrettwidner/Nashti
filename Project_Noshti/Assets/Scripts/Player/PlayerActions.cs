using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet 
{
    public PlayerAction Jump;
    public PlayerAction Attack;
    public PlayerAction GripLeft;
    public PlayerAction GripRight;

    public PlayerAction Pause;

    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerTwoAxisAction Move;

    public PlayerAction GripUp;
    public PlayerAction GripDown;

    /// <summary>
    /// Returns the absolute value of the axis with the highest magnitude
    /// </summary>
    public float MoveMagnitude
    {
        get
        {
            Vector2 moveVector = new Vector2(Move.X, Move.Y);
            return moveVector.magnitude;
        }
    }

    public Vector2 WasPressedGripVector
    {
        get
        {
            if(GripUp.WasPressed)
            {
                return Vector2.up;
            }
            else if(GripRight.WasPressed)
            {
                return Vector2.right;
            }
            else if(GripDown.WasPressed)
            {
                return Vector2.down;
            }
            else if (GripLeft.WasPressed)
            {
                return Vector2.left;
            }
            return Vector2.zero;
        }
    }

    public bool VerticalAxisGripWasPressed
    {
        get
        {
            if(GripUp.WasPressed || GripDown.WasPressed)
            {
                return true;
            }
            return false;
        }
    }

    public bool HorizontalAxisGripWasPressed
    {
        get
        {
            if (GripLeft.WasPressed || GripRight.WasPressed)
            {
                return true;
            }
            return false;
        }
    }

    public PlayerActions()
    {
        Jump = CreatePlayerAction("Jump");
        Attack = CreatePlayerAction("Attack");
        GripLeft = CreatePlayerAction("Grip Left");
        GripRight = CreatePlayerAction("Grip Right");

        Pause = CreatePlayerAction("Pause");

        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

        GripUp = CreatePlayerAction("Grip Up");
        GripDown = CreatePlayerAction("Grip Down");
    }

    public static PlayerActions CreateWithDefaultBindings()
    {
        PlayerActions playerActions = new PlayerActions();

        playerActions.Jump.AddDefaultBinding(InputControlType.Action1);
        playerActions.Jump.AddDefaultBinding(Key.Space);

        playerActions.Attack.AddDefaultBinding(InputControlType.Action4);
        playerActions.Attack.AddDefaultBinding(Key.L);

        playerActions.GripLeft.AddDefaultBinding(InputControlType.Action3);
        playerActions.GripLeft.AddDefaultBinding(InputControlType.LeftTrigger);
        playerActions.GripLeft.AddDefaultBinding(InputControlType.LeftBumper);
        playerActions.GripLeft.AddDefaultBinding(Key.J);

        playerActions.GripRight.AddDefaultBinding(InputControlType.Action2);
        playerActions.GripRight.AddDefaultBinding(InputControlType.RightTrigger);
        playerActions.GripRight.AddDefaultBinding(InputControlType.RightBumper);
        playerActions.GripRight.AddDefaultBinding(Key.K);

        playerActions.Pause.AddDefaultBinding(InputControlType.Command);
        playerActions.Pause.AddDefaultBinding(Key.Return);

        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        playerActions.Left.AddDefaultBinding(Key.A);

        playerActions.Right.AddDefaultBinding(Key.RightArrow);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        playerActions.Right.AddDefaultBinding(Key.D);

        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
        playerActions.Up.AddDefaultBinding(Key.W);


        playerActions.Down.AddDefaultBinding(Key.DownArrow);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);
        playerActions.Down.AddDefaultBinding(Key.S);

        playerActions.ListenOptions.IncludeUnknownControllers = true;
        playerActions.ListenOptions.MaxAllowedBindings = 4;

        //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
        //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
        //playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;

        return playerActions;
    }

    public static PlayerActions CreateWithTestBinding1()
    {
        PlayerActions playerActions = new PlayerActions();

        playerActions.Jump.AddDefaultBinding(InputControlType.LeftTrigger);
        playerActions.Jump.AddDefaultBinding(InputControlType.LeftBumper);
        playerActions.Jump.AddDefaultBinding(InputControlType.RightTrigger);
        playerActions.Jump.AddDefaultBinding(InputControlType.RightBumper);
        playerActions.Jump.AddDefaultBinding(Key.Space);

        playerActions.GripLeft.AddDefaultBinding(InputControlType.Action3);
        playerActions.GripLeft.AddDefaultBinding(Key.J);

        playerActions.GripRight.AddDefaultBinding(InputControlType.Action2);
        playerActions.GripRight.AddDefaultBinding(Key.L);

        playerActions.GripUp.AddDefaultBinding(InputControlType.Action4);
        playerActions.GripUp.AddDefaultBinding(Key.I);

        playerActions.GripDown.AddDefaultBinding(InputControlType.Action1);
        playerActions.GripDown.AddDefaultBinding(Key.K);

        playerActions.Pause.AddDefaultBinding(InputControlType.Command);
        playerActions.Pause.AddDefaultBinding(Key.Return);

        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        playerActions.Left.AddDefaultBinding(Key.A);

        playerActions.Right.AddDefaultBinding(Key.RightArrow);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        playerActions.Right.AddDefaultBinding(Key.D);

        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
        playerActions.Up.AddDefaultBinding(Key.W);


        playerActions.Down.AddDefaultBinding(Key.DownArrow);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
        playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);
        playerActions.Down.AddDefaultBinding(Key.S);

        playerActions.ListenOptions.IncludeUnknownControllers = true;
        playerActions.ListenOptions.MaxAllowedBindings = 4;

        //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
        //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
        //playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;

        return playerActions;
    }

}
