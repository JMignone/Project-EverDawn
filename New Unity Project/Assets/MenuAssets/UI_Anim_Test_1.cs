using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Anim_Test_1 : MonoBehaviour
{
    //Create variables that will be used to determine object animation behavior, such as if it will fade, duration of that fade, etc.
    public CanvasGroup Fade_Obj;
    public bool Move;
    public bool Fade;
    public float Fade_Duration;
    public float Move_Duration;
    public float DeltaX;
    public float DeltaY;
    public float DeltaZ;

    public void Fly_Fade()
    {
        if (Move == true)
        {
            transform.LeanMoveLocal(transform.localPosition + new Vector3(DeltaX, DeltaY, DeltaZ), Move_Duration);
        }

        if (Fade == true)
        {
            Fade_Obj.LeanAlpha(0, Fade_Duration);
            
        }
    }
}
