using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrabStats
{
    [SerializeField]
    private bool canGrab;

    [SerializeField]
    private float pullDuration;

    [SerializeField]
    private float stunDuration;

    public bool CanGrab
    {
        get { return canGrab; }
    }

    public float PullDuration
    {
        get { return pullDuration; }
    }

    public float StunDuration
    {
        get { return stunDuration; }
    }
}
