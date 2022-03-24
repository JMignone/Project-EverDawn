using UnityEngine;

[CreateAssetMenu(fileName = "NewMenuStateController", menuName = "ScriptableObjects/StateMachines/Controllers/MenuStateController")]
public class MenuStateController : BaseStateController
{
    [SerializeField] private MenuState menuStartingState;

    [ContextMenu("Run Awake")]
    private void Awake()
    {
        // Set initial state
        SetInitalState(menuStartingState);
    }

    private void SetInitalState(MenuState initalState)
    {
        this.ChangeState(initalState);
    }
}
