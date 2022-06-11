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

    private bool stopUpdate;

    public bool SummonProtection
    {
        get { if(summonProtectionDelay > 0) return true;
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
        if((unit as Component).gameObject.GetComponent<Tower>()) {
            sSCanvas.enabled = false;
            GameFunctions.EnableAbilities(unit);
            stopUpdate = true;
        }
    }

    public void UpdateStats() {
        if(!stopUpdate) {
            if(summonProtectionDelay > 0) {
                sSCanvas.enabled = false;
                summonProtectionDelay -= Time.deltaTime;

                if(summonProtectionDelay <= 0) {
                    //check if there is currently an ability hovered over this unit now
                    Collider[] colliders = Physics.OverlapSphere(unit.Agent.transform.position, unit.Agent.HitBox.radius);
                    foreach(Collider collider in colliders) {
                        if(!collider.transform.parent.parent.CompareTag((unit as Component).gameObject.tag) && collider.CompareTag("AbilityHighlight")) { //Our we getting previewed for an ability?
                            AbilityPreview ability = collider.GetComponent<AbilityPreview>();
                            if(GameFunctions.WillHit(ability.HeightAttackable, ability.TypeAttackable, (unit as Component))) {
                                unit.Stats.IncIndicatorNum();
                                ability.Targets.Add((unit as Component).gameObject);
                            }
                        }
                    }
                }
            }
            else if(currSummonSicknessDelay < summonSicknessDelay) {
                unit.Agent.HitBox.enabled = true;
                sSCanvas.enabled = true;
                currSummonSicknessDelay += Time.deltaTime;
                sSMask.fillAmount = 1 - PercentReady;
            }
            else {
                sSCanvas.enabled = false;
                GameFunctions.EnableAbilities(unit);
                stopUpdate = true;
                unit.DashStats.checkDash();
            }
        }
    }

}
