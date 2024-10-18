using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    public float timeLeft = 150f; //1.5 mins

    [SerializeField] GameController gameController;
    [SerializeField] private TextMeshProUGUI timeText;

    public string endText;
    private bool isTimerFinished = false;

    private DroneMenuController droneMenuController;
    private OrderManager orderManager;

    private MasterController masterController;
    
    // Start is called before the first frame update
    void Start()
    {
        droneMenuController = FindObjectOfType<DroneMenuController>();
        orderManager = FindObjectOfType<OrderManager>();
        gameController = FindObjectOfType<GameController>();
        timeText = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.gameStart)
        {
            if(timeLeft>0 && !isTimerFinished)
            {
                timeLeft-= Time.deltaTime; //total time left in seconds
                timeText.text = Timer(timeLeft); //set timer text
                if(timeLeft<6)
                {
                    timeText.color = Color.red;
                }

            }
            else if(timeLeft<= 0 && !isTimerFinished)
            {
                timeLeft = 0;
                timeText.text = Timer(timeLeft); //set timer text
                gameController.EndOfLevel(); //testing only. to remove
                isTimerFinished = true;
            }
            

        }

    }

    private string Timer(float time) //handle timer text
    {
        int minutes = Mathf.FloorToInt(time/60); //get minutes passed
        int seconds = Mathf.FloorToInt(time%60); //get seconds passed
        if(minutes<10)
        {
            if(seconds<10)
            {
                endText = "0"+ minutes + ":" + "0" + seconds;
            }
            else
            {
                endText = "0"+ minutes + ":" + seconds;
            }
            
        }
        else
        {
            if(seconds<10)
            {
                endText = minutes + ":" + "0" + seconds;
            }
            else
            {
                endText = minutes + ":" + seconds;
            }
            
        }


        return endText;
    }
    
}