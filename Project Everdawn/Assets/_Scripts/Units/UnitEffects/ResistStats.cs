using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResistStats
{
    [SerializeField]
    private bool resistedDamage;
    private float rdDuration;

    [SerializeField]
    private bool resistedTarget;
    private float rtDuration;

    [SerializeField]
    private bool resistedFreeze;
    private float rfDuration;

    [SerializeField]
    private bool resistedSlow;
    private float rsDuration;

    [SerializeField]
    private bool resistedRoot;
    private float rrDuration;

    [SerializeField]
    private bool resistedPoison;
    private float rpDuration;

    [SerializeField]
    private bool resistedKnockback;
    private float rkDuration;

    [SerializeField]
    private bool resistedGrab;
    private float rgDuration;

    [SerializeField]
    private bool resistedPull;
    private float rpullDuration;

    [SerializeField]
    private bool resistedBlind;
    private float rbDuration;

    [SerializeField]
    private bool resistedStun;
    private float rstunDuration;

    private IDamageable unit;

    public bool ResistedDamage
    {
        get { return resistedDamage; }
        set { resistedDamage = value; }
    }

    public float RdDuration
    {
        get { return rdDuration; }
        set { rdDuration = value; }
    }

    public bool ResistedTarget
    {
        get { return resistedTarget; }
        set { resistedTarget = value; }
    }

    public float RtDuration
    {
        get { return rtDuration; }
        set { rtDuration = value; }
    }

    public bool ResistedFreeze
    {
        get { return resistedFreeze; }
        set { resistedFreeze = value; }
    }

    public float RfDuration
    {
        get { return rfDuration; }
        set { rfDuration = value; }
    }

    public bool ResistedSlow
    {
        get { return resistedSlow; }
        set { resistedSlow = value; }
    }

    public float RsDuration
    {
        get { return rsDuration; }
        set { rsDuration = value; }
    }

    public bool ResistedRoot
    {
        get { return resistedRoot; }
        set { resistedRoot = value; }
    }

    public float RrDuration
    {
        get { return rrDuration; }
        set { rrDuration = value; }
    }

    public bool ResistedPoison
    {
        get { return resistedPoison; }
        set { resistedPoison = value; }
    }

    public float RpDuration
    {
        get { return rpDuration; }
        set { rpDuration = value; }
    }

    public bool ResistedKnockback
    {
        get { return resistedKnockback; }
        set { resistedKnockback = value; }
    }

    public float RkDuration
    {
        get { return rkDuration; }
        set { rkDuration = value; }
    }

    public bool ResistedGrab
    {
        get { return resistedGrab; }
        set { resistedGrab = value; }
    }

    public bool ResistedPull
    {
        get { return resistedPull; }
        set { resistedPull = value; }
    }

    public float RpullDuration
    {
        get { return rpullDuration; }
        set { rpullDuration = value; }
    }

    public bool ResistedBlind
    {
        get { return resistedBlind; }
        set { resistedBlind = value; }
    }

    public float RbDuration
    {
        get { return rbDuration; }
        set { rbDuration = value; }
    }

    public bool ResistedStun
    {
        get { return resistedStun; }
        set { resistedStun = value; }
    }

    public float RstunDuration
    {
        get { return rstunDuration; }
        set { rstunDuration = value; }
    }
    
    public IDamageable Unit
    {
        get { return unit; }
        set { unit = value; }
    }

    public void StartResistStats(IDamageable go) {
        unit = go;
    }

    public void ResistDamage(float duration) {
        if(!resistedDamage) {
            resistedDamage = true;
            rdDuration = duration;
        }
    }

    public void ResistTarget(float duration) {
        if(!resistedTarget) {
            resistedTarget = true;
            rtDuration = duration;
            unit.Stats.Vanish((unit as Component).gameObject, unit.EnemyHitTargets.ToArray());
        }
    }

    public void ResistFreeze(float duration) {
        if(!resistedFreeze) {
            resistedFreeze = true;
            rfDuration = duration;
            unit.Stats.EffectStats.FrozenStats.OutSideResistance = true;
        }
    }

    public void ResistSlow(float duration) {
        if(!resistedSlow) {
            resistedSlow = true;
            rsDuration = duration;
            unit.Stats.EffectStats.SlowedStats.OutSideResistance = true;
        }
    }

    public void ResistRoot(float duration) {
        if(!resistedRoot) {
            resistedRoot = true;
            rrDuration = duration;
            unit.Stats.EffectStats.RootedStats.OutSideResistance = true;
        }
    }

    public void ResistPoison(float duration) {
        if(!resistedPoison) {
            resistedPoison = true;
            rpDuration = duration;
            unit.Stats.EffectStats.PoisonedStats.OutSideResistance = true;
        }
    }

    public void ResistKnockback(float duration) {
        if(!resistedKnockback) {
            resistedKnockback = true;
            rkDuration = duration;
            unit.Stats.EffectStats.KnockbackedStats.OutSideResistance = true;
        }
    }

    public void ResistGrab(float duration) {
        if(!resistedGrab) {
            resistedGrab = true;
            rgDuration = duration;
            unit.Stats.EffectStats.GrabbedStats.OutSideResistance = true;
        }
    }

    public void ResistPull(float duration) {
        if(!resistedPull) {
            resistedPull = true;
            rpullDuration = duration;
            unit.Stats.EffectStats.PulledStats.OutSideResistance = true;
        }
    }

    public void ResistBlind(float duration) {
        if(!resistedBlind) {
            resistedBlind = true;
            rbDuration = duration;
            unit.Stats.EffectStats.BlindedStats.OutSideResistance = true;
        }
    }

    public void ResistStun(float duration) {
        if(!resistedStun) {
            resistedStun = true;
            rstunDuration = duration;
            unit.Stats.EffectStats.StunnedStats.OutSideResistance = true;
        }
    }

    public void UpdateResistanceStats() {
        if(resistedDamage) {
            if(rdDuration > 0)
                rdDuration -= Time.deltaTime;
            else {
                resistedDamage = false;

                //check if there is currently an ability hovered over this unit now
                Collider[] colliders = Physics.OverlapSphere(unit.Agent.transform.position, unit.Agent.Agent.radius);
                foreach(Collider collider in colliders) {
                    if(!collider.transform.parent.parent.CompareTag((unit as Component).gameObject.tag) && collider.CompareTag("AbilityHighlight")) { //Our we getting previewed for an ability?
                        AbilityPreview ability = collider.GetComponent<AbilityPreview>();
                        if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, (unit as Component))) 
                            unit.Stats.IncIndicatorNum();
                    }
                }
            }
        }
        if(resistedTarget) {
            if(rtDuration > 0)
                rtDuration -= Time.deltaTime;
            else {
                resistedTarget = false;
                unit.Stats.Appear((unit as Component).gameObject, unit.ShadowStats, unit.Agent);
            }
        }
        if(resistedFreeze) {
            if(rfDuration > 0)
                rfDuration -= Time.deltaTime;
            else {
                resistedFreeze = false;
                unit.Stats.EffectStats.FrozenStats.OutSideResistance = false;
            }
        }
        if(resistedSlow) {
            if(rsDuration > 0)
                rsDuration -= Time.deltaTime;
            else {
                resistedSlow = false;
                unit.Stats.EffectStats.SlowedStats.OutSideResistance = false;
            }
        }
        if(resistedRoot) {
            if(rrDuration > 0)
                rrDuration -= Time.deltaTime;
            else {
                resistedRoot = false;
                unit.Stats.EffectStats.RootedStats.OutSideResistance = false;
            }
        }
        if(resistedPoison) {
            if(rpDuration > 0)
                rpDuration -= Time.deltaTime;
            else {
                resistedPoison = false;
                unit.Stats.EffectStats.PoisonedStats.OutSideResistance = false;
            }
        }
        if(resistedKnockback) {
            if(rkDuration > 0)
                rkDuration -= Time.deltaTime;
            else {
                resistedKnockback = false;
                unit.Stats.EffectStats.KnockbackedStats.OutSideResistance = false;
            }
        }
        if(resistedGrab) {
            if(rgDuration > 0)
                rgDuration -= Time.deltaTime;
            else {
                resistedGrab = false;
                unit.Stats.EffectStats.GrabbedStats.OutSideResistance = false;
            }
        }
        if(resistedPull) {
            if(rpullDuration > 0)
                rpullDuration -= Time.deltaTime;
            else {
                resistedPull = false;
                unit.Stats.EffectStats.PulledStats.OutSideResistance = false;
            }
        }
        if(resistedBlind) {
            if(rbDuration > 0)
                rbDuration -= Time.deltaTime;
            else {
                resistedBlind = false;
                unit.Stats.EffectStats.BlindedStats.OutSideResistance = false;
            }
        }
        if(resistedStun) {
            if(rstunDuration > 0)
                rstunDuration -= Time.deltaTime;
            else {
                resistedStun = false;
                unit.Stats.EffectStats.StunnedStats.OutSideResistance = false;
            }
        }
    }
}
