using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyResistanceStats
{
    [SerializeField]
    private bool applyToSelf;

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

    public void StartResistance(IDamageable unit) {
        if(applyToSelf) {
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
        }
    }

    public void ApplyResistance(IDamageable unit) {
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
    }
}
