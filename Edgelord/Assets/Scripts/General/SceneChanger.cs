using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    public static int fromSave = -2; //what save to load on start? (-2 for none)


    // Load Autosave if necesssary on start
    void Start()
    {
        Instance = this;
        if(fromSave > -2)
        {
            SaveSystem.LoadGame(fromSave);
        }
        fromSave = -2;
    }

    // Changes to scene indicated by number (index in build settings scene list)
    public void ChangeScene(int sceneNumber)
    {
        PlayerCharacter.Instance.RemoveAllZeroCountItems();
        SaveSystem.SaveGame();
        fromSave = -1;
        SceneManager.LoadScene(sceneNumber);
    }
}
