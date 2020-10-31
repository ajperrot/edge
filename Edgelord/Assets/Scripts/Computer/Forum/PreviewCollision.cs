using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollision : MonoBehaviour
{
    public int id; //where the preview sits in the current list

    // Called when collider is clicked on
    public void OnMouseDown()
    {
        Board.Instance.OpenSpecifiedThread(id);
    }
}
