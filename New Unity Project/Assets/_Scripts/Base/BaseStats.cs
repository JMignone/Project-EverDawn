using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseStats
{
    [SerializeField]
    private float currHealth;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float range;
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float currAttackDelay;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private SphereCollider detectionObject;
    [SerializeField]
    private SphereCollider visionObject;
    [SerializeField]
    private GameConstants.OBJECT_TYPE objectType;
    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;
    [SerializeField]
    private GameConstants.UNIT_RANGE unitRange;
    
    public float PercentHealth {
        get { return currHealth/maxHealth; }
    }

    public float CurrHealth {
        get { return currHealth; }
        set { 
            if(value <= 0)
                currHealth = 0;
            else if(value >= maxHealth)
                currHealth = maxHealth;
            else
                currHealth = value;
        }
    }

    public float MaxHealth {
        get { return maxHealth; }
        //set { maxHealth = value; }
    }

    public float Range
    {
        get { return range; }
        //set { range = value; }
    }

    public float BaseDamage
    {
        get { return baseDamage; }
        //set { baseDamage = value; }
    }

    public float AttackDelay
    {
        get { return attackDelay; }
        //set { attackDelay = value; }
    }

    public float CurrAttackDelay
    {
        get { return currAttackDelay; }
        set { currAttackDelay = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public Image HealthBar
    {
        get { return healthBar; }
        //set { healthBar = value; }
    }

    public SphereCollider DetectionObject
    {
        get { return detectionObject; }
        //set { detectionObject = value; }
    }

    public SphereCollider VisionObject
    {
        get { return visionObject; }
        //set { visionObject = value; }
    }

    public GameConstants.OBJECT_TYPE ObjectType
    {
        get { return objectType; }
        //set { objectType = value; }
    }
    
    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
        //set { objectAttackable = value; }
    }

    public GameConstants.UNIT_RANGE UnitRange
    {
        get { return unitRange; }
        //set { unitRange = value; }
    }

    public void UpdateStats(int inRange) {
        if(PercentHealth == 1) {
            HealthBar.enabled = false;
            HealthBar.transform.GetChild(0).gameObject.SetActive(false); //this is the image border
        }
        else {
            HealthBar.enabled = true;
            HealthBar.transform.GetChild(0).gameObject.SetActive(true);
        }
        HealthBar.fillAmount = PercentHealth;

        detectionObject.radius = range;
        if(inRange > 0) {
            if(currAttackDelay < attackDelay) 
                currAttackDelay += Time.deltaTime;
        }
        else
            currAttackDelay = 0;
            
    }
    
}
