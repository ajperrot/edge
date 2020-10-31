using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using TMPro;

// A series of threads navigable by the player
public class Board : MonoBehaviour
{
    public static Board Instance; //singleton

    public const int previewsPerPage = 4; //previews displayed without scrolling
    public const int commentsPerPage = 3; //comments displayed without scrolling


    public GameObject PreviewPrefab; //blueprint for a preview
    public float previewSpace; //Space between previews
    public GameObject ThreadWindowPrefab; //blueprint for a thread window
    public GameObject SentCommentPrefab; //blueprint for comments left by the player character
    public GameObject OtherCommentPrefab; //blueprint for comments form other users    
    public float commentSpace; //Space between comments
    public float commentScrollSpeed; //Speed of scrolling through comments
    public int scrollFrameOffset; //frames between each mouse scroll action
    public int commentScrollFrameOffset; //frames between comment scrolls via mouse
    public Thread[] Threads; //all threads currently on the board

    private Transform PreviewRoot; //Transform which previews are children of
    private Transform Selector; //Location of the selection-indicating UI
    private GameObject[] Previews; //Array of previews on the board
    private float maxScroll; //maximum ammount the preview root can be displaced
    private float currentScroll = 0; //current ammount of preview displacement
    private int selectedPreview = 0; //preview currently selected
    private bool previewing = true; //are we looking at previews or a thread?
    private Transform CommentRoot; //parent of all comments
    private float commentScroll = 0; //current scroll in comments section
    private float maxCommentScroll = 0; //maximum ammount of scrolling we can do through comments
    private GameObject ActiveThreadWindow; //thread window currently open
    private int scrollTimer; //frames between mouse scroll input readings

    // Start is called before the first frame update
    void Start()
    {
        //set singleton
        Instance = this;
        //initialize references within object
        PreviewRoot = transform.GetChild(0);
        Selector = PreviewRoot.GetChild(0);
        //get threads from xml based on day
        LoadThreads(Calendar.currentDay);
        //allocate space in Previews
        Previews = new GameObject[Threads.Length];
        //display a preview for every thread on the board
        for(int i = 0; i < Threads.Length; i++)
        {
            Previews[i] = GeneratePreview(Threads[i], i);
        }
        //set max scroll based on number of items to scroll through (4 per page)
        maxScroll = (Threads.Length - previewsPerPage) * previewSpace * -1;
        if(maxScroll > 0) maxScroll = 0;
    }

    // Load the day's threads from xml
    void LoadThreads(int day)
    {
        //load file
        string path = Application.streamingAssetsPath + "/XML/BoardContents/BoardContent" + day + ".xml";
        XmlDocument Doc = new XmlDocument();
        Doc.Load(path);
        //get root
        XmlNode Root = Doc.FirstChild;
        //get children (threads)
        if(Root.HasChildNodes)
        {
            Threads = new Thread[Root.ChildNodes.Count];
            for(int i = 0; i < Root.ChildNodes.Count; i++)
            {
                XmlNode ThreadNode = Root.ChildNodes[i];
                //first two children of thread are title and date
                Threads[i] = new Thread();
                Threads[i].title = ThreadNode.ChildNodes[0].InnerText;
                Threads[i].date = ThreadNode.ChildNodes[1].InnerText;
                //then original post
                Threads[i].OriginalPost = LoadPost(ThreadNode.ChildNodes[2]);
                //then comments
                XmlNode CommentsNode = ThreadNode.ChildNodes[3];
                if(CommentsNode.HasChildNodes)
                {
                    Threads[i].Comments = new Post[CommentsNode.ChildNodes.Count];
                    for(int j = 0; j < CommentsNode.ChildNodes.Count; j++)
                    {
                        Threads[i].Comments[j] = LoadPost(CommentsNode.ChildNodes[j]);
                    }
                }
            }
        }
    }

    // Load a post from an xml node and return it
    Post LoadPost(XmlNode root)
    {
        Post Result = new Post();
        Result.sent = XmlConvert.ToBoolean(root.ChildNodes[0].InnerText);
        Result.poster = root.ChildNodes[1].InnerText;
        Result.content = root.ChildNodes[2].InnerText;
        return Result;
    }

    // Activate Inputs on enable
    void OnEnable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] += PreviewScroll;
        InputManager.OnInput[(int)InputManager.AxisEnum.Vertical] += CommentScroll;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += OpenThread;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] += CloseThread;
    }

    // Deactivate Inputs on disable
    void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Vertical] -= PreviewScroll;
        InputManager.OnInput[(int)InputManager.AxisEnum.Vertical] -= CommentScroll;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= OpenThread;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Cancel] -= CloseThread;
    }

    // Scroll either the previews
    void PreviewScroll(float axisValue)
    {
        //scroll through previews if previewing
        if(previewing == true)
        {
            //determine the scroll direction
            float sign = axisValue / Mathf.Abs(axisValue);
            float scrollAmmount = sign * previewSpace;
            //scroll the previews if necessary
            if( (sign > 0 && currentScroll < 0) || (sign < 0 && currentScroll > maxScroll) )
            {
                currentScroll += scrollAmmount;
                PreviewRoot.Translate(Vector3.up * scrollAmmount * -1);
            }
            //adjust the selector
            if( (sign > 0 && selectedPreview > 0) || (sign < 0 && selectedPreview < Previews.Length - 1) )
            {
                Selector.Translate(Vector3.up * scrollAmmount);
                selectedPreview += (int)(sign * -1);
            }
            //set scroll timer for mouse scroll limiting
            scrollTimer = scrollFrameOffset;
        }
    }

    // Scroll through comments
    void CommentScroll(float axisValue)
    {
        if(previewing == false)
        {
            //determine the scroll direction
            float sign = (axisValue > 0)? 1 : -1;
            float scrollAmmount = sign * commentScrollSpeed;
            //scroll the comments if necessary
            if( (sign > 0 && commentScroll < 0) || (sign < 0 && commentScroll > maxCommentScroll) )
            {
                commentScroll += scrollAmmount;
                CommentRoot.GetComponent<RectTransform>().localPosition += (Vector3.up * scrollAmmount * -1);
            }
            //set scroll timer for mouse scroll limiting
            scrollTimer = commentScrollFrameOffset;
        }
    }

    // Open up the currently selected thread
    void OpenThread(float axisValue)
    {
        //only open the thread if not alreay viewing one
        if(previewing == true)
        {
            previewing = false;
            ActiveThreadWindow = GenerateThreadWindow(Threads[selectedPreview]);
            commentScroll = 0;
        }
    }

    // Opens thread at the specified id
    public void OpenSpecifiedThread(int id)
    {
        //change selection and move selector
        selectedPreview = id;
        Selector.transform.Translate(new Vector3(0, (Previews[id].transform.position.y - Selector.transform.position.y), 0));
        OpenThread(0);
    }

    // Close the current thread if there is one
    public void CloseThread(float axisValue)
    {
        //only close the thread if not alreay viewing one
        if(previewing == false)
        {
            previewing = true;
            Destroy(ActiveThreadWindow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollTimer == 0)
        {
            //check mouse scroll 
            float mouseY = Input.mouseScrollDelta.y;
            if(Mathf.Abs(mouseY) > 0.05)
            {
                PreviewScroll(mouseY);
                CommentScroll(mouseY);
            }
        } else
        {
            //count down to next scroll chance
            scrollTimer--;
        }
    }

    // Generates a preview of the Thread to scroll past
    public GameObject GeneratePreview(Thread Base, int id)
    {
        //create
        GameObject Preview = Instantiate(PreviewPrefab, PreviewRoot);
        //set id
        Preview.GetComponent<PreviewCollision>().id = id;
        //fill text
        TMP_Text[] TextFields = Preview.GetComponentsInChildren<TMP_Text>();
        TextFields[0].text = Base.title;
        TextFields[1].text = Base.OriginalPost.content;
        //move to correct position
        Preview.GetComponent<RectTransform>().localPosition += (Vector3.down * id * previewSpace);
        return Preview;
    }

    // Generates a window displaying the base thread
    public GameObject GenerateThreadWindow(Thread Base)
    {
        GameObject ThreadWindow = Instantiate(ThreadWindowPrefab, transform);
        ThreadWindow.transform.SetSiblingIndex(transform.childCount - 2);
        TMP_Text[] TextFields = ThreadWindow.GetComponentsInChildren<TMP_Text>();
        //fill text fields which every thread window has
        TextFields[0].text = Base.title;
        TextFields[1].text = Base.date;
        TextFields[2].text = Base.OriginalPost.poster;
        TextFields[3].text = Base.OriginalPost.content;
        //add in comments
        CommentRoot = GameObject.FindWithTag("CommentSection").transform;
        for(int i = 0; i < Base.Comments.Length; i++)
        {
            GameObject Comment;
            //copy correct prefab for sent/received
            if(Base.Comments[i].sent == true)
            {
                Comment = Instantiate(SentCommentPrefab, CommentRoot); 
            } else
            {
                Comment = Instantiate(OtherCommentPrefab, CommentRoot); 
            }
            //fill text
            TMP_Text[] CommentFields = Comment.GetComponentsInChildren<TMP_Text>();
            CommentFields[0].text = Base.Comments[i].poster;
            CommentFields[1].text = Base.Comments[i].content;
            //move to correct location in thread
            Comment.GetComponent<RectTransform>().localPosition += (Vector3.down * commentSpace * i);
        }
        //set max scroll based on number of comments
        maxCommentScroll = (Base.Comments.Length - commentsPerPage) * commentSpace * -1;
        if(maxCommentScroll > 0) maxCommentScroll = 0;
        //return completed window
        return ThreadWindow;
    }
}
