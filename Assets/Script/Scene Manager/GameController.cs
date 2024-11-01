using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : SceneController
{
    [SerializeField] private GameObject startGameCountdown;
    public InputHandler inputHandler;
    public GameObject player;
    private PointTracker pointTracker;

    public bool gameStart = false;

    public int points = 0;
    public bool isGameLoopActive = false;
    public bool isPaused = true;

    public bool levelEnded = false;
    private DroneMenuController droneMenuController;
    private OrderManager orderManager;

    //testing purposes
    public DataManager dataManager;

    public bool toStartGame;
    
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
        pointTracker = FindObjectOfType<PointTracker>();
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

        // if(sceneType == "Normal")
        // {
        //     toStartGame = true;
        // }
        // else if(sceneType == "Tutorial" || sceneType == "Cutscene") //tutorial
        // {
        //     toStartGame = false;
        // }

    }

    // Update is called once per frame
    void Update()
    {

        // if(toStartGame && !gameStart)
        // {
        //     StartGame();
        //     gameStart = true;
        // }
        
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCountdown());
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
                yield return new WaitForSeconds(1f);
            }

            timerCountdown.text = "Start!";
            yield return new WaitForSeconds(1f);
        }
        else if(sceneType == "Tutorial") //tutorial
        {
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

    }

    public void AddPoints(int reward)
    {
        points+=reward;
        pointTracker.UpdatePointsUI(points);
        // Debug.Log("Submitted correct order! Add:" + reward + " current points:" + points);
    }

    public void DeductPoints(int deduct)
    {
        points-=deduct;
        pointTracker.UpdatePointsUI(points);
        // Debug.Log("Submitted wrong order! Deduct:" + deduct + " current points:" + points);

    }

    public void EndOfLevel()
    {
        Debug.Log("Level has ended");

        masterController.canPause = false;

        levelEnded = true;

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.GetComponent<PlayerMovement>().canMove = false;

        droneMenuController.StopAllProcesses();
        orderManager.StopOrders();

        masterController.LoadEndOfLevelScene();

    }

}
