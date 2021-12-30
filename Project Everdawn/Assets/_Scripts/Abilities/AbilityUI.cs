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

    public float PercentCooldown {
        get { return currCooldownDelay/cooldownDelay; }
    }

    public void StartStats() {
        cardCanvasDim = GameFunctions.GetCanvas().GetChild(0).GetComponent<RectTransform>();
        abilityCancel = GameFunctions.GetCanvas().GetChild(3).GetComponent<Image>();
    }

    public void UpdateStats() {
        if(currCooldownDelay < cooldownDelay) {
            currCooldownDelay += Time.deltaTime;
            canDrag = false;
        }
        else
            canDrag = true;
        cooldownMask.fillAmount = 1 - PercentCooldown;
    }

    //this just makes it so the icon does not rotate with the parent, note the 180, we may need to set this to 0 in the future
    public void LateUpdateStats() {
        abilitySprite.transform.parent.parent.rotation = Quaternion.Euler(0.0f, 180.0f, abilitySprite.transform.parent.parent.parent.parent.GetChild(0).rotation.z * -1.0f);
    }

    public void resetAbility() {
        currCooldownDelay = 0;
    }

}
