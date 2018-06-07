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
    [SerializeField] private float jumpConnectionModifier;
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

    protected override void Update()
    {
        base.Update();
        
        if(isStationary && stateManager.CurrentState == PlayerStateManager.State.Climbing)
        {
            StartImmediateIncrement(-staticDrainPerSecond * Time.deltaTime);
        }
        
    }

    public void MoveWasTaken(PlayerClimbingController.Move move)
    {
        isStationary = false;
        int gripQuality = 10 - move.ConnectingGrip.Quality;
        float drainModifier = move.IsJumpNecessary ? jumpConnectionModifier : moveDrainModifier;
        nextGripDrain = -gripQuality * drainModifier;
        Invoke("DrainGrip", timeBetweenMoveAndGripDrain);
    }

    public void JumpConnectionHappened(PlayerClimbingController.Move connection)
    {
        int gripQuality = 10 - connection.ConnectingGrip.Quality;
        nextGripDrain = -gripQuality * jumpConnectionModifier;
        Invoke("DrainGrip", 0.0f);
    }

    private void DrainGrip()
    {
        StartRapidIncrement(nextGripDrain);
        print("Grip drained for " + nextGripDrain);
    }

    public void MoveEnded(PlayerClimbingController.Move move)
    {
        isStationary = true;
    }

    public void MaduPickedUp(float madu)
    {
        if(madu > 1)
        {
            StartRapidIncrement(madu);
        }
        else
        {
            StartImmediateIncrement(madu);
        }
    }


}
