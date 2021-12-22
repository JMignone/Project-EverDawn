using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataStorage : MonoBehaviour
{
    public List<int> Selected_Cards;

    #region Singleton Pattern
    public static DataStorage dataStor;

    // Awake is called before start
    private void Awake()
    {
        // Set object to act like a singleton
        if (dataStor == null)
        {
            // Set object to persist when the scene it is created in is unloaded
            DontDestroyOnLoad(gameObject);
            dataStor = this;
        }

        // Only allow 1 copy of this object to be exist
        else if (dataStor != this)
        {
            Destroy(gameObject);
        }
        // </singleton code>

    }
    #endregion

    public void Save()
    {
        // Creates a binary formatter which will be used to save the data in a binary format and creates the binary file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        Debug.Log("Saving file data to: " + Application.persistentDataPath);

        // Create a new instance of the PlayerData class and set its data to be that of what's currently set
        PlayerData data = new PlayerData();
        data.Selected_Cards = Selected_Cards;

        // Write the data that was just set to the binary file
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        // Check to make sure the file exists
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            // Creates a binary formatter which will be used to load the saved data and open the binary file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            // Cast opened file as the same type as the serializable class we defined, and set our new instance to use the data from the class
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            Selected_Cards = data.Selected_Cards;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
class PlayerData
{
    public List <int> Selected_Cards;
}