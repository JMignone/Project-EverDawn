using UnityEngine;

[CreateAssetMenu(fileName = "NewMenuState", menuName = "ScriptableObjects/StateMachines/States/MenuState")]
public class MenuState : BaseState
{
    [SerializeField] private MenuStateEvent menuStateEvent;

    public override void EnterState(BaseStateController menuStateController)
    {
        menuStateEvent.Raise(this);
    }

    public override void UpdateState(BaseStateController baseStateController)
    {

    }

    public override void DestroyState(BaseStateController baseStateController)
    {

    }
}
