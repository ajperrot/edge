using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using TMPro;
using CardType = CardInfo.CardType;
using EntityType = CardInfo.EntityType;

public class Ability : MonoBehaviour
{
  public delegate void Usage(Permanent User);

  public static Dictionary<int, XmlDocument> AbilityDocs = new Dictionary<int, XmlDocument>(); //cache for ability documents
  public static Ability ActiveAbility = null;

  public TMP_Text Nametag; //displays the name of this ability
  public TMP_Text CostText; //rich text displaying the Cost
  public TMP_Text DescriptionText; //writes a description of the ability
  public GameObject HoverOnlyUI; //UI only active while hovering 
  public int hoverIndex; //give this to root when hovered over
  public Permanent User; //who is using these abilities?
  public bool autoTargeting; //if true, we don't have to target manually

  private AbilitiesRoot Root; //the root which summoned us
  private int id; //which ability is this?
  private Affinity Cost = new Affinity(); //what do we lose when using this ability

  // Fill in the data for this ability and write to UI
  public void Initialize(AbilitiesRoot Root, int id)
  {
    this.Root = Root;
    this.User = Root.User;
    this.id = id;
    //load file if not ready
    if(AbilityDocs.ContainsKey(id) == false) LoadAbilityDoc(id);
    //get Nodes
    XmlNodeList Nodes = AbilityDocs[id].FirstChild.ChildNodes;
    //store data from doc in this
    Nametag.text = Nodes[0].InnerText;
    Cost.FillFromXml(Nodes[1]);
    CostText.text = Cost.ToString();
    DescriptionText.text = Nodes[2].InnerText;
    autoTargeting = XmlConvert.ToBoolean(Nodes[3].InnerText);
  }

  // Highlight and open description when hovering over
  public void OnPointerEnter()
  {
    HoverOnlyUI.SetActive(true);
    Root.AddHover(hoverIndex);
  }

  // Undo the effects of OnPointerEnter
  public void OnPointerExit()
  {
    HoverOnlyUI.SetActive(false);
    Root.RemoveHover(hoverIndex);
  }

  // Use the ability if no targeting is needed, otherwise enter targeting
  public void OnClick()
  {
    //pay Cost, return if unable (also return if not your turn)
    if(Encounter.Instance.yourTurn == false || User.ap < 1 || Cost.Pay() == false) return;
    if(autoTargeting == true)
    {
      //wait for variableAffinity if necessary
      if(Cost > 99)
      {
        ActiveAbility = this;
        return;
      }
      //use immediately if auto-targeting
      Use();
    } else if(id == 0 && User.Grappler != null)
    {
      //special case if being grappled
      User.ap--;
      Permanent Target = User.Grappler;
      Targeting.Target = Target;
      Passive.TriggerPassives(User, 4);
      Target.hp -= User.Info.attack;
    }else
    {
      //otherwise begin targeting
      Root.TargetingArrow.SetActive(true);
      ActiveAbility = this;
    }
  }

  // Call the function associated with this anility id
  public void Use()
  {
    User.ap--;
    Permanent.CurrentActor = User;
    AbilityUsages[id](User);
    ActiveAbility = null;
  }

  // Loads the xml defining the ability with the given id
  private void LoadAbilityDoc(int id)
  {
    //get path to name list
    string path = Application.streamingAssetsPath + "/XML/Abilities/Ability" + id + ".xml";
    XmlDocument NewDoc = new XmlDocument();
    NewDoc.Load(path);
    AbilityDocs.Add(id, NewDoc);
  }


  // ALL THE ABILITIES ARE DEFINED BELOW //

  // User hits the target with their attack power
  static void Attack(Permanent User)
  {
    Passive.TriggerPassives(User, 4);
    Targeting.Target.Attacker = User;
    Targeting.Target.TakeHit(User.Info.attack + User.attackModifier);
  }

  // User draws agro and gives party 3 defense
  static void Defend(Permanent User)
  {
    if(Encounter.Instance.yourTurn == true)
    {
      //give your side 3 defense if used by a party member
      Encounter.Instance.allyDefense += 3;
      //add yourself to the frontline
      Encounter.Instance.JoinFrontLines(User, 0);
    } else
    {
      //otherwise the enemy gets 3 defense
      Encounter.Instance.enemyDefense += 3;
      //add yourself to the frontline
      Encounter.Instance.JoinFrontLines(User, 1);
    }
  }

  // User is soulbound to the target and gives them 5 rhp
  static void Devotion(Permanent User)
  {
    Permanent Target = Targeting.Target;
    //do nothing if the user is already soulbound to the target
    if(Target.SoulboundEntities.Contains(User) == true)
    {
      User.ap++;
      return;
    }
    Target.AddSoulboundEntity(User);
    Target.radiantHp += 5;
    //reflect with mirror
    if(Target.mirror == true)
    {
      Targeting.Target = User;
      Devotion(Target);
    }
  }

  // Transform User into a Symphony. Only usable after Consuming 3 human corpses.
  static void Fuse(Permanent User)
  {
    if(User.fuseCounter >= 3)
    {
      Encounter.Instance.AddAlly(new CardInfo(10), User.side);
      Encounter.Instance.Kill(User);
    } else
    {
        User.ap++; //don't waste ap on a failed attempt
    }
  }

  // If the target is a corpse, kill it and add to your fuse counter
  static void Consume(Permanent User)
  {
    Permanent Target = Targeting.Target;
    if(Target.Info.EntityClass == EntityType.Corpse)
    {
      Encounter.Instance.Kill(Target);
      if(User != null) User.fuseCounter++;
    } else
    {
      User.ap++;
    }
  }

  // Kill the user, and add its attack and upkeep to the target
  static void Parasite(Permanent User)
  {
    Permanent Target = Targeting.Target;
    Target.attackModifier += User.Info.attack;
    Target.Upkeep += User.Upkeep;
    Encounter.Instance.Kill(User);
  }

  // Deal 10 sanity damage to a human target and kill the user
  static void Vision(Permanent User)
  {
    Permanent Target = Targeting.Target;
    if(Target.Info.Type == CardType.Human)
    {
      Target.sanity -= 10;
      Encounter.Instance.Kill(User);
    } else
    {
      User.ap++;
    }
  }

  // Deal 2 damage to the opposing party
  static void Harmony(Permanent User)
  {
    int opposingSide = 0;
    if(User.side == 0) opposingSide = 1;
    List<Permanent> Party = Encounter.Instance.Parties[opposingSide];
    Permanent Target;
    for(int i = Party.Count - 1; i >= 0; i--)
    {
      Target = Party[i];
      Target.hp -= 2;
    }
  }

  // Deal 1 sanity damage to all humans
  static void Melody(Permanent User)
  {
    Permanent Target;
    List<Permanent> Party;
    for(int i = 0; i < 2; i++)
    {
      Party = Encounter.Instance.Parties[i];
      for(int j = Party.Count - 1; j >= 0; j--)
      {
        Target = Party[j];
        if(Target.Info.Type == CardType.Human) Target.sanity -= 1;
      }
    }
  }

  // Debuff the target's attack by the User's sanity * X for 1 turn
  static void Shackle(Permanent User)
  {
    Permanent Target = Targeting.Target;
    int x = 3;
    if(User.side == 0) x = VariableAffinity.x;
    int debuffAmmont = x * User.sanity;
    Target.attackModifier -= debuffAmmont;
    Encounter.Instance.PendingAttackBuffs[Target.side].Add(debuffAmmont);
    Encounter.Instance.PendingAttackBuffRecipients[Target.side].Add(Target);
    //reflect with mirror
    if(Target.mirror == true)
    {
      User.attackModifier -= debuffAmmont;
      Encounter.Instance.PendingAttackBuffs[User.side].Add(debuffAmmont);
      Encounter.Instance.PendingAttackBuffRecipients[User.side].Add(User);
    }
  }

  // Convert X corpses into reborn
  static void Reanimate(Permanent User)
  {
    List<Permanent> Party = Encounter.Instance.Parties[User.side];
    int reanimations = 0;
    int limit = 3;
    if(User.side == 0) limit = VariableAffinity.x;
    for(int i = Party.Count - 1; i >= 0 && reanimations < limit; i--)
    {
      if(Party[i].Info.id == 13)
      {
        Encounter.Instance.Kill(Party[i]);
        Encounter.Instance.AddAlly(new CardInfo(9), User.side);
        reanimations++;
      }
    }
    Party = Encounter.Instance.Parties[-1 * (User.side - 1)]; //convoluted way of flipping the bit
    for(int i = Party.Count - 1; i >= 0 && reanimations < limit; i--)
    {
      if(Party[i].Info.id == 13)
      {
        Encounter.Instance.Kill(Party[i]);
        Encounter.Instance.AddAlly(new CardInfo(9), User.side);
        reanimations++;
      }
    }
  }

  // Deal 2X damage to all enemies
  static void Scorch(Permanent User)
  {
    List<Permanent> Party;
    int x = 3;
    if(User.side == 0)
    {
      x = VariableAffinity.x;
      Party = Encounter.Instance.Parties[1];
    } else Party = Encounter.Instance.Parties[1];
    int damage = 2 * x;
    for(int i = Party.Count - 1; i >= 0; i--)
    {
      Party[i].TakeHit(damage);
    }
  }

  public static Usage[] AbilityUsages = new Usage[]
  {
    new Usage(Attack),
    new Usage(Defend),
    new Usage(Devotion),
    new Usage(Fuse),
    new Usage(Consume),
    new Usage(Parasite),
    new Usage(Vision),
    new Usage(Harmony),
    new Usage(Melody),
    new Usage(Shackle),
    new Usage(Reanimate),
    new Usage(Scorch)
  };

}