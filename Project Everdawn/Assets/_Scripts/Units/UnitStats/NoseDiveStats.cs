using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoseDiveStats
{
    [Tooltip("Makes the unit nose dive towards its target, killing itself in the process")]
    [SerializeField]
    private bool noseDives;
    private bool isDiving;

    [SerializeField]
    private GameObject createAtLocation;

    [SerializeField] [Min(0)]
    private float speed;

    private IDamageable unit;
    private Vector3 targetPosition;

    public bool NoseDives
    {
        get { return noseDives; }
    }

    public bool IsDiving
    {
        get { return isDiving; }
    }

    public void StartStats(IDamageable go) {
        if(noseDives)
            unit = go;
    }

    public void StartDive(IDamageable target) {
        isDiving = true;
        targetPosition = target.Agent.transform.position;
        targetPosition.y = 0;

        unit.SetTarget(null);

        unit.Agent.Agent.enabled = false;
        unit.Agent.transform.rotation = Quaternion.LookRotation((targetPosition - unit.Agent.transform.position).normalized);

        unit.Stats.HealthBar.transform.parent.GetComponent<Canvas>().enabled = false;

        unit.Stats.EffectStats.ResistStats.ResistDamage(100);
        unit.Stats.EffectStats.ResistStats.ResistTarget(100);
    }

    public void UpdateStats() {
        unit.Agent.transform.position += (targetPosition - unit.Agent.transform.position).normalized * speed * Time.deltaTime;

        if(Vector3.Distance(unit.Agent.transform.position, targetPosition) < 1) {
            GameFunctions.FireCAL(createAtLocation, targetPosition, targetPosition, Vector3.zero, null, (unit as Component).gameObject.tag, unit.Stats.EffectStats.StrengthenedStats.CurrentStrengthIntensity);
            GameManager.RemoveObjectsFromList((unit as Component).gameObject);
            if(unit.Target != null)
                (unit.Target.GetComponent(typeof(IDamageable)) as IDamageable).EnemyHitTargets.Remove((unit as Component).gameObject);
            MonoBehaviour.Destroy((unit as Component).gameObject);
        }
    }
}
