using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CopyPattern : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject completionWindowPrefab;
    [SerializeField] private List<Button> padButtons = new List<Button>();
    [SerializeField] private List<Image> patternBlocks = new List<Image>();
    [SerializeField] private Button closeButton;

    private List<int> pattern = new List<int>();
    private List<int> buttonIndexClicks = new List<int>();
    private int patternSize = 5;
    private int numberPressed = 0;
    private GameObject droneMenu;
    private GameObject completionWindow;

    private MinigameController minigameController;
    private OverloadBar overloadBar;
    private DroneStation droneStation;

    private bool isTaskComplete =false;
    private bool isOpen = true;


    // Start is called before the first frame update
    void Start()
    {
        minigameController = FindObjectOfType<MinigameController>();
        overloadBar = FindObjectOfType<OverloadBar>();

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
        foreach(Button button in padButtons)
        {
            int index  = padButtons.IndexOf(button);
            button.onClick.AddListener(()=>ClickOnButton(index));
        }

        GeneratePattern();

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

    private void GeneratePattern()
    {
        pattern.Clear();

        List<int> availablePatternPos = new List<int>();
        for (int i = 0; i < padButtons.Count; i++)
        {
            availablePatternPos.Add(i);
        }

        for (int i = 0; i < availablePatternPos.Count; i++)
        {
            int randomIndex = Random.Range(i, availablePatternPos.Count);
            int temp = availablePatternPos[i];
            availablePatternPos[i] = availablePatternPos[randomIndex];
            availablePatternPos[randomIndex] = temp;
        }

        for (int i = 0; i < patternSize; i++)
        {
            pattern.Add(availablePatternPos[i]);
        }

        foreach (var img in patternBlocks)
        {
            img.color = new Color(95f, 95f, 95f); 
        }

        foreach (int index in pattern)
        {
            patternBlocks[index].color = Color.blue;
        }
    }

    private void ClickOnButton(int index)
    {
        if(isTaskComplete)
        {
            return;
        }
        var clickedButton = padButtons[index];
        Image img = clickedButton.GetComponent<Image>();
        if(img.color == Color.yellow)
        {
            img.color = Color.white;
            numberPressed--;
            if(buttonIndexClicks.Contains(index))
            {
                buttonIndexClicks.Remove(index);
            }

        }
        else
        {
            img.color = Color.yellow;
            buttonIndexClicks.Add(index);
            numberPressed++;
        }
        if(numberPressed  == patternSize)
        {
            CheckPattern();
        }

    }

    private void CheckPattern()
    {
        foreach(var patternIndex in pattern)
        {
            if(!buttonIndexClicks.Contains(patternIndex))
            {
                return;
            }
        }

        isTaskComplete=true;
        Destroy(closeButton.gameObject);

        overloadBar.completedMinigames++;
        completionWindow.SetActive(true);
        Debug.Log("Task completed");
        StartCoroutine(CloseTimer());

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
