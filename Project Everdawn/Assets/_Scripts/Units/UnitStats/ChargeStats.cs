using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeStats
{
    private IDamageable unit;

    [Tooltip("Makes the unit charge fast after moving for a short time")]
    [SerializeField]
    private bool charges;
    [SerializeField]
    private bool isCharging;

    [SerializeField]
    private float chargeDamage;

    [SerializeField]
    private float chargeSpeed;
    private float speed;

    [Tooltip("Determines the amount of time walking before starting to charge")]
    [SerializeField]
    private float chargeDelay;

    [SerializeField]
    private float currentDelay;

    public bool Charges
    {
        get { return charges; }
    }

    public bool IsCharging
    {
        get { return isCharging; }
    }

    public void StartChargeStats(IDamageable go) {
        unit = go;
        speed = unit.Stats.MoveSpeed;
    }

    public void UpdateChargeStats() {
        if(charges) {
            if(unit.IsMoving && !unit.Stats.IsAttacking && !isCharging && unit.Stats.CanAct && !unit.Stats.IsCastingAbility) {
                if(currentDelay < chargeDelay) 
                    currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
                else {
                    currentDelay = 0;
                    isCharging = true;
                }
            }
            else
                currentDelay = 0;
            Charge();
        }
    }

    private void Charge() {
        if(isCharging && unit.Stats.CanAct && !unit.Stats.IsCastingAbility) {
            unit.Stats.MoveSpeed = chargeSpeed;
            if(unit.InRangeTargets.Contains(unit.Target)) {
                ChargeAttack();
                isCharging = false;
            }
        }
        else {
            unit.Stats.MoveSpeed = speed;
            isCharging = false;
        }
    }

    private void ChargeAttack() {
        Component damageable = unit.Target.GetComponent(typeof(IDamageable));
        if(damageable) { //is the target damageable
            if(unit.Stats.EffectStats.AOEStats.AreaOfEffect)
                unit.Stats.EffectStats.AOEStats.Explode((unit as Component).gameObject, unit.Target, chargeDamage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
            else {
                GameFunctions.Attack(damageable, chargeDamage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity, unit.Stats.EffectStats.CritStats);
                unit.Stats.ApplyAffects(damageable);
            }
            unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent);
        }
    }
}
