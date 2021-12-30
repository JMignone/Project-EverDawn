using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrengthStats
{
    [SerializeField]
    private bool canStrength;

    [SerializeField] [Min(0)]
    private float strengthDuration;

    [Tooltip("A number from 1 to infinity determining what percentage the effective unit will deal damage.")]
    [SerializeField] [Min(1)]
    private float strengthIntensity;

    public bool CanStrength
    {
        get { return canStrength; }
    }

    public float StrengthDuration
    {
        get { return strengthDuration; }
    }

    public float StrengthIntensity
    {
        get { return strengthIntensity; }
    }

    public void StartStrengthStats()
    {
        if(strengthIntensity < 1)
            strengthIntensity = 1;
    }
}
