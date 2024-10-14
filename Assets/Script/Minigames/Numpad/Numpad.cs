using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Numpad : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject completionWindowPrefab;
    [SerializeField] private List<Button> numpadButtons = new List<Button>();

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button enterButton;

    [SerializeField] private Button deleteButton;

    private List<int> numberPressed  = new List<int>();
    private int nextNumber = 1;
    private int buttonsPressed = 0;

    private bool isTaskComplete = false;
    private bool isOpen = true;

    private bool firstClick = true;

    [SerializeField] private Button closeButton;
    private GameObject droneMenu;
    private GameObject completionWindow;

    private MinigameController minigameController;
    private OverloadBar overloadBar;
    private DroneStation droneStation;
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
        displayText.text = "CLICK FROM 1 TO 10";
        enterButton.onClick.AddListener(()=> SubmitInput());
        deleteButton.onClick.AddListener(()=> DeleteInput());
        Randomize();

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

    private void Randomize()
    {
        List<int> numbers = new List<int>();
        for(int i=1; i<=10; i++)
        {
            numbers.Add(i);
        }

        for(int i = numbers.Count-1; i>0; i--)
        {
            int random = Random.Range(0,i+1);
            int temp = numbers[i];
            numbers[i] = numbers[random];
            numbers[random] = temp;
        }

        for(int i =0; i<numpadButtons.Count;i++)
        {
            int number = numbers[i];
            Button button = numpadButtons[i];

            button.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
            button.onClick.AddListener(() => PressNumber(number));
        }
    }

    private void PressNumber(int number)
    {
        if(isTaskComplete)
        {
            return;
        }
        if(buttonsPressed<10)
        {
            if(firstClick)
            {
                displayText.text  = "";
                firstClick = false;
            }
            buttonsPressed++;
            numberPressed.Add(number);

            displayText.text += $"{number}";
            if(number == nextNumber)
            {
                nextNumber ++;
            }
        }
    }

    private void SubmitInput()
    {
        if(isTaskComplete)
        {
            return;
        }
        if(nextNumber!=11)
        {
            displayText.text = "INCORRECT. TRY AGAIN.";
            StartCoroutine(IncorrectInput());
        }
        else if(nextNumber==11)
        {
            displayText.text = "CORRECT. CLOSING...";
            Destroy(closeButton.gameObject);

            isTaskComplete = true;
            overloadBar.completedMinigames++;

            completionWindow.SetActive(true);
            Debug.Log("Task completed");
            StartCoroutine(CloseTimer());
            
        }

    }

    private void DeleteInput()
    {
        if(isTaskComplete)
        {
            return;
        }
        if(numberPressed.Count-1>=0)
        {
            int lastNumberIndex = numberPressed.Count-1;
            string deletedNumber = numberPressed[lastNumberIndex].ToString();

            int index = displayText.text.LastIndexOf(deletedNumber);
            if (index >= 0)
            {
                displayText.text = displayText.text.Remove(index, deletedNumber.Length);

                if(numberPressed[lastNumberIndex] == nextNumber-1)
                {
                    nextNumber--;
                }
                
                buttonsPressed--;

                numberPressed.RemoveAt(lastNumberIndex);

            }
        }
    }

    private IEnumerator IncorrectInput()
    {
        yield return new WaitForSeconds(1f);
        ClearInput();
    }

    private void ClearInput()
    {
        numberPressed.Clear();
        nextNumber = 1;
        buttonsPressed = 0;
        displayText.text = "";
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
