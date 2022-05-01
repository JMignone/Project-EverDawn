using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StunnedStats
{
    [SerializeField]
    private bool cantBeStunned;
    [SerializeField]
    private bool outSideResistance;

    [SerializeField]
    private bool isStunned;

    [SerializeField] [Min(0)]
    private float stunDelay;
    private float currentDelay;

    private IDamageable unit;

    public bool CantBeStunned
    {
        get { return cantBeStunned; }
        set { cantBeStunned = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsStunned
    {
        get { return isStunned; }
        set { isStunned = value; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
        isStunned = false;
        stunDelay = 0;
    }

    public void UpdateStats() {
        if(isStunned) {
            if(currentDelay < stunDelay) 
                currentDelay += Time.deltaTime;
            else
                unStun();
        }
    }

    public void Stun(float duration) {
        if(!cantBeStunned && !outSideResistance) {
            isStunned = true;
            stunDelay = duration;
            currentDelay = 0;
            unit.SetTarget(null);
            unit.Stats.IsCastingAbility = false; //normally this is done automatically, but some abilitys use the 'abilityOverride', so we will need to set it
            GameFunctions.DisableAbilities(unit);
        }   
    }

    public void unStun() {
        isStunned = false;
        GameFunctions.EnableAbilities(unit);
    }
}
