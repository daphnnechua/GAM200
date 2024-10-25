using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    private List<GeneralDialogue> generalDialogues;
    private List<PlayerResponse> playerResponses;

    [SerializeField] private TextMeshProUGUI dialogueBy;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private GameObject dialogueInterface;
    [SerializeField] List<GameObject> responseButtons;
    [SerializeField] List<TextMeshProUGUI> responseButtonText;

    [SerializeField] private List<GameObject> gameUIs;
    private List<PlayerResponse> responseOptions;
    [SerializeField] private int nextGeneralDialogue;

    public bool dialogueOpen;

    public bool toReturnToStartMenu;
    public string currentSceneName;

    private bool isTutorialInitiationInputCompleted = false;

    private PlayerResponse response;

    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        nextGeneralDialogue = 0;
        
        OpenDialogue();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }

    }


    public void InitializeDialogues()
    {
        responseOptions = new List<PlayerResponse>();
        generalDialogues = Game.GetGeneralDialoguesByScene(currentSceneName);
        playerResponses = Game.GetPlayerResponsesInScene(currentSceneName);
    }

    public void OpenDialogue() //this opens up the dialogue interface
    {
        InitializeDialogues();

        if(gameUIs.Count>0)
        {
            foreach(var e in gameUIs)
            {
                e.SetActive(false);
            }
        }
        foreach (var button in responseButtons)
        {
            button.SetActive(false);
        }
        
        dialogueOpen = true;

        dialogueInterface.SetActive(true);

        dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
        StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));

        // dialogue.text = generalDialogues[nextGeneralDialogue].dialogue;
        nextGeneralDialogue++;


    }


    public void CloseDialogue() //this closes the dialogue interface
    {
        if(currentSceneName == "Tutorial" || currentSceneName == "Mini_Tut_01" || currentSceneName == "Mini_Tut_02")
        {
            dialogueInterface.SetActive(false);

            dialogueOpen = false;
        }
        else
        {
            MasterController masterController = FindObjectOfType<MasterController>();
            if(masterController !=null)
            {
                //implement fade to black first


                masterController.LoadScene(Game.GetLevel().levelName);
            }
        }

    }

    public void NextDialogue() //this handles the dialogue progression
    {
        foreach(var e in responseButtons)
        {
            if(e.activeInHierarchy)
            {
                return;
            }
        }
        if (isTyping) 
        {
            skipTyping = true;
            return;
        }
        if(nextGeneralDialogue < generalDialogues.Count) //check if dialogue is not at the end of list
        {
            if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
            {
                PlayerResponseButton();
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
                return;
            }
            if(response != null)
            {
                if(generalDialogues[nextGeneralDialogue].optionResponseID != response.playerDialogueID && generalDialogues[nextGeneralDialogue].optionResponseID != "null")
                {
                    while(nextGeneralDialogue < generalDialogues.Count && generalDialogues[nextGeneralDialogue].optionResponseID != "null") //iterate to the next dialogue that is not a dialogue that is generated based on player's response after pressing on next button 
                    {
                        nextGeneralDialogue++;
                    }
                }
            }
            if(nextGeneralDialogue>=generalDialogues.Count)
            {
                CloseDialogue();
            }
            else
            {
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
                nextGeneralDialogue++;
            }
            Debug.Log("Current dialogue text: " + generalDialogues[nextGeneralDialogue].dialogue);
        }
        else //if dialogue is at end of list
        {
            CloseDialogue();
        }
    }

    public void PlayerResponseButton() //this handles display of player response options
    {
        responseOptions = Game.GetPlayerResponsesByTriggerID(generalDialogues[nextGeneralDialogue].dialogueID);

        

        for(int i = 0; i<responseOptions.Count; i++) //setting the text for the response options
        {
            int index = i;
            responseButtons[i].SetActive(true);
            responseButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectResponse(index));

            if(responseOptions[i].dialogueType == "Tutorial_Initation")
            {
                responseButtons[i].GetComponentInParent<GridLayoutGroup>().cellSize = new Vector2(450, 85);
                if(responseOptions[i].nextSceneName == "Tutorial" || responseOptions[i].nextSceneName == "Mini_Tut_01" || responseOptions[i].nextSceneName == "Mini_Tut_02")
                {
                    responseButtonText[i].text = responseOptions[i].dialogue + "\n*Tutorial will be initiated*";
                }
                else
                {
                    responseButtonText[i].text = responseOptions[i].dialogue + "\n*Tutorial will be skipped*";
                }
            }
            else
            {
                responseButtons[i].GetComponentInParent<GridLayoutGroup>().cellSize = new Vector2(450, 55);
                responseButtonText[i].text = responseOptions[i].dialogue;
            }
        }
    }

    public void SelectResponse(int index) //handles selection of response
    {
        response = responseOptions[index];

        if(response.dialogueType == "Tutorial_Initation" && !isTutorialInitiationInputCompleted) //set the default weapon according to the character class chosen in the dialogue
        {
            SetNextScene(response);
            GeneralDialogue nextDialogue = Game.GetDialogueByResponseID(response.playerDialogueID)[0]; //first dialogue of this response
            //setting next line of dialogue in response to what was selected 
            dialogueBy.text = nextDialogue.dialogueBy;
            StopAllCoroutines();
            StartCoroutine(TypewriterEffect(nextDialogue.dialogue, dialogue));

            nextGeneralDialogue = generalDialogues.IndexOf(nextDialogue) + 1;            
        }
        else
        {
            GeneralDialogue nextDialogue = Game.GetDialogueByResponseID(response.playerDialogueID)[0]; //first dialogue of this response
            //setting next line of dialogue in response to what was selected 
            dialogueBy.text = nextDialogue.dialogueBy;
            StopAllCoroutines();
            StartCoroutine(TypewriterEffect(nextDialogue.dialogue, dialogue));
            // dialogue.text = nextDialogue.dialogue;
        }

        for(int i = 0; i<responseOptions.Count; i++)
        {
            responseButtons[i].SetActive(false);
        }

        

    }

    public void SetNextScene(PlayerResponse response)
    {
        Levels nextScene = Game.GetLevelByName(response.nextSceneName);
        Game.SetLevel(nextScene);

        Debug.Log($"next scene: {nextScene.levelName}");

    }
    private IEnumerator TypewriterEffect(string dialogueText, TextMeshProUGUI text)
    {
        isTyping = true;
        skipTyping = false;
        
        text.text = ""; 
        foreach (char letter in dialogueText.ToCharArray())
        {
            if (skipTyping) 
            {
                text.text = dialogueText; 
                break;
            }
            text.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        
        isTyping = false; // Typing is done
    }

    #region skip dialogue

    //first prompt player to confirm skipping dialogue
    //then check if the dialogues we're skipping involves tutorial initiation
    //do that by getting player response dialogue type
    //if it involves tutorial initiation, prompt player whether they want to skip dialogue or not
    //if it doesnt, directly fade to black

    //if the dialogue we're skipping is a part of the tutorial, check if what the dialogue is a part of: normal or player response dialogue
    //if normal dialogue, fade to black and skip to where the player is allowed to play the tutorial
    //if player response, skip to the end of the player response dialogue. it should prompt the player to pick dialogue options again

    #endregion skip dialogue

}

