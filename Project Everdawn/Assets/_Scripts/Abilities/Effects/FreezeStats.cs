using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FreezeStats
{
    [SerializeField]
    private bool canFreeze;

    [SerializeField] [Min(0)]
    private float freezeDuration;

    public bool CanFreeze
    {
        get { return canFreeze; }
    }

    public float FreezeDuration
    {
        get { return freezeDuration; }
    }
}
