using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoisonStats
{
    [SerializeField]
    private bool canPoison;

    [SerializeField]
    private float poisonDamage;

    [SerializeField]
    private float poisonDuration;

    [SerializeField]
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
