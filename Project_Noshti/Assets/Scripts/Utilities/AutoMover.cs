using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    public Vector2 direction;
    public float speed;
    public float slowSpeed;

    bool isMoving = true;
    bool isSlowed = false;

    private void Update()
    {
        if(isMoving)
        {
            float currentSpeed = isSlowed ? slowSpeed : speed;
            Vector2 movementVector = new Vector2(direction.x * currentSpeed * Time.deltaTime, direction.y * currentSpeed * Time.deltaTime);
            Vector3 current = transform.position;
            transform.position = new Vector3(current.x + movementVector.x, current.y + movementVector.y, current.z);
        }
    }

    public void StartMovement()
    {
        isMoving = true;
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void SetSpeedSlow()
    {
        isSlowed = true;
    }

    public void SetSpeedNormal()
    {
        isSlowed = false;
    }
}
