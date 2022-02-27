using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckStateController : BaseStateController
{
    [SerializeField] private DeckState defaultDeckState;

    [ContextMenu("Run Awake")]
    private void Awake()
    {
        // Set initial state
        SetInitalState(defaultDeckState);
    }

    private void SetInitalState(DeckState initalState)
    {
        this.ChangeState(initalState);
    }
}
