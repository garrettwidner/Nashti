using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalContainer
{
    public class Cardinal<T>
    {
        public T up;
        public T right;
        public T down;
        public T left;
    }

    public class Ordinal<T>
    {
        public T upperRight;
        public T lowerRight;
        public T lowerLeft;
        public T upperLeft;
    }


}
