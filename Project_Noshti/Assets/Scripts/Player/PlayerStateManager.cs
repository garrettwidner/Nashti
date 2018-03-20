using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for deciding which movement state a player is in, 
/// deciding when to switch between movement controllers,
/// and for overseeing controllers as they switch between each other
/// </summary>
[RequireComponent(typeof(PlayerClimbingController))]
[RequireComponent(typeof(PlayerPlatformingController))]
public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private PlayerClimbingController climbingController;
    [SerializeField] private PlayerPlatformingController platformingController;
    [SerializeField] private State startingState;

    private State currentState = State.Platforming;
    public State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private PlayerActions playerActions;

    private void Awake()
    {
        playerActions = PlayerActions.CreateWithDefaultBindings();
        RunSetupsBasedOnStartingState();
    }

    private void RunSetupsBasedOnStartingState()
    {
        switch (startingState)
        {
            case State.Climbing:
                climbingController.RunSetup(true, playerActions);
                platformingController.RunSetup(false, playerActions);
                break;
            case State.Platforming:
                climbingController.RunSetup(false, playerActions);
                platformingController.RunSetup(true, playerActions);
                break;
        }
    }

    private void Update()
    {
        CheckForStateSwitch();
    }

    private void CheckForStateSwitch()
    {
        //Switch from climb to platform if jump is pressed and can jump. 
        //Also trigger jump if necessary.
        if(CurrentState == State.Climbing)
        {
            if(playerActions.Jump.WasPressed)
            {
                SwitchToPlatforming();
            }
        }

        //Switch from platform to climb if grip is pressed near a valid grip area.
        if(CurrentState == State.Platforming)
        {
            if(playerActions.GripRight.IsPressed)
            {
                if (climbingController.ConnectIfPossible(true))
                {
                    SwitchToClimbing();
                }
            }
            if(playerActions.GripLeft.IsPressed)
            {
                if(climbingController.ConnectIfPossible(false))
                {
                    SwitchToClimbing();
                }
            }
            
        }
    }

    private void SwitchToClimbing()
    {
        //print("PlayerStateManager Switched to CLIMBING state");
        currentState = State.Climbing;
        climbingController.ReceiveControl();
        platformingController.LoseControl();
    }

    private void SwitchToPlatforming()
    {
        //print("PlayerStateManager Switched to PLATFORMING state");
        currentState = State.Platforming;
        platformingController.ReceiveControl();
        climbingController.LoseControl();
    }


    public enum State
    {
        Climbing,
        Platforming
    };
}
