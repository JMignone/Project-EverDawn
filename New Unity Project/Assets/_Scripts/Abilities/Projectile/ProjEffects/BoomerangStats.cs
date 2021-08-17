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

    [SerializeField]
    private float percentToSlow; //a number from 0 to 1

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

    public void StartBoomerangStats(GameObject go) {
        goingBack = false;
        startLocation = go.transform.position;
        //reboundLocation = go.GetComponent<Projectile>().TargetLocation;
        if(percentToSlow == 0) //0 will cause the projectile to not move
            percentToSlow = .05f;
        speedModifier = 1; //this value is to increase the speed as the projectile goes back to prevent it from chasing the unit if the unit gets knocked away
    }

    public void UpdateBoomerangStats() 
    {
        speedModifier += Time.deltaTime * 1f;
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
            return 1 * speedModifier;
    }

}
