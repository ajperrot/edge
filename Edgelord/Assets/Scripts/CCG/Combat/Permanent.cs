using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Permanent : MonoBehaviour
{
    public Slider HpBar; //used to display hp
    public TMP_Text HpText; //displays hp as a number
    public TMP_Text MaxHpText; //displays maxHP as a number
    public ApRoot ApDisplay; //displays and stores ap
    public Slider SanityBar; //used to display sanity
    public TMP_Text SanityText; //prints sanity to screen
    public TMP_Text MaxSanityText; //prints maxSanity to screen
    public AbilitiesRoot AbilityDisplay; //displays buttons for each ability
    public Slider RadiantHpBar; //used to display radiantHp
    public TMP_Text RadiantHpText; //displays radiantHp as a number
    public GameObject UpkeepDisplay; //activated to show/accept entity upkeep
    public bool isAlly = false; //is this a member of the player's party?
    public CardInfo Info; //tracks stats not represented by off-card UI
    public GameObject Dimmer; //dims the image of the enemy to show blocking
    public Permanent Attacker; //last permanent to attack this one
    public bool isLeader = false; //false for all but the player character
    public bool targetable = true; //only set to false when defended by frontline, used in allies to prevent attack of corpses

    public List<Permanent> Soulbinds = new List<Permanent>(); //is this permanent bound by another?
    public List<Permanent> SoulboundEntities = new List<Permanent>(); //this permanent stays free until the soulbind leaves
    public bool gated = false; //has this permanent used the passive ability gate yet?
    public bool flying = false; //does this permanent take half damage?
    public int fuseCounter = 0; //progress toward possible transformation
    public int attackModifier = 0; //extra damage added to your attacks

    // Our maximum hp
    public int maxHp
    {
        get {return (int)HpBar.maxValue;}
        set
        {
            if(value < hp) hp = value;
            HpBar.maxValue = value;
            RadiantHpBar.maxValue = value;
            MaxHpText.text = value.ToString();
        }
    }

    // Our current hp
    public int hp
    {
        get {return (int)HpBar.value;}
        set
        {
            if(value > maxHp) value = maxHp;
            if(value < 0)
            {
                radiantHp += value;
                value = 0;
            }
            HpBar.value = value;
            HpText.text = value.ToString();
            if(value <= 0 && radiantHp <= 0) Encounter.Instance.Kill(this);
        }
    }

    // Extra hp used when at 0 hp
    public int radiantHp
    {
        get {return (int)RadiantHpBar.value;}
        set
        {
            if(value > 0)
            {
                RadiantHpBar.value = value;
                RadiantHpText.text = value.ToString();
                RadiantHpBar.gameObject.SetActive(true);
            } else
            {
                RadiantHpBar.value = 0;
                RadiantHpBar.gameObject.SetActive(false);
            }
        }
    }

    // Every Action consumes 1 ap, this is the max
    public int maxAp
    {
        get {return ApDisplay.maxAp;}
        set {ApDisplay.maxAp = value;}
    }

    // Current ap
    public int ap
    {
        get {return ApDisplay.ap;}
        set {ApDisplay.ap = value;}
    }

    // Max Psychological hp
    public int maxSanity
    {
        get {return (int)SanityBar.maxValue;}
        set
        {
            if(value < sanity) sanity = value;
            SanityBar.maxValue = value;
            MaxSanityText.text = value.ToString();
        }
    }

    // Psychological hp
    public int sanity
    {
        get {return (int)SanityBar.value;}
        set
        {
            if(value > maxSanity) value = maxSanity;
            SanityBar.value = value;
            SanityText.text = value.ToString();
        }
    }

    // Upkeep for this permanent, seperate from card info for modification
    private Affinity upkeep = null;
    public Affinity Upkeep
    {
        get {return upkeep;}
        set
        {
            upkeep = value;
            if(isAlly == true) UpkeepDisplay.GetComponentInChildren<TMP_Text>().text = upkeep.ToString();
        }
    }



    // Called on Start
    void Start()
    {
        //get sprite
        GetComponent<Image>().sprite = Card.GetCardArt(Info.id);
        //use summon-triggered passives
        Passive.TriggerPassives(this, 0);//on summon
        Passive.TriggerPassives(this, 1);//per turn
    }

    // Called on mouse hover
    public void OnPointerEnter()
    {
        AbilityDisplay.AddHover(0);
        Targeting.Target = this;
    }

    // Mouse leaves
    public void OnPointerExit()
    {
        AbilityDisplay.RemoveHover(0);
    }

    // Initialize with card info
    public void Initialize(CardInfo Info, bool isAlly = true)
    {
        this.Info = Info;
        maxHp = Info.hp;
        hp = maxHp;
        radiantHp = 0;
        maxAp = Info.ap;
        ap = maxAp;
        this.isAlly = isAlly;
        this.Upkeep = Info.Upkeep;
        //activate sanity for humans
        if(Info.Type == CardInfo.CardType.Human)
        {
            maxSanity = Info.sanity;
            sanity = Info.sanity;
            SanityBar.gameObject.SetActive(true);
        }
        //activate ability buttons only for allies
        if(isAlly == true)
        {
            AbilityDisplay.InitializeAbilityButtons(Info.Abilities);
            if(Info.Upkeep != null) UpkeepDisplay.GetComponentInChildren<TMP_Text>().text = Upkeep.ToString();
        }
    }

    // Called when clicked, sets this as the target if targeting
    public void SetAsTarget()
    {
        if(Targeting.ActiveInstance != null && (targetable == true || Ability.ActiveAbility.User.isAlly == true)) Targeting.ActiveInstance.SetTarget(this);
    }

    // Lose hp, taking into account buffs/debuffs
    public void TakeHit(int damage)
    {
        //halve damage if flying
        if(flying == true) damage = damage / 2;
        //take defense into account
        int defense;
        if(isAlly == true)
        {
            //check ally defense if ally
            defense = Encounter.Instance.allyDefense;
            defense -= damage;
            Encounter.Instance.allyDefense = defense;
        } else
        {
            //use enemy defense if enemy
            defense = Encounter.Instance.enemyDefense;
            defense -= damage;
            Encounter.Instance.enemyDefense = defense;
        }
        //take damage not eaten by defense
        if(defense < 0) hp += defense;
    }

    // Activate upkeep request ui if entity, become unusable otherwise
    public void RequestUpkeep()
    {
        //only request upkeep if necessary
        if(Upkeep != null && Soulbinds.Count == 0)
        {
            UpkeepDisplay.SetActive(true);
            AbilityDisplay.ToggleActivation(false);
        }
    }

    // Pay upkeep if able, disable upkeep ui and allow entity control
    public void PayUpkeep()
    {
        //attempt to deduct upkeep, will fail if unable
        if(Upkeep.Pay())
        {
            UpkeepDisplay.SetActive(false);
            AbilityDisplay.ToggleActivation(true);
        }
    }

    // Return false if Upkeep unpaid
    public bool CheckUpkeep()
    {
        return !(UpkeepDisplay.activeSelf);
    }

    // Make untargetable and show by dimming
    public void MakeUntargetable()
    {
        targetable = false;
        Dimmer.SetActive(true);
    }

    // Undo MakeUntargetable
    public void MakeTargetable()
    {
        targetable = true;
        Dimmer.SetActive(false);
    }

    // Select and use a skill on a target based on AI
    public void Act()
    {
        //act until out of ap
        while(ap > 0)
        {
            //use an ability if AI will allow it, checking more specialized ones first
            for(int i = Info.Abilities.Length - 1; i >= 0; i--)
            {
                int a = Info.Abilities[i];
                if(AI.AbilityDecisions[a](this) == true) Ability.AbilityUsages[a](this);
            }
            ap--;
        }
    }
}
