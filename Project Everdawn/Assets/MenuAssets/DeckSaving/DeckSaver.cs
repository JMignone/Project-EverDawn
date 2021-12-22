using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DeckSaver : ScriptableObject
{
    public List<int> selectedCards;
    private readonly string deckLocation = "/playerDeck.dat";

    [ContextMenu("Save")]
    public void Save()
    {
        // Creates a binary formatter which will be used to save the data in a binary format and creates the binary file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + deckLocation);
        Debug.Log("Saving file data to: " + Application.persistentDataPath + deckLocation);

        // Create a new instance of the PlayerDeta class and set its data to be that of what's currently set
        PlayerDeck deck = new PlayerDeck();
        deck.cardsInDeck = selectedCards;

        // Write the data that was just set to the binary file
        bf.Serialize(file, deck);
        file.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        // Check to make sure the file exists
        if(File.Exists(Application.persistentDataPath + deckLocation))
        {
            // Creates a binary formatter which will be used to load the saved data and open the binary file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + deckLocation, FileMode.Open);

            // Cast opened file as the same type as the serializable class we defined, and set our new instance to use the data from the class
            PlayerDeck deck = (PlayerDeck)bf.Deserialize(file);
            file.Close();

            Debug.Log("Data from " + Application.persistentDataPath + deckLocation + " loaded");
            selectedCards = deck.cardsInDeck;
        }
    }
}
