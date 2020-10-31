using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadWindowBack : MonoBehaviour
{
    // Called when clicked on
    public void OnMouseDown()
    {
        Board.Instance.CloseThread(0);//test
    }
}
