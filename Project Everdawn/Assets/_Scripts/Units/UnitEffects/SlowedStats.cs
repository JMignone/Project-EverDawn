using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlowedStats
{
    [SerializeField]
    private bool cantBeSlowed;

    [SerializeField]
    private bool isSlowed;

    [SerializeField]
    private float slowDelay;

    [SerializeField]
    private float currentSlowDelay;

    [SerializeField]
    private float currentSlowIntensity;

    private Component damageableComponent;

    public bool CantBeSlowed
    {
        get { return cantBeSlowed; }
        set { cantBeSlowed = value; }
    }

    public bool IsSlowed
    {
        get { return isSlowed; }
        set { isSlowed = value; }
    }

    public float SlowDelay
    {
        get { return slowDelay; }
        set { slowDelay = value; }
    }

    public float CurrentSlowDelay
    {
        get { return currentSlowDelay; }
        set { currentSlowDelay = value; }
    }

    public float CurrentSlowIntensity
    {
        get { return currentSlowIntensity; }
        set { currentSlowIntensity = value; }
    }

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartSlowedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
        isSlowed = false;
        slowDelay = 0;
        currentSlowDelay = 0;
        currentSlowIntensity = 1;
    }

    public void UpdateSlowedStats() {
        if(isSlowed) {
            if(currentSlowDelay < slowDelay) 
                currentSlowDelay += Time.deltaTime;
            else
                unSlow();
        }
    }

    public void Slow(float duration, float intensity) {
        if(!cantBeSlowed) {
            isSlowed = true;
            slowDelay = duration;
            currentSlowDelay = 0;
            currentSlowIntensity = intensity;
            (damageableComponent as IDamageable).UnitSprite.Animator.speed = intensity;
            (damageableComponent as IDamageable).Stats.CurrAttackDelay = 0;
        }
    }

    public void unSlow() {
        isSlowed = false;
        currentSlowIntensity = 1;
        (damageableComponent as IDamageable).UnitSprite.Animator.speed = 1;
    }
}
