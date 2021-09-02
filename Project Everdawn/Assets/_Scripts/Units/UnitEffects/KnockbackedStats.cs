using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KnockbackedStats
{
    [Tooltip("A number from 0 to 1 that determines a units resistance to being knockedback. A resistance of 1 means they cannot be knockedback")]
    [SerializeField]
    private float knockbackResistance;

    [SerializeField]
    private bool isKnockbacked;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private float currentKnockbackDelay;

    [SerializeField]
    private float initialSpeed;

    [SerializeField]
    private float currentSpeed;

    [SerializeField]
    private Vector3 direction;

    private Component damageableComponent;

    public float KnockbackResistance
    {
        get { return knockbackResistance; }
        set { knockbackResistance = value; }
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

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartKnockbackedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
        if(knockbackResistance < 0)
            knockbackResistance = 0;
    }

    public void UpdateKnockbackedStats() {
        if(isKnockbacked) {
            if(currentKnockbackDelay > 0) { //if we havnt reached the total duration yet
                currentSpeed = initialSpeed * (currentKnockbackDelay/knockbackDuration);
                (damageableComponent as IDamageable).Agent.transform.position += direction * currentSpeed * Time.deltaTime;
                currentKnockbackDelay -= Time.deltaTime;
            }
            else
                unKnockback();
        }
    }

    public void Knockback(float duration, float speed, Vector3 sourcePosition) {
        if(knockbackResistance < 1) {
            isKnockbacked = true;
            knockbackDuration = duration * (1 - knockbackResistance);
            currentKnockbackDelay = duration * (1 - knockbackResistance);
            initialSpeed = speed;
            currentSpeed = speed;
            (damageableComponent as IDamageable).SetTarget(null);
            (damageableComponent as IDamageable).Stats.CurrAttackDelay = 0;
            if(damageableComponent.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors to red
                foreach(Transform child in damageableComponent.transform.GetChild(1).GetChild(5).GetChild(2)) {
                    if(child.childCount > 0) //this means its a complicated summon preview
                        child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
                    else
                        child.GetComponent<Image>().color = new Color32(255,0,0,50);
                }
            }

            direction = ((damageableComponent as IDamageable).Agent.transform.position - sourcePosition).normalized;
            direction.y = 0;
        }
    }

    public void unKnockback() {
        isKnockbacked = false;
        if(damageableComponent.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors back to green
            foreach(Transform child in damageableComponent.transform.GetChild(1).GetChild(5).GetChild(2)) {
                if(child.childCount > 0) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
                else
                    child.GetComponent<Image>().color = new Color32(255,255,255,100);
            }
        }
    }
}
