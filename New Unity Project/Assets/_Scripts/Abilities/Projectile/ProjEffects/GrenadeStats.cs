using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrenadeStats
{
    [SerializeField]
    private bool isGrenade;

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
    private Vector3 arcEnd;
    private float distanceCovered = 0.0f;
    public void StartGrenadeStats(GameObject go) {
        if(isGrenade) {
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.Radius = 0;
            projectile.HitBox.enabled = false;
            if(grenadeArcMultiplier < .1) //preventing divide by zero
                grenadeArcMultiplier = .1f;
            arcStart = go.transform.position;
            arcEnd = projectile.TargetLocation;
            arcApex = arcStart + (arcEnd - arcStart)/2 + Vector3.up * Vector3.Distance(arcStart, arcEnd) * grenadeArcMultiplier;
        }
    }

    public void UpdateGrenadeStats(GameObject go, float speed) {
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

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));
                if(go.GetComponent<Projectile>().WillHit(damageable))
                    GameFunctions.Attack(damageable, explosionDamage);
            }
        }
    }
}
