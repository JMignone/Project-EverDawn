using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootStats
{
    [SerializeField]
    private bool canRoot;

    [SerializeField] [Min(0)]
    private float rootDuration;

    public bool CanRoot
    {
        get { return canRoot; }
    }

    public float RootDuration
    {
        get { return rootDuration; }
    }
}
