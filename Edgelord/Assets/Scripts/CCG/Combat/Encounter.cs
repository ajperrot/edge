using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardType = CardInfo.CardType;

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
    public GameObject[] PlayerTurnUI; //toggle these for the player's turn

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
            } else
            {
                _allyDefense = value;
                DefenseDisplays[0].text = "" + value;
                DefenseDisplays[0].enabled = true;
            }
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
            } else
            {
                _enemyDefense = value;
                DefenseDisplays[1].text = "" + value;
                DefenseDisplays[1].enabled = true;
            }
        }
    }

    public List<Permanent> Enemies = new List<Permanent>(); // All opposing entities
    public List<Permanent> Allies = new List<Permanent>(); // All your entities
    public List<Permanent>[] FrontLines = new List<Permanent>[2]; //first allies/enemies to be targeted

    public Permanent NextAllySoulbind; //soulbind the next ally to this permanent if it exists

    private List<int>[] OnMemberPassives = new List<int>[2]; //passives to be used when a new ally/enemy is summoned
    private List<Permanent>[] OnMemberPassiveUsers = new List<Permanent>[2]; //Users of the above passives 


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
            Enemies[i].Initialize(new CardInfo(int.Parse(lines[i])), false);
            //move the enemy in some way
            NewEnemy.transform.localPosition -= new Vector3(charSpacing * i, Random.Range(-100, 100), 0);
        }
        // Set up front lines
        FrontLines[0] = new List<Permanent>();
        FrontLines[1] = new List<Permanent>();
        //set up passive lists
        OnMemberPassives[0] = new List<int>();
        OnMemberPassives[1] = new List<int>();
        OnMemberPassiveUsers[0] = new List<Permanent>();
        OnMemberPassiveUsers[1] = new List<Permanent>();
    }

    // Passes the turn from player to AI
    public void EndPlayerTurn()
    {
        //only end if nothing else takes priority
        if(Targeting.ActiveInstance != null) return;
        //deactivate player's ability to act
        yourTurn = false;
        foreach(GameObject O in PlayerTurnUI)
        {
            O.SetActive(false);
        }
        //un-upkept entities run away
        for(int i = Allies.Count - 1; i >= 0; i--)
        {
            if(Allies[i].CheckUpkeep() == false)
            {
                Kill(Allies[i]);
            }
        }
        //begin enemy turn
        BeginEnemyTurn();
    }

    // Plays out the AI turn
    public void BeginEnemyTurn()
    {
        //restore ap, blockers, etc
        foreach (Permanent Enemy in Enemies)
        {
            Enemy.ap = Enemy.maxAp;
            Enemy.MakeTargetable();
        }
        FrontLines[1] = new List<Permanent>();
        enemyDefense = 0;
        //let each enemy act and use its per-turn passive
        for(int i = Enemies.Count - 1; i >= 0; i--)
        {
            Passive.TriggerPassives(Enemies[i], 1);
            Enemies[i].Act();
        }
        //then resume player turn
        BeginPlayerTurn();
    }

    // Passes turn from AI to player
    public void BeginPlayerTurn()
    {
        yourTurn = true;
        //turn on player control ui
        foreach(GameObject O in PlayerTurnUI)
        {
            O.SetActive(false);
        }
        //restore ap, defense, and affinity
        foreach(Permanent Ally in Allies)
        {
            Ally.ap = Ally.maxAp;
            //demand upkeep if necessary
            Ally.RequestUpkeep();
        }
        FrontLines[0] = new List<Permanent>();
        allyDefense = 0;
        PlayerCharacter.Instance.PayableAffinity = new PlayerAffinity(PlayerCharacter.Instance.BaseAffinity);
        //trigger per-turn passives
        foreach(Permanent Ally in Allies)
        {
            Passive.TriggerPassives(Ally, 1);
        }
        //refill the player's hand
        PlayerCharacter.Instance.FillHand();
    }

    // Adds a new permanent to your side
    public void AddAlly(CardInfo AllyInfo)
    {
        //generate permanent for the new ally
        GameObject NewAlly = Instantiate(AllyPrefab, AlliesRoot);
        int allyIndex = Allies.Count;
        Allies.Add(NewAlly.GetComponent<Permanent>());
        Allies[allyIndex].Initialize(AllyInfo);
        //move the ally to its own spot
        NewAlly.transform.localPosition += new Vector3(charSpacing * allyIndex, Random.Range(-100, 100), 0);
        //soulbind if necessary
        if(NextAllySoulbind != null)
        {
            Allies[allyIndex].Soulbinds.Add(NextAllySoulbind);
            NextAllySoulbind.SoulboundEntities.Add(Allies[allyIndex]);
            NextAllySoulbind = null;
        }
        //call passives
        CallOnMemberPassives(Allies[allyIndex], 0);
        //add passives
        AddOnMemberPassives(AllyInfo, Allies[allyIndex], 0);
    }

    // Adds a new permanent to the other side
    public void AddEnemy(CardInfo EnemyInfo)
    {
        //generate permanent for the new ally
        GameObject NewEnemy = Instantiate(EnemyPrefab, EnemiesRoot);
        int enemyIndex = Enemies.Count;
        Enemies.Add(NewEnemy.GetComponent<Permanent>());
        Enemies[enemyIndex].Initialize(EnemyInfo, false);
        //move the ally to its own spot
        NewEnemy.transform.localPosition += new Vector3(charSpacing * enemyIndex, Random.Range(-100, 100), 0);
        //call passives
        CallOnMemberPassives(Enemies[enemyIndex], 1);
        //add passives
        AddOnMemberPassives(EnemyInfo, Enemies[enemyIndex], 1);
    }

    // Adds the onmember passives of the given user to the correct list
    void AddOnMemberPassives(CardInfo Info, Permanent User, int side)
    {
        foreach(int passiveId in Info.Passives)
        {
            if(Passive.TriggerPerPassive[passiveId] == 2)
            {
                OnMemberPassives[side].Add(passiveId);
                OnMemberPassiveUsers[side].Add(User);
            }
        }
    }

    // Calls the onmember passives of a given side on a specified target
    void CallOnMemberPassives(Permanent Target, int side)
    {
        Targeting.Target = Target;
        for(int i = 0; i < OnMemberPassives[side].Count; i++)
        {
            Passive.PassiveUsages[OnMemberPassives[side][i]](OnMemberPassiveUsers[side][i]);
        }
        Targeting.Target = null;
    }

    //Removes the onmember passives associated with the given user
    void RemoveOnMemberPassives(Permanent User, int side)
    {
        while(OnMemberPassiveUsers[side].Contains(User))
        {
            int index = OnMemberPassiveUsers[side].IndexOf(User);
            OnMemberPassiveUsers[side].RemoveAt(index);
            OnMemberPassives[side].RemoveAt(index);
        }
    }

    // Add a new permanent to the FrontLines
    public void JoinFrontLines(Permanent Defender, int side)
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

    // Remove a permanent from existance and also from the encounter
    public void Kill(Permanent Loser)
    {
        //kill soulbound entities first
        for(int i = Loser.SoulboundEntities.Count - 1; i >= 0; i--)
        {
            Kill(Loser.SoulboundEntities[i]);
        }
        //unbind this
        foreach (Permanent Bind in Loser.Soulbinds)
        {
            Bind.SoulboundEntities.Remove(Loser);
        }
        //create a corpse if human
        if(Loser.Info.Type == CardType.Human)
        {
            if(Loser.isAlly) AddAlly(new CardInfo(13));
        }
        //then kill this permanent
        if(Loser.isAlly == true)
        {
            RemoveOnMemberPassives(Loser, 0);
            Allies.Remove(Loser);
            FrontLines[0].Remove(Loser);
        } else
        {
            RemoveOnMemberPassives(Loser, 1);
            Enemies.Remove(Loser);
            FrontLines[1].Remove(Loser);
        }
        Destroy(Loser.gameObject);
    }
}
