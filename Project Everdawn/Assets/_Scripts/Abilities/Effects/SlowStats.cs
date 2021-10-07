using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlowStats
{
    [SerializeField]
    private bool canSlow;

    [SerializeField] [Min(0)]
    private float slowDuration;

    [Tooltip("A number from 0 to 1 determining what percentage the effective unit will move from its original speed")]
    [SerializeField] [Range(0,1)]
    private float slowIntensity;

    public bool CanSlow
    {
        get { return canSlow; }
    }

    public float SlowDuration
    {
        get { return slowDuration; }
    }

    public float SlowIntensity
    {
        get { return slowIntensity; }
    }

    public void StartSlowStats()
    {
        if(slowIntensity > 1)
            slowIntensity = 1;
        else if(slowIntensity < 0)
            slowIntensity = 0;
    }
}
