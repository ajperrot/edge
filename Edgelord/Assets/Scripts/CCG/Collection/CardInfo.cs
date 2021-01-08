using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using UnityEngine;

[Serializable]
public class CardInfo
{
    public enum CardType {Human, Entity, Phenomenon} //Enumeration of card types
    public enum EntityType {Abberation} //Extra classification for entities

    // Info About Cards In General
    public static int firstHumanId = 1; //location of the first human card
    public static int lastHumanId = 1; //location of the last human card
    public static XmlDocument NameDoc; //document storing names
    public static Dictionary<int, XmlDocument> CardDocs = new Dictionary<int, XmlDocument>(); //cache for card documents
    public static int randomIncrement = 0; //increases to keep rng cool


    // General Info About This Card
    public int id = -1; //lets us identify the card
    public string name = "DEFAULT NAME"; //the name of this card
    public CardType Type = CardType.Human; //the type of this card
    public Affinity SummonCost = new Affinity(); //cost to play the card
    public int hp = -1; //hit points, negative if not used
    public int attack = -1; //attack, negative if unused
    public string cardText = "DEFAULT CARD TEXT"; //rich-text written on the card explaining its use
    public string cartArtPath = "DEFAULT PATH"; //path to the art for this card
    public int[] Abilities; //the functions this card can perform
    public int[] Passives; //the functions this card performs automatically
    public int pattern; //the attack pattern of this card (for enemy permanents)
    public EntityType EntityClass; //entity sub-catagory
    public Affinity Upkeep = null; //cost paid to retain an entity
    public string humanClass = "";
    
    // Permanent Only Info
    public int ap = 1; //maximum ap, used for permanents

    // Human Only Info
    public int sanity = 255; //maximum sanity, used only for humans

    // Constructor
    public CardInfo(int id)
    {
        this.id = id;
        //load file if not ready
        if(CardDocs.ContainsKey(id) == false) LoadCardDoc(id);
        //get Nodes
        XmlNodeList Nodes = CardDocs[id].FirstChild.ChildNodes;
        //store stats from doc in this
        this.id = id;
        this.name = Nodes[1].InnerText;
        this.Type = (CardType)XmlConvert.ToInt32(Nodes[2].InnerText);
        this.SummonCost = GetAffinityFromXmlNodes(Nodes[3].ChildNodes);
        this.hp = XmlConvert.ToInt32(Nodes[4].InnerText);
        this.attack = XmlConvert.ToInt32(Nodes[5].InnerText);
        this.cardText = Nodes[6].InnerText;
        //add abilities
        XmlNodeList AbilityNodes = Nodes[7].ChildNodes;
        Abilities = new int[AbilityNodes.Count];
        for(int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = XmlConvert.ToInt32(AbilityNodes[i].InnerText);
        }
        //include ap, pattern and passives if not a phenomenon
        if(this.Type != CardType.Phenomenon)
        {
            //add passives like abilities
            AbilityNodes = Nodes[8].ChildNodes;
            Passives = new int[AbilityNodes.Count];
            for(int i = 0; i < Passives.Length; i++)
            {
                Passives[i] = XmlConvert.ToInt32(AbilityNodes[i].InnerText);
            }
            //ap / pattern
            this.ap = XmlConvert.ToInt32(Nodes[9].InnerText);
            this.pattern = XmlConvert.ToInt32(Nodes[11].InnerText);

            //include max sanity and unique name if human
            if(this.Type == CardType.Human)
            {
                this.sanity = XmlConvert.ToInt32(Nodes[10].InnerText);
                this.name += GenerateRandomName();
                this.humanClass = Nodes[12].InnerText;
            }
            else
            {
                //include entity class and upkeep if entity
                this.EntityClass = (EntityType)XmlConvert.ToInt32(Nodes[12].InnerText);
                this.Upkeep = GetAffinityFromXmlNodes(Nodes[13].ChildNodes);
                this.cardText += "\nUPKEEP: " + Upkeep.ToString();
            }
        }
        
    }

    // Load the document for a specified card
    private void LoadCardDoc(int id)
    {
        //get path to name list
        string path = Application.streamingAssetsPath + "/XML/Cards/Card" + id + ".xml";
        XmlDocument NewDoc = new XmlDocument();
        NewDoc.Load(path);
        CardDocs.Add(id, NewDoc);
    }

    // Create new Affinity and fill its values from an XmlNodeList
    private Affinity GetAffinityFromXmlNodes(XmlNodeList Nodes)
    {
        Affinity NewAffinity = new Affinity();
        NewAffinity.radiant = XmlConvert.ToInt32(Nodes[0].InnerText);
        NewAffinity.lush = XmlConvert.ToInt32(Nodes[1].InnerText);
        NewAffinity.crimson = XmlConvert.ToInt32(Nodes[2].InnerText);
        NewAffinity.free = XmlConvert.ToInt32(Nodes[3].InnerText);
        return NewAffinity;
    }

    // Gets a random first name from the humannames file
    private string GenerateRandomName()
    {
        //load file if not ready
        if(NameDoc == null) LoadNameDoc();
        //get Nodes
        XmlNode Nodes = NameDoc.FirstChild;
        //get first name
        XmlNodeList FirstNames = Nodes.ChildNodes;
        return FirstNames[(int)UnityEngine.Random.Range(0, FirstNames.Count - 1)].InnerText;
    }

    // Load the document of random names
    private void LoadNameDoc()
    {
        //get path to name list
        string path = Application.streamingAssetsPath + "/XML/HumanNames.xml";
        NameDoc = new XmlDocument();
        NameDoc.Load(path);
    }

}
