using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BugDestroyerMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject bugPrefab;
    [SerializeField] public RectTransform rt;

    [SerializeField] private GameObject completionWindowPrefab;

    private GameObject completionWindow;
    private GameObject bugDestroyerInterface;

    private GameObject droneMenu;
    private Button closeButton;
    private int bugNumber = 10;

    private bool isOpen = true;
    private bool isTaskComplete = false;
    private OverloadBar overloadBar;
    private MinigameController minigameController;
    private DroneStation droneStation;
    // Start is called before the first frame update
    void Start()
    {
        minigameController = FindObjectOfType<MinigameController>();
        overloadBar = minigameController.overloadBar;


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
        bugDestroyerInterface = GameObject.FindWithTag("BugDestroyer");

        closeButton = gameObject.GetComponentInChildren<Button>();
        closeButton.onClick.AddListener(()=> CloseWindow());

        SpawnBugs();
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

        completionWindow = Instantiate(completionWindowPrefab);
        completionWindow.transform.SetParent(bugDestroyerInterface.transform, false);
        completionWindow.GetComponent<Canvas>().overrideSorting = true;
        completionWindow.GetComponent<Canvas>().sortingLayerName = "UI";
        completionWindow.GetComponent<Canvas>().sortingOrder = 10;

        completionWindow.SetActive(false);
        bugDestroyerInterface.SetActive(true);
    }

    private void SpawnBugs()
    {
        for(int i =0; i<bugNumber;i++)
        {
            GameObject bug = Instantiate(bugPrefab, rt);
            bug.GetComponent<RectTransform>().localPosition=new Vector3(Random.Range(-rt.rect.width/2, rt.rect.width/2), Random.Range(-rt.rect.height/2, rt.rect.height/2), 0);

        }
    }

    private IEnumerator CheckTaskStatus()
    {
        while(!isTaskComplete)
        {
            List<Bug> bugsLeft = new List<Bug>();
            bugsLeft = FindObjectsOfType<Bug>().ToList();

            if(bugsLeft.Count ==0)
            {
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
