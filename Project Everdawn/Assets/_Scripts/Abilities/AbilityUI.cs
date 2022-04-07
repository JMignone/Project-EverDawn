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
    private Image abilitySprite;

    [SerializeField]
    private Image cooldownMask;

    [SerializeField]
    private Image abilityCancel;

    private bool canDrag;
    private bool canFire;
    private RectTransform cardCanvasDim;

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

    public bool CanDrag
    {
        get { return canDrag; }
        set { canDrag = value; }
    }

    public bool CanFire
    {
        get { return canFire; }
        set { canFire = value; }
    }

    public float PercentCooldown {
        get { return currCooldownDelay/cooldownDelay; }
    }

    public void StartStats() {
        cardCanvasDim = GameManager.Instance.Canvas.GetChild(0).GetComponent<RectTransform>();
        //abilityCancel = GameManager.Instance.Canvas.GetChild(3).GetComponent<Image>();
        abilityCancel = GameManager.Instance.AbilityCancel;
        canFire = true;
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
        else if(!canFire) {
            if(currCooldownDelay < cooldownDelay) {
                currCooldownDelay += Time.deltaTime;
                //canDrag = false;
            }
            else {
                canFire = true;
                EnableAbilities();
            }
            cooldownMask.fillAmount = 1 - PercentCooldown;
        }
    }

    //this just makes it so the icon does not rotate with the parent, note the 180, we may need to set this to 0 in the future
    // added an extra .parent on each side of the function to account for the change in hierarchy of the CanvasAbilityIcon object. Will need to remove if we revert to the old version of the object -Eagle
    public void LateUpdateStats() {
        abilitySprite.transform.parent.parent.parent.rotation = Quaternion.Euler(0.0f, 180.0f, abilitySprite.transform.parent.parent.parent.parent.parent.GetChild(0).rotation.z * -1.0f);
    }

    public void resetAbility() {
        currCooldownDelay = 0;
        canDrag = false;
        canFire = false;
        DisableAbilities();
    }

    //sets the ability previews to red to display that they are disabled
    // added an extra .parent in the foreach loop. Will need to remove if we revert to the old version of the object -Eagle
    public void DisableAbilities() {
        foreach(Transform child in abilitySprite.transform.parent.parent.parent.GetChild(2)) {
            if(child.childCount > 1) //this means its a complicated summon preview
                child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
            else
                child.GetChild(0).GetComponent<Image>().color = new Color32(255,0,0,50);
        }
    }

    // added an extra .parent in the foreach loop. Will need to remove if we revert to the old version of the object -Eagle
    public void EnableAbilities() {
        foreach(Transform child in abilitySprite.transform.parent.parent.parent.GetChild(2)) {
            if(child.childCount > 1) //this means its a complicated summon preview
                child.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
            else
                child.GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,100);
        }
    }

}
