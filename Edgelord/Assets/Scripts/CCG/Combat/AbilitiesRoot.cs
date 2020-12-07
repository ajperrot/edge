using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesRoot : MonoBehaviour
{
    public GameObject AbilityPrefab; //template for ability UI
    public float ButtonSpacing = 40; //space between buttons vertically
    public GameObject TargetingArrow; //contains ui for targeting

    private GameObject[] AbilityButtons; //array of all ability buttons
    private List<bool> HoverStatus = new List<bool>(); //what is being hovered over

    // Sets up the UI for each ability in the given array
    public void InitializeAbilityButtons(int[] Abilities)
    {
        HoverStatus.Add(false);
        HoverStatus.Add(false);
        AbilityButtons = new GameObject[Abilities.Length];
        for(int i = 0; i < AbilityButtons.Length; i++)
        {
            AbilityButtons[i] = GameObject.Instantiate(AbilityPrefab, transform);
            Ability CurrentAbility = AbilityButtons[i].GetComponent<Ability>();
            CurrentAbility.Initialize(this, Abilities[i]);
            CurrentAbility.hoverIndex = i + 2;
            AbilityButtons[i].transform.localPosition += new Vector3(0, ButtonSpacing, 0);
            HoverStatus.Add(false);
        }
    }

    // Set the given HoverStatus to true and activate self
    public void AddHover(int hoverIndex)
    {
        HoverStatus[hoverIndex] = true;
        gameObject.SetActive(true);
    }

    // Set the given HoverStatus to false and deactivate self if none are true
    public void RemoveHover(int hoverIndex)
    {
        HoverStatus[hoverIndex] = false;
        if(HoverStatus.Contains(true)) return;
        //set inactive if no hovering
        gameObject.SetActive(false);
    }
}
