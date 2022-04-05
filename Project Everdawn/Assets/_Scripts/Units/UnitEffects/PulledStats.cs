using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PulledStats
{
    [Tooltip("A number from 0 to 1 that determines a units resistance to being pulled. A resistance of 1 means they cannot be pulled")]
    [SerializeField] [Range(0, 1)]
    private float pullResistance;
    private bool outSideResistance;

    [SerializeField]
    private List<Component> pullComponents;

    private IDamageable unit;
    private Vector3 direction;

    public float PullResistance
    {
        get { return pullResistance; }
        set { pullResistance = value; }
    }

    public bool OutSideResistance
    {
        get { return outSideResistance; }
        set { outSideResistance = value; }
    }

    public List<Component> PullComponents
    {
        get { return pullComponents; }
        set { pullComponents = value; }
    }

    public void StartPulledStats(IDamageable go) {
        unit = go;
        if(pullResistance < 0)
            pullResistance = 0;
    }

    public void UpdatePulledStats() {
        if(pullResistance < 1 && !outSideResistance && pullComponents.Count > 0) {
            Vector3 totalMovement = Vector3.zero;
            pullComponents.RemoveAll(item => item == null); //removes a pull component if it becomes null
            foreach(Component IAbility in pullComponents) {
                Vector3 direction = ((IAbility as IAbility).Position() - unit.Agent.transform.position).normalized;
                direction *= (IAbility as IAbility).PullStats.Speed * (1 - pullResistance);
                totalMovement += direction;
            }
            totalMovement.y = 0;

            if(unit.JumpStats.Jumping)
                unit.JumpStats.DirectionalInfluence(totalMovement);
            else
                unit.Agent.transform.position += totalMovement * Time.deltaTime;
        }
    }

    public void AddPull(Component IAbility) {
        pullComponents.Add(IAbility);
    }

    public void RemovePull(Component IAbility) {
        pullComponents.Remove(IAbility);
    }
}
