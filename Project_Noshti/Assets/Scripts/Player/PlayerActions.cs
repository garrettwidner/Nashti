using UnityEngine;
using InControl;

public class PlayerActions : PlayerActionSet 
{


    public PlayerAction Jump;
    public PlayerAction Dismount;
    public PlayerAction Run;
    public PlayerAction Left;
    public PlayerAction Right;
    public PlayerAction Up;
    public PlayerAction Down;
    public PlayerAction HoldPosition;
    public PlayerAction HoldRotation;
    public PlayerTwoAxisAction Move;

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

    public PlayerActions()
    {
        Jump = CreatePlayerAction("Jump");
        Dismount = CreatePlayerAction("Dismount");
        Run = CreatePlayerAction("Run");
        Left = CreatePlayerAction("Move Left");
        Right = CreatePlayerAction("Move Right");
        Up = CreatePlayerAction("Move Up");
        Down = CreatePlayerAction("Move Down");
        HoldPosition = CreatePlayerAction("Hold Position");
        HoldRotation = CreatePlayerAction("Hold Rotation");
        Move = CreateTwoAxisPlayerAction(Left, Right, Down, Up);
    }

    public static PlayerActions CreateWithDefaultBindings()
    {
        PlayerActions playerActions = new PlayerActions();

        playerActions.Jump.AddDefaultBinding(InputControlType.Action1);
        playerActions.Jump.AddDefaultBinding(Key.K);

        playerActions.Dismount.AddDefaultBinding(InputControlType.Action2);
        playerActions.Dismount.AddDefaultBinding(Key.L);

        playerActions.Run.AddDefaultBinding(InputControlType.Action3);
        playerActions.Run.AddDefaultBinding(Key.J);

        playerActions.Left.AddDefaultBinding(Key.LeftArrow);
        playerActions.Right.AddDefaultBinding(Key.RightArrow);
        playerActions.Up.AddDefaultBinding(Key.UpArrow);
        playerActions.Down.AddDefaultBinding(Key.DownArrow);

        playerActions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.Down.AddDefaultBinding(InputControlType.LeftStickDown);

        playerActions.Left.AddDefaultBinding(InputControlType.DPadLeft);
        playerActions.Right.AddDefaultBinding(InputControlType.DPadRight);
        playerActions.Up.AddDefaultBinding(InputControlType.DPadUp);
        playerActions.Down.AddDefaultBinding(InputControlType.DPadDown);

        playerActions.Up.AddDefaultBinding(Key.W);
        playerActions.Down.AddDefaultBinding(Key.S);
        playerActions.Left.AddDefaultBinding(Key.A);
        playerActions.Right.AddDefaultBinding(Key.D);

        playerActions.HoldPosition.AddDefaultBinding(Key.N);
        playerActions.HoldPosition.AddDefaultBinding(InputControlType.RightTrigger);

        playerActions.HoldRotation.AddDefaultBinding(Key.N);
        playerActions.HoldRotation.AddDefaultBinding(InputControlType.LeftTrigger);

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
