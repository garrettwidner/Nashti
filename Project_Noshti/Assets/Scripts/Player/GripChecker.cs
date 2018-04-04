﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GripChecker : MonoBehaviour
{
    /// <summary>
    /// Returns closest grip point if found in proximity. If none found, returns null.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="checkRadius"></param>
    /// <returns></returns>
    public static Grip CheckAreaForGrip(Vector2 center, float checkRadius, LayerMask gripLayer)
    {
        Collider2D[] foundColliders = Physics2D.OverlapCircleAll(center, checkRadius, gripLayer);
        if(foundColliders != null)
        {
            Collider2D closest = null;

            //Find closest collider with a GripPoint component
            foreach(Collider2D collider in foundColliders)
            {
                Grip foundGrip = collider.GetComponent<Grip>();
                if(foundGrip != null)
                {
                    if(closest == null)
                    {
                        closest = collider;
                    }
                    else
                    {
                        closest = FindClosestCollider(center, collider, closest);
                    }
                }
                else
                {
                    Debug.LogWarning("Object " + collider.name + " on Grip layer missing GripPoint component. Omitted from CheckProximity() results");
                }
            }

            if(closest != null)
            {
                return closest.GetComponent<Grip>();
            }
        }

        return null;
    }

    private static Collider2D FindClosestCollider(Vector2 start, Collider2D collider1, Collider2D collider2)
    {
        float distanceTo1 = Vector2.Distance(start, collider1.transform.position);
        float distanceTo2 = Vector2.Distance(start, collider2.transform.position);

        if(distanceTo1 < distanceTo2)
        {
            return collider1;
        }

        return collider2;
    }

    /// <summary>
    /// Populates a grip square from one of its given grips. Takes the directions the other grips can be found in.
    /// </summary>
    /// <param name="startGrip"></param>
    /// <param name="hDirection"></param>
    /// <param name="vDirection"></param>
    /// <param name="gripLayer"></param>
    /// <returns></returns>
    public static Grip.Square PopulateGripSquare(Grip startGrip, Vector2 hDirection, Vector2 vDirection, LayerMask gripLayer)
    {
        hDirection.Normalize();
        vDirection.Normalize();
        if(hDirection != Vector2.left && hDirection != Vector2.right)
        {
            Debug.LogWarning("hDirection must be either left or right");
            return new Grip.Square();
        }
        if(vDirection != Vector2.up && vDirection != Vector2.down)
        {
            Debug.LogWarning("vDirection must be either down or up");
            return new Grip.Square();
        }

        Grip.Square newGripSquare = new Grip.Square();

        Vector2 hOffset = hDirection * Grip.WIDTH_BETWEEN_GRIPS;
        Vector2 vOffset = vDirection * Grip.WIDTH_BETWEEN_GRIPS;

        Vector2 horizontalGripLocation = (Vector2)startGrip.transform.position + hOffset;
        Vector2 verticalGripLocation = (Vector2)startGrip.transform.position + vOffset;
        Vector2 diagonalGripLocation = (Vector2)startGrip.transform.position + hOffset + vOffset;

        Vector2 colliderSizeOffset = new Vector2(Grip.HALF_GRIP_WIDTH, Grip.HALF_GRIP_WIDTH);

        Collider2D hCollider = Physics2D.OverlapArea(horizontalGripLocation + colliderSizeOffset, horizontalGripLocation - colliderSizeOffset, gripLayer);
        Collider2D vCollider = Physics2D.OverlapArea(verticalGripLocation + colliderSizeOffset, verticalGripLocation - colliderSizeOffset, gripLayer);
        Collider2D dCollider = Physics2D.OverlapArea(diagonalGripLocation + colliderSizeOffset, diagonalGripLocation - colliderSizeOffset, gripLayer);

        //Starting grip is on the right
        if(horizontalGripLocation.x < startGrip.transform.position.x)
        {
            //Starting grip is on the top
            if (verticalGripLocation.y < startGrip.transform.position.y)
            {
                newGripSquare.upperRight = startGrip;
                if(hCollider != null)
                {
                    newGripSquare.upperLeft = hCollider.GetComponent<Grip>();
                }
                if(vCollider != null)
                {
                    newGripSquare.lowerRight = vCollider.GetComponent<Grip>();
                }
                if(dCollider != null)
                {
                    newGripSquare.lowerLeft = dCollider.GetComponent<Grip>();
                }
            }
            //Starting grip is on the bottom
            else
            {
                newGripSquare.lowerRight = startGrip;
                if (hCollider != null)
                {
                    newGripSquare.lowerLeft = hCollider.GetComponent<Grip>();
                }
                if (vCollider != null)
                {
                    newGripSquare.upperRight = vCollider.GetComponent<Grip>();
                }
                if (dCollider != null)
                {
                    newGripSquare.upperLeft = dCollider.GetComponent<Grip>();
                }
            }
        }
        //Starting grip is on the left
        else
        {
            //Starting grip is on the top
            if (verticalGripLocation.y < startGrip.transform.position.y)
            {
                newGripSquare.upperLeft = startGrip;
                if (hCollider != null)
                {
                    newGripSquare.upperRight = hCollider.GetComponent<Grip>();
                }
                if (vCollider != null)
                {
                    newGripSquare.lowerLeft = vCollider.GetComponent<Grip>();
                }
                if (dCollider != null)
                {
                    newGripSquare.lowerRight = dCollider.GetComponent<Grip>();
                }
            }
            //Starting grip is on the bottom
            else
            {
                newGripSquare.lowerLeft = startGrip;
                if (hCollider != null)
                {
                    newGripSquare.lowerRight = hCollider.GetComponent<Grip>();
                }
                if (vCollider != null)
                {
                    newGripSquare.upperLeft = vCollider.GetComponent<Grip>();
                }
                if (dCollider != null)
                {
                    newGripSquare.upperRight = dCollider.GetComponent<Grip>();
                }
            }
        }

        return newGripSquare;
    }

    /// <summary>
    /// Returns the grip square in the given direction. If at least one Grip isn't found in that direction, returns a null Square.
    /// </summary>
    /// <param name="startingSquare"></param>
    /// <param name="direction"></param>
    /// <param name="gripLayer"></param>
    /// <returns></returns>
    public static Grip.Square FindConnectedGripSquareInDirection(Grip.Square startingSquare, Vector2 direction, LayerMask gripLayer)
    {
        direction.Normalize();
        if(direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return new Grip.Square();
        }

        if(direction == Vector2.up)
        {
            if(startingSquare.upperLeft != null)
            {
                return PopulateGripSquare(startingSquare.upperLeft, Vector2.right, Vector2.up, gripLayer);
            }
            else if(startingSquare.upperRight != null)
            {
                return PopulateGripSquare(startingSquare.upperRight, Vector2.left, Vector2.up, gripLayer);
            }
        }
        else if(direction == Vector2.right)
        {
            if(startingSquare.upperRight != null)
            {
                return PopulateGripSquare(startingSquare.upperRight, Vector2.right, Vector2.down, gripLayer);
            }
            else if(startingSquare.lowerRight != null)
            {
                return PopulateGripSquare(startingSquare.lowerRight, Vector2.right, Vector2.up, gripLayer);
            }
        }
        else if(direction == Vector2.down)
        {
            if(startingSquare.lowerRight != null)
            {
                return PopulateGripSquare(startingSquare.lowerRight, Vector2.left, Vector2.down, gripLayer);
            }
            else if(startingSquare.lowerLeft != null)
            {
                return PopulateGripSquare(startingSquare.lowerLeft, Vector2.right, Vector2.down, gripLayer);
            }
        }
        else if(direction == Vector2.left)
        {
            if(startingSquare.lowerLeft != null)
            {
                return PopulateGripSquare(startingSquare.lowerLeft, Vector2.left, Vector2.up, gripLayer);
            }
            else if(startingSquare.upperLeft != null)
            {
                return PopulateGripSquare(startingSquare.upperLeft, Vector2.left, Vector2.down, gripLayer);
            }
        }

        return new Grip.Square();
    }
    
    /*
    /// <summary>
    /// Finds first connectible grip square in the given direction from startingSquare. JumpLength measured in single grips.
    /// If no square is found within the given bounds, returns a null Grip.Square.
    /// </summary>
    /// <param name="startingSquare"></param>
    /// <param name="direction"></param>
    /// <param name="gripLayer"></param>
    /// <param name="jumpLength"></param>
    /// <returns></returns>
    private Grip.Square FindGripSquareAfterJump(Grip.Square startingSquare, Vector2 direction, LayerMask gripLayer, int jumpLength)
    {
        direction.Normalize();
        if (direction != Vector2.left && direction != Vector2.right && direction != Vector2.up && direction != Vector2.down)
        {
            Debug.LogWarning("Given direction must be a cardinal direction");
            return new Grip.Square();
        }

        if (direction == Vector2.up)
        {
            bool foundGripsAreInSameSquare = false;
            
            Grip foundLeft = FindJumpableGripInDirection(startingSquare.upperLeft.transform.position, Vector2.up, gripLayer, jumpLength);
            Grip foundRight = FindJumpableGripInDirection(startingSquare.upperRight.transform.position, Vector2.up, gripLayer, jumpLength);
            if(!foundLeft.IsNull && !foundRight.IsNull)
            {
                
            }
            else
            {

            }


        }
        else if (direction == Vector2.right)
        {
            
        }
        else if (direction == Vector2.down)
        {
            
        }
        else if (direction == Vector2.left)
        {
 
        }
    }
    */

    /// <summary>
    /// Checks in the given direction and returns the first grip found. Returns an uninitialized Grip if none is found.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="startPosition"></param>
    /// <param name="gripLayer"></param>
    /// <param name="gripsToSkip"></param>
    /// <param name="jumpLength"></param>
    /// <returns></returns>
    private Grip FindJumpableGripInDirection(Vector2 startPosition, Vector2 direction, LayerMask gripLayer, int jumpLength, int gripsToSkip = 0)
    {
        Grip foundGrip = new Grip();
        for(int i = 1 + gripsToSkip; i < jumpLength; i++)
        {
            Vector2 checkPosition = startPosition + (direction * i);
            foundGrip = CheckAreaForGrip(checkPosition, Grip.HALF_GRIP_WIDTH, gripLayer);
            if (!foundGrip.IsNull)
            {
                break;
            }
        }

        return foundGrip;
    }
    

}
