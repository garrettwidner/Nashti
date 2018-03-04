using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorGridSnap : MonoBehaviour
{

    public float cell_size = 0.5f; 
    private float x, y, z;

    void Start()
    {
        if (!Application.isPlaying)
        {
            x = 0f;
            y = 0f;
            z = 0f;
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            x = Mathf.Round(transform.position.x / cell_size) * cell_size;
            y = Mathf.Round(transform.position.y / cell_size) * cell_size;
            z = transform.position.z;
            transform.position = new Vector3(x, y, z);
        }
    }

}
