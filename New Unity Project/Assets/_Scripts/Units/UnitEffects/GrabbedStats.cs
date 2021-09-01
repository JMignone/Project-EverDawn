using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GrabbedStats
{
    [SerializeField]
    private bool cantBeGrabbed;

    [SerializeField]
    private bool isGrabbed;

    [SerializeField]
    private Vector3 direction;

    [SerializeField]
    private float pullDelay;

    [SerializeField]
    private float currentPullDelay;

    [SerializeField]
    private float currentStunDelay;

    private float totalDistance;
    private Component damageableComponent;
    private IDamageable enemyUnit;

    public bool CantBeKnockbacked
    {
        get { return cantBeGrabbed; }
        set { cantBeGrabbed = value; }
    }

    public bool IsGrabbed
    {
        get { return isGrabbed; }
        set { isGrabbed = value; }
    }

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public float PullDelay
    {
        get { return pullDelay; }
        set { pullDelay = value; }
    }

    public float CurrentPullDelay
    {
        get { return currentPullDelay; }
        set { currentPullDelay = value; }
    }

    public float CurrentStunDelay
    {
        get { return currentStunDelay; }
        set { currentStunDelay = value; }
    }

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartGrabbedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
    }

    public void UpdateGrabbedStats() {
        if(isGrabbed) {
            if(enemyUnit.Agent != null && enemyUnit.Stats.CanAct && Vector3.Distance((damageableComponent as IDamageable).Agent.Agent.transform.position, enemyUnit.Agent.Agent.transform.position) > 
            (damageableComponent as IDamageable).Agent.HitBox.radius + enemyUnit.Agent.HitBox.radius ) {
                Vector3 direction = (enemyUnit.Agent.Agent.transform.position - (damageableComponent as IDamageable).Agent.Agent.transform.position).normalized;
                direction.y = 0;
                (damageableComponent as IDamageable).Agent.Agent.transform.position += direction * totalDistance/pullDelay * Time.deltaTime;
            }
            else if(currentStunDelay > 0) {
                (damageableComponent as IDamageable).Agent.Agent.enabled = true;
                currentStunDelay -= Time.deltaTime;
            }
            else
                unGrab();
        }
    }

    public void Grab(float pullDuration, float stunDuration, IDamageable unit) {
        if(!cantBeGrabbed && unit.Agent != null && unit.Stats.CanAct) {
            isGrabbed = true;
            pullDelay = pullDuration;
            currentPullDelay = pullDuration;
            currentStunDelay = stunDuration;
            enemyUnit = unit;

            totalDistance = Vector3.Distance((damageableComponent as IDamageable).Agent.Agent.transform.position, enemyUnit.Agent.Agent.transform.position);

            (damageableComponent as IDamageable).Agent.Agent.enabled = false;
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
        }
    }

    public void unGrab() {
        isGrabbed = false;
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
