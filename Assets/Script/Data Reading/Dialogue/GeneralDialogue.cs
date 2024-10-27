using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDialogue
{
    public string dialogueID {get;}
    public string dialogue {get;}
    public string dialogueBy {get;}
    public bool isDialogueSelection {get;}
    public string optionResponseID {get;}
    public string sceneName {get;}
    public string leftSpriteFilePath {get;}
    public string rightSpriteFilePath{get;}

    public string tutorialImage {get;}

    public bool repeatDialogue {get;}

    public GeneralDialogue(string dialogueID, string dialogue, string dialogueBy, bool isDialogueSelection, string optionResponseID, string sceneName, string leftSpriteFilePath, string rightSpriteFilePath, string tutorialImage, bool repeatDialogue)
    {
        this.dialogueID = dialogueID;
        this.dialogue = dialogue;
        this.dialogueBy = dialogueBy;
        this.isDialogueSelection = isDialogueSelection;
        this.optionResponseID = optionResponseID;
        this.sceneName = sceneName;
        this.leftSpriteFilePath = leftSpriteFilePath;
        this.rightSpriteFilePath = rightSpriteFilePath;
        this.tutorialImage = tutorialImage;
        this.repeatDialogue = repeatDialogue;
    }


}

public class RefGeneralDialogue
{
    public string dialogueID;
    public string dialogue;
    public string dialogueBy;
    public bool isDialogueSelection;
    public string optionResponseID;
    public string sceneName;
    public string leftSpriteFilePath;
    public string rightSpriteFilePath;

    public string tutorialImage;
    public bool repeatDialogue;
}