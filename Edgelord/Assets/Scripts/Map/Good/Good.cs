using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Good : MonoBehaviour
{
    public int stockIndex; //where this is in the overall stock
    public int id = -1; //id of the associated item/card
    public TMP_Text NameTextBox; //displays the name of the associated item
    public TMP_Text CostTextBox; //displays the cost of this item
    public HelpWindow Help; //spawns help text on mouseover
    
    // Start is called before the first frame update
    public void Start()
    {
        //get out HelpWindow
        Help = gameObject.GetComponent<HelpWindow>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
