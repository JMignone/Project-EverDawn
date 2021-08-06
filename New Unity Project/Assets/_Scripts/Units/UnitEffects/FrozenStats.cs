using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FrozenStats
{
    private bool isFrozen;

    [SerializeField]
    private float frozenDelay;

    [SerializeField]
    private float currentFrozenDelay;

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

    public void StartFrozenStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
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
        isFrozen = true;
        frozenDelay = duration;
        currentFrozenDelay = 0;
        (damageableComponent as IDamageable).UnitSprite.Animator.enabled = false;
        (damageableComponent as IDamageable).Target = null;
        (damageableComponent as IDamageable).Stats.CurrAttackDelay = 0;
        if(damageableComponent.transform.GetChild(1).GetChild(4).childCount > 1) { //if the unit has an ability, set its image colors to red
            foreach(Transform child in damageableComponent.transform.GetChild(1).GetChild(4).GetChild(2)) {
                if(child.childCount > 0) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
                else
                    child.GetComponent<Image>().color = new Color32(255,0,0,50);
            }
        }
            
    }

    public void unFreeze() {
        isFrozen = false;
        (damageableComponent as IDamageable).UnitSprite.Animator.enabled = true;
        if(damageableComponent.transform.GetChild(1).GetChild(4).childCount > 1) { //if the unit has an ability, set its image colors back to green
            foreach(Transform child in damageableComponent.transform.GetChild(1).GetChild(4).GetChild(2)) {
                if(child.childCount > 0) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
                else
                    child.GetComponent<Image>().color = new Color32(255,255,255,100);
            }
        }
    }
}
