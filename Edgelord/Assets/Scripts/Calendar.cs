using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar
{
    public static int currentDay = 0;

    // Advance the calendar and change scene to comp
    public static void AdvanceDay()
    {
        currentDay++;
        SceneChanger.Instance.ChangeScene(1);
    }
}
