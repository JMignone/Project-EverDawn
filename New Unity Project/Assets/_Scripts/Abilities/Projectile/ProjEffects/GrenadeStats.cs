using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrenadeStats
{
    [SerializeField]
    private bool isGrenade;

    [SerializeField]
    private bool isAirStrike;

    [SerializeField]
    private GameConstants.AIR_STRIKE_LOCATION startLocation;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private float explosionDamage;

    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private float grenadeArcMultiplier; //1 sets the grenade to a circular arc, a smaller number makes the arc smaller, a bigger number makes the arc larger

    public bool IsGrenade
    {
        get { return isGrenade; }
    }

    public bool IsAirStrike
    {
        get { return isAirStrike; }
    }

    public GameConstants.AIR_STRIKE_LOCATION StartLocation
    {
        get { return startLocation; }
    }

    public GameObject ExplosionEffect
    {
        get { return explosionEffect; }
    }

    public float ExplosionDamage
    {
        get { return explosionDamage; }
    }

    public float ExplosionRadius
    {
        get { return explosionRadius; }
    }

    public float GrenadeArcMultiplier
    {
        get { return grenadeArcMultiplier; }
    }

    /*
        The following 4 variables
        are used for the grenade arc
    */
    private Vector3 arcStart;
    private Vector3 arcApex;
    //private Vector3 arcEnd;
    private float distanceCovered = 0.0f;
    public void StartGrenadeStats(GameObject go) {
        if(isGrenade) {
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Radius = 0;
            projectile.HitBox.enabled = false;
            
            if(grenadeArcMultiplier < .1) //preventing divide by zero
                grenadeArcMultiplier = .1f;
            Vector3 arcEnd = projectile.TargetLocation;
            if(isAirStrike) {
                if(startLocation == GameConstants.AIR_STRIKE_LOCATION.BOTTOM)
                    arcStart = new Vector3(0, 0, GameManager.Instance.Ground.transform.localScale.z*-5 - 10);
                else
                    arcStart = new Vector3(GameManager.Instance.Ground.transform.localScale.x*-5 - 20, 0, 0);
                arcStart = new Vector3(arcStart.x, 15, arcStart.z);
                arcApex = arcStart + (arcEnd - arcStart)/6 + Vector3.up * Vector3.Distance(arcStart, arcEnd) * grenadeArcMultiplier;
            }
            else {
                arcStart = go.transform.position;
                arcApex = arcStart + (arcEnd - arcStart)/2 + Vector3.up * Vector3.Distance(arcStart, arcEnd) * grenadeArcMultiplier;
            }
        }
    }

    public void UpdateGrenadeStats(GameObject go, Vector3 arcEnd, float speed) {
        if(isGrenade) {//do some kind of arcing here
            distanceCovered += speed/Vector3.Distance(arcStart, arcEnd) * Time.deltaTime;

            Vector3 m1 = Vector3.Lerp( arcStart, arcApex, distanceCovered);
            Vector3 m2 = Vector3.Lerp( arcApex, arcEnd, distanceCovered);

            go.transform.GetChild(0).GetChild(0).rotation = Quaternion.LookRotation(Vector3.Lerp(m1, m2, distanceCovered) - go.transform.position) * Quaternion.Euler(90, 0, 0);
            go.transform.position = Vector3.Lerp(m1, m2, distanceCovered);
        }
    }

    public void Explode(GameObject go) {
        //Instantiate(explosionEffect, go.transform.position, go.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(go.transform.position, explosionRadius);
        Projectile projectile = go.GetComponent<Projectile>();

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(GameFunctions.WillHit(projectile.ObjectAttackable, damageable)) {
                    GameFunctions.Attack(damageable, explosionDamage);
                    projectile.ApplyAffects(damageable);
                }
            }
        }
    }
}
