using UnityEngine;

public abstract class BaseState : ScriptableObject
{
    public BaseStateController owner;

    public abstract void EnterState(BaseStateController baseStateController);
    public abstract void UpdateState(BaseStateController baseStateController);
    public abstract void DestroyState(BaseStateController baseStateController);
}
