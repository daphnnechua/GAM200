using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WireTask : MonoBehaviour, IMinigame
{
    public List<Color> wireColors = new List<Color>();
    public List<Wires> leftWires = new List<Wires>();
    public List<Wires> rightWires = new List<Wires>();

    private List<Color> availableColor;
    private List<int> availableLeftWireIndex;
    private List<int> availableRightWireIndex;
    public Wires currentDragged;
    public Wires currentHovered;

    private bool isTaskComplete =  false;

    // [SerializeField] private GameObject wireTaskInterface;
    [SerializeField] private GameObject droneMenu;
    [SerializeField] private GameObject completionWindowPrefab;
    private OverloadBar overloadBar;
    private DroneStation droneStation;
    private bool isOpen = true;

    [SerializeField] GameObject check;
    [SerializeField] private GameObject completionWindow;
    [SerializeField] private Button closeButton;

    [SerializeField] private List<AudioClip> clickButtonSound;

    void Start()
    {
        overloadBar = FindObjectOfType<OverloadBar>();
        droneStation = FindObjectOfType<DroneStation>();
    }

    public void StartMinigame()
    {
        droneStation = FindObjectOfType<DroneStation>();

        InitializeMinigame();
        if(droneStation==null)
        {
            Debug.LogWarning("drone station script is null");
        }
        droneMenu = droneStation.droneMenu;
        if(droneMenu.activeInHierarchy)
        {
            droneMenu.SetActive(false);
        }

        //setting sorting order to completion window

        completionWindow = Instantiate(completionWindowPrefab, gameObject.transform);

        //resize completion window to same size as minigame
        RectTransform rt = completionWindow.GetComponent<RectTransform>();
        RectTransform refRt = gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(refRt.rect.width, refRt.rect.height);

        completionWindow.GetComponent<Canvas>().overrideSorting = true;
        completionWindow.GetComponent<Canvas>().sortingLayerName = "UI";
        completionWindow.GetComponent<Canvas>().sortingOrder = 10;

        // wireTaskInterface.SetActive(true);

        completionWindow.SetActive(false);
    }    
    public void InitializeMinigame()
    {
        isTaskComplete = false;
        // wireTaskInterface = GameObject.FindWithTag("WireTask");
        closeButton = gameObject.GetComponentInChildren<Button>();

        closeButton.onClick.AddListener(()=> CloseWindow());

        availableColor = new List<Color>(wireColors);
        availableLeftWireIndex = new List<int>();
        availableRightWireIndex = new List<int>();

        for(int i=0; i<leftWires.Count; i++)
        {
            availableLeftWireIndex.Add(i);
        }
        for(int i =0; i<rightWires.Count;i++)
        {
            availableRightWireIndex.Add(i);
        }

        while(availableColor.Count> 0 && availableLeftWireIndex.Count> 0 && availableRightWireIndex.Count>0)
        {
            int randomColorIndex = Random.Range(0, availableColor.Count);
            Color randomColor = availableColor[randomColorIndex];
            randomColor.a = 1;
            int leftWire = Random.Range(0, availableLeftWireIndex.Count);
            int rightWire = Random.Range(0, availableRightWireIndex.Count);

            leftWires[availableLeftWireIndex[leftWire]].SetImageColour(randomColor);
            rightWires[availableRightWireIndex[rightWire]].SetImageColour(randomColor);

            availableColor.RemoveAt(randomColorIndex);
            availableLeftWireIndex.RemoveAt(leftWire);
            availableRightWireIndex.RemoveAt(rightWire);
        }
        StartCoroutine(CheckTaskStatus());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isOpen && !isTaskComplete)
        {
            droneMenu.SetActive(true);
            Debug.Log($"closing minigame! {overloadBar.minigamesToComplete-overloadBar.completedMinigames} more minigames to complete!");
            MinigameController minigameController = FindObjectOfType<MinigameController>();
            minigameController.exitedWithoutCompletion = true;
            droneStation.isinteracting = true;
            Destroy(gameObject);
        }
    }

    private IEnumerator CheckTaskStatus()
    {
        while(!isTaskComplete)
        {
            int successfulMatches = 0;
            for(int i =0; i<rightWires.Count;i++)
            {
                if(rightWires[i].isCorrectMatch)
                {
                    successfulMatches++;
                }
            } 
            if(successfulMatches >= leftWires.Count)
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
        // Debug.Log("taask complete. closing in 3");
        yield return new WaitForSeconds(1.5f);
        // Debug.Log("closing...");
        //close task window after 3 seconds
        CloseWindow();
    }

    private void CloseWindow()
    {
        if(!isOpen) {return;}
        
        foreach (var wire in leftWires)
        {
            wire.ResetWires(); // Ensure this method exists in your Wires class
        }
        
        foreach (var wire in rightWires)
        {
            wire.ResetWires(); // Ensure this method exists in your Wires class
        }
        // wireTaskInterface.SetActive(false);
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
            int random = Random.Range(0, clickButtonSound.Count);
            SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

            droneMenu.SetActive(true);
            Debug.Log($"closing minigame! {overloadBar.minigamesToComplete-overloadBar.completedMinigames} more minigames to complete!");
            MinigameController minigameController = FindObjectOfType<MinigameController>();
            minigameController.exitedWithoutCompletion = true;
            droneStation.isinteracting = true;
            Destroy(gameObject);
        }
        
    }
    
}
