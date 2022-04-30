using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DeckManager : ScriptableObject
{
    #region Variables
    [Header("Deck Limitations and Settings")]
        [SerializeField] [Min(1)] private int maxNumberOfDecks;
        [SerializeField] private PlayerDeck defaultDeck;
        [SerializeField] private int deckSize;

    [Header("Selected Deck")]
        [SerializeField] private int selectedDeckNumber;
        [SerializeField] private PlayerDeck selectedDeck;

    [Header("Decks")]
        [SerializeField] private PlayerDeckList deckList = new PlayerDeckList();

    [Header("Other")]
        [SerializeField] private SO_CardList cardDatabase;

    public int SelectedDeckNumber
    {
        get{return selectedDeckNumber;}
    }

    public PlayerDeck SelectedDeck
    {
        get{return selectedDeck;}
    }

    private readonly string selectedPlayerDeck = "selectedPlayerDeck";
    private readonly string deckLocation = "/playerDecks.dat";
    private readonly string oldDeckLocation = "/playerDeck.dat";
    #endregion

    #region PlayerDeckList Internal Class (Used in Serialization)
        [System.Serializable]
        private class PlayerDeckList
        {
            public List<PlayerDeck> playerDecks = new List<PlayerDeck>();

            public PlayerDeckList Copy()
            {
                var result = new PlayerDeckList();
                result.playerDecks = new List<PlayerDeck>(this.playerDecks);
                return result;
            }
        }
    #endregion

    #region Lifecycle Methods
    [ContextMenu("Run Awake")]
    private void Awake()
    {
        DeleteOldDeck();
        PlayerDeckList pdl = LoadDeckList();
        if(pdl != null)
        {
            bool validConfiguration = ValidateDecks(deckList.playerDecks);
            if(validConfiguration == false)
            {
                ResetDecks();
            }
        }
        else
        {
            GenerateDecks(maxNumberOfDecks, deckList.playerDecks.Count, deckList.playerDecks);
        }
        LoadAsSelectedDeck(LoadDeckNumberFromPlayerPrefs());
    }

    private void OnDisable()
    {
        SaveDeckNumberToPlayerPrefs(selectedDeckNumber);
    }
    #endregion

    #region Deck Checking and Generation Methods
    private bool ValidateDecks(List<PlayerDeck> decks)
    {
        try
        {
            if(decks.Count == maxNumberOfDecks) // Check there's the expected number of decks
            {
                //Debug.Log("Found " + decks.Count.ToString() + " decks.");
                foreach(PlayerDeck deck in decks)
                {
                    if(deck.CardsInDeck != null && deck.CardsInDeck.Count == deckSize) // Check each deck is not null and has the appropriate number of cards
                    {
                        //Debug.Log("Found " + deck.CardsInDeck.Count.ToString() + " cards");
                        foreach(int cardID in deck.CardsInDeck)
                        {
                            if(cardID >= 0) // Check each card in each deck has a valid ID
                            {
                                //Debug.Log("Found card with ID: " + cardID.ToString());
                                continue;
                            }
                            else
                            {
                                Debug.Log("Found an invalid card ID");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Found deck of incorrect size");
                        return false;
                    }
                }
            }
            else
            {
                Debug.Log("Found an invalid number of decks");
                return false;
            }
            return true; // Returns true if all checks pass
        }
        catch
        {
            Debug.Log("Encountered some kind of error, decks assumed to not be valid");
            return false;
        }
    }

    private void GenerateDecks(int deckUpperLimit, int deckCount, List<PlayerDeck> targetList) // Adds the default deck according to 
    {
        for(int i = 1; i <= deckUpperLimit - deckCount; i++)
        {
            targetList.Add(defaultDeck.Copy());
            //Debug.Log(i.ToString() + " added");
        }
    }

    [ContextMenu("Reset Decks to Default")]
    private void ResetDecks() // Resets all decks the player has to default
    {
        deckList.playerDecks.Clear();
        GenerateDecks(maxNumberOfDecks, deckList.playerDecks.Count, deckList.playerDecks);
        SaveDeckList();
    }
    #endregion

    #region Deck Saving and Loading Methods
    public void SaveDeck(int deckNumber, PlayerDeck playerDeck) // Saves a deck's data to the given deckNumber
    {
        int deckIndex = deckNumber - 1;
        if(deckIndex >= 0 && deckIndex <= deckList.playerDecks.Count)
        {
            deckList.playerDecks[deckIndex] = playerDeck.Copy();
            SaveDeckList();
        }
        else
        {
            //Debug.Log("Deck Number not found");
        }
    }

    //This should be made private once we don't need it for loading the AI deck anymore
    public PlayerDeck LoadDeck(int deckNumber) // Loads a deck's data via the deck's number
    {
        int deckIndex = deckNumber - 1;
        PlayerDeck result = new PlayerDeck();
        LoadDeckList();
        if(deckIndex >= 0 && deckIndex <= deckList.playerDecks.Count)
        {
            result = deckList.playerDecks[deckIndex];
            //selectedDeck = result.Copy();
            return result;
        }
        else
        {
            //Debug.Log("Deck with key " + deckIndex.ToString() + " not found");
            return null;
        }
    }

    [ContextMenu("Save Deck List")]
    private void SaveDeckList() // Create serialized copy of the deckList
    {
        try
        {
            // Creates a binary formatter which will be used to save the data in a binary format and creates the binary file
            BinaryFormatter bf = new BinaryFormatter();

            PlayerDeckList pdl = new PlayerDeckList(); // Create a PlayerDeckList
            if(!File.Exists(Application.persistentDataPath + deckLocation)) // Check if file exists
            {
                GenerateDecks(maxNumberOfDecks, 0, pdl.playerDecks); // Populate deckList
                //Debug.Log("No saved decks found, generating decks to: " + Application.persistentDataPath + deckLocation);
                //Debug.Log("Serializing " + pdl.playerDecks.Count.ToString() + " decks");
            }
            // Create instace of PlayerDeckList and set its data to the currently set data
            pdl = deckList.Copy();

            //Debug.Log("Saving file data to: " + Application.persistentDataPath + deckLocation);
            FileStream file = File.Create(Application.persistentDataPath + deckLocation);
            bf.Serialize(file, pdl); // Serialize the file
            file.Close(); // Close the file stream
        }
        catch(IOException e) when (e.Data != null)
        {
            if (e.Source != null)
            {
                Debug.Log("IOException source: {0}" + e.Data.ToString());
            }
            throw;
        }
    }

    [ContextMenu("Load Deck List")]
    private PlayerDeckList LoadDeckList()
    {
        try
        {
            if(!File.Exists(Application.persistentDataPath + deckLocation))
            {
                return null;
            }
            // Creates a binary formatter which will be used to load the saved data and open the binary file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + deckLocation, FileMode.Open);

            // Cast opened file as the same type as the serializable class we defined, and set our new instance to use the data from the class
            PlayerDeckList pdl = (PlayerDeckList)bf.Deserialize(file);
            file.Close();

            //Debug.Log("Data from " + Application.persistentDataPath + deckLocation + " loaded");
            deckList = pdl.Copy();
            return pdl;
        }
        catch(IOException e) when (e.Data != null)
        {
            if (e.Source != null)
            {
                Debug.Log("IOException source: {0}" + e.Data.ToString());
            }
            throw;
        }
    }

    private void LoadAsSelectedDeck(int deckNumber)
    {
        selectedDeck = LoadDeck(deckNumber);
        selectedDeckNumber = deckNumber;
    }

    private void SaveSelectedDeckAs(int deckNumber)
    {
        SaveDeck(deckNumber, selectedDeck);
    }

    public void ChangeSelectedDeck(int deckNumber)
    {
        SaveSelectedDeckAs(selectedDeckNumber);
        LoadAsSelectedDeck(deckNumber);
    }

    public void ChangeSelectedDeckName(string deckName)
    {
        selectedDeck.DeckName = deckName;
        SaveSelectedDeckAs(selectedDeckNumber);
    }
    #endregion

    #region Data Type Conversion Methods
    public List<SO_Card> ConvertIntListToCardList(List<int> cardIDs) // Converts an int list to a SO_Card list
    {
        List<SO_Card> result = new List<SO_Card>();
        if(cardIDs != null)
        {
            for(int i = 0; i <= cardIDs.Count - 1; i++)
            {
                result.Add(cardDatabase.cardList[cardIDs[i]]);
            }
            return result;
        }
        else
        {
            for(int i = 0; i <= deckSize - 1; i++)
            {
                result.Add(cardDatabase.cardList[i]);
            }
            return result;
        }
    }

    private void DeleteOldDeck()
    {
        if(File.Exists(Application.persistentDataPath + oldDeckLocation))
        {
            File.Delete(Application.persistentDataPath + oldDeckLocation);
            //Debug.Log("Deleted file: " + Application.persistentDataPath + oldDeckLocation);
        }
    }
    #endregion

    #region PlayerPrefs Saving and Loading Methods
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
            result = 1; // Return 1 by default
        }
        //Debug.Log("Loading " + result.ToString() + " as selected deck from before shutdown");
        return result;
    }
    #endregion

    #region Debug Methods
    [ContextMenu("Clear Decks")]
    private void ClearDecks()
    {
        deckList.playerDecks.Clear();
    } 

    [ContextMenu("Save Selected Deck")]
    private void InspectorSaveDeck()
    {
        SaveSelectedDeckAs(selectedDeckNumber);
    }

    [ContextMenu("Load Selected Deck")]
    private void InspectorLoadDeck()
    {
        LoadAsSelectedDeck(selectedDeckNumber);
    }

    [ContextMenu("Validate Decks")]
    private void InspectorValidateDecks()
    {
        bool result = ValidateDecks(deckList.playerDecks);
        Debug.Log(result.ToString());
    }
    #endregion
}