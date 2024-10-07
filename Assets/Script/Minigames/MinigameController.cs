using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    // Start is called before the first frame update
    public OverloadBar overloadBar;
    public DroneStation droneStation;
    public MaintenanceManager maintenanceManager;
    public bool isFirstMinigame = false;

    public int minigameIndex;
    public bool exitedWithoutCompletion = false;
    void Start()
    {
        overloadBar = FindObjectOfType<OverloadBar>();
        droneStation = FindObjectOfType<DroneStation>();
        maintenanceManager = FindObjectOfType<MaintenanceManager>();
    }

    void Update()
    {
        if(overloadBar.completedMinigames ==0) //player has not completed any minigames, this is the first minigame that the player plays
        {
            isFirstMinigame = true;
        }
        else
        {
            isFirstMinigame= false;
        }

    }


    public void OpenNewMinigame(GameObject currentMinigame)
    {
        if(overloadBar.completedMinigames < overloadBar.minigamesToComplete)
        {
            Destroy(currentMinigame);

            int random = Random.Range(0, maintenanceManager.minigamePrefabs.Count);
            minigameIndex = random;
                
            GameObject minigame = Instantiate(maintenanceManager.minigamePrefabs[random]);
                
            RectTransform minigameRT = minigame.GetComponent<RectTransform>();
            minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                
            IMinigame game = minigame.GetComponent<IMinigame>();
            game.StartMinigame();
        }
    }

}

