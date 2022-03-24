using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewDeckStateController", menuName = "ScriptableObjects/StateMachines/Controllers/DeckStateController")]
public class DeckStateController : BaseStateController
{
    [SerializeField] private DeckState defaultDeckState;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] [Tooltip("Used to check through possible states to load after application quit")]
        private List<DeckState> states;

    private readonly string selectedPlayerDeck = "selectedPlayerDeck";

    public PlayerDeck selectedDeck = new PlayerDeck();
    public List<SO_Card> selectedCards = new List<SO_Card>();

    [ContextMenu("Run Awake")]
    private void Awake()
    {
        int prevDeckID = LoadDeckNumberFromPlayerPrefs();
        foreach(DeckState ds in states)
        {
            if(prevDeckID != ds.deckNumber)
            {
                continue;
            }
            else if(prevDeckID == ds.deckNumber)
            {
                ChangeState(ds);
            }
        }
    }

    private void OnDisable()
    {
        //Debug.Log("DSC Disabled");
        DeckState deckState = (DeckState)this.currentState;
        if(deckState is DeckState)
        {
            //Debug.Log(deckState.deckNumber.ToString() + " is currentState on shutdown");
            SaveDeckNumberToPlayerPrefs(deckState.deckNumber);
        }
    }

    //[ContextMenu("GetCurrentDeck")]
    public void GetCurrentDeck() // Sets selectedDeck and selectedCards according to the current DeckState
    {
        DeckState deckState = (DeckState)this.currentState;
        if(deckState is DeckState)
        {
            selectedDeck = deckManager.LoadDeck(deckState.deckNumber);
            selectedCards = deckManager.ConvertIntListToCardList(selectedDeck.CardsInDeck);
        }
    }

    public void SaveCurrentDeck() // Saves the current selectedDeck and selectedCards to the current DeckState
    {
        DeckState deckState = (DeckState)this.currentState;
        if(deckState is DeckState)
        {
            //deckManager.SelectedDeck = selectedDeck;
            //deckManager.SaveDeck(deckState.deckNumber);
        }
        
    }

    private void SaveDeckNumberToPlayerPrefs(int deckNumber)
    {
        //Debug.Log("Saving " + deckNumber.ToString() + " as selected deck before shutdown");
        PlayerPrefs.SetInt(selectedPlayerDeck, deckNumber);
    }

    //[ContextMenu("Load from PlayPref")]
    private int LoadDeckNumberFromPlayerPrefs()
    {
        int result;

        if(PlayerPrefs.HasKey(selectedPlayerDeck))
        {
            result = PlayerPrefs.GetInt(selectedPlayerDeck);
        }
        else
        {
            result = defaultDeckState.deckNumber;
        }
        //Debug.Log("Loading " + result.ToString() + " as selected deck from before shutdown");
        return result;
    }

    /*
    [ContextMenu("Save currentState to PlayPref")]
    private void test()
    {
        DeckState deckState = (DeckState)currentState;
        SaveDeckNumberToPlayerPrefs(deckState.deckNumber);
    }
    */
}