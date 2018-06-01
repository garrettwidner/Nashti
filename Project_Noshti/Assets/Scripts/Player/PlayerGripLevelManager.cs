using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads from events and environmental factors to modify and maintain the player's gripLevel variable.
/// </summary>
public class PlayerGripLevelManager : StatusLevel
{
    [SerializeField] private float staticDrainPerSecond;
    [SerializeField] private float moveDrainModifier;
    [SerializeField] private float timeBetweenMoveAndGripDrain;
    [SerializeField] private PlayerStateManager stateManager;

    private bool isStationary = true;
    private float nextGripDrain;

    public float GripLevel
    {
        get
        {
            return StatLevel;
        }
    }

    public void Update()
    {
        /*
        if(isStationary && stateManager.CurrentState == PlayerStateManager.State.Climbing)
        {
            StartImmediateIncrement(-staticDrainPerSecond * Time.deltaTime);
        }
        */
    }

    public void MoveWasTaken(PlayerClimbingController.Move move)
    {
        isStationary = false;
        int gripQuality = move.ConnectingGrip.Quality;
        nextGripDrain = -gripQuality * moveDrainModifier;
        Invoke("DrainGrip", timeBetweenMoveAndGripDrain);
    }

    private void DrainGrip()
    {
        print("Grip drained for " + nextGripDrain + " ------------------");
        StartRapidIncrement(nextGripDrain);
    }

    public void MoveEnded(PlayerClimbingController.Move move)
    {
        isStationary = true;
    }




}
