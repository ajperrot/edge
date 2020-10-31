using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A series of posts forming a discussion
[System.Serializable]
public class Thread
{
    public string title; //the subject of this thread
    public string date;
    public Post OriginalPost; //the original post in this thread
    public Post[] Comments = new Post[0]; //the discussion displayed alongside the original
}
