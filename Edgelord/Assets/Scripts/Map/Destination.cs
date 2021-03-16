using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used to define destinations on the map
public class Destination : MonoBehaviour
{
    private static List<GameObject> All = new List<GameObject>(); //all destinations

    private GameObject Overlay; //overlay spawned on click

    // Start is called before the first frame update
    void Start()
    {
        All.Add(gameObject);
        Overlay = transform.GetChild(0).gameObject;
    }

    // Deactivate all shops
    public static void ToggleMapIcons(bool state)
    {
        foreach(GameObject Object in All)
        {
            Object.SetActive(state);
        }
    }

    // Spawn the overlay object on click and deactivate shops
    public void SpawnOverlay()
    {
        ToggleMapIcons(false);
        gameObject.SetActive(true);
        Overlay.SetActive(true);
    }
}
