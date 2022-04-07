using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrozenStats
{
    [SerializeField]
    private bool cantBeFrozen;
    [SerializeField]
    private bool outSideResistance;

    [SerializeField]
    private bool isFrozen;

    [SerializeField] [Min(0)]
    private float frozenDelay;
    private float currentFrozenDelay;

    private IDamageable unit;

    public bool CantBeFrozen
    {
        get { return cantBeFrozen; }
        set { cantBeFrozen = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsFrozen
    {
        get { return isFrozen; }
        set { isFrozen = value; }
    }

    public float FrozenDelay
    {
        get { return frozenDelay; }
        set { frozenDelay = value; }
    }

    public float CurrentFrozenDelay
    {
        get { return currentFrozenDelay; }
        set { currentFrozenDelay = value; }
    }

    public void StartFrozenStats(IDamageable go) {
        unit = go;
        isFrozen = false;
        frozenDelay = 0;
        currentFrozenDelay = 0;
    }

    public void UpdateFrozenStats() {
        if(isFrozen) {
            if(currentFrozenDelay < frozenDelay) 
                currentFrozenDelay += Time.deltaTime;
            else
                unFreeze();
        }
    }

    public void Freeze(float duration) {
        if(!cantBeFrozen && !outSideResistance) {
            if(!isFrozen)
                unit.Stats.UnitMaterials.TintCyan();

            isFrozen = true;
            frozenDelay = duration;
            currentFrozenDelay = 0;
            if (unit.UnitSprite.Animator != null)
            {
                unit.UnitSprite.Animator.enabled = false;
            }
            unit.SetTarget(null);
            unit.Stats.IsCastingAbility = false; //normally this is done automatically, but some abilitys use the 'abilityOverride', so we will need to set it
            GameFunctions.DisableAbilities((unit as Component).gameObject);
        }   
    }

    public void unFreeze() {
        unit.Stats.UnitMaterials.RemoveCyan();

        isFrozen = false;
        if (unit.UnitSprite.Animator != null)
        {
            unit.UnitSprite.Animator.enabled = true;
        }
        GameFunctions.EnableAbilities((unit as Component).gameObject);
    }
}
