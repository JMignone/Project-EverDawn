using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LingeringStats
{
    [SerializeField]
    private bool lingering;

    [SerializeField]
    private bool lingerAtEnd;

    [SerializeField]
    private bool lingerDuringFlight;

    private bool isInFlight;
    private bool currentlyLingering;

    [SerializeField]
    private GameObject lingeringEffect;

    [SerializeField] [Min(0)]
    private float lingeringDamage;

    [SerializeField] [Min(0)]
    private float towerDamage;
    private float damageMultiplier;

    [SerializeField] [Min(0)]
    private float lingeringRadius;

    [SerializeField] [Min(0)]
    private float lingeringDuration;

    [SerializeField] [Min(0)]
    private float lingeringTick;

    [SerializeField] [Min(0)]
    private float currentLingeringTime;

    private IAbility ability;

    public bool Lingering
    {
        get { return lingering; }
    }

    public bool LingerAtEnd
    {
        get { return lingerAtEnd; }
    }

    public bool LingerDuringFlight
    {
        get { return lingerDuringFlight; }
    }

    public bool IsInFlight
    {
        get { return isInFlight; }
        set { isInFlight = value; }
    }

    public bool CurrentlyLingering
    {
        get { return currentlyLingering; }
        set { currentlyLingering = value; }
    }

    public float LingeringDamage
    {
        get { return lingeringDamage; }
    }

    public float LingeringRadius
    {
        get { return lingeringRadius; }
        set { lingeringRadius = value; }
    }

    public float LingeringDuration
    {
        get { return lingeringDuration; }
    }

    public float LingeringTick
    {
        get { return lingeringTick; }
    }

    public float CurrentLingeringTime
    {
        get { return currentLingeringTime; }
        set { currentLingeringTime = value; }
    }

    public void StartLingeringStats(GameObject go) {
        isInFlight = true;
        currentLingeringTime = 0;
        ability = (go.GetComponent(typeof(IAbility)) as IAbility);
        if(lingerDuringFlight)
            currentlyLingering = true;
        else
            currentlyLingering = false;

        if(!lingerDuringFlight && !lingerAtEnd) //if lingering is on but it doesnt linger both in flight and at end, default to end
            lingerAtEnd = true;
        damageMultiplier = ability.DamageMultiplier;
    }

    public void UpdateLingeringStats(GameObject go) {
        if(currentlyLingering) {
            if(lingeringDuration > 0) {//if we havnt reached the total duration yet
                if(currentLingeringTime < lingeringTick)
                    currentLingeringTime += Time.deltaTime;
                else {
                    LingerDamage(go);
                    currentLingeringTime = 0;
                    if(!isInFlight || !lingerDuringFlight) //dont allow the projectile to die if its in flight
                        lingeringDuration -= lingeringTick;
                }
            }
            else
                MonoBehaviour.Destroy(go);
        }
    }

    public void LingerDamage(GameObject go) {
        Vector3 position = new Vector3(go.transform.position.x, 0, go.transform.position.z);
        
        GameObject damageZone = MonoBehaviour.Instantiate(lingeringEffect, position, Quaternion.identity);
        damageZone.transform.localScale = new Vector3(lingeringRadius*2, .1f, lingeringRadius*2);

        Collider[] colliders = Physics.OverlapSphere(position, lingeringRadius);

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, damageable)) {
                    ability.SetHit = true;
                    
                    float damage = lingeringDamage*ability.DamageMultiplier;
                    if(towerDamage > 0 && damageable.GetComponent<Tower>())
                        damage = towerDamage*ability.DamageMultiplier;

                    GameFunctions.Attack(damageable, lingeringDamage*damageMultiplier, ability.CritStats);
                    ability.ApplyAffects(damageable);
                }
            }
        }
    }
}
