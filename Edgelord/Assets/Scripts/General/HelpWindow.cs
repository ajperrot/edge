using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpWindow : MonoBehaviour
{
    public GameObject HelpWindowPrefab; //prefab of help window
    public Vector3 WindowDisplacement; //distance between object and help window

    private string _info = ""; //text to display on mouseover
    public string info
    {
        get {return _info;}
        set
        {
            _info = value;
            if(Window != null) Window.GetComponentInChildren<TMP_Text>().text = info;
        }
    }

    private GameObject Window; //object representing the help window

    // Start is called before the first frame update
    void Start()
    {
        //create help window
        Window = Instantiate(HelpWindowPrefab, transform);
        //move via displacement
        Window.GetComponent<RectTransform>().localPosition += WindowDisplacement;
        //set text
        Window.GetComponentInChildren<TMP_Text>().text = info;
        //make child of canvas so we can see it in front
        Window.transform.SetParent(GameObject.FindGameObjectsWithTag("Canvas")[0].transform);
        //deactivate until mouseover
        Window.SetActive(false);
    }

    // Activate the Help window whenever the mouse hovers over this object
    public void OnMouseEnter()
    {
        Window.SetActive(true);
    }

    // Deactivate the Help window whenever the mouse DOES NOT hover over this object
    public void OnMouseExit()
    {
        Window.SetActive(false);
    }

    // Deactivate window when this is disabled
    void OnDisable()
    {
        Window.SetActive(false);
    }
}
