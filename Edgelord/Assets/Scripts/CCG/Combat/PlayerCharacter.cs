using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : Permanent
{
    public static PlayerCharacter Instance; //singleton

    public TMP_Text[] AffinityDisplay; //UI displaying radiance, lush, and crimson affinity

    public TMP_Text[] PopularityDisplay; //UI displaying folower count and popularity
    public TMP_Text MoneyDisplay; //UI displaying dollar count
    public TMP_Text MovementDisplay; //UI displaying movement

    public Deck PlayerDeck; //your deck

    public List<Item> Inventory = new List<Item>();

    // Your normal affinity
    [SerializeField]
    private PlayerAffinity _BaseAffinity = new PlayerAffinity();
    public PlayerAffinity BaseAffinity
    {
        get {return _BaseAffinity;}
        set
        {
            _BaseAffinity = value;
            //write new values to screen
            AffinityDisplay[0].text = "" + value.radiant;
            AffinityDisplay[1].text = "" + value.lush;
            AffinityDisplay[2].text = "" + value.crimson;
        }
    }

    // Your payable Affinity
    public Affinity PayableAffinity = new Affinity();

    // Your follower count
    [SerializeField]
    private int _followerCount = 0;
    public int followerCount
    {
        get {return _followerCount;}
        set
        {
            _followerCount = value;
            PopularityDisplay[0].text = "" + value;
        }
    }

    // Your popularity
    [SerializeField]
    private int _popularity = 0;
    public int popularity
    {
        get {return _popularity;}
        set
        {
            _popularity = value;
            PopularityDisplay[1].text = "" + value;
        }
    }

    // Your money
    [SerializeField]
    private int _money = 0;
    public int money
    {
        get {return _money;}
        set
        {
            _money = value;
            MoneyDisplay.text = "" + value;
        }
    }

    // Your movement per day
    public int maxMovement = 1;

    // Your movement remaining for this day
    [SerializeField]
    private int _movement = 0;
    public int movement
    {
        get {return _movement;}
        set
        {
            _movement = value;
            MovementDisplay.text = "" + value;
        }
    }


    // Called before Start
    void Awake()
    {
        Instance = this;
    }

    // Called once on startup
    void Start()
    {
        //set depletable stats to their maximum
        PayableAffinity = BaseAffinity;
        movement = maxMovement;
        money = money;//test
        //do some permanent init if necessary
        if(gameObject.tag == "PlayerPermanent") InitializePermanent();
    }

    // Called at start if in combat
    void InitializePermanent()
    {
        // ASK MATT WHICH STATS ARE VARIABLE SO WE KNOW WHAT TO SAVE/LOAD
        maxHp = 20;
        hp = maxHp;
        maxSanity = 10;
        sanity = maxSanity;
        // INITIALIZE ABILITIES BASED ON EQUIPMENT
        maxAp = 3;
        ap = maxAp;
    }

    // Add item to inventory and stack it if stackable
    public void GetItem(Item NewItem)
    {
        //check if stackable
        if(NewItem.stackable == true)
        {
            //if stackable, search for matching item and increase its quantity
            for(int i = 0; i < Inventory.Count; i++)
            {
                if(Inventory[i].id == NewItem.id)
                {
                    Inventory[i].count += NewItem.count;
                    InventoryDisplay.RedrawAll();
                    return;
                }
            }
        }
        //otherwise just add it to a new item slot
        Inventory.Add(NewItem);
        InventoryDisplay.RedrawAll();
    }

    // Remove the item with the id matching that given
    public void RemoveAllZeroCountItems()
    {
        //find the index
        for(int i = Inventory.Count - 1; i >= 0; i--)
        {
            if(Inventory[i].count <= 0) Inventory.RemoveAt(i);
        }
    }
}
