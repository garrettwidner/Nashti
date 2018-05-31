﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Controller2D platformingController;

    private int facingDirection = 1;

    private void Start()
    {
        ChangeStateToPlatforming();
    }

    public void ChangeStateToClimbing()
    {
        animator.SetBool("IsClimbing", true);
        animator.SetBool("IsPlatforming", false);
    }

    public void ChangeStateToPlatforming()
    {
        animator.SetBool("IsPlatforming", true);
        animator.SetBool("IsClimbing", false);
    }


    public void ClimbMovementHappened(bool rightGripWasChosen, bool jumpWasNecessary, Vector2 direction, Grip grabbedGrip)
    {
        int vertMoveDirection = direction.y == 0 ? 0 : (int)Mathf.Sign(direction.y);
        int horMoveDirection = direction.x == 0 ? 0 : (int)Mathf.Sign(direction.x);
        string gripTrigger = rightGripWasChosen ? "moveWithRightGrip" : "moveWithLeftGrip";

        animator.SetInteger("vertMoveDirection", vertMoveDirection);
        animator.SetInteger("horMoveDirection", horMoveDirection);
        animator.SetTrigger(gripTrigger);

        //print(gripTrigger + "( " + horMoveDirection + " , " + vertMoveDirection + ")");

    }

    public void ClimbMoveHappened(PlayerClimbingController.Move move)
    {
        int vertMoveDirection = move.Direction.y == 0 ? 0 : (int)Mathf.Sign(move.Direction.y);
        int horMoveDirection = move.Direction.x == 0 ? 0 : (int)Mathf.Sign(move.Direction.x);
        string gripTrigger = move.ConnectedThroughLeftGrip ? "moveWithLeftGrip" : "moveWithRightGrip";

        animator.SetInteger("vertMoveDirection", vertMoveDirection);
        animator.SetInteger("horMoveDirection", horMoveDirection);
        animator.SetTrigger(gripTrigger);
    }

    private void Update()
    {
        if(platformingController.FacingDirection != facingDirection)
        {
            animator.SetBool("isFacingRight", !animator.GetBool("isFacingRight"));
            facingDirection *= -1;
        }

        if(platformingController.IsMovingHorizontally)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        animator.SetBool("IsGrounded", platformingController.IsGrounded);
    }

}
