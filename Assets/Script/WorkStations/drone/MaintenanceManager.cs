using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceManager : MonoBehaviour
{
    [SerializeField] private Button maintenanceButton;
    [SerializeField] private TextMeshProUGUI maintenanceTextUI;

    private List<string> minigameFilePaths = new List<string>();
    [SerializeField] public List<GameObject> minigamePrefabs = new List<GameObject>();

    private OverloadBar overloadBar;
    private RestockingController restockingController;
    private MinigameController minigameController;
    private DroneStation droneStation;

    private int minigameIndex;
    public bool hasButtonBeenUpdated = false;

    [SerializeField] private List<AudioClip> clickButtonSound;
    // Start is called before the first frame update
    void Start()
    {
        droneStation = FindObjectOfType<DroneStation>();
        minigameController = FindObjectOfType<MinigameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(droneStation.isinteracting)
        {
            
            UpdateUI();
            UpdateMaintenanceButtonImage();
        }

        
    }

    public void Initialize()
    {
        List<Minigames> minigameList = new List<Minigames>();
        // if(Game.GetMinigameList() == null)
        // {
        //     Debug.Log("minigame list is null");
        // }
        foreach(Minigames minigames in Game.GetMinigameList())
        {
            minigameList.Add(minigames);
        }

        // Debug.Log($"minigame number: {minigameList.Count}");


        foreach(Minigames minigame in minigameList)
        {
            minigameFilePaths.Add(minigame.filePath);
        }
        LoadMinigames();

        maintenanceButton.onClick.AddListener(()=> OpenMinigame());
    }

    private void OpenMinigame()
    {
        int randomSoundFx = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[randomSoundFx], transform, 1f);

        // Debug.Log("Clicking");
        overloadBar = FindObjectOfType<OverloadBar>();
        restockingController = FindObjectOfType<RestockingController>();

        if(overloadBar.currentOverloadCount > 0 && restockingController.droneAvailable)
        {
            // Debug.Log("Can do maintenance");
            // Debug.Log($"minigames available: {minigames.Count}");
            if(minigamePrefabs.Count>0)
            {
                if(!minigameController.exitedWithoutCompletion) //load any minigame
                {
                    int random = Random.Range(0, minigamePrefabs.Count);
                    minigameIndex = random;
                    minigameController.minigameParentObj.SetActive(true);
                    GameObject minigame = Instantiate(minigamePrefabs[random], minigameController.minigameParentObj.transform);
                    
                    // RectTransform minigameRT = minigame.GetComponent<RectTransform>();
                    // minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                    
                    IMinigame game = minigame.GetComponent<IMinigame>();
                    game.StartMinigame();
                    droneStation.isinteracting = false;
                }
                else if(minigameController.exitedWithoutCompletion) //load the same minigame if playyer did not complete the prev minigame
                {
                    if(minigameController.isFirstMinigame) //this is the first minigame that the player plays
                    {
                        minigameController.minigameParentObj.SetActive(true);
                        GameObject minigame = Instantiate(minigamePrefabs[minigameIndex], minigameController.minigameParentObj.transform); //load back the first minigame index
                        
                        // RectTransform minigameRT = minigame.GetComponent<RectTransform>();
                        // minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                        
                        IMinigame game = minigame.GetComponent<IMinigame>();
                        game.StartMinigame();
                        droneStation.isinteracting = false;
                        minigameController.exitedWithoutCompletion = false; //reset bool
                    }
                    else //this is not the first minigame
                    {
                        minigameController.minigameParentObj.SetActive(true);
                        GameObject minigame = Instantiate(minigamePrefabs[minigameController.minigameIndex], minigameController.minigameParentObj.transform); //load minigame according to the index set by minigame controller
                    
                        // RectTransform minigameRT = minigame.GetComponent<RectTransform>();
                        // minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                        
                        IMinigame game = minigame.GetComponent<IMinigame>();
                        game.StartMinigame();
                        droneStation.isinteracting = false;
                        minigameController.exitedWithoutCompletion = false; //reset bool

                    }
                }

            }
        }
    }

    private void UpdateUI()
    {
        overloadBar = FindObjectOfType<OverloadBar>();
        if(overloadBar.currentOverloadCount <= 3)
        {
            maintenanceTextUI.text = "Overload Status: <color=green>Low</color> \nMaintenance Priority: <color=green>Low</color>";
        }
        else if (overloadBar.currentOverloadCount > 3 && overloadBar.currentOverloadCount <= 8)
        {
            maintenanceTextUI.text = "Overload Status: <color=yellow>Moderate</color> \nMaintenance Priority: <color=yellow>Medium</color>";
        }
        else if(overloadBar.currentOverloadCount > 8)
        {
            maintenanceTextUI.text = "Overload Status: <color=red>High</color> \nMaintenance Priority: <color=red>High</color>";
        }
        else if(overloadBar.currentOverloadCount == overloadBar.maxOverloadCount)
        {
            maintenanceTextUI.text = "Overload Status: <color=red>Critical</color> \nMaintenance Priority: <color=red>Extreme</color>";
        }
        TabController tabController = FindObjectOfType<TabController>();  
    
    }

    private void LoadMinigames()
    {
        foreach(string filePath in minigameFilePaths)
        {
            AssetManager.LoadPrefab(filePath, (GameObject prefab) =>
            {
                if (prefab != null)
                {
                    minigamePrefabs.Add(prefab); 
                }
            });
        }

    }

    private void SetMaintenanceButtonImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    public void UpdateMaintenanceButtonImage()
    {
        OverloadBar overloadBar = FindObjectOfType<OverloadBar>();
        restockingController = FindObjectOfType<RestockingController>();
        string path = "";
        if(overloadBar.currentOverloadCount>0 && restockingController.droneAvailable)
        {
            path = "drone menu/Drone Menu/orangebutton";
        }
        else
        {
            path = "drone menu/Drone Menu/greybutton";
        }
        SetMaintenanceButtonImage(path, maintenanceButton.gameObject.GetComponent<Image>());
        
        hasButtonBeenUpdated = true; 
    }

}
