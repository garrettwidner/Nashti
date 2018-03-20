using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementController : MonoBehaviour
{
    protected PlayerActions playerActions;
    protected bool isSetUp = false;
    protected bool isControllingMovement = false;

    public virtual void RunSetup(bool isInControlAtStart, PlayerActions actions)
    {
        playerActions = actions;
        isSetUp = true;
        isControllingMovement = isInControlAtStart;
    }

    public virtual void ReceiveControl()
    {
        isControllingMovement = true;
    }

    public virtual void LoseControl()
    {
        isControllingMovement = false;
    }

}
