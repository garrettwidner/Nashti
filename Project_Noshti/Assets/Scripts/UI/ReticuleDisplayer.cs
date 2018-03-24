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

        if (climbingController.LeaningDirection == Vector2.up)
        {
            transform.position = new Vector3(0, distanceFromPlayer, 0) + playerTransform.position;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.right)
        {
            transform.position = new Vector3(distanceFromPlayer, 0, 0) + playerTransform.position;
            transform.rotation = Quaternion.Euler(0, 0, -90);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.down)
        {
            transform.position = new Vector3(0, -distanceFromPlayer, 0) + playerTransform.position;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, -1, 1);
        }
        else if (climbingController.LeaningDirection == Vector2.left)
        {
            transform.position = new Vector3(-distanceFromPlayer, 0, 0) + playerTransform.position;
            transform.rotation = Quaternion.Euler(0, 0, 90);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
