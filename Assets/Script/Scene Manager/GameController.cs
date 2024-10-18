using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SceneController
{
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
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        inputHandler = FindObjectOfType<InputHandler>();
        pointTracker = FindObjectOfType<PointTracker>();
        droneMenuController = FindObjectOfType<DroneMenuController>();
        orderManager = FindObjectOfType<OrderManager>();

        StartGame(); //testing purposes, change to accomodate gameplay flow

    }

    // Update is called once per frame
    void Update()
    {
        //debug purposes
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            points += 20;
            pointTracker.UpdatePointsUI(points);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            EndOfLevel();
        }
    }

    public void StartGame()
    {
        gameStart = true;
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
        levelEnded = true;

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PickUpObjs>().enabled = false;

        droneMenuController.StopAllProcesses();
        orderManager.StopOrders();

        MasterController masterController = FindObjectOfType<MasterController>(); 
        masterController.LoadEndOfLevelScene();
    }

}
