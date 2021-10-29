using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonBackgroundTempAnimator : MonoBehaviour
{
    [SerializeField] private float deltaX;
    [SerializeField] private float time;

    // Start is called before the first frame update
    void Start()
    {
        transform.LeanMoveLocal(transform.localPosition + new Vector3(deltaX, 0, 0), time).setEaseInOutSine().setLoopPingPong();
    }
}
