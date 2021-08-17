using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockbackStats
{
    [SerializeField]
    private bool canKnockback;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private float initialSpeed;

    public bool CanKnockback
    {
        get { return canKnockback; }
    }

    public float KnockbackDuration
    {
        get { return knockbackDuration; }
    }

    public float InitialSpeed
    {
        get { return initialSpeed; }
    }
}
