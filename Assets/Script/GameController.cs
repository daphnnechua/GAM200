using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public InputHandler inputHandler;
    public DataManager dataManager;
    public GameObject player;
    private PointTracker pointTracker;

    public bool gameStart = false;

    public int points = 0;
    public bool isGameLoopActive = false;
    public bool isPaused = true;
    
    // Start is called before the first frame update
    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        dataManager.LoadAllData();
        player = GameObject.FindWithTag("Player");
        inputHandler = FindObjectOfType<InputHandler>();
        pointTracker = FindObjectOfType<PointTracker>();

        StartGame(); //testing purposes, change to accomodate gameplay flow
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
