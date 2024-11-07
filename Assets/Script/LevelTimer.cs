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

    [SerializeField] private GameObject timerDisplay;

    private GameObject player;

    public string endText;
    private bool isTimerFinished = false;

    private DroneMenuController droneMenuController;
    private OrderManager orderManager;

    private MasterController masterController;

    private bool hasFadeOutStarted = false;

    private bool hasLevelEndBeenInitiated = false;

    [SerializeField] private AudioClip timerRunningOutSound;

    [SerializeField] private AudioClip outOfTimeSound;

    [SerializeField] private AudioClip countdownSound;

    private bool hasTimerWarningBeenPlayed = false;
    private bool hasCountdownStarted = false;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        droneMenuController = FindObjectOfType<DroneMenuController>();
        orderManager = FindObjectOfType<OrderManager>();
        gameController = FindObjectOfType<GameController>();
        timeText = GetComponent<TextMeshProUGUI>();

        if(gameController.sceneType == "Tutorial" || gameController.sceneType == "Cutscene")
        {
            timerDisplay.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            timeLeft -= 10;
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            timeLeft +=10;
        }


        if(timerDisplay.activeInHierarchy && gameController.gameStart)
        {
            if(timeLeft>0 && !isTimerFinished)
            {
                timeLeft-= Time.deltaTime; //total time left in seconds

                if (timeLeft < 0) 
                {
                    timeLeft = 0;
                }

                timeText.text = Timer(timeLeft); //set timer text

                if(timeLeft<30 && !hasTimerWarningBeenPlayed)
                {
                    SoundFXManager.instance.PlaySound(timerRunningOutSound, transform, 1f);
                    hasTimerWarningBeenPlayed = true;
                }

                if(timeLeft<10 && !hasCountdownStarted)
                {
                    StartCoroutine(Countdown());
                    hasCountdownStarted = true;
                }

                if(timeLeft<5)
                {
                    timeText.color = Color.red;

                    if(!hasFadeOutStarted && timeLeft<=1)
                    {
                        StartCoroutine(SoundFXManager.instance.FadeOutMusic(1));
                        hasFadeOutStarted = true;
                    }
                }
                

            }
            else
            {
                if(!hasLevelEndBeenInitiated)
                {
                    StartCoroutine(GameOver());

                    timeText.text = Timer(0); //set timer text                    
                    isTimerFinished = true;
                    hasLevelEndBeenInitiated = true;
                }
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
    
    private IEnumerator GameOver()
    {
        droneMenuController.StopAllProcesses();
        orderManager.StopOrders();

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.GetComponent<PlayerMovement>().canMove = false;

        SoundFXManager.instance.PlaySound(outOfTimeSound, transform, 1f);

        yield return new WaitForSeconds(outOfTimeSound.length);

        gameController.EndOfLevel();
    }

    private IEnumerator Countdown()
    {
        for(int i =0; i<10; i++)
        {
            SoundFXManager.instance.PlaySound(countdownSound, transform, 1f);
            yield return new WaitForSeconds(1);
        }
    }
}
