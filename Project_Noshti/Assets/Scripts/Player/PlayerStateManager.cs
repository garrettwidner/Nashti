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
    [SerializeField] private PlayerClimbingControllerAlternate alternateClimbingController;
    [SerializeField] private PlayerPlatformingController platformingController;
    [SerializeField] private State startingState;

    public bool useAlternateClimbingController;

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
        if(useAlternateClimbingController)
        {
            playerActions = PlayerActions.CreateWithTestBinding1();
        }
        else
        {
            playerActions = PlayerActions.CreateWithDefaultBindings();
        }
        RunSetupsBasedOnStartingState();
    }

    private void RunSetupsBasedOnStartingState()
    {
        switch (startingState)
        {
            case State.Climbing:
                if(useAlternateClimbingController)
                {
                    alternateClimbingController.RunSetup(true, playerActions);
                }
                else
                {
                    climbingController.RunSetup(true, playerActions);
                }
                platformingController.RunSetup(false, playerActions);
                break;
            case State.Platforming:
                if (useAlternateClimbingController)
                {
                    alternateClimbingController.RunSetup(false, playerActions);
                }
                else
                {
                    climbingController.RunSetup(false, playerActions);
                }
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
            if(useAlternateClimbingController)
            {
                if (playerActions.GripRight.IsPressed)
                {
                    if (alternateClimbingController.ConnectIfPossible(true))
                    {
                        SwitchToClimbing();
                    }
                }
                if (playerActions.GripLeft.IsPressed)
                {
                    if (alternateClimbingController.ConnectIfPossible(false))
                    {
                        SwitchToClimbing();
                    }
                }
            }
            else
            {
                if (playerActions.GripRight.IsPressed)
                {
                    if (climbingController.ConnectIfPossible(true))
                    {
                        SwitchToClimbing();
                    }
                }
                if (playerActions.GripLeft.IsPressed)
                {
                    if (climbingController.ConnectIfPossible(false))
                    {
                        SwitchToClimbing();
                    }
                }
            }
            
            
        }
    }

    private void SwitchToClimbing()
    {
        //print("PlayerStateManager Switched to CLIMBING state");
        currentState = State.Climbing;
        if(useAlternateClimbingController)
        {
            alternateClimbingController.ReceiveControl();
        }
        else
        {
            climbingController.ReceiveControl();
        }
        platformingController.LoseControl();
    }

    private void SwitchToPlatforming()
    {
        //print("PlayerStateManager Switched to PLATFORMING state");
        currentState = State.Platforming;
        if (useAlternateClimbingController)
        {
            alternateClimbingController.LoseControl();
        }
        else
        {
            climbingController.LoseControl();
        }
        platformingController.ReceiveControl();

        platformingController.JumpNextFrame();
    }


    public enum State
    {
        Climbing,
        Platforming
    };
}
