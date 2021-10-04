using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SummonStats
{
    [SerializeField]
    private bool isSummon;

    [SerializeField]
    private GameObject summonEffect;

    [SerializeField]
    private GameObject summonUnit;

    [SerializeField]
    private GameConstants.SUMMON_SIZE size;

    [SerializeField]
    private GameObject summonPreview;

    [SerializeField] [Min(0)]
    private float timeToSummon;

    [SerializeField] [Min(0)]
    private float currentSummonTime;

    public bool IsSummon
    {
        get { return isSummon; }
    }

    public GameObject SummonEffect
    {
        get { return summonEffect; }
    }

    public GameObject SummonUnit
    {
        get { return summonUnit; }
    }

    public GameConstants.SUMMON_SIZE Size
    {
        get { return size; }
    }

    public GameObject SummonPreview
    {
        get { return summonPreview; }
    }

    public float TimeToSummon
    {
        get { return timeToSummon; }
    }

    public float CurrentSummonTime
    {
        get { return currentSummonTime; }
    }

    public int AreaMask()
    {
        if(size == GameConstants.SUMMON_SIZE.BIG)
            return 32; //32 = Big Building area
        else if(size == GameConstants.SUMMON_SIZE.SMALL)
            return 16; //16 = Small Building area
        else
            return 1; //1 = Walkable area
    }

    public void StartSummonStats()
    {
        currentSummonTime = 0;
    }

    public void UpdateSummonStats(GameObject go, bool canAct) 
    {
        if(currentSummonTime < timeToSummon && canAct) 
            currentSummonTime += Time.deltaTime;
        else
            Summon(go, currentSummonTime/timeToSummon);
    }

    private void Summon(GameObject go, float percentHealth) {
        if(percentHealth < .25f) {
            MonoBehaviour.Destroy(go);
            return;
        }
        else if(percentHealth < .5f)
            percentHealth = .25f;
        else if(percentHealth < .75f)
            percentHealth = .5f;
        else if(percentHealth < .75f)
            percentHealth = .5f;
        else if(percentHealth < .1f)
            percentHealth = .75f;
        else 
            percentHealth = 1;

        CreateAtLocation cal = go.GetComponent<CreateAtLocation>();
        MonoBehaviour.Destroy(go);
        Vector3 position = cal.TargetLocation;

        GameObject summonGo = GameFunctions.SpawnUnit(summonUnit, GameManager.GetUnitsFolder(), position);

        Component damageable = summonGo.GetComponent(typeof(IDamageable));

        (damageable as IDamageable).Stats.CurrHealth = (damageable as IDamageable).Stats.MaxHealth * percentHealth;
        (damageable as IDamageable).Stats.MaxHealth = (damageable as IDamageable).Stats.MaxHealth * percentHealth;

        MonoBehaviour.print("SUMMMONED AT " + percentHealth + " HP");
    }

}
