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

    [SerializeField]
    private float lingeringDamage;

    [SerializeField]
    private float lingeringRadius;

    [SerializeField]
    private float lingeringDuration;

    [SerializeField]
    private float lingeringTick;

    [SerializeField]
    private float currentLingeringTime;

    private Component abilityComponent;

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
        abilityComponent = go.GetComponent(typeof(IAbility));
        if(lingerDuringFlight)
            currentlyLingering = true;
        else
            currentlyLingering = false;

        if(!lingerDuringFlight && !lingerAtEnd) //if lingering is on but it doesnt linger both in flight and at end, default to end
            lingerAtEnd = true;
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
        //Instantiate(lingeringEffect, go.transform.position, go.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(go.transform.position, lingeringRadius);

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(GameFunctions.WillHit((abilityComponent as IAbility).ObjectAttackable, damageable)) {
                    GameFunctions.Attack(damageable, lingeringDamage);
                    (abilityComponent as IAbility).ApplyAffects(damageable);
                }
            }
        }
    }
}
