using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public InputHandler inputHandler;

    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        inputHandler = FindObjectOfType<InputHandler>();

        StartGame(); //testing purposes, change to accomodate gameplay flow
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        //player.transform.position = Vector2.zero;
        foreach(PlayerScript playerScript in player.GetComponents<PlayerScript>())
        {
            playerScript.Initialize(this);
        }
        inputHandler.SetInputReceiver(player.GetComponent<PlayerMovement>());

    }
}
