using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticuleDisplayer : MonoBehaviour
{
    [SerializeField] private PlayerClimbingController climbingController;
    public float distanceFromPlayer = 0.45f;
    public Transform playerTransform;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer leftRenderer;
    [SerializeField] private SpriteRenderer rightRenderer;

    [Header("Sprites")]
    [SerializeField] private ReticuleSprites leftSprites;
    [SerializeField] private ReticuleSprites rightSprites;

    private PlayerActions playerActions;

    private void Start()
    {
        playerActions = PlayerActions.CreateWithDefaultBindings();
    }

    private void Update()
    {
        if(climbingController.IsConnectingAfterJump)
        {
            return;
        }

        Vector2 vectorDirection = climbingController.LeaningDirection;

        if (vectorDirection == Vector2.zero)
        {
            EnableRenderers(false);
        }
        else
        {
            Orientation.Direction direction = Orientation.Vector2ToDirection(vectorDirection);
            PlayerClimbingController.Movement potentialMovement = climbingController.GetPotentialMovements.Vector2ToObject(climbingController.LeaningDirection);

            EnableRenderers(true);
            SetLocalScale(direction);
            SetRotation(direction);
            SetPosition(direction, potentialMovement);
            SetSprite(direction, potentialMovement.square);
        }
    }

    private void EnableRenderers(bool enabled)
    {
        if(enabled)
        {
            leftRenderer.enabled = true;
            rightRenderer.enabled = true;
        }
        else
        {
            leftRenderer.enabled = false;
            rightRenderer.enabled = false;
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

    private void SetRotation(Orientation.Direction direction)
    {
        switch (direction)
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

    private void SetPosition(Orientation.Direction direction, PlayerClimbingController.Movement potentialMovement)
    {
        if (potentialMovement.isJumpNecessary)
        {
            transform.position = potentialMovement.square.Center + (Orientation.DirectionToVector2(direction) * Grip.WIDTH_BETWEEN_GRIPS / 2);
        }
        else
        {
            switch (direction)
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

    private void SetSprite(Orientation.Direction direction, Grip.Square square)
    {
        Grip leftGrip = square.FindLeftSelectingGripGivenDirection(Orientation.DirectionToVector2(direction));
        bool leftExists = (leftGrip != null);
        bool leftIsPressed = playerActions.GripLeft.IsPressed;
        SetReticuleSprite(leftRenderer, leftExists, leftIsPressed, leftSprites);

        Grip rightGrip = square.FindRightSelectingGripGivenDirection(Orientation.DirectionToVector2(direction));
        bool rightExists = (rightGrip != null);
        bool rightIsPressed = playerActions.GripRight.IsPressed;
        SetReticuleSprite(rightRenderer, rightExists, rightIsPressed, rightSprites);
    }

    private void SetReticuleSprite(SpriteRenderer renderer, bool gripExists, bool gripIsPressed, ReticuleSprites sprites)
    {
        if(!gripExists)
        {
            if(gripIsPressed)
            {
                renderer.sprite = sprites.errored;
            }
            else
            {
                renderer.sprite = sprites.greyed;
            }
        }
        else
        {
            if (gripIsPressed)
            {
                renderer.sprite = sprites.highlighted;
            }
            else
            {
                renderer.sprite = sprites.selected;
            }
        }
    }

    [System.Serializable]
    private struct ReticuleSprites
    {
        public Sprite highlighted;
        public Sprite selected;
        public Sprite greyed;
        public Sprite errored;
    }


}
