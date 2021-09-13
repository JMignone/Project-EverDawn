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
    private ResistStats resistStats;

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

    public ResistStats ResistStats
    {
        get { return resistStats; }
    }

    public void StartStats(GameObject go) {
        frozenStats.StartFrozenStats(go);
        slowedStats.StartSlowedStats(go);
        poisonedStats.StartPoisonedStats(go);
        rootedStats.StartRootedStats(go);
        knockbackedStats.StartKnockbackedStats(go);
        knockbackStats.StartKnockbackStats(go);
        pulledStats.StartPulledStats(go);
        grabbedStats.StartGrabbedStats(go);
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
        resistStats.UpdateResistanceStats();
    }

    public bool CanAct() {
        if(frozenStats.IsFrozen)
            return false;
        if(knockbackedStats.IsKnockbacked)
            return false;
        if(grabbedStats.IsGrabbed)
            return false;
        return true;
    }
}
