using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DashStats
{
    private IDamageable unit;
    private IDamageable chosenTarget;

    [Tooltip("Makes the unit dash towards its target")]
    [SerializeField]
    private bool dashes;
    [SerializeField]
    private bool isDashing;
    private bool isMoving;

    [SerializeField] [Min(0)]
    private float dashDamage;

    [SerializeField] [Min(0)]
    private float dashSpeed;
    private float speed;

    [SerializeField] [Min(0)]
    private float dashRange;

    [Tooltip("Determines the amount of time before starting to dash")]
    [SerializeField] [Min(0)]
    private float dashDelay;

    [SerializeField] [Min(0)]
    private float currentDelay;

    public bool Dashes
    {
        get { return dashes; }
    }

    public bool IsDashing
    {
        get { return isDashing; }
    }

    public void StartDashStats(IDamageable go) {
        if(dashes) {
            unit = go;
            speed = unit.Stats.MoveSpeed;
        
            GameObject dashGo = new GameObject();
            dashGo.name = "DashDetectionObject";
            dashGo.tag = "Dash";

            SphereCollider dashBox = dashGo.AddComponent<SphereCollider>();
            dashBox.radius = dashRange;
            dashBox.center = new Vector3(0, 0, 0);
            dashBox.enabled = true;

            dashGo.SetActive(true);
            dashGo.transform.SetParent((unit as Component).gameObject.transform.GetChild(1));
            dashGo.transform.localPosition = Vector3.zero;
        }
    }

    public void UpdateDashStats() {
        if(dashes && isDashing) {
            if(!unit.Stats.CanAct || unit.Target == null || unit.Stats.IsCastingAbility
               || Vector3.Distance(new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z), new Vector3(unit.Target.transform.GetChild(0).position.x, 0, unit.Target.transform.GetChild(0).position.z)) > dashRange + (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable).Agent.HitBox.radius) {
                currentDelay = 0;
                isDashing = false;
                isMoving = false;
                unit.Agent.Agent.enabled = true;
            }
            else {
                if(isDashing && !isMoving) {
                    if(currentDelay < dashDelay) 
                        currentDelay += Time.deltaTime * unit.Stats.EffectStats.SlowedStats.CurrentSlowIntensity;
                    else {
                        currentDelay = 0;
                        isMoving = true;
                        unit.Agent.Agent.enabled = false;
                        chosenTarget = (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable);
                    }
                }
                else if(isDashing && isMoving && chosenTarget != null) {
                    Vector3 direction = (chosenTarget.Agent.transform.position - unit.Agent.transform.position).normalized;
                    unit.Agent.transform.position += direction * dashSpeed * Time.deltaTime;
                    if(Vector3.Distance(unit.Agent.transform.position, chosenTarget.Agent.transform.position) < unit.Stats.Range + chosenTarget.Agent.Agent.radius) {
                        DashAttack();
                        unit.SetTarget((chosenTarget as Component).gameObject);
                        currentDelay = 0;
                        isDashing = false;
                        isMoving = false;
                        unit.Agent.Agent.enabled = true;
                    }
                }
                else {
                    currentDelay = 0;
                    isDashing = false;
                    isMoving = false;
                    unit.Agent.Agent.enabled = true;
                }
            }
        }
    }

    public void StartDash(GameObject go) {
        if(go == unit.Target && unit.Stats.CanAct && !unit.Stats.IsAttacking && !isDashing && !unit.Stats.IsCastingAbility
           && Vector3.Distance(new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z), new Vector3(go.transform.GetChild(0).position.x, 0, go.transform.GetChild(0).position.z)) >= dashRange) {
            isDashing = true;
            unit.Agent.Agent.ResetPath();
        }
    }

    private void DashAttack() {
        if(unit.Stats.EffectStats.AOEStats.AreaOfEffect)
            unit.Stats.EffectStats.AOEStats.Explode((unit as Component).gameObject, (chosenTarget as Component).gameObject, dashDamage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
        else {
            GameFunctions.Attack((chosenTarget as Component), dashDamage * unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
            unit.Stats.ApplyAffects((chosenTarget as Component));
        }
        unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent);
    }
}
