using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedArenaStateDisplay : MonoBehaviour
{
    [SerializeField] private ArenaDisplay arenaDisplay;
    [SerializeField] private ArenaStateController arenaStateController;
    
    private void OnEnable()
    {
        SetArenaDisplayToCurrentArenaState();
    }

    private void SetArenaDisplayToCurrentArenaState()
    {
        ArenaState state = (ArenaState)arenaStateController.CurrentState;
        arenaDisplay.BindArenaData(state.Arena);
    }
}
