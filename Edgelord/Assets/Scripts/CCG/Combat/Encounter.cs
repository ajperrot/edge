using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class Encounter : MonoBehaviour
{
    public static Encounter Instance; //makes this a singleton

    public List<Sprite> Backgrounds; //list of backgrounds for each location
    public GameObject EnemyPrefab; //prefab copied to create enemy permanents
    public GameObject AllyPrefab; //prefab copied to create allied permanents
    public Transform EnemiesRoot; //all enemy permanents are children of this
    public Transform AlliesRoot; //all allied permanents are children of this
    public float charSpacing = 250; //space between characters
    public bool yourTurn = true; //can the player take actions?
    public TMP_Text[] DefenseDisplays; //displays ally and enemy defense

    //takes damage for the team
    private int _allyDefense = 0;
    public int allyDefense
    {
        get {return _allyDefense;}
        set
        {
            if(value <= 0)
            {
                _allyDefense = 0;
                DefenseDisplays[0].enabled = false;
            }
            _allyDefense = value;
            DefenseDisplays[0].text = "" + value;
            DefenseDisplays[0].enabled = true;
        }
    }
    private int _enemyDefense = 0;
    public int enemyDefense
    {
        get {return _enemyDefense;}
        set
        {
            if(value <= 0)
            {
                _enemyDefense = 0;
                DefenseDisplays[1].enabled = false;
            }
            _enemyDefense = value;
            DefenseDisplays[1].text = "" + value;
            DefenseDisplays[1].enabled = true;
        }
    }

    private List<Permanent> Enemies = new List<Permanent>(); // All opposing entities
    private List<Permanent> Allies = new List<Permanent>(); // All your entities
    private List<Permanent>[] FrontLines = new List<Permanent>[2]; //first allies/enemies to be targeted


    // Start is called before the first frame update
    void Start()
    {
        //singleton
        Instance = this;
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
            NewEnemy.transform.localPosition -= new Vector3(charSpacing * i, Random.Range(-100, 100), 0);
        }
        // Set up front lines
        FrontLines[0] = new List<Permanent>();
        FrontLines[1] = new List<Permanent>();
    }

    // Adds a new permanent to your side
    public void AddAlly(CardInfo AllyInfo)
    {
        //generate permanent for the new ally
        GameObject NewAlly = Instantiate(AllyPrefab, AlliesRoot);
        int allyIndex = Allies.Count;
        Allies.Add(NewAlly.GetComponent<Permanent>());
        Allies[allyIndex].Initialize(AllyInfo);
        Allies[allyIndex].isAlly = true;
        //move the ally to its own spot
        NewAlly.transform.localPosition += new Vector3(charSpacing * allyIndex, Random.Range(-100, 100), 0);
    }

    // Add a new permanent to the FrontLines
    public static void JoinFrontLines(Permanent Defender, int side)
    {
        //join the front line
        FrontLines[side].Add(Defender);
        if(side == 1)
        {
            //if enemy, prevent player from targeting others
            if(FrontLines[1].Count == 1)
            {
                for(int i = 0; i < Enemies.Count; i++)
                {
                    Enemies[i].MakeUntargetable();
                }
            }
            Defender.MakeTargetable();
        }
    }
}
