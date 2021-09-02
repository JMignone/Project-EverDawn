using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PulledStats
{
    [SerializeField]
    private bool cantBePulled;

    [SerializeField]
    private List<Component> pullComponents;

    private Component damageableComponent;
    private Vector3 direction;

    public bool CantBePulled
    {
        get { return cantBePulled; }
        set { cantBePulled = value; }
    }

    public List<Component> PullComponents
    {
        get { return pullComponents; }
        set { pullComponents = value; }
    }

    public Component DamageableComponent
    {
        get { return damageableComponent; }
        set { damageableComponent = value; }
    }

    public void StartPulledStats(GameObject go) {
        damageableComponent = go.GetComponent(typeof(IDamageable));
    }

    public void UpdatePulledStats() {
        if(!cantBePulled && pullComponents.Count > 0) {
            Vector3 totalMovement = Vector3.zero;
            pullComponents.RemoveAll(item => item == null);
            foreach(Component IAbility in pullComponents) {
                Vector3 direction = ((IAbility as IAbility).Position() - (damageableComponent as IDamageable).Agent.transform.position).normalized;
                direction *= (IAbility as IAbility).PullStats.Speed;
                totalMovement += direction;
            }
            totalMovement.y = 0;
            (damageableComponent as IDamageable).Agent.transform.position += totalMovement * Time.deltaTime;
        }
    }

    public void AddPull(Component IAbility) {
        pullComponents.Add(IAbility);
    }

    public void RemovePull(Component IAbility) {
        pullComponents.Remove(IAbility);
    }
}
