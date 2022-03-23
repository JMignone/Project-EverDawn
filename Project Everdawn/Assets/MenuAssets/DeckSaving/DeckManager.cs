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
        [Min(1)] public int selectedDeckNumber;
        public PlayerDeck selectedDeck;

    [Header("Decks")]
        [SerializeField] private PlayerDeckList deckList = new PlayerDeckList();

    [Header("Other")]
        [SerializeField] private SO_CardDatabase cardDatabase;

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
  
    private void Awake()
    {
        DeleteOldDeck();
        LoadDeckList();
        CheckForDecks();
    } 

    #region Deck Checking and Generation Methods
    [ContextMenu("Check for Decks")]
    private void CheckForDecks() // Checks that there are only as many decks as we'd like to allow; resets or generates them depending on case
    {
        try
        {
            if(deckList.playerDecks.Count < maxNumberOfDecks) // Check if there's too few decks; create if there are none, reset if there are some
            {
                // Maybe just drop, this would reset everyone's decks if we decide to up the limit later
                if(deckList.playerDecks.Count == 0)
                {
                    Debug.Log("No decks found; Generating decks");
                    GenerateDecks(maxNumberOfDecks, deckList.playerDecks.Count, deckList.playerDecks);
                }
                else if(deckList.playerDecks.Count != 0)
                {
                    Debug.Log("Too few decks found; Resetting to default");
                    ResetDecks();
                }
            }
            else if(deckList.playerDecks.Count > maxNumberOfDecks) // Check if there's too many decks; reset to defaults if so
            {
                //Remove Decks; reset them to default; throw error
                Debug.Log("More decks than allowed found; Resetting to default");
                ResetDecks();
            }
            else if(deckList.playerDecks.Count == maxNumberOfDecks) // Currently just for debug purposes, if it's working as indended this shouldn't be necessary
            {
                // Do nothing, this should be the usual case
                Debug.Log("Number of Decks that exist is correct");
            }
        }
        catch
        {
            throw;
        }
    }

    private void GenerateDecks(int deckUpperLimit, int deckCount, List<PlayerDeck> listToGenerateTo) // Adds the default deck according to 
    {
        try
        {
            for(int i = 1; i <= deckUpperLimit - deckCount; i++)
            {
                listToGenerateTo.Add(defaultDeck.Copy());
                //Debug.Log(i.ToString() + " added");
            }
        }
        catch
        {
            throw;
        }
    }

    [ContextMenu("Reset Decks to Default")]
    private void ResetDecks() // Resets all decks the player has to default
    {
        deckList.playerDecks.Clear();
        GenerateDecks(maxNumberOfDecks, deckList.playerDecks.Count, deckList.playerDecks);
    }

    private void ResetDeck(PlayerDeck pd) // Attempts to reset deck to default
    {
        try
        {
            pd = defaultDeck.Copy();
        }
        catch
        {
            throw;
        }
    }

    private void CheckDeckSize(PlayerDeck pd) // WIP don't use
    {
        try
        {
            if(pd.cardsInDeck.Count != deckSize)
            {
                //Reset the deck; show error message; cancel any actions needed
                ResetDeck(pd);
            }
        }
        catch
        {
            throw;
        }
    }
    #endregion

    #region Deck Saving and Loading Methods
    public void SaveSelectedDeck(int deckNumber) // Saves a deck's data via the deck's number
    {
        try
        {
            int deckIndex = deckNumber - 1;
            if(deckIndex >= 0 && deckIndex <= deckList.playerDecks.Count)
            {
                deckList.playerDecks[deckIndex] = selectedDeck.Copy();
                SaveDeckList();
            }
            else
            {
                Debug.Log("Deck Number not found");
            }
        }
        catch
        {
            throw;
        }
    }

    public PlayerDeck LoadDeck(int deckNumber) // Loads a deck's data via the deck's number
    {
        try
        {
            int deckIndex = deckNumber - 1;
            PlayerDeck result = new PlayerDeck();
            LoadDeckList();
            if(deckIndex >= 0 && deckIndex <= deckList.playerDecks.Count)
            {
                result = deckList.playerDecks[deckIndex];
                selectedDeck = result.Copy();
                return result;
            }
            else
            {
                Debug.Log("Deck with key " + deckIndex.ToString() + " not found");
                return null;
            }
        }
        catch
        {
            throw;
        }
    }

    [ContextMenu("Save Deck List")]
    public void SaveDeckList() // Create serialized copy of the deckList
    {
        try
        {
            // Creates a binary formatter which will be used to save the data in a binary format and creates the binary file
            BinaryFormatter bf = new BinaryFormatter();

            PlayerDeckList pdl = new PlayerDeckList(); // Create a PlayerDeckList
            if(!File.Exists(Application.persistentDataPath + deckLocation)) // Check if file exists
            {
                GenerateDecks(maxNumberOfDecks, 0, pdl.playerDecks); // Populate deckList
                Debug.Log("No saved decks found, generating decks to: " + Application.persistentDataPath + deckLocation);
                Debug.Log("Serializing " + pdl.playerDecks.Count.ToString() + " decks");
            }
            // Create instace of PlayerDeckList and set its data to the currently set data
            pdl = deckList.Copy();

            Debug.Log("Saving file data to: " + Application.persistentDataPath + deckLocation);
            FileStream file = File.Create(Application.persistentDataPath + deckLocation);
            bf.Serialize(file, pdl); // Serialize the file
            file.Close(); // Close the file stream
        }
        catch(IOException)
        {
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

            Debug.Log("Data from " + Application.persistentDataPath + deckLocation + " loaded");
            deckList = pdl.Copy();
            return pdl;
        }
        catch
        {
            throw;
        }
    }
    #endregion

    #region Data Type Conversion Methods
    public List<SO_Card> ConvertIntListToCardList(List<int> cardIDs) // Converts an int list to a SO_Card list
    {
        try
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
        catch
        {
            throw;
        }
    }

    private void DeleteOldDeck()
    {
        if(File.Exists(Application.persistentDataPath + oldDeckLocation))
        {
            File.Delete(Application.persistentDataPath + oldDeckLocation);
            Debug.Log("Deleted file: " + Application.persistentDataPath + oldDeckLocation);
        }
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
        SaveSelectedDeck(selectedDeckNumber);
    }

    [ContextMenu("Load Selected Deck")]
    private void InspectorLoadDeck()
    {
        LoadDeck(selectedDeckNumber);
    }
    #endregion
}