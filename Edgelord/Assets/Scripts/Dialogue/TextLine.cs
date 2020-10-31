using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLine
{
    public string speaker; //the speaker of the line
    public bool isLeftActor; //does the sprite appear on the left
    public string spriteName; //name of the sprite if applicable
    public string text; //the line of text to be written to the screen
    public bool isLastLine; //indicates if the cutscene should end after this line
    public Choice[] Choices; //the options presented to us after the line
}
