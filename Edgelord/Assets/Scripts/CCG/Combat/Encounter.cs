using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    public GameObject EnemyPrefab; //prefab copied to create enemy permanents
    public Transform EnemiesRoot; //all enemy permanents are children of this

    private List<Permanent> Enemies = new List<Permanent>(); // All opposing entities
    private List<Permanent> Allies = new List<Permanent>(); // All your entities

    private bool yourTurn = true; //can the player take actions?

    // Start is called before the first frame update
    void Start()
    {
        // Add the player to the list of allies
        Allies.Add(GameObject.FindWithTag("PlayerPermanent").GetComponent<PlayerCharacter>());
        // Generate the encounter
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + "/Encounters/" + Setting.location + "/" + Setting.currentDay + ".txt");
        for(int i = 0; i < lines.Length; i++)
        {
            //generate permanent for each enemy
            GameObject NewEnemy = Instantiate(EnemyPrefab, EnemiesRoot);
            Enemies.Add(NewEnemy.GetComponent<Permanent>());
            Enemies[i].Initialize(new CardInfo(int.Parse(lines[i])), true);
            //move the enemy in some way
            NewEnemy.transform.localPosition += new Vector3(300 * i, Random.Range(-100, 100), 0);
        }
    }
}
