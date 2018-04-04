using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripSquareDebugger : MonoBehaviour
{
    public Grip grip1;
    public Grip grip2;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            print("Grips are in same grip square: " + grip1.IsInSameSquareAs(grip2));
            SuperDebugger.DrawPlus(grip1.transform.position, Color.green, .2f, 3);
            SuperDebugger.DrawPlus(grip2.transform.position, Color.red, .2f, 3);
        }
    }
}
