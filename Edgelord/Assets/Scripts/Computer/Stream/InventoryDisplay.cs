using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDisplay : MonoBehaviour
{
    public static List<Item> Inventory; //the player's items

    public GameObject ItemPrefab; //item icons come from this
    public Transform ItemsRoot; //all item icons are children of this for placement
    public int columnCount; //number of columns the items fall into
    public int rowCount; //number of rows for items
    public float xSpace; //space between items in the x-axis
    public float ySpace; //space between items in the y-axis
    public List<GameObject> ItemIcons = new List<GameObject>(); //array of item icons (children of itemsroot)

    //Redraws all InventoryDisplays
    public static void RedrawAll()
    {
        foreach(RitualNavigator Navigator in RitualNavigator.RitualNavigators) Navigator.InventoryBox.RedrawUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        //get inventory
        Inventory = PlayerCharacter.Instance.Inventory;
        //create display
        GenerateItemUI();

    }

    // Create an ItemUI object for every item
    void GenerateItemUI()
    {
        //create ui for each item in inventory
        for(int i = 0; i < Inventory.Count; i++)
        {
            GameObject ItemUI = Instantiate(ItemPrefab, ItemsRoot);
            ItemIcons.Add(ItemUI);
            //move to correct location
            int column = i % columnCount;
            ItemUI.GetComponent<RectTransform>().localPosition += new Vector3(column * xSpace, (i / columnCount) * ySpace, 0);
            //fill values
            ItemUI.GetComponent<Image>().sprite = Resources.Load<Sprite>(Inventory[i].iconPath);
            ItemUI.GetComponentInChildren<TMP_Text>().text = "" + Inventory[i].count;
            ItemUI.GetComponent<HelpWindow>().info = Inventory[i].description;
        }
    }

    // Clear the current UI and generate a new one
    public void RedrawUI()
    {
        //destroy current UI
        for(int i = 0; i < ItemIcons.Count; i++)
        {
            Destroy(ItemIcons[i]);
        }
        //clear the list
        ItemIcons = new List<GameObject>();
        //generate new item icons
        GenerateItemUI();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
