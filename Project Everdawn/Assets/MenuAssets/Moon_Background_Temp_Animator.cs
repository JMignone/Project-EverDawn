using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon_Background_Temp_Animator : MonoBehaviour
{
    public float delta_x;

    // Start is called before the first frame update
    void Start()
    {
        transform.LeanMoveLocal(transform.localPosition + new Vector3(delta_x, 0, 0), 10f).setEaseInOutSine().setLoopPingPong();
    }
}
