using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResponse
{
    public string[] triggerID {get;}
    public string playerDialogueID {get;}
    public string dialogue {get;}
    public string dialogueType {get;}
    public string nextSceneName {get;}
    public string[] currentSceneName {get;}

    public PlayerResponse(string[] triggerID, string playerDialogueID, string dialogue, string dialogueType, string nextSceneName, string[] currentSceneName)

    {
        this.triggerID = triggerID;
        this.playerDialogueID = playerDialogueID;
        this.dialogue = dialogue;
        this.dialogue = dialogue;
        this.dialogueType = dialogueType;
        this.nextSceneName = nextSceneName;
        this.currentSceneName = currentSceneName;
    }
}


public class RefPlayerResponse
{
    public string[] triggerID;
    public string playerDialogueID;
    public string dialogue;
    public string dialogueType;
    public string nextSceneName;
    public string[] currentSceneName;
}