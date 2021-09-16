using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardType = CardInfo.CardType;
using CardCallback = Card.CardCallback;

public class CardPrompt : MonoBehaviour
{
    public static CardPrompt Instance; //singleton

    public Transform HandRoot; //root of card objects in hand

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    // Prompt the user to play a card of the desired type from their hand, if one exists
    public bool PromptPlayFromHand(CardType DesiredType)
    {
        List<int> ElligibleCards = new List<int>();
        List<CardInfo> Hand = PlayerCharacter.Instance.Hand;
        for(int i = 0; i < PlayerCharacter.Instance.Hand.Count; i++)
        {
            if(Hand[i].Type == DesiredType)
            {
                ElligibleCards.Add(i);
            }
        }
        //return false if no card meets the criteria
        if(ElligibleCards.Count == 0) return false;
        //otherwise provide the prompt for which to use
        List<GameObject> HandCards = PlayerCharacter.Instance.HandCards;
        for(int i = 0; i < ElligibleCards.Count; i++)
        {
            HandCards[ElligibleCards[i]].transform.SetParent(transform);
            HandCards[ElligibleCards[i]].GetComponent<Card>().OnUse = new CardCallback(CloseHandPrompt);
        }
        gameObject.SetActive(true);
        return true;
    }

    // Close the prompt and return cards to hand
    public void CloseHandPrompt()
    {
        gameObject.SetActive(false);
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            transform.GetChild(i).SetParent(HandRoot);
        }
    }
}
