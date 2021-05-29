using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// taken from https://weeklyhow.com/how-to-make-a-health-bar-in-unity/

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Health playerHealth;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;
    }

    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    }
}