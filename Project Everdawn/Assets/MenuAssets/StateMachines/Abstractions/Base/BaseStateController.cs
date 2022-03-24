using UnityEngine;

public abstract class BaseStateController : ScriptableObject
{
    // Reference to currently operating state.
    public BaseState currentState { get; private set; }

    public void ChangeState(BaseState newState) // Changes state
    {
        // Destroy current state
        if (currentState != null)
        {
            currentState.DestroyState(this);
        }

        currentState = newState; // Swap reference
        //Debug.Log(currentState.name + " is now current state.");
        
        if (currentState != null)
        {
            currentState.owner = this;
            currentState.EnterState(this);
        }
    }

    [ContextMenu("Print Current State")]
    private void PrintCurrentState()
    {
        Debug.Log(currentState.name + " is now current state.");
    }
}
