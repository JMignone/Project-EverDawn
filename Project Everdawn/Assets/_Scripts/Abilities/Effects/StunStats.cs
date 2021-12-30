using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StunStats
{
    [SerializeField]
    private bool canStun;

    [SerializeField] [Min(0)]
    private float stunDuration;

    public bool CanStun
    {
        get { return canStun; }
    }

    public float StunDuration
    {
        get { return stunDuration; }
    }
}
