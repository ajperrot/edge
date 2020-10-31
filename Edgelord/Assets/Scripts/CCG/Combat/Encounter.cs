using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public List<Permanent> Enemies = new List<Permanent>(); // All opposing entities
    public List<Permanent> Allies = new List<Permanent>(); // All your entities

    private bool yourTurn = true; //can the player take actions?

    // Start is called before the first frame update
    void Start()
    {
        // GENERATE THE ENCOUNTER

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
