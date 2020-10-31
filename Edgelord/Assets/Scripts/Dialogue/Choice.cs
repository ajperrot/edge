using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Condition
{
    enum ConditionalType {itemUse, };
}

public class Choice
{
    public string buttonText; //text to be printed om teh UI associated with this choice
    public TextLine nextLine; //the line following this choice

}
