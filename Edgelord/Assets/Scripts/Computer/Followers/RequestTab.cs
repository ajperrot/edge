using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestTab : MonoBehaviour
{
    public GameObject Selector; //ui indicating selected request

    private GameObject[] Requests; //collection of follower request ui
    private int currentSelection = 0;

    // Start is called before the first frame update
    void Start()
    {
        Requests = GameObject.FindGameObjectsWithTag("Request");
    }

    // Called whenever enabled
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Horizontal] += ChangeSelection;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += ConfirmSelection;
    }

    // Called whenever disabled
    void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Horizontal] -= ChangeSelection;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= ConfirmSelection;
    }

    // Change currently selected request
    public void ChangeSelection(float axisValue)
    {
        //modify current selection
        if(axisValue > 0)
        {
            currentSelection++;
            if(currentSelection >= Requests.Length) currentSelection = 0;
        } else
        {
            currentSelection--;
            if(currentSelection < 0) currentSelection = Requests.Length - 1;
        }
        //move UI to selected button
        MoveSelectorToSelection();
    }

    // Moves the selector to the button selected
    void MoveSelectorToSelection()
    {
        float xDiff = Requests[currentSelection].transform.position.x - Selector.transform.position.x;
        Selector.transform.Translate(new Vector3(xDiff, 0, 0));
    }

    // Change selection to a specific one
    public void ChangeSelectionTo(int newSelection)
    {
        currentSelection = newSelection;
        //move UI to selected button
        MoveSelectorToSelection();
    }

    // Confirm your current selection
    public void ConfirmSelection(float axisValue)
    {
        Requests[currentSelection].GetComponent<Recruit>().Accept();
    }
}
