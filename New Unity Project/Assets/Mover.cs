using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 1f;

    private void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = (Vector2)transform.position + (Vector2.up * speed);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = (Vector2)transform.position + (Vector2.right * speed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = (Vector2)transform.position + (Vector2.down * speed);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = (Vector2)transform.position + (Vector2.left * speed);
        }
    }
}
