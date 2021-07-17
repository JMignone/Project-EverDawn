using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrozenStats
{
    private bool isFrozen;
    private float frozenDelay;
    private float currentFrozenDelay;

    [SerializeField]
    private Component damageableComponent;

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

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void UpdateFrozenStats() {
        if(isFrozen) {
            if(currentFrozenDelay < frozenDelay) 
                currentFrozenDelay += Time.deltaTime;
            else
                unFreeze();
        }
    }

    void Freeze(float time) {
        isFrozen = true;
        frozenDelay = time;
        currentFrozenDelay = 0;
        (damageableComponent as IDamageable).UnitSprite.Animator.enabled = false;
        (damageableComponent as IDamageable).Target = null;
        (damageableComponent as IDamageable).Stats.CurrAttackDelay = 0;
    }

    void unFreeze() {
        isFrozen = false;
        (damageableComponent as IDamageable).UnitSprite.Animator.enabled = true;
    }
}