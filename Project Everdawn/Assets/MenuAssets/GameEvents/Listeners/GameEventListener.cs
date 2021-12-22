using UnityEngine;
using UnityEngine.Events;

public abstract class GameEventListener<T, GE, UER> : MonoBehaviour
    where GE : GameEvent<T>
    where UER : UnityEvent<T>
{
    [SerializeField] protected GE gameEvent;
    [SerializeField] protected UER unityEventResponse;

    protected void OnEnable()
    {
        if (gameEvent is null) return;
        gameEvent.eventListeners += TriggerResponses; // Subscribe
    }

    protected void OnDisable()
    {
        if (gameEvent is null) return;
        gameEvent.eventListeners -= TriggerResponses; // Unsubscribe
    }

    [ContextMenu("Trigger Responses")]
    public void TriggerResponses(T val)
    {
        //UnityEvents do nullchecks themselves
        unityEventResponse.Invoke(val);
    }
}
