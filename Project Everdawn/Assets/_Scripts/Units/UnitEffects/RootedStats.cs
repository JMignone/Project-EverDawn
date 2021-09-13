using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootedStats
{
    [SerializeField]
    private bool cantBeRooted;
    private bool outSideResistance;

    [SerializeField]
    private bool isRooted;

    [SerializeField]
    private float rootDelay;

    [SerializeField]
    private float currentRootDelay;

    private Component damageableComponent;
    private float speed;

    public bool CantBeRooted
    {
        get { return cantBeRooted; }
        set { cantBeRooted = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public bool IsRooted
    {
        get { return isRooted; }
        set { isRooted = value; }
    }

    public float RootDelay
    {
        get { return rootDelay; }
        set { rootDelay = value; }
    }

    public float CurrentRootDelay
    {
        get { return currentRootDelay; }
        set { currentRootDelay = value; }
    }


    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public void StartRootedStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
        speed = (damageableComponent as IDamageable).Stats.MoveSpeed;
        isRooted = false;
        rootDelay = 0;
        currentRootDelay = 0;
    }

    public void UpdateRootedStats() {
        if(isRooted) {
            (damageableComponent as IDamageable).Stats.MoveSpeed = 0;
            if(currentRootDelay < rootDelay) 
                currentRootDelay += Time.deltaTime;
            else
                unRoot();
        }
    }

    public void Root(float duration) {
        if(!cantBeRooted && !outSideResistance) {
            isRooted = true;
            rootDelay = duration;
            currentRootDelay = 0;
            (damageableComponent as IDamageable).Stats.MoveSpeed = 0;
        }
    }

    public void unRoot() {
        isRooted = false;
        (damageableComponent as IDamageable).Stats.MoveSpeed = speed;
    }
}
