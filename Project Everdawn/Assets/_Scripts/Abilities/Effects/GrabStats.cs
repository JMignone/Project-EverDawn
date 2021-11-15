using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrabStats
{
    [SerializeField]
    private bool canGrab;

    [SerializeField] [Min(0)]
    private float pullDuration;

    [Tooltip("If not set to 0, speed will override the duration")]
    [SerializeField] [Min(0)]
    private float speed;

    [SerializeField] [Min(0)]
    private float stunDuration;

    public bool CanGrab
    {
        get { return canGrab; }
    }

    public float PullDuration
    {
        get { return pullDuration; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public float StunDuration
    {
        get { return stunDuration; }
    }
}
