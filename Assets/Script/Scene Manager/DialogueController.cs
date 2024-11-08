using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] private Button puddlesButton;

    [SerializeField] private Button tutorialManaul;


    [SerializeField] private TextMeshProUGUI skipTutorialText;

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

    [SerializeField] private Image leftSpeaker;
    [SerializeField] private Image rightSpeaker;

    [SerializeField] private GameObject gameCountdownTimer;
    [SerializeField] private GameObject pointsPanel;

    [SerializeField] private string levelType;

    [SerializeField] private GameObject droneMenu;

    [SerializeField] private GameObject tutorialManualInterface;

    public bool dialogueOpen;

    public bool toReturnToStartMenu;
    public string currentSceneName;

    private bool isTutorialInitiationInputCompleted = false;

    private PlayerResponse response;

    private MasterController masterController;

    private bool isTyping = false;
    private bool skipTyping = false;

    private float fadeToBlackDuration = 1f;

    public bool canInteract = true;

    private bool endOfDialogue = false;

    [SerializeField] private List<AudioClip> clickButtonSound;

    private GameObject player;
    void Start()
    {   
        player = GameObject.FindWithTag("Player");

        if(levelType!="Normal")
        {
            if(gameCountdownTimer!=null)
            {
                gameCountdownTimer.SetActive(false);
            }

            if(pointsPanel!=null)
            {
                pointsPanel.SetActive(false);
            }

            if(puddlesButton!=null)
            {
                puddlesButton.onClick.AddListener(()=>ReOpenDialogue());
                puddlesButton.gameObject.SetActive(false);
            }

            OpenDialogue();
        }
        else
        {
            dialogueInterface.SetActive(false);

            if(puddlesButton!=null)
            {
                puddlesButton.gameObject.SetActive(false);

            }
            
            // Debug.Log(puddlesButton.gameObject.activeInHierarchy);

            dialogueOpen = false;

        }

    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && canInteract && dialogueInterface.activeInHierarchy)
        {
            // Debug.Log("Spacebar pressed, advancing dialogue.");
            EventSystem .current.SetSelectedGameObject(null);
            NextDialogue();
        }

        if (Input.GetMouseButtonDown(0) && canInteract && dialogueInterface.activeInHierarchy)
        {
            EventSystem .current.SetSelectedGameObject(null);
            NextDialogue();
        }

        GameController gameController =FindObjectOfType<GameController>();
        if(levelType == "Tutorial")
        {
            if(!dialogueInterface.activeInHierarchy)
            {
                dialogueOpen=false;

                if(gameController!=null && gameController.gameStart)
                {
                    if(puddlesButton !=null)
                    {
                        puddlesButton.gameObject.SetActive(true);
                    }
                    if(tutorialManaul!=null)
                    {
                        tutorialManaul.gameObject.SetActive(true);
                    }
                    
                }
            }
            else
            {
                dialogueOpen=true;
                
                if(puddlesButton!=null)
                {
                    puddlesButton.gameObject.SetActive(false);
                }
                if(tutorialManaul!=null)
                {
                    tutorialManaul.gameObject.SetActive(false);
                }
            }
        }

        if(puddlesButton!=null)
        {
            if(puddlesButton.gameObject.activeInHierarchy && !dialogueInterface.activeInHierarchy)
            {
                if(!droneMenu.activeInHierarchy && !tutorialManualInterface.activeInHierarchy)
                {
                    if(Input.GetKeyDown(KeyCode.T))
                    {
                        ReOpenDialogue();
                    }
                }
            }
        }

    }


    public void InitializeDialogues()
    {
        responseOptions = new List<PlayerResponse>();
        generalDialogues = Game.GetGeneralDialoguesByScene(currentSceneName);
        playerResponses = Game.GetPlayerResponsesInScene(currentSceneName);


        // for(int i =0; i<generalDialogues.Count;i++)
        // {
        //     Debug.Log(generalDialogues[i].dialogue);
        // }
    }

    public void ReOpenDialogue() //for interaction with npc
    {
        if(!droneMenu.activeInHierarchy && !tutorialManualInterface.activeInHierarchy)
        {
            if(player!=null)
            {
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                player.GetComponent<PlayerMovement>().canMove = false;
            }
            int random = Random.Range(0, clickButtonSound.Count);
            SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

            masterController = FindObjectOfType<MasterController>();
            masterController.canPause = true;
            // the whole game

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
            
            dialogueInterface.SetActive(true);
            skipDialoguePrompt.SetActive(false);


            skipDialogueButton.gameObject.SetActive(false); //set to active at specific triggers

            if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
            {
                
                PlayerResponseButton();
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
            }
        }

    }

    public void OpenDialogue()
    {
        if(player!=null)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            player.GetComponent<PlayerMovement>().canMove = false;
        }

        StartCoroutine(FadeIn());
    }

    public IEnumerator InitializeStartDialogue() //this opens up the dialogue interface
    {
        
        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = true;

        if(currentSceneName!="cutscene_04")
        {
            Levels nextLevel = Game.GetNextLevel(currentSceneName);
            string levelDetails = nextLevel.description;
            skipTutorialText.text = $"Would you like to play the tutorial? \n\nTutorial contents: \n{levelDetails}";
        }

        //tutorial prompt: 
        // 1. You're about to start a tutorial.
        // 2. Would you like to play the tutorial?
        // 3. Tutorial contents: --> csv
        // 4. Yes/No options


        nextGeneralDialogue = 0;

        InitializeDialogues();

        Debug.Log(generalDialogues[nextGeneralDialogue].dialogue);

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
        
        dialogueInterface.SetActive(true);
        skipDialoguePrompt.SetActive(false);

        // tutorialPrompt.SetActive(false);

        
        if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
        {
            PlayerResponseButton();
        }

        dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
        StopAllCoroutines();
        StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));

        // dialogue.text = generalDialogues[nextGeneralDialogue].dialogue;
        nextGeneralDialogue++;

        yield return null;
    }

    private void ToggleTutorialManual()
    {
        
        if(generalDialogues[nextGeneralDialogue].tutorialImage != "null")
        {
            SetTutorialImage();
            tutorialManual.SetActive(true);
        }
        else if(generalDialogues[nextGeneralDialogue].tutorialImage == "null")
        {
            tutorialManual.SetActive(false);
        }
        
    }

    private void SetTutorialImage()
    {
        Image tutorialImage = tutorialManual.GetComponentInChildren<Image>();
        if(generalDialogues[nextGeneralDialogue].optionResponseID!="null")
        {
            RectTransform rt = tutorialImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(948, 585);
            SetImage(generalDialogues[nextGeneralDialogue].tutorialImage, tutorialImage);
        }
        else if(generalDialogues[nextGeneralDialogue].optionResponseID=="null")
        {
            RectTransform rt = tutorialImage.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(439, 439);
            SetImage(generalDialogues[nextGeneralDialogue].tutorialImage, tutorialImage);
        }
    }

    public void CloseDialogue() //this closes the dialogue interface
    {
        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = true;

        if(currentSceneName == "Tutorial" || currentSceneName == "Mini_Tut_01" || currentSceneName == "Mini_Tut_02") //meant to be called in tutorial levels
        {
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P007") //close npc dialogue in tutorial
            {
                // Debug.Log($"current dialogue is to close the interface.");
                for(int i =0; i<generalDialogues.Count; i++)
                {
                    if(generalDialogues[i].repeatDialogue)
                    {
                        nextGeneralDialogue = generalDialogues.IndexOf(generalDialogues[i]); //set opening line to the tutorial type prompt
                        break;
                    }
                }
                dialogueInterface.SetActive(false);
                // Debug.Log("interface closed");
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

        //start game upon close --> mostly applicable to tutorial as player has to talk to npc again to trigger dialogue 
        gameController = FindObjectOfType<GameController>();
        if(gameController!=null && !gameController.gameStart)
        {
            gameController.toStartGame = true; //start the game
        }
        else if(gameController!=null && gameController.gameStart) //game has already been started
        {
            Time.timeScale = 1f; //resume game upon closing dialogue
        }
        canInteract = true;

        if(pointsPanel!=null)
        {
            pointsPanel.SetActive(true);
        }

        if(player!=null)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<PlayerMovement>().canMove = true;
        }
    }

    public void NextDialogue() //this handles the dialogue progression
    {

        foreach (var e in responseButtons)
        {
            // Debug.Log($"Button: {e.name}, Active in hierarchy: {e.activeInHierarchy}");
            if (e.activeInHierarchy)
            {
                // Debug.Log("cannot advance dialogue");
                return;
            }
        }
        if (isTyping) 
        {
            skipTyping = true;
            return;
        }
        
        ToggleSkipButton();

        if(nextGeneralDialogue < generalDialogues.Count) //check if dialogue is not at the end of list
        {
            if(generalDialogues[nextGeneralDialogue].repeatDialogue)
            {
                // Debug.Log("current dialogue end. reopen dialogue to continue");
                CloseDialogue();
                return;
            }

            else if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
            {
                PlayerResponseButton();
                dialogueBy.text = generalDialogues[nextGeneralDialogue].dialogueBy;
                StopAllCoroutines();
                StartCoroutine(TypewriterEffect(generalDialogues[nextGeneralDialogue].dialogue, dialogue));
                return;
            }

            if(generalDialogues[nextGeneralDialogue-1].toCloseDialogue && !generalDialogues[nextGeneralDialogue].toCloseDialogue)
            {                
                CloseDialogue();
                return;
            }
            
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P007" && generalDialogues[nextGeneralDialogue].optionResponseID != "P007")
            {
                CloseDialogue();
                return;
            }
            if(generalDialogues[nextGeneralDialogue-1].optionResponseID == "P008" && generalDialogues[nextGeneralDialogue].optionResponseID != "P008")
            {
                CloseDialogue();
                return;
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

    private void ToggleSkipButton()
    {
        if(response!=null && nextGeneralDialogue + 1 < generalDialogues.Count)
        {
            if(generalDialogues[nextGeneralDialogue + 1].optionResponseID == response.playerDialogueID)
            {
                if(response.dialogueType == "Tutorial_Type")
                {
                    skipDialogueButton.gameObject.SetActive(true);
                }
                if(generalDialogues[nextGeneralDialogue].isDialogueSelection)
                {
                    skipDialogueButton.gameObject.SetActive(false);
                }
            }
            else
            {
                skipDialogueButton.gameObject.SetActive(false);
            }
        }
    }

    public void PlayerResponseButton() //this handles display of player response options
    {
        responseOptions = Game.GetPlayerResponsesByTriggerID(generalDialogues[nextGeneralDialogue].dialogueID);

        List<PlayerResponse> questionPrompts = new List<PlayerResponse>();
        PlayerResponse P007 = null;
        PlayerResponse P008 = null;

        for (int i = 0; i < responseOptions.Count; i++)
        {
            if (responseOptions[i].playerDialogueID == "P007")
            {
                P007 = responseOptions[i];

                Debug.Log(P007.playerDialogueID);
                Debug.Log(Game.GetDialogueByResponseID(P007.playerDialogueID)[0].dialogue);
            }
            else if (responseOptions[i].playerDialogueID == "P008")
            {
                P008 = responseOptions[i];
            }
            else
            {
                questionPrompts.Add(responseOptions[i]);
            }
        }

        responseOptions.Clear();
        responseOptions = questionPrompts;

        if(P007!=null)
        {
            responseOptions.Add(P007);
        }
        if(P008!=null)
        {
            responseOptions.Add(P008);
        }

        for(int i =0; i<responseOptions.Count; i++)
        {
            int index = i;
            responseButtons[i].SetActive(true);
            responseButtons[i].GetComponent<Button>().onClick.AddListener(() => SelectResponse(index));

            Debug.Log(responseOptions[i].dialogue);

            if (responseOptions[i].dialogueType == "Tutorial_Initation")
            {
                responseButtons[i].GetComponentInParent<GridLayoutGroup>().cellSize = new Vector2(450, 85);
                if (responseOptions[i].nextSceneName == "Tutorial" || responseOptions[i].nextSceneName == "Mini_Tut_01" || responseOptions[i].nextSceneName == "Mini_Tut_02")
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
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        skipDialogueButton.gameObject.SetActive(true);

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
        SetSpeakerImage();
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

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(InitializeStartDialogue());

        float elapsedTime = 0f;
        Color color = fadeToBlack.color;

        while (elapsedTime < fadeToBlackDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / fadeToBlackDuration));
            fadeToBlack.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeToBlack.color = color;
        
    }

    private IEnumerator FadeToBlack()
    {
        if(endOfDialogue)
        {
            // Debug.LogWarning("fadeToBlack to black has already been triggered");
            yield break;
        }
        
        endOfDialogue = true;

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

        color.a = 1f;
        fadeToBlack.color = color;

        if(generalDialogues[nextGeneralDialogue-1].sceneName == "cutscene_04")
        {
            masterController.QuitToStart();
        }

        else if(generalDialogues[nextGeneralDialogue-1].optionResponseID != "P008")
        {
            MasterController masterController = FindObjectOfType<MasterController>();
            if(masterController !=null)
            {
                Levels levelToLoad = Game.GetLevel();

                if(levelToLoad != null) //the player has gone through tutorial prompt before
                {
                    int indexOfToLoad = Game.GetLevelList().IndexOf(levelToLoad);

                    Levels currentLevel = Game.GetLevelByName(currentSceneName);
                    int indexOfCurrentLevel = Game.GetLevelList().IndexOf(currentLevel);

                    if(indexOfToLoad <= indexOfCurrentLevel) //this cutscene did not set a new level to load
                    {
                        Levels nextLevel = Game.GetLevelList()[indexOfCurrentLevel+1]; //load the level after this cutscene
                        masterController.LoadScene(nextLevel.levelName);

                        // Debug.Log($"this cutscene did not set a new level to load. loading: {nextLevel.levelName}");
                    }
                    else
                    {
                        masterController.LoadScene(levelToLoad.levelName); //cutscene did set a new level to load --> tutorial
                        // Debug.Log($"cutscene did set a new level to load. loading: {levelToLoad.levelName}");
                    }
                }
                else //player has not gone through tutorial prompt
                {
                    Levels currentLevel = Game.GetLevelByName(currentSceneName);
                    int indexOfCurrentLevel = Game.GetLevelList().IndexOf(currentLevel);
                    Levels nextLevel = Game.GetLevelList()[indexOfCurrentLevel+1];
                    masterController.LoadScene(nextLevel.levelName);

                    // Debug.Log($"loading: {nextLevel.levelName}");
                }
            }
        }
        else //end of tutorial --> go to level completion page 
        {
            // MasterController masterController = FindObjectOfType<MasterController>();
            // masterController.LoadEndOfLevelScene();

            GameController gameController =FindObjectOfType<GameController>();
            if(gameController!=null)
            {
                gameController.EndOfLevel();
            }
        }


        //after completing, load in the next scene
        
    }

    #region skip dialogue
    private void OpenSkipDialoguePrompt()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        masterController.canPause = false;

        skipDialoguePrompt.SetActive(true);
        canInteract = false;
    }

    private void CloseSkipDialoguePrompt()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        masterController.canPause = true; 

        skipDialoguePrompt.SetActive(false);
        canInteract = true;
    }

    private void OpenTutorialForNextScene()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        Levels currentLevel = Game.GetLevelByName(currentSceneName);
        List<Levels> allLevels = Game.GetLevelList();

        int levelIndex = allLevels.IndexOf(currentLevel);

        for(int i = levelIndex; i<allLevels.Count; i++)
        {
            if(allLevels[i].levelType == "Tutorial")
            {
                Game.SetLevel(allLevels[i]);
                break;
            }
        }
        // Debug.Log($"next level is: {Game.GetLevel().levelName}");
        CloseDialogue();
    }

    private void OpenLevelForNextScene()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        Levels currentLevel = Game.GetLevelByName(currentSceneName);
        List<Levels> allLevels = Game.GetLevelList();

        int levelIndex = allLevels.IndexOf(currentLevel);

        for(int i = levelIndex; i<allLevels.Count; i++)
        {
            if(allLevels[i].levelType == "Normal")
            {
                Game.SetLevel(allLevels[i]);
                break;
            }
        }
        // Debug.Log($"next level is: {Game.GetLevel().levelName}");
        CloseDialogue();
    }

    private void OpenTutorialPrompt()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = false;
        tutorialPrompt.SetActive(true);
        canInteract = false;
    }

    private void SkipDialogue() //confirm that player wants to skip the dialogue --> check if the next scene is tutorial
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

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

    #region set speaker image

    private void SetSpeakerImage()
    {
        ToggleSpeakerVisibility();
        ToggleSpeakerFocus();
    }

    private void ToggleSpeakerFocus()
    {
        if(generalDialogues[nextGeneralDialogue].isLeftSpeaker)
        {
            leftSpeaker.color = Color.white;
            rightSpeaker.color = Color.gray;

        }
        else 
        {
            leftSpeaker.color = Color.gray;
            rightSpeaker.color = Color.white;
        }
    }

    private void ToggleSpeakerVisibility()
    {
        if(generalDialogues[nextGeneralDialogue].rightSpriteFilePath == "null")
        {
            rightSpeaker.gameObject.SetActive(false);
        }
        else
        {
            rightSpeaker.gameObject.SetActive(true);
            SetImage(generalDialogues[nextGeneralDialogue].rightSpriteFilePath, rightSpeaker);
        }

        if(generalDialogues[nextGeneralDialogue].leftSpriteFilePath == "null")
        {
            leftSpeaker.gameObject.SetActive(false);
        }
        else
        {
            leftSpeaker.gameObject.SetActive(true);
            SetImage(generalDialogues[nextGeneralDialogue].leftSpriteFilePath, leftSpeaker);
        }
    }
    
    private void SetImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    #endregion set speaker image

}

