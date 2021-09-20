using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FrozenStats
{
    [SerializeField]
    private bool cantBeFrozen;
    [SerializeField]
    private bool outSideResistance;

    [SerializeField]
    private bool isFrozen;

    [SerializeField]
    private float frozenDelay;

    [SerializeField]
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

    public void StartFrozenStats(GameObject go) {
        unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
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
            isFrozen = true;
            frozenDelay = duration;
            currentFrozenDelay = 0;
            unit.UnitSprite.Animator.enabled = false;
            unit.SetTarget(null);
            unit.Stats.CurrAttackDelay = 0;
            if((unit as Component).gameObject.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors to red
                foreach(Transform child in (unit as Component).gameObject.transform.GetChild(1).GetChild(5).GetChild(2)) {
                    if(child.childCount > 0) //this means its a complicated summon preview
                        child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
                    else
                        child.GetComponent<Image>().color = new Color32(255,0,0,50);
                }
            }
        }   
    }

    public void unFreeze() {
        isFrozen = false;
        unit.UnitSprite.Animator.enabled = true;
        if((unit as Component).gameObject.transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors back to green
            foreach(Transform child in (unit as Component).gameObject.transform.GetChild(1).GetChild(5).GetChild(2)) {
                if(child.childCount > 0) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
                else
                    child.GetComponent<Image>().color = new Color32(255,255,255,100);
            }
        }
    }
}
