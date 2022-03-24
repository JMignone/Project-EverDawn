using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShadowStats
{
    private GameObject gameObj;
    private IDamageable unit;

    [Tooltip("Makes the unit invisible to units after moving for a short time")]
    [SerializeField]
    private bool shades;

    [SerializeField]
    private bool interruptsByDamage;

    [Tooltip("Determines the amount of time walking before starting to charge")]
    [SerializeField]
    private float shadeDelay;

    [SerializeField]
    private float currentDelay;

    [SerializeField]
    private List<Renderer> mats;

    public bool Shades
    {
        get { return shades; }
    }

    public float CurrentDelay
    {
        get { return currentDelay; }
        set { currentDelay = value; }
    }

    public bool InterruptsByDamage
    {
        get { return interruptsByDamage; }
    }

    public void StartShadowStats(IDamageable go) {
        unit = go;
        gameObj = (unit as Component).gameObject;
    }

    public void UpdateShadowStats() {
        if(shades) {
            if(!unit.Stats.IsShadow && unit.Stats.CanAct && !unit.Stats.IsCastingAbility && !unit.Stats.IsAttacking) {
                if(currentDelay < shadeDelay) 
                    currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
                else {
                    currentDelay = 0;
                    unit.Stats.Vanish(gameObj, unit.Agent); //unit.EnemyHitTargets.ToArray());
                    unit.Stats.UnitMaterials.MakeTransparent();
                }
            }
            else if(unit.Stats.IsShadow && (!unit.Stats.CanAct || unit.Stats.IsCastingAbility || unit.Stats.IsAttacking) )
                unit.Stats.Appear(gameObj, unit.ShadowStats, unit.Agent);
            else
                currentDelay = 0;
        }
    }
/*
    private void Vanish() {
        if(shades && !unit.Stats.IsShadow) {
            unit.Stats.IsShadow = true;
            foreach(GameObject go in unit.EnemyHitTargets.ToArray()) {
                Component targetComponent = go.GetComponent(typeof(IDamageable));
                if(targetComponent) {
                    (targetComponent as IDamageable).SetTarget(null);
                    if((targetComponent as IDamageable).InRangeTargets.Contains(gameObj))
                        (targetComponent as IDamageable).InRangeTargets.Remove(gameObj);
                    if((targetComponent as IDamageable).HitTargets.Contains(gameObj))
                        (targetComponent as IDamageable).HitTargets.Remove(gameObj);
                }
            }
            //make unit transparent
        }
    }

    public void Appear() {
        if(shades && unit.Stats.IsShadow) {
            unit.Stats.IsShadow = false;
            Collider[] colliders = Physics.OverlapSphere(unit.Agent.Agent.transform.position, unit.Agent.Agent.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(gameObj.tag) && collider.CompareTag("Vision")) { //Are we in their vision detection object?
                        if(!(enemy as IDamageable).HitTargets.Contains(gameObj))
                            (enemy as IDamageable).HitTargets.Add(gameObj);
                    }
                }
            }
            colliders = Physics.OverlapSphere(unit.Agent.Agent.transform.position, unit.Agent.Agent.radius);
            foreach(Collider collider in colliders) {
                Component enemy = collider.transform.parent.parent.GetComponent(typeof(IDamageable));
                if(enemy) {
                    if(!enemy.CompareTag(gameObj.tag) && collider.CompareTag("Range")) {
                        if(GameFunctions.CanAttack(enemy.tag, gameObj.tag, gameObj.GetComponent(typeof(IDamageable)), (enemy as IDamageable).Stats)) { //only if the unit can actually target this one should we adjust this value
                            if(!(enemy as IDamageable).InRangeTargets.Contains(gameObj))
                                (enemy as IDamageable).InRangeTargets.Add(gameObj);
                            if( ((enemy as IDamageable).InRangeTargets.Count == 1 || (enemy as IDamageable).Target == null) && (enemy as IDamageable).Stats.CanAct) { //we need this block here as well as stay in the case that a unit is placed inside a units range
                                GameObject go = GameFunctions.GetNearestTarget((enemy as IDamageable).HitTargets, collider.transform.parent.parent.tag, (enemy as IDamageable).Stats);
                                if(go != null)
                                    (enemy as IDamageable).SetTarget(go);
                            }
                        }
                    }
                }
            }
            //make unit appear
        }
        else
            currentDelay = 0;
    }*/
}
