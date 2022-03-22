using UnityEngine;

//[CreateAssetMenu(fileName = "NewDeckState", menuName = "ScriptableObjects/StateMachines/States/DeckState")]
public class DeckState : BaseState
{
    [Min(0)] public int deckNumber;
    private DeckStateController controller;

    public override void EnterState(BaseStateController baseStateController)
    {
        controller = (DeckStateController)this.owner;
        if(controller is DeckStateController)
        {
            controller.GetCurrentDeck();
        }
    }

    public override void UpdateState(BaseStateController baseStateController)
    {

    }

    public override void DestroyState(BaseStateController baseStateController)
    {
        controller = (DeckStateController)this.owner;
        if(controller is DeckStateController)
        {
            controller.SaveCurrentDeck();
        }
    }
}