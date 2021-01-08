using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffinityPicker : MonoBehaviour
{
    public static AffinityPicker Instance; //singleton

    public Affinity[] Costs; // costs paid when chosen

    void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }


    // Prompt for basic payment
    public void Prompt()
    {
        gameObject.SetActive(true);
    }

    // Pay selected cost type
    public void Pay(int costType)
    {
        gameObject.SetActive(false);
        //re-prompt in case of failure to pay
        if(Costs[costType].Pay() == false) gameObject.SetActive(true);;
    }
}
