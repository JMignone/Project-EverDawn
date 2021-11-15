using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GrabbedStats
{
    [SerializeField]
    private bool cantBeGrabbed;
    private bool outSideResistance;

    [SerializeField]
    private bool isGrabbed;

    [SerializeField]
    private Vector3 direction;

    [SerializeField] [Min(0)]
    private float grabDelay;

    [SerializeField] [Min(0)]
    private float currentStunDelay;

    private float totalDistance;
    private IDamageable unit;
    private IDamageable enemyUnit;

    public bool CantBeGrabbed
    {
        get { return cantBeGrabbed; }
        set { cantBeGrabbed = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
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

    public float GrabDelay
    {
        get { return grabDelay; }
        set { grabDelay = value; }
    }

    public float CurrentStunDelay
    {
        get { return currentStunDelay; }
        set { currentStunDelay = value; }
    }

    public void StartGrabbedStats(IDamageable go) {
        unit = go;
    }

    public void UpdateGrabbedStats() {
        if(isGrabbed) {

            Vector3 unitAgentPos = Vector3.zero;
            Vector3 enemyAgentPos = Vector3.zero;

            bool enemyCanAct = false;
            if(enemyUnit.Agent != null && enemyUnit.Stats.CanAct) {
                unitAgentPos = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
                enemyAgentPos = new Vector3(enemyUnit.Agent.transform.position.x, 0, enemyUnit.Agent.transform.position.z);
                enemyCanAct = true;
            }
            
            if(enemyCanAct && Vector3.Distance(unitAgentPos, enemyAgentPos) > unit.Agent.HitBox.radius + enemyUnit.Agent.HitBox.radius) {
                Vector3 direction = enemyAgentPos - unitAgentPos;
                direction.y = 0;
                unit.Agent.transform.position += direction.normalized * totalDistance/grabDelay * Time.deltaTime;
            }
            else if(currentStunDelay > 0) {
                unit.Agent.Agent.enabled = true;
                currentStunDelay -= Time.deltaTime;
            }
            else
                unGrab();
        }
    }

    public void Grab(float grabSpeed, float grabDuration, float stunDuration, IDamageable enemy) {
        if(!cantBeGrabbed && !outSideResistance && unit.Agent != null && unit.Stats.CanAct) {
            isGrabbed = true;
            grabDelay = grabDuration;
            currentStunDelay = stunDuration;
            enemyUnit = enemy;

            Vector3 unitAgentPos = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
            Vector3 enemyAgentPos = new Vector3(enemyUnit.Agent.transform.position.x, 0, enemyUnit.Agent.transform.position.z);

            totalDistance = Vector3.Distance(unitAgentPos, enemyAgentPos);

            //if the speed option was used, set for correct speed
            if(grabSpeed != 0) 
                grabDelay = totalDistance/grabSpeed;

            unit.Agent.Agent.enabled = false;
            unit.SetTarget(null);
            unit.Stats.CurrAttackDelay = 0;
            if((unit as Component).transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors to red
                foreach(Transform child in (unit as Component).transform.GetChild(1).GetChild(5).GetChild(2)) {
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
        if((unit as Component).transform.GetChild(1).GetChild(5).childCount > 1) { //if the unit has an ability, set its image colors back to green
            foreach(Transform child in (unit as Component).transform.GetChild(1).GetChild(5).GetChild(2)) {
                if(child.childCount > 0) //this means its a complicated summon preview
                    child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
                else
                    child.GetComponent<Image>().color = new Color32(255,255,255,100);
            }
        }
    }
}
