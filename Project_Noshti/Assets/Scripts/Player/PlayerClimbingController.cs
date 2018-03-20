using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingController : PlayerMovementController
{

    private void Update()
    {
        if (!isSetUp || !isInControl)
        {
            return;
        }
    }

}
