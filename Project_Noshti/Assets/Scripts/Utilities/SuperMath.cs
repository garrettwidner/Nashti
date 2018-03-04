using UnityEngine;
using System.Collections;

public static class SuperMath 
{
    public static float ZeroSign(float number)
    {
        if (number > 0)
        {
            return 1;
        }
        else if (number < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public static Vector2 ZeroSign(Vector2 vector)
    {
        return new Vector2(ZeroSign(vector.x), ZeroSign(vector.y));
    }

}
