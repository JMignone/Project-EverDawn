using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//This script is WIP and non functional, much of its code has been commented out to prevent compile errors.

public class UIAnimator : MonoBehaviour
{
    [SerializeField] private LeanTweenType inType;
    [SerializeField] private LeanTweenType outType;
    [SerializeField] private float duration;
    [SerializeField] private float delay;
    [SerializeField] private UnityEvent onCompleteCallback;

    public void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
        //LeanTween.scale(gameObject, new Vector3(1, 1, 1), duration).setDelay(delay).setOnComplete(onComplete).setEase(inType);
    }

    public void OnComplete()
    {
        if (onCompleteCallback != null)
        {
            onCompleteCallback.Invoke();
        }
    }

    //When the close button is pressed
    public void OnClose()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.25f).setEase(outType).setOnComplete(DestroyMe);
    }

    void DestroyMe()
    {
        //Destroy(gameobject);
    }

};
