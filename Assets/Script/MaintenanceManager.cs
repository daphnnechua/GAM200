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

    private List<IMinigame> minigames = new List<IMinigame>();  //store all minigames
    [SerializeField] private List<GameObject> minigamePrefabs = new List<GameObject>();

    private OverloadBar overloadBar;
    private RestockingController restockingController;

    private DroneStation droneStation;
    // Start is called before the first frame update
    void Start()
    {
        droneStation = FindObjectOfType<DroneStation>();

    }

    // Update is called once per frame
    void Update()
    {
        if(droneStation.isinteracting)
        {
            UpdateUI();
        }
    }

    public void Initialize()
    {
        minigames = FindObjectsOfType<MonoBehaviour>().OfType<IMinigame>().ToList();
        maintenanceButton.onClick.AddListener(()=> OpenMinigame());
    }

    private void OpenMinigame()
    {

        Debug.Log("Clicking");
        overloadBar = FindObjectOfType<OverloadBar>();
        restockingController = FindObjectOfType<RestockingController>();
        if(overloadBar.currentOverloadCount > 0 && restockingController.droneAvailable)
        {
            // Debug.Log("Can do maintenance");
            // Debug.Log($"minigames available: {minigames.Count}");
            if(minigamePrefabs.Count>0)
            {
                int random = Random.Range(0, minigamePrefabs.Count);
                
                GameObject minigame = Instantiate(minigamePrefabs[random]);
                
                RectTransform minigameRT = minigame.GetComponent<RectTransform>();
                minigameRT.SetParent(GameObject.Find("Canvas").transform, false); 
                
                IMinigame game = minigame.GetComponent<IMinigame>();
                game.StartMinigame();
                droneStation.isinteracting = false;
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

    }


}