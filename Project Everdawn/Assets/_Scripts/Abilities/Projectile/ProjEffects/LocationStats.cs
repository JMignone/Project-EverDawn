using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationStats
{
    [SerializeField]
    private bool overridesLocation;
    private IDamageable unit;
    private GameObject projectile;
    private ICaster caster;

    [Tooltip("If checked, the location will be overridden when the projectile dies")]
    [SerializeField]
    private bool overridesAtEnd;

    [Tooltip("If checked, the location will be overridden constantly over a set delay")]
    [SerializeField]
    private bool overridesDuringFlight;

    [Tooltip("Only used if overridesDuringFlight is checked")]
    [SerializeField]
    private float overrideDelay;
    private float currentDelay;

    [SerializeField]
    private GameConstants.NEW_ABILITY_LOCATION newLocation;

    [Tooltip("Further adjustments for the new ability location. Negative numbers go backwards")]
    [SerializeField]
    private float forwardAdjustment;

    public bool OverridesLocation
    {
        get { return overridesLocation; }
    }

    public bool OverridesAtEnd
    {
        get { return overridesAtEnd; }
    }

    public void StartStats(IDamageable unit, GameObject proj, ICaster caster) {
        this.unit = unit;
        projectile = proj;
        this.caster = caster;
    }

    public void UpdateStats() {
        if(overridesDuringFlight) {
            if(currentDelay < overrideDelay)
                currentDelay += Time.deltaTime;
            else {
                currentDelay = 0;
                LocationOveride();
            }
        }
    }

    public void LocationOveride() {
        Vector3 location;
        if(newLocation == GameConstants.NEW_ABILITY_LOCATION.ON_ABILITY)
            location = projectile.transform.position;
        else if(newLocation == GameConstants.NEW_ABILITY_LOCATION.HALFWAY)
            location = new Vector3((projectile.transform.position.x + unit.Agent.transform.position.x)/2, 0, (projectile.transform.position.z + unit.Agent.transform.position.z)/2);
        else 
            location = unit.Agent.transform.position;

        location.y = 0;
        Vector3 direction = location - unit.Agent.transform.position;
        
        location += direction.normalized * forwardAdjustment;
        if(direction.normalized != (location - unit.Agent.transform.position).normalized) //if this adjustment makes the unit fire backwards, undo it
            location -= direction.normalized * forwardAdjustment;
        direction.y = 0;

        caster.SetNewLocation(location, direction);
    }
}
