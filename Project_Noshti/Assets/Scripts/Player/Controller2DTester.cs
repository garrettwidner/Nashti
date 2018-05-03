using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2DTester : MonoBehaviour
{
    public Controller2D controller;

    private void Update()
    {
        //print("player is moving Horizontally: " + controller.IsMovingHorizontally);

        //print("Actual velocity : " + controller.ActualVelocity.ToString("F9"));


        //These Work
        //string facingDirection = controller.FacingDirection >= 0 ? "Right" : "Left";
        //print("player is facing " + facingDirection);
        //print("player is grounded: " + controller.isGrounded);
        //print("player is moving Vertically: " + controller.IsMovingVertically);



    }
}
