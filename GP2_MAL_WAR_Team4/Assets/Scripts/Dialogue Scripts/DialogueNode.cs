using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 15)] 
    public string dialogueText;
    public List<DialogueResponse> responses;

    internal bool IsLastNode()
    {
        if( responses == null ) { return true; }
        return responses.Count <= 0;
    }
}