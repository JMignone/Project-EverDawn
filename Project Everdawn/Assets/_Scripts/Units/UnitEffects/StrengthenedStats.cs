using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrengthenedStats
{
    [SerializeField]
    private bool cantBeStrengthened;
    private bool outSideResistance;

    [SerializeField]
    private bool isStrengthened;

    [SerializeField]
    private float strengthDelay;

    [SerializeField]
    private float currentDelay;

    [SerializeField]
    private float currentStrengthIntensity;

    private Component damageableComponent;

    public bool CantBeStrengthened
    {
        get { return cantBeStrengthened; }
        set { cantBeStrengthened = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsStrengthened
    {
        get { return isStrengthened; }
        set { isStrengthened = value; }
    }

    public float StrengthDelay
    {
        get { return strengthDelay; }
        set { strengthDelay = value; }
    }

    public float CurrentDelay
    {
        get { return currentDelay; }
        set { currentDelay = value; }
    }

    public float CurrentStrengthIntensity
    {
        get { return currentStrengthIntensity; }
        set { currentStrengthIntensity = value; }
    }

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartStrengthenedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
        currentStrengthIntensity = 1;
    }

    public void UpdateStrengthenedStats() {
        if(isStrengthened) {
            if(currentDelay < strengthDelay) 
                currentDelay += Time.deltaTime;
            else
                unStrengthen();
        }
    }

    public void Strengthen(float duration, float intensity) {
        if(!cantBeStrengthened && !outSideResistance) {
            isStrengthened = true;
            strengthDelay = duration;
            currentDelay = 0;
            currentStrengthIntensity = intensity;
        }
    }

    public void unStrengthen() {
        isStrengthened = false;
        currentStrengthIntensity = 1;
    }
}
