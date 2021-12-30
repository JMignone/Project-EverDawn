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

    private float rootDelay;
    private float currentRootDelay;

    private IDamageable unit;
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

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public void StartRootedStats(IDamageable go) {
        unit = go;
        speed = unit.Stats.MoveSpeed;
        isRooted = false;
    }

    public void UpdateRootedStats() {
        if(isRooted) {
            unit.Stats.MoveSpeed = 0;
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
            unit.Stats.MoveSpeed = 0;
        }
    }

    public void unRoot() {
        isRooted = false;
        unit.Stats.MoveSpeed = speed;
    }
}
