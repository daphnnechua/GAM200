using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
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

    [SerializeField] private GameObject tutorialManual;
    private List<PlayerResponse> responseOptions;
    [SerializeField] private int nextGeneralDialogue;

    [SerializeField] private Button skipDialogueButton;

    [SerializeField] private GameObject skipDialoguePrompt;
    [SerializeField] private Button confirmSkip;
    [SerializeField] private Button declineSkip;
    [SerializeField] private GameObject tutorialPrompt;
    [SerializeField] private Button confirmTutorial;
    [SerializeField] private Button declineTutorial;

    [SerializeField] private Image fadeToBlack;

    public bool dialogueOpen;

    public bool toReturnToStartMenu;
    public string currentSceneName;

    private bool isTutorialInitiationInputCompleted = false;

    private PlayerResponse response;

    private MasterController masterController;

    private bool isTyping = false;
    private bool skipTyping = false;

    private float fadeToBlackDuration = 1.5f;

    private bool canInteract = true;


    void Start()
    {
        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = true;

        nextGeneralDialogue = 0;
        
        OpenDialogue();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canInteract|| Input.GetMouseButtonDown(0) && canInteract)
        {
            NextDialogue();

        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            dialogueInterface.SetActive(true);
            InitializeDialogues();

            skipDialogueButton.onClick.AddListener(() => OpenSkipDialoguePrompt());
            declineSkip.onClick.AddListener(() => CloseSkipDialoguePrompt());
            confirmSkip.onClick.AddListener(() => SkipDialogue());

            foreach (var button in responseButtons)
            {
                button.SetActive(false);
            }

            tutorialManual.SetActive(false);
            
            dialogueOpen = true;

            dialogueInterface.SetActive(true);
            skipDialoguePrompt.SetActive(false);

            if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
            {
                PlayerResponseButton();
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
            }

            nextGeneralDialogue++;
        }
    }


    public void InitializeDialogues()
    {
        responseOptions = new List<PlayerResponse>();
        generalDialogues = Game.GetGeneralDialoguesByScene(currentSceneName);
        playerResponses = Game.GetPlayerResponsesInScene(currentSceneName);
    }

    public void ReOpenDialogue() //for interaction with npc
    {
        dialogueInterface.SetActive(true);
        InitializeDialogues();

        skipDialogueButton.onClick.AddListener(() => OpenSkipDialoguePrompt());
        declineSkip.onClick.AddListener(() => CloseSkipDialoguePrompt());
        confirmSkip.onClick.AddListener(() => SkipDialogue());

        confirmTutorial.onClick.AddListener(()=> OpenTutorialForNextScene());
        declineTutorial.onClick.AddListener(()=> OpenLevelForNextScene());


        foreach (var button in responseButtons)
        {
            button.SetActive(false);
        }

        tutorialManual.SetActive(false);
        tutorialPrompt.SetActive(false);

            
        dialogueOpen = true;

        dialogueInterface.SetActive(true);
        skipDialoguePrompt.SetActive(false);

        if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
        {
            PlayerResponseButton();
            dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
            StopAllCoroutines();
            StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
        }

        nextGeneralDialogue++;

    }

    public void OpenDialogue() //this opens up the dialogue interface
    {
        InitializeDialogues();

        skipDialogueButton.onClick.RemoveAllListeners();
        declineSkip.onClick.RemoveAllListeners();
        confirmSkip.onClick.RemoveAllListeners();

        skipDialogueButton.onClick.AddListener(() => OpenSkipDialoguePrompt());
        declineSkip.onClick.AddListener(() => CloseSkipDialoguePrompt());
        confirmSkip.onClick.AddListener(() => SkipDialogue());

        confirmTutorial.onClick.AddListener(()=> OpenTutorialForNextScene());
        declineTutorial.onClick.AddListener(()=> OpenLevelForNextScene());

        foreach (var button in responseButtons)
        {
            button.SetActive(false);
        }

        tutorialManual.SetActive(false);

        tutorialPrompt.SetActive(false);
        
        dialogueOpen = true;

        dialogueInterface.SetActive(true);
        skipDialoguePrompt.SetActive(false);

        // tutorialPrompt.SetActive(false);

        dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
        StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
        ToggleTutorialManual();

        // dialogue.text = generalDialogues[nextGeneralDialogue].dialogue;
        nextGeneralDialogue++;

    }

    private void ToggleTutorialManual()
    {
        
        if(generalDialogues[nextGeneralDialogue].tutorialImage != "null")
        {
            tutorialManual.SetActive(true);
        }
        else if(generalDialogues[nextGeneralDialogue].tutorialImage == "null")
        {
            tutorialManual.SetActive(false);
        }
        
    }

    public void CloseDialogue() //this closes the dialogue interface
    {
        if(currentSceneName == "Tutorial" || currentSceneName == "Mini_Tut_01" || currentSceneName == "Mini_Tut_02") //meant to be called in tutorial levels
        {
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P007") //close npc dialogue in tutorial
            {
                Debug.Log("closed npc dialogue in tutorial");
                for(int i =0; i<generalDialogues.Count; i++)
                {
                    if(generalDialogues[i].repeatDialogue)
                    {
                        nextGeneralDialogue = generalDialogues.IndexOf(generalDialogues[i]); //set opening line to the tutorial type prompt
                        break;
                    }
                }

                dialogueInterface.SetActive(false);
            }
            
            else if (generalDialogues[nextGeneralDialogue-1].optionResponseID == "P008")//player chooses to close tutorial
            {
                StartCoroutine(FadeToBlack());
            }

            else if(generalDialogues[nextGeneralDialogue].optionResponseID != "null" && generalDialogues[nextGeneralDialogue].optionResponseID != "P008") //tutorial explanation
            {
                //skip to where the npc prompts the player if there is something else they want to ask
                skipDialoguePrompt.SetActive(false);

                string temp = generalDialogues[nextGeneralDialogue].optionResponseID;

                while(!generalDialogues[nextGeneralDialogue].isDialogueSelection && generalDialogues[nextGeneralDialogue].optionResponseID == temp)
                {
                    nextGeneralDialogue++;
                }

                StopAllCoroutines();
                PlayerResponseButton();
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));

            }
            
            else if(generalDialogues[nextGeneralDialogue].optionResponseID == "null") //normal dialogue in tutorial
            {
                for(int i =0; i<generalDialogues.Count; i++)
                {
                    if(generalDialogues[i].repeatDialogue)
                    {
                        nextGeneralDialogue = generalDialogues.IndexOf(generalDialogues[i]); //set opening line to the tutorial type prompt
                        break;
                    }
                }
                
                dialogueInterface.SetActive(false);

            }
        }
        else
        {
            StartCoroutine(FadeToBlack());
        }

        canInteract = true;
    }

    public void NextDialogue() //this handles the dialogue progression
    {
        
        if (isTyping) 
        {
            skipTyping = true;
            return;
        }
        
        foreach(var e in responseButtons)
        {
            if(e.activeInHierarchy)
            {
                return;
            }
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
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P007" && generalDialogues[nextGeneralDialogue].optionResponseID != "P007")
            {
                Debug.Log("closing dialogue");
                CloseDialogue();
            }
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P008" && generalDialogues[nextGeneralDialogue].optionResponseID != "P008")
            {
                Debug.Log("close tutorial");
                CloseDialogue();
            }
            else
            {
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
                
                nextGeneralDialogue++;
            }
        }
        else 
        {
            canInteract = false;
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

        if(response.dialogueType == "Tutorial_Initation" && !isTutorialInitiationInputCompleted)
        {
            SetNextScene(response);
            GeneralDialogue nextDialogue = Game.GetDialogueByResponseID(response.playerDialogueID)[0]; //first dialogue of this response
            //setting next line of dialogue in response to what was selected 
            nextGeneralDialogue = generalDialogues.IndexOf(nextDialogue);
            dialogueBy.text = nextDialogue.dialogueBy;
            StopAllCoroutines();
            StartCoroutine(TypewriterEffect(nextDialogue.dialogue, dialogue));

            nextGeneralDialogue = generalDialogues.IndexOf(nextDialogue) + 1;        
        }
        else
        {
            GeneralDialogue nextDialogue = Game.GetDialogueByResponseID(response.playerDialogueID)[0]; //first dialogue of this response
            //setting next line of dialogue in response to what was selected 
            nextGeneralDialogue = generalDialogues.IndexOf(nextDialogue);
            dialogueBy.text = nextDialogue.dialogueBy;
            StopAllCoroutines();
            StartCoroutine(TypewriterEffect(nextDialogue.dialogue, dialogue));

            // dialogue.text = nextDialogue.dialogue;

            nextGeneralDialogue = generalDialogues.IndexOf(nextDialogue) + 1;        
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

    }
    private IEnumerator TypewriterEffect(string dialogueText, TextMeshProUGUI text)
    {
        ToggleTutorialManual();
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

    private IEnumerator FadeToBlack()
    {
        Debug.Log("fading to black...");
        fadeToBlack.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeToBlack.color;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeToBlackDuration);
            fadeToBlack.color = color;
            yield return null;
        }

        Debug.Log("fade to black completed");
        color.a = 1f; // Ensure it’s fully opaque at the end
        fadeToBlack.color = color;

        MasterController masterController = FindObjectOfType<MasterController>();
        if(masterController !=null)
        {
            masterController.LoadScene(Game.GetLevel().levelName);
        }

        //after completing, load in the next scene
        
    }

    #region skip dialogue

    //first prompt player to confirm skipping dialogue
    //then check if the dialogues we're skipping involves tutorial initiation
    //do that by getting player response dialogue type
    //if it involves tutorial initiation, prompt player whether they want to skip tutorial or not
    //if it doesnt, directly fade to black

    //if the dialogue we're skipping is a part of the tutorial, check if what the dialogue is a part of: normal or player response dialogue
    //if normal dialogue, directly skip to where the player is allowed to play the tutorial --> call closedialogue
    //if player response, skip to the end of the player response dialogue. it should prompt the player to pick dialogue options again

    //tutorial prompt: 
    // 1. You're about to start a tutorial.
    // 2. Would you like to play the tutorial?
    // 3. Tutorial contents: --> csv
    // 4. Yes/No options

    private void OpenSkipDialoguePrompt()
    {
        masterController.canPause = false;

        skipDialoguePrompt.SetActive(true);
        canInteract = false;
    }

    private void CloseSkipDialoguePrompt()
    {
        masterController.canPause = true; 

        skipDialoguePrompt.SetActive(false);
        canInteract = true;
    }

    private void OpenTutorialForNextScene()
    {
        Levels currentLevel = Game.GetLevelByName(currentSceneName);
        List<Levels> allLevels = Game.GetLevelList();

        int levelIndex = allLevels.IndexOf(currentLevel);

        for(int i = levelIndex; i<allLevels.Count; i++)
        {
            if(allLevels[i].levelType == "tutorial")
            {
                Game.SetLevel(allLevels[i]);
                break;
            }
        }
        Debug.Log($"next level is: {Game.GetLevel().levelName}");
        CloseDialogue();
    }

    private void OpenLevelForNextScene()
    {
        Levels currentLevel = Game.GetLevelByName(currentSceneName);
        List<Levels> allLevels = Game.GetLevelList();

        int levelIndex = allLevels.IndexOf(currentLevel);

        for(int i = levelIndex; i<allLevels.Count; i++)
        {
            if(allLevels[i].levelType == "normal")
            {
                Game.SetLevel(allLevels[i]);
                break;
            }
        }
        Debug.Log($"next level is: {Game.GetLevel().levelName}");
        CloseDialogue();
    }

    private void OpenTutorialPrompt()
    {
        tutorialPrompt.SetActive(true);
        canInteract = false;
    }

    private void SkipDialogue() //confirm that player wants to skip the dialogue --> check if the next scene is tutorial
    {

        bool openTutorialPrompt = false;

        List<PlayerResponse> allPlayerResponseInScene = Game.GetPlayerResponsesInScene(currentSceneName);

        for(int i =0; i<allPlayerResponseInScene.Count;i++)
        {
            if(allPlayerResponseInScene[i].dialogueType == "Tutorial_Initation")
            {
                openTutorialPrompt = true;
            }
        }
        if(openTutorialPrompt)
        {
            skipDialoguePrompt.SetActive(false);
            OpenTutorialPrompt();
        }
        else
        {
            CloseDialogue();
        }
        
    }

    #endregion skip dialogue

}

