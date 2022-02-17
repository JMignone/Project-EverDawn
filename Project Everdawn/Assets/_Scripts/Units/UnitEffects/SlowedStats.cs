using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlowedStats
{
    [SerializeField]
    private bool cantBeSlowed;
    private bool outSideResistance;

    [SerializeField]
    private bool isSlowed;

    [SerializeField]
    private float slowDelay;

    [SerializeField]
    private float currentSlowDelay;

    [SerializeField]
    private float currentSlowIntensity;

    private IDamageable unit;

    public bool CantBeSlowed
    {
        get { return cantBeSlowed; }
        set { cantBeSlowed = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
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

    public void StartSlowedStats(IDamageable go) {
        unit = go;
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
        if(!cantBeSlowed && !outSideResistance) {
            isSlowed = true;
            slowDelay = duration;
            currentSlowDelay = 0;
            currentSlowIntensity = intensity;
            unit.UnitSprite.Animator.speed = intensity;
        }
    }

    public void unSlow() {
        isSlowed = false;
        currentSlowIntensity = 1;
        unit.UnitSprite.Animator.speed = 1;
    }
}
