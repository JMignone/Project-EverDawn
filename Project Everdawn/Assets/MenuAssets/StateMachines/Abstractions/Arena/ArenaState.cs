using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArenaState", menuName = "ScriptableObjects/StateMachines/States/ArenaState")]
public class ArenaState : BaseState
{
    [SerializeField] private SO_Arena arena;

    public SO_Arena Arena
    {
        get{return arena;}
    }

    public override void EnterState(BaseStateController baseStateController)
    {

    }

    public override void UpdateState(BaseStateController baseStateController)
    {

    }

    public override void DestroyState(BaseStateController baseStateController)
    {

    }
}
