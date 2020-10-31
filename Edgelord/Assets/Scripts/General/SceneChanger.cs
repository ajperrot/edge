using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;
    public static bool fromAutoSave = false; //are we to load the save on start


    // Load Autosave if necesssary on start
    void Start()
    {
        Instance = this;
        if(fromAutoSave == true)
        {
            SaveSystem.LoadGame(-1);
        }
    }

    // Changes to scene indicated by number (index in build settings scene list)
    public void ChangeScene(int sceneNumber)
    {
        PlayerCharacter.Instance.RemoveAllZeroCountItems();
        SaveSystem.SaveGame();
        fromAutoSave = true;
        SceneManager.LoadScene(sceneNumber);
    }
}
