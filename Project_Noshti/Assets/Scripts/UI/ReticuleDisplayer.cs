using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleDisplayer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerClimbingController climbingController;

    public float distanceFromPlayer = 0.3f;
    public Transform playerTransform;

    /*
    private void SetEnabledStatus(Vector2 vectorDirection, bool squareWasFound)
    {
        if (vectorDirection == Vector2.zero || !squareWasFound)
        {
            spriteRenderer.enabled = false;
            return;
        }
        else
        {
            spriteRenderer.enabled = true;
        }
    }
    */

    private void SetRotation(Orientation.Direction direction)
    {
        switch(direction)
        {
            case Orientation.Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            case Orientation.Direction.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                return;
            case Orientation.Direction.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            case Orientation.Direction.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                return;
            default:
                Debug.LogWarning("Rotation not set. Can only set rotation with a cardinal direction.");
                return;
        }
    }

    private void SetLocalScale(Orientation.Direction direction)
    {
        switch (direction)
        {
            case Orientation.Direction.Up:
                transform.localScale = new Vector3(1, 1, 1);
                return;
            case Orientation.Direction.Right:
                transform.localScale = new Vector3(1, 1, 1);
                return;
            case Orientation.Direction.Down:
                transform.localScale = new Vector3(1, -1, 1);
                return;
            case Orientation.Direction.Left:
                transform.localScale = new Vector3(1, 1, 1);
                return;
            default:
                Debug.LogWarning("Local scale not set. Can only set local scale with a cardinal direction.");
                return;
        }
    }

    private void SetPosition(Orientation.Direction direction, PlayerClimbingController.Movement potentialMovement)
    {
        if(potentialMovement.isJumpNecessary)
        {
            transform.position = potentialMovement.square.Center + (Orientation.DirectionToVector2(direction) * Grip.WIDTH_BETWEEN_GRIPS / 2);
        }
        else
        {
            switch(direction)
            {
                case Orientation.Direction.Up:
                    transform.position = new Vector3(0, distanceFromPlayer, 0) + playerTransform.position;
                    return;
                case Orientation.Direction.Right:
                    transform.position = new Vector3(distanceFromPlayer, 0, 0) + playerTransform.position;
                    return;
                case Orientation.Direction.Down:
                    transform.position = new Vector3(0, -distanceFromPlayer, 0) + playerTransform.position;
                    return;
                case Orientation.Direction.Left:
                    transform.position = new Vector3(-distanceFromPlayer, 0, 0) + playerTransform.position;
                    return;
                default:
                    Debug.LogWarning("Given direction must be cardinal");
                    return;
            }
        }
    }

    private void Update()
    {
        Vector2 vectorDirection = climbingController.LeaningDirection;
        Orientation.Direction direction = Orientation.Vector2ToDirection(vectorDirection);
        PlayerClimbingController.Movement potentialMovement = climbingController.GetPotentialMovements.Vector2ToObject(climbingController.LeaningDirection);

        if (vectorDirection == Vector2.zero || !potentialMovement.foundSquare)
        {
            spriteRenderer.enabled = false;
            return;
        }
        else
        {
            spriteRenderer.enabled = true;
        }


        SetLocalScale(direction);
        SetRotation(direction);
        SetPosition(direction, potentialMovement);


    }
}
