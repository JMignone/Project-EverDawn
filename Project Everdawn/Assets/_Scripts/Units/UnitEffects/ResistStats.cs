using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResistStats
{
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

    private Component damageableComponent;

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
    
    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartResistStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
    }

    public void ResistFreeze(float duration) {
        if(!resistedFreeze) {
            resistedFreeze = true;
            rfDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.FrozenStats.OutSideResistance = true;
        }
    }

    public void ResistSlow(float duration) {
        if(!resistedSlow) {
            resistedSlow = true;
            rsDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.SlowedStats.OutSideResistance = true;
        }
    }

    public void ResistRoot(float duration) {
        if(!resistedRoot) {
            resistedRoot = true;
            rrDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.RootedStats.OutSideResistance = true;
        }
    }

    public void ResistPoison(float duration) {
        if(!resistedPoison) {
            resistedPoison = true;
            rpDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.PoisonedStats.OutSideResistance = true;
        }
    }

    public void ResistKnockback(float duration) {
        if(!resistedKnockback) {
            resistedKnockback = true;
            rkDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.KnockbackedStats.OutSideResistance = true;
        }
    }

    public void ResistGrab(float duration) {
        if(!resistedGrab) {
            resistedGrab = true;
            rgDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.GrabbedStats.OutSideResistance = true;
        }
    }

    public void ResistPull(float duration) {
        if(!resistedPull) {
            resistedPull = true;
            rpullDuration = duration;
            (damageableComponent as IDamageable).Stats.EffectStats.PulledStats.OutSideResistance = true;
        }
    }

    public void UpdateResistanceStats() {
        if(resistedFreeze) {
            if(rfDuration > 0)
                rfDuration -= Time.deltaTime;
            else {
                resistedFreeze = false;
                (damageableComponent as IDamageable).Stats.EffectStats.FrozenStats.OutSideResistance = false;
            }
        }
        if(resistedSlow) {
            if(rsDuration > 0)
                rsDuration -= Time.deltaTime;
            else {
                resistedSlow = false;
                (damageableComponent as IDamageable).Stats.EffectStats.SlowedStats.OutSideResistance = false;
            }
        }
        if(resistedRoot) {
            if(rrDuration > 0)
                rrDuration -= Time.deltaTime;
            else {
                resistedRoot = false;
                (damageableComponent as IDamageable).Stats.EffectStats.RootedStats.OutSideResistance = false;
            }
        }
        if(resistedPoison) {
            if(rpDuration > 0)
                rpDuration -= Time.deltaTime;
            else {
                resistedPoison = false;
                (damageableComponent as IDamageable).Stats.EffectStats.PoisonedStats.OutSideResistance = false;
            }
        }
        if(resistedKnockback) {
            if(rkDuration > 0)
                rkDuration -= Time.deltaTime;
            else {
                resistedKnockback = false;
                (damageableComponent as IDamageable).Stats.EffectStats.KnockbackedStats.OutSideResistance = false;
            }
        }
        if(resistedGrab) {
            if(rgDuration > 0)
                rgDuration -= Time.deltaTime;
            else {
                resistedGrab = false;
                (damageableComponent as IDamageable).Stats.EffectStats.GrabbedStats.OutSideResistance = false;
            }
        }
        if(resistedPull) {
            if(rpullDuration > 0)
                rpullDuration -= Time.deltaTime;
            else {
                resistedPull = false;
                (damageableComponent as IDamageable).Stats.EffectStats.PulledStats.OutSideResistance = false;
            }
        }
    }
}
