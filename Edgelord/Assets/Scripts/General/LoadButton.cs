using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour
{
    public TMP_Text Label;

    private int _day;
    public int day
    {
        get{return _day;}
        set
        {
            _day = value;
            Label.text += "" + value;
            print(value);//test
        }
    }

    public void LoadGivenDay()
    {
        Setting.currentDay = day;
        SceneChanger.fromSave = day;
        SceneManager.LoadScene(1);
    }
}
