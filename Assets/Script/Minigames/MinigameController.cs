using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    // Start is called before the first frame update
    public OverloadBar overloadBar;
    public DroneStation droneStation;
    public MaintenanceManager maintenanceManager;
    private GameController gameController;
    public bool isFirstMinigame = false;

    public GameObject minigameParentObj;
    private bool hasBeenInitialized = false;

    public int minigameIndex;
    public bool exitedWithoutCompletion = false;
    void Start()
    {
        overloadBar = FindObjectOfType<OverloadBar>();
        droneStation = FindObjectOfType<DroneStation>();
        maintenanceManager = FindObjectOfType<MaintenanceManager>();
        minigameParentObj = GameObject.FindWithTag("Minigame");
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if(gameController.gameStart && !hasBeenInitialized)
        {
            minigameParentObj.SetActive(false);
            hasBeenInitialized = true;
            
            // Debug.Log("initializing...");
        }
        
        if(overloadBar.completedMinigames ==0) //player has not completed any minigames, this is the first minigame that the player plays
        {
            isFirstMinigame = true;
        }
        else
        {
            isFirstMinigame= false;
        }

        if(droneStation.isinteracting && minigameParentObj.activeInHierarchy)
        {
            minigameParentObj.SetActive(false);
        }

    }


    public void OpenNewMinigame(GameObject currentMinigame)
    {
        if(overloadBar.completedMinigames < overloadBar.minigamesToComplete)
        {
            Destroy(currentMinigame);

            int random = Random.Range(0, maintenanceManager.minigamePrefabs.Count);
            minigameIndex = random;
            
            minigameParentObj.SetActive(true);
            GameObject minigame = Instantiate(maintenanceManager.minigamePrefabs[random], minigameParentObj.transform);
                
            // RectTransform minigameRT = minigame.GetComponent<RectTransform>();
            // minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                
            IMinigame game = minigame.GetComponent<IMinigame>();
            game.StartMinigame();
        }
    }

}

