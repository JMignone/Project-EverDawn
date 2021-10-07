using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoisonStats
{
    [SerializeField]
    private bool canPoison;

    [SerializeField] [Min(0)]
    private float poisonDamage;

    [SerializeField] [Min(0)]
    private float poisonDuration;

    [SerializeField] [Min(0)]
    private float poisonTick;

    public bool CanPoison
    {
        get { return canPoison; }
    }

    public float PoisonDamage
    {
        get { return poisonDamage; }
    }

    public float PoisonDuration
    {
        get { return poisonDuration; }
    }

    public float PoisonTick
    {
        get { return poisonTick; }
    }
}
