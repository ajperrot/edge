using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using TMPro;

public class Ability : MonoBehaviour
{
  public static Dictionary<int, XmlDocument> AbilityDocs = new Dictionary<int, XmlDocument>(); //cache for ability documents

  public TMP_Text Nametag; //displays the name of this ability
  public TMP_Text CostText; //rich text displaying the cost
  public TMP_Text DescriptionText; //writes a description of the ability
  public GameObject HoverOnlyUI; //UI only active while hovering 
  //public int userId; //who is using this ability?
  public int hoverIndex; //give this to root when hovered over
  public AbilitiesRoot Root; //the root which summoned us

  private int id; //which ability is this?
  private List<Payable> cost; //what do we lose when using this ability
  private bool autoTargeting; //if true, we don't have to target manually

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

  // Fill in the data for this ability and write to UI
  public void Initialize(AbilitiesRoot Root, int id)
  {
    this.Root = Root;
    this.id = id;
    //load file if not ready
    if(AbilityDocs.ContainsKey(id) == false) LoadAbilityDoc(id);
    //get Nodes
    XmlNodeList Nodes = AbilityDocs[id].FirstChild.ChildNodes;
    //store data from doc in this
    Nametag.text = Nodes[0].InnerText;
    //COST DATA AND DISPLAY
    DescriptionText.text = Nodes[2].InnerText;
    autoTargeting = XmlConvert.ToBoolean(Nodes[3].InnerText);
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

}