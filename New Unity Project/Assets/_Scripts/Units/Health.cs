using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from https://weeklyhow.com/how-to-make-a-health-bar-in-unity/

public class Health : MonoBehaviour
{
    public int curHealth = 0;
    public int maxHealth = 100;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            DamageUnit(10);
        }
    }

    public void DamageUnit( int damage )
    {
        curHealth -= damage;

        healthBar.SetHealth( curHealth );
    }
}