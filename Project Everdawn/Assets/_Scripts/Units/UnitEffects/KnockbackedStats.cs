using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KnockbackedStats
{
    [Tooltip("A number from 0 to 1 that determines a units resistance to being knockedback. A resistance of 1 means they cannot be knockedback")]
    [SerializeField]  [Range(0,1)]
    private float knockbackResistance;
    private bool outSideResistance;

    [SerializeField]
    private bool isKnockbacked;

    [SerializeField] [Min(0)]
    private float knockbackDuration;

    [SerializeField] [Min(0)]
    private float currentKnockbackDelay;

    [SerializeField] [Min(0)]
    private float initialSpeed;

    [SerializeField] [Min(0)]
    private float currentSpeed;

    [SerializeField]
    private Vector3 direction;

    private IDamageable unit;

    public float KnockbackResistance
    {
        get { return knockbackResistance; }
        set { knockbackResistance = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsKnockbacked
    {
        get { return isKnockbacked; }
        set { isKnockbacked = value; }
    }

    public float KnockbackDuration
    {
        get { return knockbackDuration; }
        set { knockbackDuration = value; }
    }

    public float CurrentKnockbackDelay
    {
        get { return currentKnockbackDelay; }
        set { currentKnockbackDelay = value; }
    }

    public float InitialSpeed
    {
        get { return initialSpeed; }
        set { initialSpeed = value; }
    }

    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
    }

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public void StartKnockbackedStats(IDamageable go) {
        unit = go;
        if(knockbackResistance < 0)
            knockbackResistance = 0;
    }

    public void UpdateKnockbackedStats() {
        if(isKnockbacked) {
            if(currentKnockbackDelay > 0) { //if we havnt reached the total duration yet
                currentSpeed = initialSpeed * (currentKnockbackDelay/knockbackDuration);
                unit.Agent.transform.position += direction * currentSpeed * Time.deltaTime;
                currentKnockbackDelay -= Time.deltaTime;
            }
            else
                unKnockback();
        }
    }

    public void Knockback(float duration, float speed, Vector3 sourcePosition) {
        if(knockbackResistance < 1 && !outSideResistance) {
            direction = unit.Agent.transform.position - sourcePosition;
            direction.y = 0;
            direction = direction.normalized;

            if(unit.JumpStats.Jumping) {
                unit.JumpStats.DirectionalInfluence(direction * speed * 10000);
                return;
            }

            isKnockbacked = true;
            knockbackDuration = duration * (1 - knockbackResistance);
            currentKnockbackDelay = duration * (1 - knockbackResistance);
            initialSpeed = speed;
            currentSpeed = speed;
            unit.SetTarget(null);
            unit.Stats.IsCastingAbility = false; //normally this is done automatically, but some abilitys use the 'abilityOverride' AND it doesnt set isCastingAbility via just getting destroyed, so we will need to set it
            GameFunctions.DisableAbilities((unit as Component).gameObject);

        }
    }

    public void unKnockback() {
        isKnockbacked = false;
        GameFunctions.EnableAbilities((unit as Component).gameObject);
    }
}
