using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoisonedStats
{
    [SerializeField]
    private bool cantBePoisoned;
    private bool outSideResistance;

    [SerializeField]
    private bool isPoisoned;
    private float poisonedDamage;

    [SerializeField] [Min(0)]
    private float poisonedDuration;

    [SerializeField] [Min(0)]
    private float poisonedTick;

    [SerializeField] [Min(0)]
    private float currentPoisonDelay;

    private IDamageable unit;

    public bool CantBePoisoned
    {
        get { return cantBePoisoned; }
        set { cantBePoisoned = value; }
    }
    
    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsPoisoned
    {
        get { return isPoisoned; }
        set { isPoisoned = value; }
    }

    public float PoisonedDamage
    {
        get { return poisonedDamage; }
        set { poisonedDamage = value; }
    }

    public float PoisonedDuration
    {
        get { return poisonedDuration; }
        set { poisonedDuration = value; }
    }

    public float PoisonedTick
    {
        get { return poisonedTick; }
    }

    public float CurrentPoisonDelay
    {
        get { return currentPoisonDelay; }
        set { currentPoisonDelay = value; }
    }

    public void StartPoisonedStats(IDamageable go) {
        unit = go;
        isPoisoned = false;
    }

    public void UpdatePoisonedStats() {
        if(isPoisoned) {
            if(poisonedDuration > 0) { //if we havnt reached the total duration yet
                if(currentPoisonDelay < poisonedTick)
                    currentPoisonDelay += Time.deltaTime;
                else {
                    GameFunctions.Attack((unit as Component), poisonedDamage);
                    currentPoisonDelay = 0;
                    poisonedDuration -= poisonedTick;
                }
            }
            else {
                isPoisoned = false;

                unit.Stats.UnitMaterials.RemovePurple();
            }
        }
    }

    public void Poison(float duration, float tick, float damage) {
        if(!cantBePoisoned && !outSideResistance) {
            if(!isPoisoned)
                unit.Stats.UnitMaterials.TintPurple();

            isPoisoned = true;
            poisonedDuration = duration;
            poisonedTick = tick;
            currentPoisonDelay = 0;
            poisonedDamage = damage;
        }
    }
}
