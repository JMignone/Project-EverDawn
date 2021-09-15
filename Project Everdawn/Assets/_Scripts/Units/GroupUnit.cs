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
    private ShadowStats shadowStats;
    private List<GameObject> hitTargets;
    private List<GameObject> inRangeTargets;
    private List<GameObject> enemyHitTargets;
    private bool isHoveringAbility;
    private bool isCastingAbility;
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
        //set { agent = value; }
    }

    public Actor2D UnitSprite
    {
        get { return unitSprite; }
        //set { unitSprite = value; }
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

    public ShadowStats ShadowStats
    {
        get { return shadowStats; }
    }

    public bool IsMoving
    {
        get { return false; }
    }

    private void Start() {
        isHoveringAbility = false;
    }

    private void Update()
    {
        if(gameObject.transform.childCount == 0) {
            print(gameObject.name + " has died!");
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
