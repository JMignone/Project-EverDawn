using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrenadeStats
{
    [SerializeField]
    private bool isGrenade;

    [Tooltip("If checked, a grenade will come from the edges of the arena rather than a unit. isGrenade must also be checked for this to work.")]
    [SerializeField]
    private bool isAirStrike;

    [Tooltip("If isAirStrike ischecked, determines where the airstrike will come from.")]
    [SerializeField]
    private GameConstants.AIR_STRIKE_LOCATION startLocation;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField] [Min(0)]
    private float explosionRadius;

    [Tooltip("A number from [0-infinity) that determines how large the arc of the grenade. 1 will set the arc to be a circle.")]
    [SerializeField] [Min(.1f)]
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
            projectile.Radius = .1f; //setting it to zero will not work if targeting moving targets
            projectile.HitBox.enabled = false;
            
            arcStart = go.transform.position;
            Vector3 arcEnd = projectile.TargetLocation;
            if(isAirStrike)
                arcApex = arcStart + (arcEnd - arcStart)/6 + Vector3.up * Vector3.Distance(arcStart, arcEnd) * grenadeArcMultiplier;
            else
                arcApex = arcStart + (arcEnd - arcStart)/2 + Vector3.up * Vector3.Distance(arcStart, arcEnd) * grenadeArcMultiplier;
            //this value arpApex is actually twice as high as the projectile would actually go, being fixed by the strange lerps below
        }
    }

    public void UpdateGrenadeStats(GameObject go, Vector3 arcEnd, float speed) {
        if(isGrenade) {//do some kind of arcing here
            distanceCovered += speed/Vector3.Distance(arcStart, arcEnd) * Time.deltaTime;

            Vector3 m1 = Vector3.Lerp( arcStart, arcApex, distanceCovered);
            Vector3 m2 = Vector3.Lerp( arcApex, arcEnd, distanceCovered);

            Vector3 direction = Vector3.Lerp(m1, m2, distanceCovered) - go.transform.position;
            if(direction != Vector3.zero)
                go.transform.GetChild(0).GetChild(0).rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
            go.transform.position = Vector3.Lerp(m1, m2, distanceCovered);
        }
    }

    public void Explode(GameObject go) {
        Vector3 position = new Vector3(go.transform.position.x, 0, go.transform.position.z);

        GameObject damageZone = MonoBehaviour.Instantiate(explosionEffect, position, Quaternion.identity);
        damageZone.transform.localScale = new Vector3(explosionRadius*2, .1f, explosionRadius*2);

        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        Projectile projectile = go.GetComponent<Projectile>();

        foreach(Collider collider in colliders) {
            if(!collider.CompareTag(go.tag) && collider.name == "Agent") {
                Component damageable = collider.transform.parent.GetComponent(typeof(IDamageable));

                if(GameFunctions.WillHit(projectile.HeightAttackable, projectile.TypeAttackable, damageable)) {
                    projectile.SetHit = true;

                    float damage = projectile.BaseDamage*projectile.DamageMultiplier;
                    if(damageable.GetComponent<Tower>())
                        damage = projectile.TowerDamage*projectile.DamageMultiplier;

                    GameFunctions.Attack(damageable, damage, projectile.CritStats);
                    projectile.ApplyAffects(damageable);
                }
            }
        }
    }
}