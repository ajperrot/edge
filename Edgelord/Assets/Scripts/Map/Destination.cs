using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used to define destinations on the map
public class Destination : MonoBehaviour
{
    private GameObject Overlay; //overlay spawned on click

    // Start is called before the first frame update
    void Start()
    {
        Overlay = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawn the overlay object on click
    public void SpawnOverlay()
    {
        Overlay.SetActive(true);
    }
}
