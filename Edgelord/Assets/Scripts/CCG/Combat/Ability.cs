using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using TMPro;

public class Ability : MonoBehaviour
{
  public delegate void Usage(Permanent User);

  public static Dictionary<int, XmlDocument> AbilityDocs = new Dictionary<int, XmlDocument>(); //cache for ability documents

  public TMP_Text Nametag; //displays the name of this ability
  public TMP_Text CostText; //rich text displaying the Cost
  public TMP_Text DescriptionText; //writes a description of the ability
  public GameObject HoverOnlyUI; //UI only active while hovering 
  public int hoverIndex; //give this to root when hovered over

  private AbilitiesRoot Root; //the root which summoned us
  private int id; //which ability is this?
  private Affinity Cost = new Affinity(); //what do we lose when using this ability
  private bool autoTargeting; //if true, we don't have to target manually
  private Permanent User; //who is using these abilities?

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
    //pay Cost, return if unable
    print(User);//test
    if(User.ap < 1 || Cost.Pay() == false) return;
    User.ap--;
    if(autoTargeting == true)
    {
      //use immediately if auto-targeting
      Use();
    } else
    {
      //otherwise begin targeting
      Root.TargetingArrow.SetActive(true);
    }
  }

  // Call the function associated with this anility id
  public void Use()
  {
    AbilityUsages[id](User);
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
    Targeting.Target.TakeHit(User.Info.attack);
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


  private static Usage[] AbilityUsages = new Usage[]
  {
    new Usage(Attack),
    new Usage(Defend)
  };

}