using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusLevel : MonoBehaviour
{
    [SerializeField] private int startingLevel = 100;
    [SerializeField] private int maxLevel = 100;
    [Range(0, 50)]
    [Tooltip("Represents a percentage of the status bar per second")]
    [SerializeField] private float rapidIncrementSpeed;
    [Range(0,20)]
    [Tooltip("Represents a percentage of the status bar per second")]
    [SerializeField] private float slowIncrementSpeed;

    private float minimumAllowedError = 0.01f;

    private float statusLevel;
    private float slowIncrementPool;
    private float rapidIncrementPool;

    private float slowGoal;
    private float rapidGoal;

    public float TEST_RAPID_INCREMENT = 30f;
    public float TEST_SLOW_INCREMENT = 30f;
    public float TEST_IMMEDIATE_INCREMENT = 30f;

    public bool IsIncrementing
    {
        get
        {
            if(rapidIncrementPool != 0 || slowIncrementPool != 0)
            {
                return true;
            }
            return false;
        }
    }

    private void Start()
    {
        statusLevel = startingLevel > maxLevel ? maxLevel : startingLevel;
    }

    public void RapidIncrement(float increment)
    {
        rapidIncrementPool += increment;
        KeepStatusLevelWithinBounds();

    }

    public void SlowIncrement(float increment)
    {
        slowIncrementPool += increment;
        KeepStatusLevelWithinBounds();
    }

    public void ImmediateIncrement(float increment)
    {
        statusLevel += increment;
    }

    private void Update()
    { 
        RunTest();

        if(statusLevel != 0)
        {
            if (slowIncrementPool != 0)
            {
                RunIncrement(ref slowIncrementPool, slowIncrementSpeed);
            }

            if (rapidIncrementPool != 0)
            {
                RunIncrement(ref rapidIncrementPool, rapidIncrementSpeed);
            }

            KeepStatusLevelWithinBounds();
            StabilizeIncrement(ref rapidIncrementPool);
            StabilizeIncrement(ref slowIncrementPool);
        }
    }

    private void RunTest()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            RapidIncrement(TEST_RAPID_INCREMENT);
            print("---------- Set to increment " + TEST_RAPID_INCREMENT + " rapidly.");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            SlowIncrement(TEST_SLOW_INCREMENT);
            print("---------- Set to increment " + TEST_SLOW_INCREMENT + " slowly.");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ImmediateIncrement(TEST_IMMEDIATE_INCREMENT);
            print("---------- Set to increment " + TEST_IMMEDIATE_INCREMENT + " rapidly.");
            print(statusLevel);
        }
    }

    private void RunIncrement(ref float incrementPool, float incrementSpeed)
    {
        bool incrementIsEnding = false;
        float framewiseIncrement = incrementSpeed * Time.deltaTime * Mathf.Sign(incrementPool);
        if (Mathf.Abs(framewiseIncrement) > Mathf.Abs(incrementPool))
        {
            framewiseIncrement = incrementPool;
            incrementIsEnding = true;
        }
        incrementPool -= framewiseIncrement;
        statusLevel += framewiseIncrement;

        if(incrementIsEnding)
        {
            KeepStatusLevelClean();
        }

        print(statusLevel);
    }

    private void KeepStatusLevelClean()
    {
        //TODO:-----------------------Check for opposite way, say if it settles on 39.9998
        int statusInt = (int)statusLevel;
        float statusDecimal = statusLevel - statusInt;

        if(statusDecimal < minimumAllowedError)
        {
            statusLevel = statusInt;
        }
    }

    private void KeepStatusLevelWithinBounds()
    {
        if(statusLevel <= 0)
        {
            statusLevel = 0;
            print("Status level below zero. Incrementing stopped");
            ResetIncrements();
        }
        if(statusLevel > maxLevel)
        {
            statusLevel = maxLevel;
            print("Status level above maximum. Incrementing stopped");
            //TODO: What if we want a timed heal effect that will last through damage done?
            //like healing on a timer that heals a certain amount every frame no matter what else changes
            ResetIncrements();
        }
    }

    private void StabilizeIncrement(ref float increment)
    {
        if(Mathf.Abs(increment) <= minimumAllowedError)
        {
            increment = 0;
        }
    }

    private void ResetIncrements()
    {
        //TODO------------------: Only reset increments if they're not going in opposite directions.
        //If we reach the top with speedy healing and we're in poison we don't want to stop the poison.
        //Brings up: How do we deal with constant drains like poison? We need to be able to start and stop it.
        rapidIncrementPool = 0;
        slowIncrementPool = 0;
    }
}
