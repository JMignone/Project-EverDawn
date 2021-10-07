using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SummoningSicknessUI
{
    private IDamageable unit;

    [Tooltip("Determines how long a unit will be able to act after its summonProtectionDelay is up")]
    [SerializeField]
    private float summonSicknessDelay;

    [SerializeField]
    private float currSummonSicknessDelay;

    [Tooltip("Determines how long a unit will be undamageable and untargetable when summoned")]
    [SerializeField]
    private float summonProtectionDelay;

    //[Tooltip("Keeps the unit ")]
    //[SerializeField]
    //private float stayUntargetable;

    [SerializeField]
    private Canvas sSCanvas;

    [SerializeField]
    private Image sSSprite;

    [SerializeField]
    private Image sSMask;

    public bool SummonProtection
    {
        get { if(summonProtectionDelay < 0) return true;
              else return false; }
    }

    public float PercentReady 
    {
        get { if(summonSicknessDelay == 0) return 1;
              else return currSummonSicknessDelay/summonSicknessDelay; }
    }

    public bool IsReady 
    {
        get { if(summonSicknessDelay == 0) return true;
              else if(currSummonSicknessDelay < summonSicknessDelay) return false;
              else return true; }
    }

    public void StartStats(IDamageable go) {
        unit = go;
        if(summonSicknessDelay > 0)
            unit.Agent.HitBox.enabled = false;
    }

    public void UpdateStats() {
        if(summonProtectionDelay > 0) {
            sSCanvas.enabled = false;
            summonProtectionDelay -= Time.deltaTime;
        }
        else if(currSummonSicknessDelay < summonSicknessDelay) {
            unit.Agent.HitBox.enabled = true;
            sSCanvas.enabled = true;
            currSummonSicknessDelay += Time.deltaTime;
            sSMask.fillAmount = 1 - PercentReady;
            //sSSprite.transform.parent.rotation = Quaternion.Euler(0.0f, 180.0f, sSSprite.transform.parent.parent.parent.GetChild(0).rotation.z * -1.0f);
        }
        else
            sSCanvas.enabled = false;
    }

}
