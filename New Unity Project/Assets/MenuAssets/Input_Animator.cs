using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Input_Animator : MonoBehaviour
{
    public ParticleSystem Tap_Animation_Particle_System;
    //public ParticleSystem Cursor_Trail_Particle_System;
    //public GameObject Instantiation_Object;
    private Vector2 Input_Position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Change Instantiation location to be the same as the input position
        //Instantiation_Object.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if a tap/click has occurred
        if (Input.GetMouseButtonDown(0) == true)
        {
            //Set Input_Position variable to be the location of the click
            Input_Position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Play tap animation
            Tap_Animation_Play();

            //Play cursor trail
            //Cursor_Trail_Play();
        }

        /*
        if((Input.GetMouseButtonDown(0) == false) && (Instantiation_Object.transform.hierarchyCount > 0))
        {
            for (var i = Instantiation_Object.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(Instantiation_Object.transform.GetChild(i).gameObject, 5f);
            }
        }
        */
    }

    // Play the tap animation particle system at input location
    public void Tap_Animation_Play()
    {
        //Intatntiate the Tap_Animation particle system
        Instantiate(Tap_Animation_Particle_System, Input_Position, Quaternion.identity);
    }

    /*
    // Play the cursor trail particle system at input location
    public void Cursor_Trail_Play()
    {
        //Intatntiate the cursor trail particle system
        Instantiate(Cursor_Trail_Particle_System, Instantiation_Object.transform);
    }
    */

} // End of Class
