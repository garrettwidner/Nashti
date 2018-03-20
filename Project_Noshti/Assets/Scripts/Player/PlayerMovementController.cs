using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementController : MonoBehaviour
{
    protected PlayerActions playerActions;
    protected bool isSetUp = false;
    protected bool isInControl = false;

    public virtual void RunSetup(bool isInControlAtStart, PlayerActions actions)
    {
        playerActions = actions;
        isSetUp = true;
        isInControl = isInControlAtStart;
    }

}
