using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoomerangStats
{
    [SerializeField]
    private bool isBoomerang;

    private Vector3 startLocation;
    //private Vector3 reboundLocation;
    private bool goingBack;
    private float speedModifier;

    [Tooltip("A number from (0-1], determines how far along the projectile will travel to its destination before starting to slow down. 1 will cause it to not slow down")]
    [SerializeField] [Range(0.01f,1)]
    private float percentToSlow; //a number from 0 to 1

    [Tooltip("A number from [0-inf), how long should the projectile wait before coming back")]
    [SerializeField] [Min(0)]
    private float returnDelay;
    private bool startDelay;

    [Tooltip("How much the projectiles damage should change when starting to come back")]
    [SerializeField]
    private float damageChange;
    [SerializeField]
    private float towerDamageChange;

    public bool IsBoomerang
    {
        get { return isBoomerang; }
        set { isBoomerang = value; }
    }

    public Vector3 StartLocation
    {
        get { return startLocation; }
        set { startLocation = value; }
    }
/*
    public Vector3 ReboundLocation
    {
        get { return reboundLocation; }
        set { reboundLocation = value; }
    }*/

    public bool GoingBack
    {
        get { return goingBack; }
        set { goingBack = value; }
    }

    public float PercentToSlow
    {
        get { return percentToSlow; }
    }

    public float SpeedModifier
    {
        get { return speedModifier; }
        set { speedModifier = value; }
    }

    public float ReturnDelay
    {
        get { return returnDelay; }
        set { returnDelay = value; }
    }

    public bool StartDelay
    {
        get { return startDelay; }
        set { startDelay = value; }
    }

    public void StartStats(GameObject go) {
        goingBack = false;
        startLocation = go.transform.position;
        speedModifier = 1; //this value is to increase the speed as the projectile goes back to prevent it from chasing the unit if the unit gets knocked away
    }

    public void SetBack(GameObject go) {
        Projectile ability = (go.GetComponent(typeof(Projectile)) as Projectile);

        goingBack = true;
        startDelay = true;
        startLocation = ability.TargetLocation;
        //startLocation = transform.position;

        ability.BaseDamage += damageChange;
        ability.TowerDamage += towerDamageChange;

        ability.KnockbackStats.InitialSpeed += ability.KnockbackStats.SpeedChange; //a very unique stat that only Ali'Ikai uses
        ability.KnockbackStats.KnockbackDuration *= 1.5f;
    }

    public float SpeedReduction(GameObject go, Vector3 reboundLocation, Vector3 lastKnownLocation) {
        Vector3 currentPostion = go.transform.position;
        float totalDistance;
        float distance;
        
        if(goingBack) {
            totalDistance = Vector3.Distance(reboundLocation, lastKnownLocation);
            distance = totalDistance - Vector3.Distance(currentPostion, reboundLocation); //distance from the rebound
        }
        else {
            totalDistance = Vector3.Distance(startLocation, reboundLocation);
            distance = totalDistance - Vector3.Distance(currentPostion, reboundLocation);
        }

        if(distance/totalDistance > percentToSlow) //if distance has reached the start slow threshold
            return Math.Max(1 - ( (distance - totalDistance*percentToSlow) / (totalDistance - totalDistance*percentToSlow) ), .1f) * speedModifier;
        else //else maintain normal speed
            return speedModifier;
    }

    public void Explode(GameObject go) {
        Projectile ability = (go.GetComponent(typeof(Projectile)) as Projectile);

        Vector3 position = new Vector3(go.transform.position.x, 0, go.transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(position, ability.Radius);
        
        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, damageable)) {
                    ability.SetHit = true;

                    float damage = ability.BaseDamage*ability.DamageMultiplier;
                    if(ability.TowerDamage > 0 && damageable.GetComponent<Tower>())
                        damage = ability.TowerDamage*ability.DamageMultiplier;

                    GameFunctions.Attack(damageable, damage, ability.CritStats);
                    ability.ApplyAffects(damageable);
                }
            }
        }
    }

}
