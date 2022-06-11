using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockupedStats
{
    [Tooltip("A number from 0 to 1 that determines a units resistance to being knockuped. A resistance of 1 means they cannot be knockuped")]
    [SerializeField]  [Range(0,1)]
    private float knockupResistance;
    private bool outSideResistance;

    [SerializeField]
    private bool isKnockuped;

    private float currentTime;
    private float duration;
    private Vector3 startLocation;
    private Vector3 endLocation;
    private Vector3 direction;

    private IDamageable unit;

    public bool IsKnockuped
    {
        get { return isKnockuped; }
        set { isKnockuped = value; }
    }

    public float KnockupResistance
    {
        get { return knockupResistance; }
        set { knockupResistance = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
        if(knockupResistance < 0)
            knockupResistance = 0;
    }

    public void UpdateStats() {
        if(isKnockuped) {
            if(currentTime < duration) {
                Debug.DrawRay(startLocation, direction*Vector3.Distance(startLocation, endLocation), Color.green);
                Debug.DrawRay(endLocation, Vector3.up*10, Color.green);
                unit.Agent.transform.position = Vector3.Lerp(startLocation, endLocation, currentTime/duration);
                currentTime += Time.deltaTime;
            }
            else {
                unit.Agent.transform.position = endLocation;
                unKnockup();
            }
        }
    }

    public void Knockup(float distance, float duration, bool towardsUnit, IDamageable enemyUnit) {
        if(knockupResistance < 1 && !outSideResistance) {
            if(unit.Stats.EffectStats.KnockbackedStats.IsKnockbacked)
                unit.Stats.EffectStats.KnockbackedStats.unKnockback();
            if(unit.Stats.EffectStats.GrabbedStats.IsGrabbed)
                unit.Stats.EffectStats.GrabbedStats.unGrab();

            unit.Stats.EffectStats.ResistStats.ResistKnockback(duration);
            unit.Stats.EffectStats.ResistStats.ResistGrab(duration);
            unit.Stats.EffectStats.ResistStats.ResistPull(duration);

            Vector3 sourcePosition;
            if(enemyUnit.Equals(null))
                direction = Vector3.zero;
            else {
                sourcePosition = enemyUnit.Agent.transform.position;
                if(towardsUnit)
                    direction = sourcePosition - unit.Agent.transform.position;
                else
                    direction = unit.Agent.transform.position - sourcePosition;
            }
            direction.y = 0;
            direction = direction.normalized;

            int areaMask = 1;
            if(unit.Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
                areaMask = 8;

            startLocation = unit.Agent.transform.position;
            
            endLocation = GameFunctions.adjustForBoundary(distance * direction + startLocation);
            UnityEngine.AI.NavMeshHit hit;
            if(UnityEngine.AI.NavMesh.SamplePosition(endLocation, out hit, 4.5f, areaMask)) //4.5 is set such that the unit cannot make an aggregious jump, it will only extend its jump distance by a little bit
                endLocation = hit.position;
            else {
                UnityEngine.AI.NavMesh.Raycast(unit.Agent.transform.position, endLocation, out hit, areaMask);
                endLocation = hit.position;
            }

            unit.JumpStats.CancelJump();
            isKnockuped = true;
            this.duration = duration;
            unit.SetTarget(null);
            unit.Agent.Agent.enabled = false;
            unit.Stats.IsCastingAbility = false; //normally this is done automatically, but some abilitys use the 'abilityOverride' AND it doesnt set isCastingAbility via just getting destroyed, so we will need to set it
            GameFunctions.DisableAbilities(unit);
        }
    }

    public void unKnockup() {
        isKnockuped = false;
        currentTime = 0;
        unit.Agent.Agent.enabled = true;
        GameFunctions.EnableAbilities(unit);
    }
}
