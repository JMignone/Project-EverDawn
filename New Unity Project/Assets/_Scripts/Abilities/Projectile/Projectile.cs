using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private ProjActor3D agent;

    [SerializeField]
    private ProjActor2D unitSprite;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float range;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private bool canPierce;

    [SerializeField]
    private bool isGrenade;

    [SerializeField]
    private bool areaOfEffect;

    [SerializeField]
    private bool selfDestructs;

    [SerializeField]
    private float explosionRadius;

    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;

    public ProjActor3D Agent
    {
        get { return agent; }
    }

    public ProjActor2D UnitSprite
    {
        get { return unitSprite; }
    }

    public float Radius
    {
        get { return radius; }
    }

    public float Speed
    {
        get { return speed; }
    }

    public float Range
    {
        get { return range; }
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        //set { baseDamage = value; }
    }

    public bool CanPierce
    {
        get { return canPierce; }
    }

    public bool IsGrenade
    {
        get { return isGrenade; }
    }

    public bool AreaOfEffect
    {
        get { return areaOfEffect; }
    }

    public bool SelfDestructs
    {
        get { return selfDestructs; }
    }

    public float ExplosionRadius
    {
        get { return explosionRadius; }
    }

    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
    }

    private void Start() {
        agent.HitBox.radius = radius;
        agent.Agent.speed = speed;
        agent.Agent.stoppingDistance = radius;
    }

    public void Fire(Vector3 startPosition, Vector3 mousePosition, Vector3 direction) {
        Quaternion targetRotation = Quaternion.LookRotation(-direction);
        float distance = Vector3.Distance(startPosition, mousePosition);
        Vector3 endPosition = mousePosition;
        if(distance > range)
            endPosition = startPosition + (direction.normalized * range * -1);
        else if(distance < range && !isGrenade && !selfDestructs)
            endPosition = startPosition + (direction.normalized * range * -1);
        startPosition += direction.normalized * radius * -1;
        GameObject go = GameObject.Instantiate(gameObject, startPosition, targetRotation, GameManager.GetUnitsFolder());
        go.GetComponent<Projectile>().Agent.Agent.SetDestination(endPosition);
    }

    public void hit(Component damageable) {
        bool willHit = false;
        if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.BOTH) //If the unit can attack the flying or ground unit, continue
            willHit = true;
        else if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.GROUND && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.GROUND)
            willHit = true;
        else if(objectAttackable == GameConstants.OBJECT_ATTACKABLE.FLYING && (damageable as IDamageable).Stats.MovementType == GameConstants.MOVEMENT_TYPE.FLYING)
            willHit = true;

        if(willHit) {
            if(areaOfEffect) {
                //do later
            }
            else {
                GameFunctions.Attack(damageable, baseDamage);
            }

            if(!canPierce)
                Destroy(gameObject);
        }
    }
}
