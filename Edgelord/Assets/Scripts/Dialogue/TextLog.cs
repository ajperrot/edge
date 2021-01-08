using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using UnityEngine.SceneManagement;
using TMPro;


public class TextLog : MonoBehaviour
{
    public TMP_Text Dialogue; //text element used for output
    public TMP_Text Speaker; //text element used to indicate speaker
    public int sceneNumber; //used to get proper scene from files
    public TextLine[] Lines; //series of lines to write to output
    public bool goToComputer; //should we change scenes to the computer afterwards

    private int currentLine = 0; //which line are we reading??

    // Freeze/Activate Inputs on enable
    void OnEnable()
    {
        InputManager.SaveAndClearInputEvents();
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += PrintNextLine;
        //get series of lines to read
        LoadTextLines(Setting.currentDay);
        //print first line
        currentLine = 0;
        PrintLine(Lines[currentLine]);
    }

    // Deactivate Inputs on enable, and restore old ones on disable
    void OnDisable()
    {
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= PrintNextLine;
        InputManager.RestoreInputState();
    }

    // Advances text on click
    public void OnClick()
    {
        //halt the typing process
        StopAllCoroutines();
        //start a new one
        PrintNextLine(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Load the scene's TextLines from xml
    void LoadTextLines(int day)
    {
        //load file
        string path = Application.streamingAssetsPath + "/XML/Cutscenes/Scene" + day + "-" + sceneNumber + ".xml";
        XmlDocument Doc = new XmlDocument();
        Doc.Load(path);
        //get root
        XmlNode Root = Doc.FirstChild;
        //get children (Lines)
        Lines = new TextLine[Root.ChildNodes.Count];
        for(int i = 0; i < Root.ChildNodes.Count; i++)
        {
            XmlNode LineNode = Root.ChildNodes[i];
            //first two children of thread are title and date
            Lines[i] = new TextLine();
            Lines[i].speaker = LineNode.ChildNodes[0].InnerText;
            Lines[i].isLeftActor = XmlConvert.ToBoolean(LineNode.ChildNodes[1].InnerText);
            Lines[i].spriteName = LineNode.ChildNodes[2].InnerText;
            Lines[i].text = LineNode.ChildNodes[3].InnerText;
            Lines[i].isLastLine = XmlConvert.ToBoolean(LineNode.ChildNodes[4].InnerText);
        }
    }

    // Advance the line and print it
    public void PrintNextLine(float axisValue)
    {
        if(Lines[currentLine].isLastLine == true)
        {
            //end scene if we've seen the last line
            gameObject.SetActive(false);
            if(goToComputer)
            {
                //go to computer scene if necessary
                SceneChanger.Instance.ChangeScene(1);
            }
        } else
        {
            //otherwise, advance the scene
            currentLine++;
            PrintLine(Lines[currentLine]);
        }
    }

    // Replace current output with line
    public void PrintLine(TextLine Line)
    {
        //clear existing text
        ClearOutput();
        //do not allow us to advance while text is printing
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= PrintNextLine;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += FinishCurrentLine;
        //show new speaker
        Speaker.text = Line.speaker;
        //write new text
        StartCoroutine(TypeOut(Line.text));
    }

    // Clears the text display
    public void ClearOutput()
    {
        Dialogue.text = "";
    }

    // Write out the line one char at a time
    private IEnumerator TypeOut(string text)
    {
        //write new text
        for (int i = 0; i < text.Length; ++i)
        {
            Dialogue.text += text[i];
            //if (audioOn == true) PlayRandomTextSound();
            //yield return new WaitForSeconds(0.0167f);
            yield return new WaitForEndOfFrame();
        }
        //restore ability to advance text, remove ability to skip this text as it is printed
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= FinishCurrentLine;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += PrintNextLine;
    }

    // Write whole line at once
    public void FinishCurrentLine(float axisValue)
    {
        //halt the typing process
        StopAllCoroutines();
        //write the line
        Dialogue.text = Lines[currentLine].text;
        //restore ability to advance text, remove ability to skip this text as it is printed
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] -= FinishCurrentLine;
        InputManager.OnInputHit[(int)InputManager.AxisEnum.Confirm] += PrintNextLine;
    }
}
