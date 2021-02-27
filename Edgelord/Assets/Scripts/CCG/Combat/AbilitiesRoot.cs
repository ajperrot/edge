using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesRoot : MonoBehaviour
{
    public static Ability LastAddedAbility = null; //used by variable affinity to fill cost of phenomena

    public GameObject AbilityPrefab; //template for ability UI
    public float buttonSpacing = 60; //space between buttons vertically
    public Permanent User; //who is using these abilities?
    public GameObject TargetingArrow; //contains ui for targeting

    private List<GameObject> AbilityButtons = new List<GameObject>; //array of all ability buttons
    private List<bool> HoverStatus = new List<bool>(); //what is being hovered over
    private bool on = true;

    private static float abilityOffset = -200;

    // Sets up the UI for each ability in the given array
    public void InitializeAbilityButtons(int[] Abilities)
    {
        User = transform.parent.GetComponent<Permanent>();
        HoverStatus.Add(false);
        HoverStatus.Add(false);
        for(int i = 0; i < Abilities.Length; i++)
        {
            AbilityButtons.Add(GameObject.Instantiate(AbilityPrefab, transform))
            Ability CurrentAbility = AbilityButtons[i].GetComponent<Ability>();
            CurrentAbility.Initialize(this, Abilities[i]);
            CurrentAbility.hoverIndex = i + 2;
            AbilityButtons[i].transform.localPosition += new Vector3(0, buttonSpacing * i + abilityOffset, 0);
            HoverStatus.Add(false);
        }
        gameObject.SetActive(false);
    }

    // Adds an additional ability button
    public void AddAbilityButton(int ability)
    {
        AbilityButtons.Add(GameObject.Instantiate(AbilityPrefab, transform));
        int i = AbilityButtons.Count - 1;
        Ability CurrentAbility = AbilityButtons[i].GetComponent<Ability>();
        CurrentAbility.Initialize(this, Abilities[i]);
        CurrentAbility.hoverIndex = i + 2;
        AbilityButtons[i].transform.localPosition += new Vector3(0, buttonSpacing * i + abilityOffset, 0);
        HoverStatus.Add(false);
        LastAddedAbility = CurrentAbility;
    }

    // Set the given HoverStatus to true and activate self
    public void AddHover(int hoverIndex)
    {
        HoverStatus[hoverIndex] = true;
        if(Encounter.Instance.yourTurn == true && on == true) gameObject.SetActive(true);
    }

    // Set the given HoverStatus to false and deactivate self if none are true
    public void RemoveHover(int hoverIndex)
    {
        HoverStatus[hoverIndex] = false;
        if(HoverStatus.Contains(true)) return;
        //set inactive if no hovering
        gameObject.SetActive(false);
    }

    // Do not allow ui activation
    public void ToggleActivation(bool active)
    {
        gameObject.SetActive(active);
        on = active;
    }

    // Treat an ability as though it were clicked
    public void UseAbility(int index = 0)
    {
        print("0");//test
        AbilityButtons[index].GetComponent<Ability>().OnClick();
    }
}
