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
        speedModifier = 1; //this value is to increase the speed as the projectile goes back to prevent it from chasing the unit if the unit gets knocked away
    }

    public void UpdateBoomerangStats() 
    {
        speedModifier += Time.deltaTime;
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

}
