using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlindStats
{
    [SerializeField]
    private bool canBlind;

    [SerializeField] [Min(0)]
    private float blindDuration;

    public bool CanBlind
    {
        get { return canBlind; }
    }

    public float BlindDuration
    {
        get { return blindDuration; }
    }
}
