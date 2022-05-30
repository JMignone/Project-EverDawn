using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockupStats
{
    [SerializeField]
    private bool canKnockup;

    [Tooltip("By degault, knockup victims will be thrown away from the unit")]
    [SerializeField]
    private bool towardsUnit;

    [SerializeField] [Min(0)]
    private float distance;

    [SerializeField] [Min(0)]
    private float duration;

    public bool CanKnockup
    {
        get { return canKnockup; }
    }

    public bool TowardsUnit
    {
        get { return towardsUnit; }
    }

    public float Distance
    {
        get { return distance; }
    }

    public float Duration
    {
        get { return duration; }
    }
}
