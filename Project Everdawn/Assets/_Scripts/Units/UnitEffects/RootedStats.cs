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

    public void StartRootedStats(IDamageable go) {
        unit = go;
    }

    public void UpdateRootedStats() {
        if(isRooted) {
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
        }
    }

    public void unRoot() {
        isRooted = false;
    }
}
