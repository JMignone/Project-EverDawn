using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimTest1 : MonoBehaviour
{
    //Create variables that will be used to determine object animation behavior, such as if it will fade, duration of that fade, etc.
    [SerializeField] private CanvasGroup objectToFade;
    [SerializeField] private bool move;
    [SerializeField] private bool fade;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float movementDuration;
    [SerializeField] private float deltaX;
    [SerializeField] private float deltaY;
    [SerializeField] private float deltaZ;

    public void flyFade()
    {
        if (move == true)
        {
            transform.LeanMoveLocal(transform.localPosition + new Vector3(deltaX, deltaY, deltaZ), movementDuration);
        }

        if (fade == true)
        {
            objectToFade.LeanAlpha(0, fadeDuration);
            
        }
    }
}
