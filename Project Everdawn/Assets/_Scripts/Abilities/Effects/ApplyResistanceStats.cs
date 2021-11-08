using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyResistanceStats //this type of resistance persists for a given duration
{
    [SerializeField]
    private bool applyToSelf;

    [SerializeField]
    private bool resistDamage;
    [SerializeField]
    private float rdDuration;

    [SerializeField]
    private bool untargetable;
    [SerializeField]
    private float rtDuration;

    [SerializeField]
    private bool resistFreeze;
    [SerializeField]
    private float rfDuration;

    [SerializeField]
    private bool resistSlow;
    [SerializeField]
    private float rsDuration;

    [SerializeField]
    private bool resistRoot;
    [SerializeField]
    private float rrDuration;

    [SerializeField]
    private bool resistPoison;
    [SerializeField]
    private float rpDuration;

    [SerializeField]
    private bool resistKnockback;
    [SerializeField]
    private float rkDuration;

    [SerializeField]
    private bool resistGrab;
    [SerializeField]
    private float rgDuration;

    [SerializeField]
    private bool resistPull;
    [SerializeField]
    private float rpullDuration;

    [SerializeField]
    private bool resistBlind;
    [SerializeField]
    private float rbDuration;

    public void StartResistance(IDamageable unit) {
        if(applyToSelf) {
            if(resistDamage)
                unit.Stats.EffectStats.ResistStats.ResistDamage(rdDuration);
            if(untargetable)
                unit.Stats.EffectStats.ResistStats.ResistTarget(rtDuration);
            if(resistFreeze)
                unit.Stats.EffectStats.ResistStats.ResistFreeze(rfDuration);
            if(resistSlow)
                unit.Stats.EffectStats.ResistStats.ResistSlow(rsDuration);
            if(resistRoot)
                unit.Stats.EffectStats.ResistStats.ResistRoot(rrDuration);
            if(resistPoison)
                unit.Stats.EffectStats.ResistStats.ResistPoison(rpDuration);
            if(resistKnockback)
                unit.Stats.EffectStats.ResistStats.ResistKnockback(rkDuration);
            if(resistGrab)
                unit.Stats.EffectStats.ResistStats.ResistGrab(rgDuration);
            if(resistPull)
                unit.Stats.EffectStats.ResistStats.ResistPull(rpullDuration);
            if(resistBlind)
                unit.Stats.EffectStats.ResistStats.ResistBlind(rbDuration);
        }
    }

    public void ApplyResistance(IDamageable unit) {
        if(resistDamage)
            unit.Stats.EffectStats.ResistStats.ResistDamage(rdDuration);
        if(untargetable)
                unit.Stats.EffectStats.ResistStats.ResistTarget(rtDuration);
        if(resistFreeze)
            unit.Stats.EffectStats.ResistStats.ResistFreeze(rfDuration);
        if(resistSlow)
            unit.Stats.EffectStats.ResistStats.ResistSlow(rsDuration);
        if(resistRoot)
            unit.Stats.EffectStats.ResistStats.ResistRoot(rrDuration);
        if(resistPoison)
            unit.Stats.EffectStats.ResistStats.ResistPoison(rpDuration);
        if(resistKnockback)
            unit.Stats.EffectStats.ResistStats.ResistKnockback(rkDuration);
        if(resistGrab)
            unit.Stats.EffectStats.ResistStats.ResistGrab(rgDuration);
        if(resistPull)
            unit.Stats.EffectStats.ResistStats.ResistPull(rpullDuration);
        if(resistBlind)
            unit.Stats.EffectStats.ResistStats.ResistBlind(rbDuration);
    }
}
