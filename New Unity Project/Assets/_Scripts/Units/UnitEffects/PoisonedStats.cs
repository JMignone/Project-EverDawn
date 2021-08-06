using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoisonedStats
{
    private bool isPoisoned;
    private float poisonedDamage;

    [SerializeField]
    private float poisonedDuration;

    [SerializeField]
    private float poisonedTick;

    [SerializeField]
    private float currentPoisonDelay;

    private Component damageableComponent;

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

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartPoisonedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
        currentPoisonDelay = 0;
    }

    public void UpdatePoisonedStats() {
        if(isPoisoned) {
            if(poisonedDuration > 0) { //if we havnt reached the total duration yet
                if(currentPoisonDelay < poisonedTick)
                    currentPoisonDelay += Time.deltaTime;
                else {
                    GameFunctions.Attack(damageableComponent, poisonedDamage);
                    currentPoisonDelay = 0;
                    poisonedDuration -= poisonedTick;
                }
            }
            else
                isPoisoned = false;
        }
    }

    public void Poison(float duration, float tick, float damage) {
        isPoisoned = true;
        poisonedDuration = duration;
        poisonedTick = tick;
        currentPoisonDelay = 0;
        poisonedDamage = damage;
    }

    public void unSlow() {
        isPoisoned = false;
    }
}
