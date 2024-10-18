using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteryMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private List<BatterySlots> batterySlotsList;
    [SerializeField] private GameObject completionWindowPrefab;
    [SerializeField] private Button closeButton;
    private GameObject droneMenu;
    private GameObject completionWindow;

    private MinigameController minigameController;
    private OverloadBar overloadBar;
    private DroneStation droneStation;

    public bool isTaskComplete = false;
    private bool isOpen = true;

    // Start is called before the first frame update
    void Start()
    {
        minigameController = FindObjectOfType<MinigameController>();
        overloadBar = FindObjectOfType<OverloadBar>();

        InitializeMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isOpen && !isTaskComplete)
        {
            CloseWindow();
            droneMenu.SetActive(true);
        }
    }

    public void InitializeMinigame()
    {
        isTaskComplete = false;

        closeButton.onClick.AddListener(()=> CloseWindow());
        StartCoroutine(CheckTaskStatus());
    }

    public void StartMinigame()
    {
        droneStation = FindObjectOfType<DroneStation>();
        droneMenu = droneStation.droneMenu;
        if(droneMenu.activeInHierarchy)
        {
            droneMenu.SetActive(false);
        }
        InitializeMinigame();

        completionWindow = Instantiate(completionWindowPrefab, gameObject.transform);

        //resize completion window to same size as minigame
        RectTransform rt = completionWindow.GetComponent<RectTransform>();
        RectTransform refRt = gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(refRt.rect.width, refRt.rect.height);

        completionWindow.GetComponent<Canvas>().overrideSorting = true;
        completionWindow.GetComponent<Canvas>().sortingLayerName = "UI";
        completionWindow.GetComponent<Canvas>().sortingOrder = 10;

        completionWindow.SetActive(false);
    }

    private IEnumerator CheckTaskStatus()
    {
        while(!isTaskComplete)
        {
            int successfulReplacments = 0;
            foreach(BatterySlots batterySlot in batterySlotsList)
            {
                if(batterySlot.currentBattery!=null)
                {
                    successfulReplacments++;
                }
            }
            // Debug.Log($"Batteries replaced: {successfulReplacments}");
            if(successfulReplacments >=3)
            {
                Debug.Log("Task complete");

                completionWindow.SetActive(true);
                Destroy(closeButton.gameObject);
                completionWindow.GetComponentInChildren<TextMeshProUGUI>().text = "Task Complete!";
                isTaskComplete = true;

                overloadBar.completedMinigames++;
                StartCoroutine(CloseTimer());
            }

            yield return new WaitForSeconds(0.5f);
        }
    }


    private IEnumerator CloseTimer()
    {
        yield return new WaitForSeconds(1.5f);
        CloseWindow();
    }

    private void CloseWindow()
    {
        if(!isOpen) {return;}
        
        isOpen = false;

        if(isTaskComplete)
        {
            
            if(overloadBar.completedMinigames>=overloadBar.minigamesToComplete)
            {
                overloadBar.DecreaseOverloadValue();
                droneMenu.SetActive(true);
                droneStation.isinteracting = true;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"completed {overloadBar.completedMinigames} minigames. left: {overloadBar.minigamesToComplete - overloadBar.completedMinigames} minigames to complette!");
                MinigameController minigameController = FindObjectOfType<MinigameController>();
                minigameController.OpenNewMinigame(gameObject);
            }
        }
        else
        {
            droneMenu.SetActive(true);
            Debug.Log($"closing minigame! {overloadBar.minigamesToComplete-overloadBar.completedMinigames} more minigames to complete!");
            MinigameController minigameController = FindObjectOfType<MinigameController>();
            minigameController.exitedWithoutCompletion = true;
            droneStation.isinteracting = true;
            Destroy(gameObject);
        }
        
    }
}
