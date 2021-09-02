using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SummoningSicknessUI
{
    [SerializeField]
    private float summonSicknessDelay;

    [SerializeField]
    private float currSummonSicknessDelay;

    [SerializeField]
    private Canvas sSCanvas;

    [SerializeField]
    private Image sSSprite;

    [SerializeField]
    private Image sSMask;

    public float SummonSicknessDelay
    {
        get { return summonSicknessDelay; }
        set { summonSicknessDelay = value; }
    }

    public float SurrSummonSicknessDelay
    {
        get { return currSummonSicknessDelay; }
        set { currSummonSicknessDelay = value; }
    }

    public Canvas SSCanvas
    {
        get { return sSCanvas; }
    }

    public Image SSSprite
    {
        get { return sSSprite; }
    }

    public Image SSMask
    {
        get { return sSMask; }
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

    public void UpdateStats() {
        if(currSummonSicknessDelay < summonSicknessDelay) {
            sSCanvas.enabled = true;
            currSummonSicknessDelay += Time.deltaTime;
            sSMask.fillAmount = 1 - PercentReady;
            //sSSprite.transform.parent.rotation = Quaternion.Euler(0.0f, 180.0f, sSSprite.transform.parent.parent.parent.GetChild(0).rotation.z * -1.0f);
        }
        else
            sSCanvas.enabled = false;
    }

}
