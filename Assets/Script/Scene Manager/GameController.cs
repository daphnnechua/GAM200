using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : SceneController
{
    [SerializeField] private GameObject startGameCountdown;
    public InputHandler inputHandler;
    public GameObject player;
    public bool gameStart = false;

    public int points = 0;

    public int ordersDelivered =0;

    public int ordersFailed =0;
    public bool isGameLoopActive = false;
    public bool isPaused = true;

    public bool levelEnded = false;
    [SerializeField] private DroneMenuController droneMenuController;
    [SerializeField] private OrderManager orderManager;

    [SerializeField] private AudioClip gameplayBGM;
    [SerializeField] private AudioClip endLevelBGM;

    [SerializeField] private GameObject fadeOverlay;

    private float fadeDuration =1f;

    private bool hasLoadEndOfLevelScene  = false;

    //testing purposes
    // public DataManager dataManager;

    public bool toStartGame;

    [SerializeField] private bool hasCoroutineForStartGameBennStarted = false;

    [SerializeField] private AudioClip failedOrderSfx;

    [SerializeField] private AudioClip correctOrderSfx;

    [SerializeField] private AudioClip countdownSound;

    [SerializeField] private List<AudioClip> clickButtonSound;

    [SerializeField] private GameObject endTutorialButton;
    
    // Start is called before the first frame update
    void Start()
    {        
        masterController = FindObjectOfType<MasterController>(); 
        masterController.canPause = true;

        //testing purposes
        // dataManager = FindObjectOfType<DataManager>();
        // dataManager.LoadAllData();

        player = GameObject.FindWithTag("Player");
        inputHandler = FindObjectOfType<InputHandler>();
        droneMenuController = FindObjectOfType<DroneMenuController>();
        orderManager = FindObjectOfType<OrderManager>();

        // StartGame();

        // if(sceneName == "Tutorial" || sceneName == "Mini_Tut_01" || sceneName == "Mini_Tut_02")
        // {
        //     if(dialogueInterface!=null)
        //     {
        //         dialogueInterface.SetActive(false);
        //     }
        // }

        if(endTutorialButton!=null)
        {
            endTutorialButton.GetComponent<Button>().onClick.AddListener(()=>EndTutorial());

            endTutorialButton.SetActive(false);
        }

        if(sceneType == "Normal")
        {
            toStartGame = true;
            startGameCountdown.SetActive(false);
        }
        else if(sceneType == "Tutorial" || sceneType == "Cutscene") //tutorial
        {
            toStartGame = false;
        }

        player.GetComponent<PlayerMovement>().canMove = false;

    }

    // Update is called once per frame
    void Update()
    {

        if(!hasCoroutineForStartGameBennStarted && toStartGame && !gameStart)
        {
            hasCoroutineForStartGameBennStarted = true;
            StartGame();
        }
        
        //debug purposes
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            AddPoints(10);
        }
        if(Input.GetKeyDown(KeyCode.Backslash))
        {
            DeductPoints(10);
        }
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            EndOfLevel();
        }
    }

    public void StartGame()
    {
        StartCoroutine(EnterGame());
    }

    private IEnumerator StartGameCountdown()
    {
        //countdown 3 seconds before game actually starts to give player some time to get ready
        startGameCountdown.SetActive(true); // Show countdown text

        TextMeshProUGUI timerCountdown = startGameCountdown.GetComponentInChildren<TextMeshProUGUI>();

        if(sceneType == "Normal")
        {
            for (int i = 3; i > 0; i--)
            {
                timerCountdown.text = i.ToString();
                SoundFXManager.instance.PlaySound(countdownSound, transform, 1f);
                yield return new WaitForSeconds(1f);
            }

            SoundFXManager.instance.PlaySound(countdownSound, transform, 1f);
            timerCountdown.text = "Start!";
            yield return new WaitForSeconds(1f);
        }
        else if(sceneType == "Tutorial") //tutorial
        {
            SoundFXManager.instance.PlaySound(countdownSound, transform, 1f);
            timerCountdown.text = "Tutorial Start!";
            yield return new WaitForSeconds(1f);
        }

        startGameCountdown.SetActive(false);

        gameStart = true;
        isGameLoopActive = true;
        isPaused = false;

        isGameLoopActive = true;
        isPaused = false;
        //player.transform.position = Vector2.zero;
        foreach(PlayerScript playerScript in player.GetComponents<PlayerScript>())
        {
            playerScript.Initialize(this);
        }
        inputHandler.SetInputReceiver(player.GetComponent<PlayerMovement>());

        if(endTutorialButton!=null)
        {
            endTutorialButton.SetActive(true);
        }
        
        player.GetComponent<PlayerMovement>().canMove = true;

        SoundFXManager.instance.PlayBackgroundMusic(gameplayBGM, 1);
    }

    public void AddPoints(int reward)
    {
        points+=reward;

        PointTracker pointTracker = FindObjectOfType<PointTracker>();
        pointTracker.UpdatePointsUI(points);

        SoundFXManager.instance.PlaySound(correctOrderSfx, transform, 0.5f);

        // Debug.Log("Submitted correct order! Add:" + reward + " current points:" + points);
    }

    public void DeductPoints(int deduct)
    {
        points-=deduct;

        PointTracker pointTracker = FindObjectOfType<PointTracker>();
        pointTracker.UpdatePointsUI(points);

        SoundFXManager.instance.PlaySound(failedOrderSfx, transform, 0.5f);
        // Debug.Log("Submitted wrong order! Deduct:" + deduct + " current points:" + points);

        ordersFailed++;

    }

    private void EndTutorial()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        EndOfLevel();
    }

    public void EndOfLevel()
    {
        if(hasLoadEndOfLevelScene)
        {
            Debug.LogWarning("end of level has already been loaded");
            return;
        }

        masterController.canPause = false;

        levelEnded = true;

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.GetComponent<PlayerMovement>().canMove = false;

        droneMenuController.StopAllProcesses();
        orderManager.StopOrders();

        StartCoroutine(FadeToBlack());

        hasLoadEndOfLevelScene = true;

    }

    private IEnumerator EnterGame()
    {
        float elapsedTime = 0f;
        Color color = fadeOverlay.GetComponent<Image>().color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            fadeOverlay.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 0f;
        fadeOverlay.GetComponent<Image>().color = color;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(StartGameCountdown());

    }

    private IEnumerator FadeToBlack()
    {        
        fadeOverlay.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeOverlay.GetComponent<Image>().color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.GetComponent<Image>().color = color;

        StartCoroutine(SoundFXManager.instance.FadeOutMusic(fadeDuration));
        yield return new WaitForSeconds(fadeDuration);

        masterController.LoadEndOfLevelScene();

        SoundFXManager.instance.StopAmbientSFX();

        SoundFXManager.instance.PlayBackgroundMusic(endLevelBGM, 1);
    }

}
