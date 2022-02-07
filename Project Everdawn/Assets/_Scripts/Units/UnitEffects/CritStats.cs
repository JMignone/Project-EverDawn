using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CritStats
{
    [SerializeField] [Min(1)]
    private float critOnFrozen;
    [SerializeField] [Min(1)]
    private float critOnSlow;
    [SerializeField] [Min(1)]
    private float critOnRoot;
    [SerializeField] [Min(1)]
    private float critOnPoison;
    [SerializeField] [Min(1)]
    private float critOnBlind;
    [SerializeField] [Min(1)]
    private float critOnStun;

    public float CritOnFrozen
    {
        get { return critOnFrozen; }
    }

    public float CritOnSlow
    {
        get { return critOnSlow; }
    }

    public float CritOnRoot
    {
        get { return critOnRoot; }
    }

    public float CritOnPoison
    {
        get { return critOnPoison; }
    }

    public float CritOnBlind
    {
        get { return critOnBlind; }
    }

    public float CritOnStun
    {
        get { return critOnStun; }
    }
}
