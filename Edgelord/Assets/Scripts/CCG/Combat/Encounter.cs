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
    public static Affinity UpkeepBonus; //upkeep paayable by units

    public List<Sprite> Backgrounds; //list of backgrounds for each location
    public GameObject[] AllyPrefabs; //prefab copied to create allied permanents
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

    public List<Permanent>[] Parties = new List<Permanent>[2]; // All opposing entities
    public List<Permanent>[] FrontLines = new List<Permanent>[2]; //first allies/enemies to be targeted
    public List<Permanent>[] Wards = new List<Permanent>[2]; //permanents which take damage for their side 
    public List<int>[] PendingAttackBuffs = new List<int>[2]; //ammounts that permanents in the following list should get buffed after their next turn
    public List<Permanent>[] PendingAttackBuffRecipients = new List<Permanent>[2]; //Permanents to receive the above buffs at the end of their next turn

    public Permanent NextAllySoulbind; //soulbind the next ally to this permanent if it exists

    private List<int>[] OnMemberPassives = new List<int>[2]; //passives to be used when a new ally/enemy is summoned
    private List<Permanent>[] OnMemberPassiveUsers = new List<Permanent>[2]; //Users of the above passives 
    private Transform[] PartyRoots = new Transform[2]; //first position in party line

    // Start is called before the first frame update
    void Start()
    {
        //singleton
        Instance = this;
        // Set up parties
        Parties[0] = new List<Permanent>();
        Parties[1] = new List<Permanent>();
        // Add the player to the list of allies
        Parties[0].Add(GameObject.FindWithTag("PlayerPermanent").GetComponent<PlayerCharacter>());
        // Store roots
        PartyRoots[0] = AlliesRoot;
        PartyRoots[1] = EnemiesRoot;
        // Generate the encounter
        string[] lines = File.ReadAllLines(Application.streamingAssetsPath + "/Encounters/" + Setting.location + "/" + Setting.currentDay + ".txt");
        for(int i = 0; i < lines.Length; i++)
        {
            //generate permanent for each enemy
            GameObject NewEnemy = Instantiate(AllyPrefabs[1], EnemiesRoot);
            Parties[1].Add(NewEnemy.GetComponent<Permanent>());
            Parties[1][i].Initialize(new CardInfo(int.Parse(lines[i])), 1);
            //move the enemy in some way
            NewEnemy.transform.localPosition -= new Vector3(charSpacing * i, Random.Range(-100, 100), 0);
        }
        for(int i = 0; i < 2; i++)
        {
            // Set up front lines
            FrontLines[i] = new List<Permanent>();
            //set up passive lists
            OnMemberPassives[i] = new List<int>();
            OnMemberPassiveUsers[i] = new List<Permanent>();
            //set up ward lists
            Wards[i] = new List<Permanent>();
            //set up buffies
            PendingAttackBuffs[i] = new List<int>();
            PendingAttackBuffRecipients[i] = new List<Permanent>();
        }
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
        //un-upkept entities run away, and grapplers are reset
        for(int i = Parties[0].Count - 1; i >= 0; i--)
        {
            Parties[0][i].Grappler = null;
            if(Parties[0][i].CheckUpkeep() == false)
            {
                Kill(Parties[0][i]);
            }
        }
        for(int i = 0; i < PendingAttackBuffRecipients[0].Count; i++)
        {
            PendingAttackBuffRecipients[0][i].attackModifier += PendingAttackBuffs[0][i];
        }
        PendingAttackBuffs[0] = new List<int>();
        PendingAttackBuffRecipients[0] = new List<Permanent>();
        //begin enemy turn
        BeginEnemyTurn();
    }

    // Plays out the AI turn
    public void BeginEnemyTurn()
    {
        //restore ap, blockers, etc
        foreach (Permanent Enemy in Parties[1])
        {
            Enemy.ap = Enemy.maxAp;
            Enemy.MakeTargetable();
        }
        FrontLines[1] = new List<Permanent>();
        enemyDefense = 0;
        //let each enemy act and use its per-turn passive
        for(int i = Parties[1].Count - 1; i >= 0; i--)
        {
            Passive.TriggerPassives(Parties[1][i], 1);
            Parties[1][i].Act();
        }
        //then resume player turn
        BeginPlayerTurn();
    }

    // Passes turn from AI to player
    public void BeginPlayerTurn()
    {
        //resolve pending attack buffs for enemies
        for(int i = 0; i < PendingAttackBuffRecipients[1].Count; i++)
        {
            PendingAttackBuffRecipients[1][i].attackModifier += PendingAttackBuffs[1][i];
        }
        PendingAttackBuffs[1] = new List<int>();
        PendingAttackBuffRecipients[1] = new List<Permanent>();
        //this
        yourTurn = true;
        UpkeepBonus = new Affinity();
        //turn on player control ui
        foreach(GameObject O in PlayerTurnUI)
        {
            O.SetActive(false);
        }
        //restore ap, defense, and affinity
        foreach(Permanent Ally in Parties[0])
        {
            Ally.ap = Ally.maxAp;
            //demand upkeep if necessary
            Ally.RequestUpkeep();
        }
        //reset enemy Grapplers
        foreach (Permanent Enemy in Parties[1])
        {
            Enemy.Grappler = null;
        }
        FrontLines[0] = new List<Permanent>();
        allyDefense = 0;
        PlayerCharacter.Instance.PayableAffinity = new PlayerAffinity(PlayerCharacter.Instance.BaseAffinity);
        //trigger per-turn passives
        foreach(Permanent Ally in Parties[0])
        {
            Passive.TriggerPassives(Ally, 1);
        }
        //refill the player's hand
        PlayerCharacter.Instance.FillHand();
    }

    // Adds a new permanent to your side
    public void AddAlly(CardInfo AllyInfo, int side = 0)
    {
        //generate permanent for the new ally
        GameObject NewAlly = Instantiate(AllyPrefabs[side], PartyRoots[side]);
        int allyIndex = Parties[side].Count;
        Parties[side].Add(NewAlly.GetComponent<Permanent>());
        Parties[side][allyIndex].Initialize(AllyInfo);
        //move the ally to its own spot
        NewAlly.transform.localPosition += new Vector3(charSpacing * allyIndex, Random.Range(-100, 100), 0);
        //soulbind if necessary
        if(side == 0 && NextAllySoulbind != null)
        {
            NextAllySoulbind.AddSoulboundEntity(Parties[side][allyIndex]);
            NextAllySoulbind = null;
        }
        //call passives
        CallOnMemberPassives(Parties[side][allyIndex], 0);
        //add passives
        AddOnMemberPassives(AllyInfo, Parties[side][allyIndex], 0);
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
                for(int i = 0; i < Parties[1].Count; i++)
                {
                    Parties[1][i].MakeUntargetable();
                }
            }
            Defender.MakeTargetable();
        }
    }

    // Remove a permanent from existance and also from the encounter
    public void Kill(Permanent Loser)
    {
        int side = Loser.side;
        //call your "on death" passives
        Passive.TriggerPassives(Loser, 3);
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
        //create a corpse if a human ally
        if(Loser.Info.Type == CardType.Human && side == 0)
        {
            AddAlly(new CardInfo(13));
        }
        //then kill this permanent
        RemoveOnMemberPassives(Loser, side);
        Parties[side].Remove(Loser);
        FrontLines[side].Remove(Loser);
        Destroy(Loser.gameObject);
        //Finally, move back the others on the party
        for(int i = 0; i < Parties[side].Count; i++)
        {
            Parties[side][i].transform.localPosition = new Vector3(charSpacing * i, Random.Range(-100, 100), 0);
        }
    }

    public static Affinity UseUpkeepBonus(Affinity Upkeep)
    {
        Affinity RemainingUpkeep = Upkeep;
        //process upkeep bonus paid from studied passive
        if(UpkeepBonus > 0)
        {
            RemainingUpkeep -= UpkeepBonus;
            if(RemainingUpkeep.radiant < 0)
            {
                UpkeepBonus.radiant = 0 - RemainingUpkeep.radiant;
                RemainingUpkeep.radiant = 0;
            } else
            {
                UpkeepBonus.radiant = 0;
            }
            if(RemainingUpkeep.lush < 0)
            {
                UpkeepBonus.lush = 0 - RemainingUpkeep.lush;
                RemainingUpkeep.lush = 0;
            } else
            {
                UpkeepBonus.lush = 0;
            }
            if(RemainingUpkeep.crimson < 0)
            {
                UpkeepBonus.crimson = 0 - RemainingUpkeep.crimson;
                RemainingUpkeep.crimson = 0;
            } else
            {
                UpkeepBonus.crimson = 0;
            }
        }
        return RemainingUpkeep;
    }
}
