using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeathStats
{
    private IDamageable unit;

    [Tooltip("Makes the unit fire projectiles on death")]
    [SerializeField]
    private bool deathSkill;
    private bool isDying;

    [SerializeField]
    private List<GameObject> abilityPrefabs;

    [Tooltip("Determines the amount of time waited before firing each shot. Number of delays must be 1 more than the number of projectiles")]
    [SerializeField]
    private List<float> abilityDelays;

    private float currentDelay;
    private int currentProjectileIndex;

    public bool DeathSkill
    {
        get { return deathSkill; }
    }

    public bool IsDying
    {
        get { return isDying; }
    }

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 fireDirection;
    public void StartStats(GameObject go) {
        unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
        unit.SetTarget(null);
        //unit.Stats.Vanish((unit as Component).gameObject, unit.Agent); //unit.EnemyHitTargets.ToArray());
        unit.Stats.EffectStats.ResistStats.ResistDamage(100);
        unit.Stats.EffectStats.ResistStats.ResistTarget(100);

        unit.Agent.Agent.enabled = false;
        unit.Agent.HitBox.enabled = false;
        unit.Stats.DetectionObject.enabled = false;
        unit.Stats.VisionObject.enabled = false;

        isDying = true;
        if(unit.Stats.CanvasAbility != null)
            MonoBehaviour.Destroy(unit.Stats.CanvasAbility);
        MonoBehaviour.Destroy(unit.Stats.HealthBar.transform.parent.gameObject);

        if(unit.Stats.IsHoveringAbility) {
            GameManager.removeAbililtyIndicators();
            GameManager.GetPlayer(go.tag).NumDragging--;
        }

        startPosition = new Vector3(unit.Agent.transform.position.x, 0, unit.Agent.transform.position.z);
        endPosition = startPosition + unit.Agent.transform.forward * .001f; //moves the end position slightly forward 
        fireDirection = endPosition - startPosition;
    }

    public void FireDeathSkill() {
        if(currentDelay < abilityDelays[currentProjectileIndex]) //if we havnt reached the delay yet
            currentDelay += Time.deltaTime;
        else if(currentProjectileIndex == abilityPrefabs.Count) { //if we completed the last delay
            GameManager.RemoveObjectsFromList((unit as Component).gameObject);
            MonoBehaviour.Destroy((unit as Component).gameObject);
            MonoBehaviour.print((unit as Component).gameObject.name + " has died!");
        }
        else { //if we completed a delay
            if(abilityPrefabs[currentProjectileIndex].GetComponent<Projectile>())
                GameFunctions.FireProjectile(abilityPrefabs[currentProjectileIndex], startPosition, endPosition, fireDirection, unit, (unit as Component).gameObject.tag, 1);
            else if(abilityPrefabs[currentProjectileIndex].GetComponent<CreateAtLocation>())
                GameFunctions.FireCAL(abilityPrefabs[currentProjectileIndex], startPosition, endPosition, fireDirection, unit, (unit as Component).gameObject.tag, 1);
            currentDelay = 0;
            currentProjectileIndex++;
        }
    }
}
