using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerClimbingController climbingController;

    public float distanceFromPlayer = 0.3f;
    public Transform playerTransform;

    private void Update()
    {
        if(climbingController.LeaningDirection != Vector2.zero)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }

        CardinalContainer<PlayerClimbingController.ProximalSquare> PotentialMovements = climbingController.GetPotentialMovements;

        if (climbingController.LeaningDirection == Vector2.up)
        {
            
            if(!PotentialMovements.up.foundSquare || !PotentialMovements.up.isJumpNecessary)
            {
                transform.position = new Vector3(0, distanceFromPlayer, 0) + playerTransform.position;
            }
            else
            {
                transform.position = PotentialMovements.up.square.Center + (Vector2.up * Grip.WIDTH_BETWEEN_GRIPS / 2);
            }
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.right)
        {
            if (!PotentialMovements.right.foundSquare || !PotentialMovements.right.isJumpNecessary)
            {
                transform.position = new Vector3(distanceFromPlayer, 0 , 0) + playerTransform.position;
            }
            else
            {
                transform.position = PotentialMovements.right.square.Center + (Vector2.right * Grip.WIDTH_BETWEEN_GRIPS / 2);
            }
            transform.rotation = Quaternion.Euler(0, 0, -90);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.down)
        {
            if (!PotentialMovements.down.foundSquare || !PotentialMovements.down.isJumpNecessary)
            {
                transform.position = new Vector3(0, -distanceFromPlayer, 0) + playerTransform.position;
            }
            else
            {
                transform.position = PotentialMovements.down.square.Center + (Vector2.down * Grip.WIDTH_BETWEEN_GRIPS / 2);
            }
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, -1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.left)
        {
            if (!PotentialMovements.down.foundSquare || !PotentialMovements.down.isJumpNecessary)
            {
                transform.position = new Vector3(-distanceFromPlayer, 0, 0) + playerTransform.position;
            }
            else
            {
                transform.position = PotentialMovements.left.square.Center + (Vector2.left * Grip.WIDTH_BETWEEN_GRIPS / 2);
            }
            transform.rotation = Quaternion.Euler(0, 0, 90);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
