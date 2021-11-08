using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlindedStats
{
    [SerializeField]
    private bool cantBeBlinded;
    private bool outSideResistance;

    private bool isBlinded;

    private float blindDelay;
    private float currentBlindDelay;
    private float blindVisionRadius;
    private float blindAttackRadius;

    //saved unit values
    private IDamageable unit;
    private float attackRadius;
    private float visionRadius;

    public bool CantBeBlinded
    {
        get { return cantBeBlinded; }
        set { cantBeBlinded = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsBlinded
    {
        get { return isBlinded; }
        set { isBlinded = value; }
    }

    public float BlindDelay
    {
        get { return blindDelay; }
        set { blindDelay = value; }
    }

    public float CurrentBlindDelay
    {
        get { return currentBlindDelay; }
        set { currentBlindDelay = value; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
        visionRadius = unit.Stats.VisionRange;
        attackRadius = unit.Stats.Range;
        isBlinded = false;
    }

    public void UpdateStats() {
        if(isBlinded) {
            if(currentBlindDelay < blindDelay) 
                currentBlindDelay += Time.deltaTime;
            else
                unBlind();
        }
    }

    public void Blind(float duration) {
        if(!cantBeBlinded && !outSideResistance) {
            isBlinded = true;
            blindDelay = duration;
            currentBlindDelay = 0;

            unit.Stats.Range = unit.Agent.HitBox.radius + 1.5f;
            unit.Stats.VisionRange = unit.Stats.Range + 1;
            (unit as IDamageable).SetTarget(null);
        }
    }

    public void unBlind() {
        isBlinded = false;
        unit.Stats.VisionRange = visionRadius;
        unit.Stats.Range = attackRadius;
    }

}
