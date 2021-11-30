using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrabStats
{
    [SerializeField]
    private bool canGrab;

    [SerializeField]
    private bool obstaclesBlockGrab;

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

    public bool ObstaclesBlockGrab
    {
        get { return obstaclesBlockGrab; }
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
