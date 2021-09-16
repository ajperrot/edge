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

    private int[] savedDays;

    // Start
    void Start()
    {
        List<string> FileNames = new List<string>(Directory.GetFiles(Application.persistentDataPath));
        if(FileNames.Count < 2) return;
        savedDays = new int[FileNames.Count - 1];
        FileNames.Sort();
        for(int i = 1; i < FileNames.Count; i++)
        {
            string name = FileNames[i];
            savedDays[i - 1] = int.Parse(name[name.Length - 5].ToString());
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
        int day = savedDays[savedDays.Length - 1];
        Setting.currentDay = day;
        SceneChanger.fromSave = day;
        SceneManager.LoadScene(1);
    }

    // Present a list of all numbered saves to choose from
    public void LoadGame()
    {
        LoadMenu.SetActive(true);
        for(int i = 0; i < savedDays.Length; i++)
        {
            GameObject NewButton = Instantiate(LoadButtonPrefab, FilesRoot);
            NewButton.GetComponent<LoadButton>().day = savedDays[i];
            NewButton.transform.localPosition -= new Vector3(0, 125 * i, 0);
        }
    }
}
