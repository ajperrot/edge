using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffinityPicker : MonoBehaviour
{
    public static AffinityPicker Instance; //singleton
    public static bool payingUpkeep = false;

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
        //first try to pay via upkeep bonus if in upkeep
        Affinity Cost = Encounter.UseUpkeepBonus(Costs[costType]);
        //re-prompt in case of failure to pay
        payingUpkeep = false;
        if(Cost.Pay() == false) gameObject.SetActive(true);;
    }
}
