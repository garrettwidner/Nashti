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

    public float J_TEST_RAPID_INCREMENT = 30f;
    public float K_TEST_SLOW_INCREMENT = 30f;
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

    public void StartRapidIncrement(float increment)
    {
        rapidIncrementPool += increment;
        KeepStatusBetweenBounds();
    }

    public void StartSlowIncrement(float increment)
    {
        slowIncrementPool += increment;
        KeepStatusBetweenBounds();
    }

    public void StartImmediateIncrement(float increment)
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

            KeepStatusBetweenBounds();
        }
    }

    private void RunTest()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartRapidIncrement(J_TEST_RAPID_INCREMENT);
            print("---------- Set to increment " + J_TEST_RAPID_INCREMENT + " rapidly.");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            StartSlowIncrement(K_TEST_SLOW_INCREMENT);
            print("---------- Set to increment " + K_TEST_SLOW_INCREMENT + " slowly.");
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            StartImmediateIncrement(TEST_IMMEDIATE_INCREMENT);
            print("---------- Set to increment " + TEST_IMMEDIATE_INCREMENT + " immediately.");
            print(statusLevel);
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            print("slow pool: " + slowIncrementPool.ToString("F4"));
            print("rapid pool: " + rapidIncrementPool.ToString("F4"));
            print("Current status level: " + statusLevel);
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
            //print("Increment ended explicitly.");
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
        int statusInt = Mathf.Abs((int)statusLevel);
        float statusDecimal = Mathf.Abs(statusLevel) - statusInt;

        //print("Status decimal: " + statusDecimal.ToString("F5"));
        //print("Status Int: " + statusInt.ToString("F5"));

        if(statusDecimal < minimumAllowedError)
        {
            statusLevel = statusInt;
            //print("Status decimal less than " + minimumAllowedError + ". Setting to integer.");
        }
        else if((1 - statusDecimal) < minimumAllowedError)
        {
            statusLevel = statusInt + 1;
            //print("Status decimal less than " + minimumAllowedError + ". Setting to integer.");
        }
    }

    private void KeepStatusBetweenBounds()
    {
        if(statusLevel <= 0)
        {
            statusLevel = 0;
            //print("Status level below zero. Incrementing stopped");
            ResetIncrements(false);
        }
        if(statusLevel > maxLevel)
        {
            statusLevel = maxLevel;
            //print("Status level above maximum. Incrementing stopped");
            ResetIncrements(true);
        }
    }

    /*
    private void CheckForZeroingIncrement(ref float increment)
    {
        if(Mathf.Abs(increment) <= minimumAllowedError)
        {
            increment = 0;
        }
    }
    */

    private void ResetIncrements(bool resetIncreasing)
    {
        //Brings up: How do we deal with constant drains like poison? We need to be able to start and stop it.
        //Can just detract each frame. Easy. We add another function. But how to turn off?
        //Perhaps create an instance of a ConstantDecrement class which will go in an array and be 
        //called on every turn. Can search for the instance of the class (provided on creation) in order to destroy it.
        //Or class can have a name like 'poison' for ease of recovery.
        //Add timed increments (heal x amount every frame for p seconds)
        //Add constant increments (heal x amount every frame until told to stop) (StartConstantIncrement() StopConstantIncrement())

        if (resetIncreasing)
        {
            if (rapidIncrementPool >= 0)
            {
                rapidIncrementPool = 0;
                //print("Still increasing. RAPID pool cleared.");
            }
            if (slowIncrementPool >= 0)
            {
                slowIncrementPool = 0;
                //print("Still increasing. SLOW pool cleared.");
            }
        }
        else
        {
            if (rapidIncrementPool <= 0)
            {
                rapidIncrementPool = 0;
                //print("Still decreasing. RAPID pool cleared.");
            }
            if (slowIncrementPool <= 0)
            {
                slowIncrementPool = 0;
                //print("Still decreasing. SLOW pool cleared.");
            }
        }
    }
}
