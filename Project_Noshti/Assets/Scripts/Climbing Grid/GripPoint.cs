using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripPoint : MonoBehaviour
{
    [SerializeField]
    [Range(1,10)]
    private int quality;
    [SerializeField]
    private Type type;

    public enum Type
    {
        Backwall,
        Sidewall,
        Ladder
    };
}
