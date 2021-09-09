using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResistEffects
{
    [SerializeField]
    private bool resistFreeze;

    [SerializeField]
    private bool resistSlow;

    [SerializeField]
    private bool resistRoot;

    [SerializeField]
    private bool resistPoison;

    [SerializeField]
    private bool resistKnockback;

    [SerializeField]
    private bool resistGrab;

    [SerializeField]
    private bool resistPull;

    public void StartResistance(IDamageable unit) {
        if(unit.Stats.EffectStats.FrozenStats.CantBeFrozen)
            resistFreeze = false;
        else if(resistFreeze)
            unit.Stats.EffectStats.FrozenStats.CantBeFrozen = true;

        if(unit.Stats.EffectStats.SlowedStats.CantBeSlowed)
            resistSlow = false;
        else if(resistSlow)
            unit.Stats.EffectStats.SlowedStats.CantBeSlowed = true;

        if(unit.Stats.EffectStats.RootedStats.CantBeRooted)
            resistRoot = false;
        else if(resistRoot)
            unit.Stats.EffectStats.RootedStats.CantBeRooted = true;

        if(unit.Stats.EffectStats.PoisonedStats.CantBePoisoned)
            resistPoison = false;
        else if(resistPoison)
            unit.Stats.EffectStats.PoisonedStats.CantBePoisoned = true;

        if(unit.Stats.EffectStats.KnockbackedStats.KnockbackResistance >= 1)
            resistKnockback = false;
        else if(resistKnockback)
            unit.Stats.EffectStats.KnockbackedStats.KnockbackResistance += 1;

        if(unit.Stats.EffectStats.GrabbedStats.CantBeGrabbed)
            resistGrab = false;
        else if(resistGrab)
            unit.Stats.EffectStats.GrabbedStats.CantBeGrabbed = true;

        if(unit.Stats.EffectStats.PulledStats.PullResistance >= 1)
            resistPull = false;
        else if(resistPull)
            unit.Stats.EffectStats.PulledStats.PullResistance += 1;
    }

    public void StopResistance(IDamageable unit) {
        if(resistFreeze)
            unit.Stats.EffectStats.FrozenStats.CantBeFrozen = false;

        if(resistSlow)
            unit.Stats.EffectStats.SlowedStats.CantBeSlowed = false;

        if(resistRoot)
            unit.Stats.EffectStats.RootedStats.CantBeRooted = false;

        if(resistPoison)
            unit.Stats.EffectStats.PoisonedStats.CantBePoisoned = false;

        if(resistKnockback)
            unit.Stats.EffectStats.KnockbackedStats.KnockbackResistance -= 1;

        if(resistGrab)
            unit.Stats.EffectStats.GrabbedStats.CantBeGrabbed = false;

        if(resistPull)
            unit.Stats.EffectStats.PulledStats.PullResistance -= 1;
    }
}
