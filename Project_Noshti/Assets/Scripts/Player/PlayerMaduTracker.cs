using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Responsible for sensing any Madu consumed or come into contact with and alerting the PlayerGripLevelManager of any changes
/// </summary>
public class PlayerMaduTracker : MonoBehaviour
{
    [SerializeField] private BoxCollider2D checkableArea;
    [SerializeField] private LayerMask edibleMaduLayer;
    [SerializeField] private LayerMask touchableMaduLayer;

    [SerializeField] private MaduEvent OnMaduUtilized;

    public MaduObject CurrentEdible
    {
        get
        {
            return currentEdible;
        }
    }

    private bool isChecking;
    private MaduObject currentEdible;


    private void Update()
    {
        if(isChecking)
        {
            CheckForEdible();
            CheckForTouchable();
        }
    }

    private void CheckForEdible()
    {
        Collider2D foundCollider = Physics2D.OverlapBox(checkableArea.bounds.center, checkableArea.bounds.size, 0f, edibleMaduLayer);
        MaduObject foundEdible = foundCollider.GetComponent<MaduObject>();
        if(foundEdible != null && foundEdible.IsEdible)
        {
            currentEdible = foundEdible;
        }
        else
        {
            currentEdible = null;
        }
    }

    private void EatCurrentEdible()
    {
        if(currentEdible != null && OnMaduUtilized != null)
        {
            OnMaduUtilized.Invoke(currentEdible.Madu);
            if(currentEdible.IsDestroyedAfterUse)
            {
                currentEdible.DestroySelf();
                currentEdible = null;
            }
        }
    }

    private void CheckForTouchable()
    {
        Collider2D foundCollider = Physics2D.OverlapBox(checkableArea.bounds.center, checkableArea.bounds.size, 0f, touchableMaduLayer);
        MaduObject foundTouchable = foundCollider.GetComponent<MaduObject>();
        if(foundTouchable != null && !foundTouchable.IsEdible)
        {
            if(OnMaduUtilized != null)
            {
                OnMaduUtilized.Invoke(foundTouchable.Madu);
            }
        }
    }

}

[System.Serializable]
public class MaduEvent : UnityEvent<float>
{

}
