using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Controller2D platformingController;

    private int facingDirection = 1;


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

    public void ClimbMovementHappened(bool rightGripWasChosen, Vector2 direction, Grip grabbedGrip)
    {
        int vertMoveDirection = direction.y == 0 ? 0 : (int)Mathf.Sign(direction.y);
        int horMoveDirection = direction.x == 0 ? 0 : (int)Mathf.Sign(direction.x);
        string gripTrigger = rightGripWasChosen ? "moveWithRightGrip" : "moveWithLeftGrip";
        
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
    }

}
