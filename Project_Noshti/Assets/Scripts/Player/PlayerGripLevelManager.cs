using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reads from events and environmental factors to modify and maintain the player's gripLevel variable.
/// </summary>
public class PlayerGripLevelManager : StatusLevel
{
    public float GripLevel
    {
        get
        {
            return gripLevel;
        }
    }

    private float gripLevel
    {
        get
        {
            return statusLevel;
        }
        set
        {
            statusLevel = value;
        }
    }

    public void MoveWasTaken(PlayerClimbingController.Movement move)
    {
        print("Move taken");
    }



}
