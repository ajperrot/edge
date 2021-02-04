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
    public Affinity BaseAffinity = new Affinity();

    // Your payable Affinity
    private PlayerAffinity _PayableAffinity = new PlayerAffinity();
    public PlayerAffinity PayableAffinity
    {
        get {return _PayableAffinity;}
        set
        {
            _PayableAffinity.radiant = value.radiant;
            _PayableAffinity.lush = value.lush;
            _PayableAffinity.crimson = value.crimson;
            _PayableAffinity.free = value.free;
        }
    }

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

    // Combat Stats
    public int baseHp = 5;
    public int baseSanity = 10;
    public int baseAp = 1;

    [Header("COMBAT ONLY")]
    public Transform HandRoot; //parent to cards in hand
    public GameObject HumanCardPrefab; //copied to make human cards in hand
    public GameObject EntityCardPrefab; //copied to make entity cards in hand
    public GameObject PhenomenonCardPrefab; //copied to make phenomenon cards in hand
    public float cardSpacing; //space between each card in the hand

    public List<CardInfo> Hand; //your hand in combat
    public List<GameObject> HandCards = new List<GameObject>(); //card ui for each card in hand


    // Called before Start
    void Awake()
    {
        Instance = this;
    }

    // Called once on startup
    void Start()
    {
        //set depletable stats to their maximum
        PayableAffinity = new PlayerAffinity(BaseAffinity);
        movement = maxMovement;
        money = money;//test
        //do some permanent init if necessary
        if(gameObject.tag == "PlayerPermanent")
        {
            PlayerDeck.AddNewCard(13);//test
            //PlayerDeck.AddNewCard(7);//test
            PlayerDeck.AddNewCard(13);//test
            PlayerDeck.AddNewCard(13);//test
            PlayerDeck.AddNewCard(18);//test
            InitializePermanent();
            side = 0;
            isLeader = true;
        }
    }

    // COMBAT
    // Called at start if in combat
    void InitializePermanent()
    {
        //set stats to their base values
        maxHp = baseHp;
        hp = maxHp;
        maxSanity = baseSanity;
        sanity = maxSanity;
        maxAp = baseAp;
        ap = maxAp;
        //set cardinfo stuff
        Info = new CardInfo(0);
        // INITIALIZE ABILITIES BASED ON EQUIPMENT
        AbilityDisplay.InitializeAbilityButtons(Info.Abilities);
        //set up hand
        DealStartingHand();
    }

    // Shuffles the deck, gets a starting hand, and puts it onscreen
    void DealStartingHand()
    {
        //get cards
        PlayerDeck.Shuffle();
        Hand = PlayerDeck.GetHand();
        //display cards
        DisplayHandFrom(0);
    }

    // Add cards until the hand is full
    public void FillHand()
    {
        int oldCount = Hand.Count;
        Hand.AddRange(PlayerDeck.DrawUntilFullFrom(Hand.Count));
        DisplayHandFrom(oldCount);
    }

    // Remove the card at the given index from the hand
    public void RemoveFromHand(int index)
    {
        //delete card
        Hand.RemoveAt(index);
        Destroy(HandCards[index]);
        HandCards.RemoveAt(index);
        //move over other cards
        for(int i = index; i < HandCards.Count; i++)
        {
            HandCards[i].transform.localPosition -= new Vector3(cardSpacing, 0, 0);
        }
    }

    // Creates Card Objects for cards from a certain index in the hand
    void DisplayHandFrom(int start = 0)
    {
        for(int i = start; i < Hand.Count; i++)
        {
            if(Hand[i].Type == CardInfo.CardType.Human)
            {
                //make human card
                HandCards.Add(Instantiate(HumanCardPrefab, HandRoot));
                HandCards[i].GetComponent<HumanCard>().Info = Hand[i];
            } else if(Hand[i].Type == CardInfo.CardType.Entity)
            {
                //make entity card
                HandCards.Add(Instantiate(EntityCardPrefab, HandRoot));
                HandCards[i].GetComponent<EntityCard>().Info = Hand[i];
            } else
            {
                //make phenomenon card
                HandCards.Add(Instantiate(PhenomenonCardPrefab, HandRoot));
                HandCards[i].GetComponent<PhenomenonCard>().Info = Hand[i];
            }
            HandCards[i].transform.localPosition += new Vector3(cardSpacing * i, 0, 0);
        }
    }

    //INVENTORY
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
