using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    // Table showing which encounter occurs per location/day
    public static Dictionary<(int, int), int>[] encounterPerLocationPerDay
    {
        //dictionary 0
        {
            
        }
    };

    private List<Permanent> Enemies = new List<Permanent>(); // All opposing entities
    private List<Permanent> Allies = new List<Permanent>(); // All your entities

    private bool yourTurn = true; //can the player take actions?

    // Start is called before the first frame update
    void Start()
    {
        // Add the player to the list of allies
        Allies.Add(GameObject.FindWithTag("PlayerPermanent").GetComponent<PlayerCharacter>());
        // Generate the encounter


    }
}
