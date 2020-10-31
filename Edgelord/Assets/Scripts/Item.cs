using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    const string iconPathRoot = "ItemSprites/";
    public static XmlDocument ItemDoc; //stores info on all items

    public int id = -1; //default to invalid id
    public string name; //what is this item called
    public int cost; //cost in dollars
    public int count = 1; //number of this item had
    public bool stackable = false; //can we even have multiple?
    public string iconPath = "ItemSprites/100x100"; //path in resources to load the icon
    public string description; //mouseover text

    // Set id to given value and fill other fields from xml
    public Item(int id)
    {
        //set id
        this.id = id;
        //load file if not ready
        if(ItemDoc == null) LoadItemDoc();
        //get root
        XmlNode Root = ItemDoc.FirstChild;
        //get item as node
        XmlNodeList ItemData = Root.ChildNodes[id].ChildNodes;
        //fill values from node
        this.name = ItemData[1].InnerText;
        this.cost = XmlConvert.ToInt32(ItemData[2].InnerText);
        this.stackable = XmlConvert.ToBoolean(ItemData[3].InnerText);
        this.iconPath = iconPathRoot + ItemData[4].InnerText;
        this.description = ItemData[5].InnerText;
    }

    // Load the document of item data
    public static void LoadItemDoc()
    {
        //get path to name list
        string path = Application.streamingAssetsPath + "/XML/Items.xml";
        ItemDoc = new XmlDocument();
        ItemDoc.Load(path);  
    }
}
