using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupUnit : MonoBehaviour, IDamageable
{


    [SerializeField]
    private BaseStats stats;

    private Actor3D agent;
    private GameObject target;
    private int inRange;
    private List<GameObject> hitTargets;
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
        //set { hitTargets = value; }
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
}
