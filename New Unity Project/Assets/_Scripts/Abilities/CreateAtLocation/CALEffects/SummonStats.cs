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
    private GameObject summonPreview;

    [SerializeField]
    private float timeToSummon;

    [SerializeField]
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

    public void StartSummonStats()
    {
        currentSummonTime = 0;
    }

    public void UpdateSummonStats(GameObject go, bool isFrozen) 
    {
        if(currentSummonTime < timeToSummon && !isFrozen) 
            currentSummonTime += Time.deltaTime;
        else {
            Summon(go, currentSummonTime/timeToSummon);
            //MonoBehaviour.Destroy(go);
        }
    }

    private void Summon(GameObject go, float percentHealth) {
        if(percentHealth < .25f)
            return;
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
        /*
        position = GameFunctions.adjustForTowers(position, cal.Radius);

        UnityEngine.AI.NavMeshHit hit;
        GameObject summonGo;
        if(UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 6.1f, 9))
            summonGo = GameFunctions.SpawnUnit(summonUnit, GameManager.GetUnitsFolder(), hit.position);
        else
            summonGo = GameFunctions.SpawnUnit(summonUnit, GameManager.GetUnitsFolder(), position);
        */
        GameObject summonGo = GameFunctions.SpawnUnit(summonUnit, GameManager.GetUnitsFolder(), position);

        Component damageable = summonGo.GetComponent(typeof(IDamageable));
        //Component unit = damageable.gameObject.GetComponent(typeof(IDamageable)); //The unit to update

        (damageable as IDamageable).Stats.CurrHealth = (damageable as IDamageable).Stats.MaxHealth * percentHealth;
        (damageable as IDamageable).Stats.MaxHealth = (damageable as IDamageable).Stats.MaxHealth * percentHealth;

        MonoBehaviour.print("SUMMMONED AT " + percentHealth + " HP");
    }

}
