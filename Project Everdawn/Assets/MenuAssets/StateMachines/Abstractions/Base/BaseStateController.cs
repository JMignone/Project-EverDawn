using UnityEngine;

[System.Serializable]
public abstract class BaseStateController : ScriptableObject
{
    // Reference to currently operating state.
    [SerializeField] protected BaseState currentState;

    public BaseState CurrentState
    {
        get {return currentState;}
    }

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
