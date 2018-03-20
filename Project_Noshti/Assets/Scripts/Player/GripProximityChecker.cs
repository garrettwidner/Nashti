using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GripProximityChecker : MonoBehaviour
{
    //[SerializeField] private LayerMask gripLayer;

    /// <summary>
    /// Returns closest grip point if found in proximity. If none found, returns null.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="proximalRadius"></param>
    /// <returns></returns>
    public static Grip CheckProximity(Vector2 center, float proximalRadius, LayerMask gripLayer)
    {
        Collider2D[] foundColliders = Physics2D.OverlapCircleAll(center, proximalRadius, gripLayer);
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

}
