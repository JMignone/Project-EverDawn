using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildUpStats
{
    private IDamageable unit;

    private float initialDamage;
    private float initialAttackDelay;

    [SerializeField]
    private bool buildsUp;

    [Tooltip("If enabled, the unit will retain its power while a new target is within its range")]
    [SerializeField]
    private bool retainOnRetarget;

    [Tooltip("Determines how much the units damage increases at the end of each delay")]
    [SerializeField] [Min(0)]
    private float damageIncrease;

    [Tooltip("Determines how much the units attack delay decreases at the end of each delay")]
    [SerializeField] [Min(0)]
    private float delayDecrease;

    [Tooltip("The maximum damage for the unit")]
    [SerializeField]
    private float damageCap;

    [Tooltip("The minimum delay for the unit")]
    [SerializeField] [Min(0)]
    private float delayCap;

    public void StartStats(IDamageable go) {
        unit = go;
        initialDamage = unit.Stats.BaseDamage;
        initialAttackDelay = unit.Stats.AttackDelay;
    }

    public void UpdateStats() {
        if(buildsUp && !unit.Stats.IsAttacking) {
            unit.Stats.BaseDamage = initialDamage;
            unit.Stats.AttackDelay = initialAttackDelay;
        }
    }

    public void BuildUp() {
        if(buildsUp) {
            if(unit.Stats.BaseDamage < damageCap)
                unit.Stats.BaseDamage += damageIncrease;
            if(unit.Stats.AttackDelay > delayCap)
                unit.Stats.AttackDelay -= delayDecrease;
        }
    }

    public void ResetStats(bool retargeting) {
        if(buildsUp) {
            if(!(retargeting && retainOnRetarget)) {
                unit.Stats.BaseDamage = initialDamage;
                unit.Stats.AttackDelay = initialAttackDelay;
            }
        }
    }

}
