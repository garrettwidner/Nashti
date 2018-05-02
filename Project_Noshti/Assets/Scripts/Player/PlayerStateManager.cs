﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private UnityEvent OnStateChangedToClimbing;
    [SerializeField] private UnityEvent OnStateChangedToPlatforming;


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
            if(playerActions.Action.WasPressed)
            {
                SwitchToPlatforming();
            }
        }

        //Switch from platform to climb if grip is pressed near a valid grip area.
        if(CurrentState == State.Platforming)
        {
            if (playerActions.GripRight.IsPressed)
            {
                if (climbingController.ConnectAtHandsIfPossible(true))
                {
                    SwitchToClimbing();
                }
            }
            if (playerActions.GripLeft.IsPressed)
            {
                if (climbingController.ConnectAtHandsIfPossible(false))
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
        platformingController.LoseMovementControl();
        if(OnStateChangedToClimbing != null)
        {
            OnStateChangedToClimbing.Invoke();
        }
    }

    private void SwitchToPlatforming()
    {
        //print("PlayerStateManager Switched to PLATFORMING state");
        currentState = State.Platforming;
        climbingController.LoseMovementControl();
        platformingController.ReceiveControl();

        platformingController.JumpNextFrame();

        if(OnStateChangedToPlatforming != null)
        {
            OnStateChangedToPlatforming.Invoke();
        }
    }


    public enum State
    {
        Climbing,
        Platforming
    };
}
