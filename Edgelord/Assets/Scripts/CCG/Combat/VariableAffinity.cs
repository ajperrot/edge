using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableAffinity : MonoBehaviour
{
    public static VariableAffinity Instance; //singleton
    public static int x; //ammount paid

    public Image[] Icons;
    public int affinityType;

    private string[] types = {"radiant", "lush", "crimson"};
    private Affinity[,] Costs = new Affinity[3,3];

    // Start
    void Start()
    {
        Instance = this;
        for(int i = 0; i < 3; i++)
        {
            Costs[0,i] = new Affinity();
            Costs[0,i].radiant = i + 1;
        }
        for(int i = 0; i < 3; i++)
        {
            Costs[0,i] = new Affinity();
            Costs[0,i].lush = i + 1;
        }
        for(int i = 0; i < 3; i++)
        {
            Costs[0,i] = new Affinity();
            Costs[0,i].crimson = i + 1;
        }
        gameObject.SetActive(false);
    }

    // Decide which affinity type to pay
    public void SetAffinityType(int affinityType)
    {
        this.affinityType = affinityType;
        SetIcons(types[affinityType]);
        gameObject.SetActive(true);
    }

    // Set the icons on the buttons based on the given type
    private void SetIcons(string type)
    {
        foreach(Image Icon in Icons)
        {
            Icon.sprite = Resources.Load<Sprite>("Sprites/icon_" + type);
        }
    }

    // Multiply chosen affinity by the given count and pay
    public void PayChosenAffinity(int count)
    {
        if(Costs[affinityType, count].Pay() == false) return;
        x = count + 1;
        gameObject.SetActive(false);
        if(Ability.ActiveAbility.autoTargeting == true)
        {
            Ability.ActiveAbility.Use();
            Ability.ActiveAbility = null;
        }
    }

}
