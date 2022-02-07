using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectStats
{
    [SerializeField]
    private UAOEStats aoeStats;
    [SerializeField]
    private FrozenStats frozenStats;
    [SerializeField]
    private SlowStats slowStats;
    [SerializeField]
    private SlowedStats slowedStats;
    [SerializeField]
    private RootedStats rootedStats;
    [SerializeField]
    private PoisonedStats poisonedStats;
    [SerializeField]
    private UKnockbackStats knockbackStats;
    [SerializeField]
    private KnockbackedStats knockbackedStats;
    [SerializeField]
    private PulledStats pulledStats;
    [SerializeField]
    private GrabbedStats grabbedStats;
    [SerializeField]
    private StrengthenedStats strengthenedStats;
    [SerializeField]
    private BlindedStats blindedStats;
    [SerializeField]
    private StunnedStats stunnedStats;
    [SerializeField]
    private ResistStats resistStats;
    [SerializeField]
    private CritStats critStats;

    public UAOEStats AOEStats
    {
        get { return aoeStats; }
    }

    public FrozenStats FrozenStats 
    {
        get { return frozenStats; }
    }

    public SlowStats SlowStats
    {
        get { return slowStats; }
    }

    public SlowedStats SlowedStats 
    {
        get { return slowedStats; }
    }

    public RootedStats RootedStats 
    {
        get { return rootedStats; }
    }

    public PoisonedStats PoisonedStats
    {
        get { return poisonedStats; }
    }

    public UKnockbackStats KnockbackStats
    {
        get { return knockbackStats; }
    }

    public KnockbackedStats KnockbackedStats
    {
        get { return knockbackedStats; }
    }

    public PulledStats PulledStats
    {
        get { return pulledStats; }
    }

    public GrabbedStats GrabbedStats
    {
        get { return grabbedStats; }
    }

    public StrengthenedStats StrengthenedStats
    {
        get { return strengthenedStats; }
    }

    public BlindedStats BlindedStats
    {
        get { return blindedStats; }
    }

    public StunnedStats StunnedStats
    {
        get { return stunnedStats; }
    }

    public ResistStats ResistStats
    {
        get { return resistStats; }
    }

    public CritStats CritStats
    {
        get { return critStats; }
    }

    public void StartStats(IDamageable go) {
        frozenStats.StartFrozenStats(go);
        slowedStats.StartSlowedStats(go);
        poisonedStats.StartPoisonedStats(go);
        rootedStats.StartRootedStats(go);
        knockbackedStats.StartKnockbackedStats(go);
        knockbackStats.StartKnockbackStats(go);
        pulledStats.StartPulledStats(go);
        grabbedStats.StartGrabbedStats(go);
        strengthenedStats.StartStrengthenedStats(go);
        blindedStats.StartStats(go);
        stunnedStats.StartStats(go);
        aoeStats.StartStats(go);
        resistStats.StartResistStats(go);
    }

    public void UpdateStats() {
        frozenStats.UpdateFrozenStats();
        slowedStats.UpdateSlowedStats();
        poisonedStats.UpdatePoisonedStats();
        rootedStats.UpdateRootedStats();
        knockbackedStats.UpdateKnockbackedStats();
        pulledStats.UpdatePulledStats();
        grabbedStats.UpdateGrabbedStats();
        strengthenedStats.UpdateStrengthenedStats();
        blindedStats.UpdateStats();
        stunnedStats.UpdateStats();
        resistStats.UpdateResistanceStats();
    }

    public bool CanAct() {
        if(frozenStats.IsFrozen)
            return false;
        if(knockbackedStats.IsKnockbacked)
            return false;
        if(grabbedStats.IsGrabbed)
            return false;
        if(stunnedStats.IsStunned)
            return false;
        return true;
    }
}
