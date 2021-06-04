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
    private GameConstants.OBJECT_TYPE objectType;
    [SerializeField]
    private GameConstants.OBJECT_ATTACKABLE objectAttackable;
    
    public GameConstants.OBJECT_ATTACKABLE ObjectAttackable
    {
        get { return objectAttackable; }
        //set { myVar = value; }
    }
    
    public GameConstants.OBJECT_TYPE ObjectType
    {
        get { return objectType; }
        //set { myVar = value; }
    }
    
    public SphereCollider DetectionObject
    {
        get { return detectionObject; }
        //set { detectionObject = value; }
    }
    
    public Image HealthBar
    {
        get { return healthBar; }
        //set { healthBar = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float CurrAttackDelay
    {
        get { return currAttackDelay; }
        set { currAttackDelay = value; }
    }
    
    public float AttackDelay
    {
        get { return attackDelay; }
        //set { attackDelay = value; }
    }
    
    public float BaseDamage
    {
        get { return baseDamage; }
        //set { baseDamage = value; }
    }

    public float Range
    {
        get { return range; }
        //set { range = value; }
    }

    public float PercentHealth {
        get { return currHealth/maxHealth; }
    }

    public float MaxHealth {
        get { return maxHealth; }
        //set { maxHealth = value; }
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

    public void UpdateStats() {
        //This if/else block will hide/unhide the healthbar, but is this effecient?
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
        if(currAttackDelay < attackDelay) 
            currAttackDelay += Time.deltaTime;
            
    }
    
}
