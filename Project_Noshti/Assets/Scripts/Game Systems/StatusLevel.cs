using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusLevel : MonoBehaviour
{
    [SerializeField] private int startingLevel;
    [SerializeField] private int maxLevel;
    [Range(0, 50)]
    [Tooltip("Represents a percentage of the status bar per second")]
    [SerializeField] private float rapidIncrementSpeed;
    [Range(0,20)]
    [Tooltip("Represents a percentage of the status bar per second")]
    [SerializeField] private float slowIncrementSpeed;

    private float currentLevel;
    private float toIncrementSlow;
    private float toIncrementRapid;

    private void Start()
    {
        currentLevel = startingLevel > maxLevel ? maxLevel : startingLevel;
    }

    public void RapidIncrement(float increment)
    {
        toIncrementRapid += increment;
    }

    public void SlowIncrement(float increment)
    {
        toIncrementSlow += increment;
    }

    public void ImmediateIncrement(float increment)
    {
        currentLevel += increment;
    }

    private void Update()
    {
        if(toIncrementSlow > 0)
        {
            RunSlowIncrement();
        }

        if(toIncrementRapid > 0)
        {
            RunRapidIncrement();
        }

        KeepCurrentLevelWithinBounds();
    }

    private void RunRapidIncrement()
    {
        float currentIncrement = rapidIncrementSpeed * Time.deltaTime;
        currentLevel -= currentIncrement;
    }

    private void RunSlowIncrement()
    {
        float currentIncrement = slowIncrementSpeed * Time.deltaTime;
        currentLevel -= currentIncrement;
    }

    private void KeepCurrentLevelWithinBounds()
    {
        if(currentLevel < 0)
        {
            currentLevel = 0;
        }
        if(currentLevel > maxLevel)
        {
            currentLevel = maxLevel;
        }
    }


    

}
