using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A text post made by a user
[System.Serializable]
public class Post
{
    public bool sent; //did our player character send this
    public string poster; //name of the poster/commenter
    public string content; //message written by the poster
}
