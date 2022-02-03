using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private float timer = .5f;

    void FixedUpdate()
    {
        if(timer > 0) {
            timer -= Time.deltaTime;
            gameObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0, timer);
        }
        else
            Destroy(gameObject);
        
    }

}