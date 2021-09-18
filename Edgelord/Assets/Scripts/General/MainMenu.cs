using System.Collections;
using System.Collections.Generic;
using System.IO;  
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject LoadButtonPrefab;
    public GameObject LoadMenu;
    public Transform FilesRoot;

    private List<int> savedDays;

    // Start
    void Start()
    {
        List<string> FileNames = new List<string>(Directory.GetFiles(Application.persistentDataPath));
        savedDays = new List<int>();
        FileNames.Sort();
        for(int i = 0; i < FileNames.Count; i++)
        {
            string name = FileNames[i];
            if(name[name.Length -1] != 't') continue; //no DS_Store, only .dat
            string back = name.Substring(name.Length - 6, 2);
            Debug.Log(back);//test
            if(back == "-1") continue;
            savedDays.Add(int.Parse(back));
        }
    }

    // Start at day 0 without loading a save
    public void NewGame()
    {
        Setting.currentDay = 0;
        SceneManager.LoadScene(0);
    }

    // Load the last save
    public void Continue()
    {
        int day = savedDays[savedDays.Count - 1];
        Setting.currentDay = day;
        SceneChanger.fromSave = day;
        SceneManager.LoadScene(1);
    }

    // Present a list of all numbered saves to choose from
    public void LoadGame()
    {
        LoadMenu.SetActive(true);
        for(int i = 0; i < savedDays.Count; i++)
        {
            GameObject NewButton = Instantiate(LoadButtonPrefab, FilesRoot);
            NewButton.GetComponent<LoadButton>().day = savedDays[i];
            NewButton.transform.localPosition -= new Vector3(0, 125 * i, 0);
        }
    }
}
