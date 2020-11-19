using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[Serializable]
// The data we need to save
class SaveData
{
    public int day; //day of the Setting we saved just before
    public int location; //where we at? (Only needed for moving to combat)
    public PlayerAffinity BaseAffinity; //affinity the player character has
    public Deck PlayerDeck; //your deck as of this day
    public int followerCount; //your follower count at end of day
    public int popularity; //your popularity at end of day
    public int money; //your money at end of day
    public int maxMovement; //your movement per day
    public List<Item> Inventory; //player's items at end of day
    public int baseHp; //players maxHp at end of day
    public int baseSanity; //players maxSanity at end of day
    public int baseAp; //players maxAp at end of day
}

// Processes for saving and loaing data
public class SaveSystem
{
    // Save data of this day to that slot, then delete last save if it was not a week start
    public static void SaveGame()
    {
        string path = Application.persistentDataPath + "/SaveData.dat";
        //rename previous save if a week start
        if(File.Exists(path))
        {
            int yesterday = Setting.currentDay - 1;
            if(yesterday % 7 == 0)
            {
                string newPath = Application.persistentDataPath + "/SaveData" + yesterday + ".dat";
                if(File.Exists(newPath)) File.Delete(newPath);
                File.Move(path, newPath);
            } else
            {
                ClearSave(-1);
            }
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path); 
        SaveData Data = new SaveData();
        Data.day = Setting.currentDay;
        Data.location = Setting.location;
        Data.BaseAffinity = PlayerCharacter.Instance.BaseAffinity;
        Data.PlayerDeck = PlayerCharacter.Instance.PlayerDeck;
        Data.followerCount = PlayerCharacter.Instance.followerCount;
        Data.popularity = PlayerCharacter.Instance.popularity;
        Data.money = PlayerCharacter.Instance.money;
        Data.Inventory = PlayerCharacter.Instance.Inventory;
        Data.maxMovement = PlayerCharacter.Instance.maxMovement;
        Data.baseHp = PlayerCharacter.Instance.baseHp;
        Data.baseSanity = PlayerCharacter.Instance.baseSanity;
        Data.baseAp = PlayerCharacter.Instance.baseAp;
        bf.Serialize(file, Data);
        file.Close();
        //Debug.Log("Game data saved!");//test
    }

    // Load previous save
    public static void Continue()
    {
        LoadGame(-1);
    }

    // Load saved data of old day
    public static void LoadGame(int oldDay)
    {
        //path has no number for most recent save
        string path;
        if(oldDay < 0)
        {
            path = Application.persistentDataPath + "/SaveData.dat";
        } else
        {
            path = Application.persistentDataPath + "/SaveData" + oldDay + ".dat";
        }
        //search for file and load if possible
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            SaveData Data = (SaveData)bf.Deserialize(file);
            file.Close();
            Setting.currentDay = Data.day;
            Setting.location = Data.location;
            PlayerCharacter.Instance.BaseAffinity = Data.BaseAffinity;
            PlayerCharacter.Instance.PlayerDeck = Data.PlayerDeck;
            PlayerCharacter.Instance.followerCount = Data.followerCount;
            PlayerCharacter.Instance.popularity = Data.popularity;
            PlayerCharacter.Instance.money = Data.money;
            PlayerCharacter.Instance.Inventory = Data.Inventory;
            PlayerCharacter.Instance.maxMovement = Data.maxMovement;
            PlayerCharacter.Instance.baseHp = Data.baseHp;
            PlayerCharacter.Instance.baseSanity = Data.baseSanity;
            PlayerCharacter.Instance.baseAp = Data.baseAp;
            //Debug.Log("Game data loaded!");//test
        }
        else
            Debug.LogError("There is no save data!");//test
    }

    // Delete a save from a certain day
    public static void ClearSave(int oldDay)
    {
        //path has no number for most recent save
        string path;
        if(oldDay < 0)
        {
            path = Application.persistentDataPath + "/SaveData.dat";
        } else
        {
            path = Application.persistentDataPath + "/SaveData" + oldDay + ".dat";
        }
        //then clear save if it exists
        if (File.Exists(path))
        {
            File.Delete(path);
            //Debug.Log("Data wipe complete!");//test
        }
        else
            Debug.LogError("No save data to delete.");//test
    }
}