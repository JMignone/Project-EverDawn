using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AbilityUI
{
    [SerializeField] [Min(0)]
    private float cooldownDelay;

    [SerializeField] [Min(0)]
    private float currCooldownDelay;

    [SerializeField]
    private Transform abilityObject;

    [SerializeField]
    private Image abilitySprite;

    [SerializeField]
    private Image cooldownMask;

    [SerializeField]
    private Image abilityCancel;

    private Transform agentTranform;
    private Transform abilityPreviewCanvas;

    private bool canDrag;
    private bool canFire;
    private bool offCooldown;
    private bool cantFire;
    private RectTransform cardCanvasDim;
    private float cardCanvasScale;

    public float CooldownDelay
    {
        get { return cooldownDelay; }
        set { cooldownDelay = value; }
    }

    public float CurrCooldownDelay
    {
        get { return currCooldownDelay; }
        set { currCooldownDelay = value; }
    }

    public Image AbilitySprite
    {
        get { return abilitySprite; }
        //set { abilitySprite = value; }
    }

    public Image CooldownMask
    {
        get { return cooldownMask; }
        //set { cooldownMask = value; }
    }

    public Image AbilityCancel
    {
        get { return abilityCancel; }
    }

    public RectTransform CardCanvasDim
    {
        get { return cardCanvasDim; }
        set { cardCanvasDim = value; }
    }

    public float CardCanvasScale
    {
        get { return cardCanvasScale; }
        set { cardCanvasScale = value; }
    }

    public bool CanDrag
    {
        get { return canDrag; }
        set { canDrag = value; }
    }

    public bool CanFire
    {
        get { if(!cantFire && offCooldown) return true; else return false; }
    }

    public bool OffCooldown
    {
        get { return offCooldown; }
    }

    public bool CantFire
    {
        get { return cantFire; }
        set { cantFire = value; }
    }

    public float PercentCooldown {
        get { return currCooldownDelay/cooldownDelay; }
    }

    public void StartStats(GameObject unit, Canvas previewCanvas) {
        agentTranform = unit.transform.GetChild(0).transform;
        abilityPreviewCanvas = previewCanvas.transform;
        cardCanvasDim = GameManager.Instance.Canvas.GetChild(0).GetComponent<RectTransform>();
        cardCanvasScale = GameManager.Instance.Canvas.GetComponent<RectTransform>().localScale.x; //this is used to compensate for differing resolutions
        //abilityCancel = GameManager.Instance.Canvas.GetChild(3).GetComponent<Image>();
        abilityCancel = GameManager.Instance.AbilityCancel;
        offCooldown = true;
    }
    /*
    public void UpdateStats() {
        if(!canDrag) {
            if(currCooldownDelay < cooldownDelay) {
                currCooldownDelay += Time.deltaTime;
                //canDrag = false;
            }
            else
                canDrag = true;
            cooldownMask.fillAmount = 1 - PercentCooldown;
        }
    }*/

    public void UpdateStats() {
        if(!canDrag) {
            if(currCooldownDelay < cooldownDelay - 2) {
                currCooldownDelay += Time.deltaTime;
                //canDrag = false;
            }
            else
                canDrag = true;
            cooldownMask.fillAmount = 1 - PercentCooldown;
        }
        else if(!offCooldown) {
            if(currCooldownDelay < cooldownDelay) {
                currCooldownDelay += Time.deltaTime;
                //canDrag = false;
            }
            else {
                offCooldown = true;
                EnableAbilities();
            }
            cooldownMask.fillAmount = 1 - PercentCooldown;
        }
    }

    //this just makes it so the icon does not rotate with the parent, note the 180, we may need to set this to 0 in the future
    public void LateUpdateStats() {
        abilityObject.rotation = Quaternion.Euler(0.0f, 180.0f, agentTranform.rotation.z * -1.0f);
    }

    public void resetAbility() {
        currCooldownDelay = 0;
        canDrag = false;
        offCooldown = false;
        DisableAbilities();
    }

    //sets the ability previews to red to display that they are disabled
    public void DisableAbilities() {
        foreach(Transform child in abilityPreviewCanvas) {
            if(child.childCount > 1) //this means its a complicated summon preview
                child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
            else
                child.GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
        }
    }

    public void EnableAbilities() {
        foreach(Transform child in abilityPreviewCanvas) {
            if(child.childCount > 1) //this means its a complicated summon preview
                child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
            else
                child.GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
        }
    }

}
