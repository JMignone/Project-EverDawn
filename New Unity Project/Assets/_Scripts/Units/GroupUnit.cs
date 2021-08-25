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
    private int inRange;
    private List<GameObject> hitTargets;
    private List<GameObject> inRangeTargets;
    private List<GameObject> enemyHitTargets;
    private bool isHoveringAbility;
    private bool isCastingAbility;
    private Image abilityIndicator;
    private int indicatorNum;
    //Right now I need these so it has the interface. I need the interface because other parts of the code retrieve values by looknig for the interface component.
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

    public int InRange
    {
        get { return inRange; }
        set { inRange = value; }
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

    public Image AbilityIndicator
    {
        get { return abilityIndicator; }
    }

    public int IndicatorNum
    {
        get { return indicatorNum; }
        set { indicatorNum = value; }
    }

    private void Start() {
        isHoveringAbility = false;
        indicatorNum = 0;
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
}
