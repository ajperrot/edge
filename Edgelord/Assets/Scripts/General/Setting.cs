using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting
{
    public static int currentDay = 1;
    public static int location = 0;

    // Advance the Setting and change scene to comp
    public static void AdvanceDay()
    {
        currentDay++;
        SceneChanger.Instance.ChangeScene(1);
    }
}
