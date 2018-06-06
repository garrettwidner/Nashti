using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaduObject : MonoBehaviour
{
    [SerializeField] private float madu;
    [SerializeField] private bool isDestroyedAfterUse;
    [SerializeField] private bool isEdible;

    

    public float Madu
    {
        get
        {
            return madu;
        }
    }

    public bool IsDestroyedAfterUse
    {
        get
        {
            return isDestroyedAfterUse;
        }
    }

    public bool IsEdible
    {
        get
        {
            return isEdible;
        }
    }

    public virtual void DestroySelf()
    {
        Destroy(this);
    }




}
