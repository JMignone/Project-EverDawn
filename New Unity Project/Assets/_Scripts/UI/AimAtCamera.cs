using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// taken from https://www.youtube.com/watch?v=ViZto58MgjM

//how will this work when there are 2 players?
//2 cameras? 2 sets of healthbars?

public class AimAtCamera : MonoBehaviour
{
    Camera my_camera;

    void Awake() {
        my_camera = Camera.main;
    }

    void Update() {
        transform.LookAt(transform.position + my_camera.transform.rotation * Vector3.back,
                        my_camera.transform.rotation * Vector3.down);
    }
}
