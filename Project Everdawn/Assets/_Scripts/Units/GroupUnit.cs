using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupUnit : MonoBehaviour, IDamageable
{


    [SerializeField]
    private BaseStats stats;

    private Actor3D agent;
    private Actor2D unitSprite;
    private GameObject target;
    private DashStats dashStats;
    private ShadowStats shadowStats;
    private DeathStats deathStats;
    private List<GameObject> hitTargets;
    private List<GameObject> inRangeTargets;
    private List<GameObject> enemyHitTargets;
    private List<GameObject> projectiles;
    private List<Component> applyEffectsComponents;
    //private bool isHoveringAbility;
    //private bool isCastingAbility;
    //Right now I need these so it has the interface. I need the interface because other parts of the code retrieve values by looking for the interface component.
    //These values will just be null

    public BaseStats Stats
    {
        get { return stats; }
        //set { stats = value; }
    }

    public Actor3D Agent
    {
        get { return agent; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    public List<GameObject> HitTargets
    {
        get { return hitTargets; }
    }

    public List<GameObject> InRangeTargets
    {
        get { return inRangeTargets; }
    }

    public List<GameObject> EnemyHitTargets
    {
        get { return enemyHitTargets; }
    }

    public List<GameObject> Projectiles
    {
        get { return projectiles; }
    }
    /*
    public bool IsHoveringAbility
    {
        get { return isHoveringAbility; }
        set { isHoveringAbility = value; }
    }

    public bool IsCastingAbility
    {
        get { return isCastingAbility; }
        set { isCastingAbility = value; }
    }
    */
    public DashStats DashStats
    {
        get { return dashStats; }
    }

    public ShadowStats ShadowStats
    {
        get { return shadowStats; }
    }

    public DeathStats DeathStats
    {
        get { return deathStats; }
    }

    public List<Component> ApplyEffectsComponents
    {
        get { return applyEffectsComponents; }
    }

    public bool IsMoving
    {
        get { return false; }
    }
/*
    private void Start() {
        isHoveringAbility = false;
    }*/

    private float currentDelay;
    private void FixedUpdate()
    {
        if(currentDelay < 10)
            currentDelay += Time.deltaTime;
        else {
            currentDelay = 0;
            if(gameObject.transform.childCount == 1)
                Destroy(gameObject);
        }
    }

    void IDamageable.TakeDamage(float amount) {
        //pass
    }

    public void SetTarget(GameObject newTarget) {
        //pass
    }

    public void ReTarget() {
        //pass
    }
}
