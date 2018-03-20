using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingController : PlayerMovementController
{
    [SerializeField] private Transform leftHandConnectionPoint;
    [SerializeField] private Transform rightHandConnectionPoint;
    [SerializeField] private Transform leftFootConnectionPoint;
    [SerializeField] private Transform rightFootConnectionPoint;

    [SerializeField] private float limbToGripConnectionProximity = .5f;
    [SerializeField] private float limbToLimbDistance = .5f;

    [SerializeField] private LayerMask gripLayer;

    public bool ConnectIfPossible(bool rightHandIsConnecting)
    {
        //First checks the hand being used to see if it can connect. 
        if(rightHandIsConnecting)
        {
            Grip rightHand = GripProximityChecker.CheckProximity(rightHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(rightHand != null)
            {
                Grip.Square foundSquare = GripProximityChecker.PopulateGripSquare(rightHand, Vector2.left, Vector2.down, gripLayer);
                if (foundSquare.GripCount >= 2)
                {
                    ConnectToWall(foundSquare);
                    return true;
                }
            }
        }
        else
        {
            Grip leftHand = GripProximityChecker.CheckProximity(leftHandConnectionPoint.position, limbToGripConnectionProximity, gripLayer);
            if(leftHand != null)
            {
                Grip.Square foundSquare = GripProximityChecker.PopulateGripSquare(leftHand, Vector2.right, Vector2.down, gripLayer);
                if(foundSquare.GripCount >= 2)
                {
                    ConnectToWall(foundSquare);
                    return true;
                }
            }
        }

        //If so, checks to find another hold in the same square. If so, returns true
        //and connects to the found grip points.

        return false;
    }

    private void ConnectToWall(Grip.Square gripSquare)
    {
        gripSquare.DebugSquare();
        transform.position = gripSquare.Center;
    }

    private void Update()
    {
        if (!isSetUp || !isInControl)
        {
            return;
        }
    }

}
