using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject completionWindowPrefab;
    [SerializeField] private List<Button> padButtons = new List<Button>();

    [SerializeField] private GameObject passwordDisplay;

    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Button enterButton;

    [SerializeField] private Button deleteButton;

    private List<string> pressedButtons  = new List<string>();

    private List<string> password = new List<string>();
    private int buttonsPressed = 0;

    private int index =0;

    private bool isTaskComplete = false;
    private bool isOpen = true;

    private bool firstClick = true;

    [SerializeField] private Button closeButton;
    private GameObject droneMenu;
    private GameObject completionWindow;

    private MinigameController minigameController;
    private OverloadBar overloadBar;

    [SerializeField] private List<AudioClip> clickButtonSound;
    [SerializeField] private List<AudioClip> beepSound;

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
            droneMenu.SetActive(true);
            Debug.Log($"closing minigame! {overloadBar.minigamesToComplete-overloadBar.completedMinigames} more minigames to complete!");
            MinigameController minigameController = FindObjectOfType<MinigameController>();
            minigameController.exitedWithoutCompletion = true;
            droneStation.isinteracting = true;
            Destroy(gameObject);
        }
    }

    public void InitializeMinigame()
    {
        isTaskComplete = false;

        closeButton.onClick.AddListener(()=> CloseWindow());
        displayText.text = "INPUT PASSWORD";
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
        List<string> availableButtons = new List<string>();
        for(int i=1; i<=9; i++)
        {
            availableButtons.Add(i.ToString());
        }
        for(char c = 'A'; c <= 'Z'; c++)
        {
            availableButtons.Add(c.ToString());
        }

        for (int i = availableButtons.Count - 1; i > 0; i--)
        {
            int random = Random.Range(0, i + 1);
            string temp = availableButtons[i];
            availableButtons[i] = availableButtons[random];
            availableButtons[random] = temp;
        }
        List<string> refPadBUttons = new List<string>();

        for (int i = 0; i < padButtons.Count; i++)
        {
            string input = availableButtons[i];
            Button button = padButtons[i];

            button.GetComponentInChildren<TextMeshProUGUI>().text = input;

            refPadBUttons.Add(input);

            button.onClick.AddListener(() => PressButton(input));
        }

        password.Clear();
        string currentPassword = "";

        for(int i=0; i<5; i++)
        {
            int randomIndex = Random.Range(0, refPadBUttons.Count-1);
            password.Add(refPadBUttons[randomIndex]);
            refPadBUttons.RemoveAt(randomIndex);
            
            currentPassword += password[i];
        }
        passwordDisplay.GetComponentInChildren<TextMeshProUGUI>().text = currentPassword;
    }


    private void PressButton(string buttonInput)
    {
        if(isTaskComplete)
        {
            return;
        }

        int random = Random.Range(0, beepSound.Count);
        SoundFXManager.instance.PlaySound(beepSound[random], transform, 0.5f);

        if(buttonsPressed<10)
        {
            if(firstClick)
            {
                displayText.text  = "";
                firstClick = false;
            }
            buttonsPressed++;
            pressedButtons.Add(buttonInput);

            displayText.text += $"{buttonInput}";

            if(buttonInput == password[index] && index<4)
            {
                index ++;
            }
        }
    }

    private void SubmitInput()
    {
        if(isTaskComplete)
        {
            return;
        }

        int random = Random.Range(0, beepSound.Count);
        SoundFXManager.instance.PlaySound(beepSound[random], transform, 0.5f);

        if(pressedButtons.Count!=password.Count)
        {
            displayText.text = "INCORRECT. TRY AGAIN.";
            StartCoroutine(IncorrectInput());
        }
        else
        {
            bool isMatching = true;
            for(int i =0; i<password.Count;i++)
            {
                if(pressedButtons[i] != password[i])
                {
                    isMatching = false;
                    break;
                }
            }

            if(isMatching)
            {
                displayText.text = "CORRECT. CLOSING...";
                Destroy(closeButton.gameObject);
                Destroy(passwordDisplay);

                isTaskComplete = true;
                overloadBar.completedMinigames++;
                completionWindow.SetActive(true);
                Debug.Log("Task completed");
                StartCoroutine(CloseTimer());
            }
            else
            {
                displayText.text = "INCORRECT. TRY AGAIN.";
                StartCoroutine(IncorrectInput());
            }
            
        }

    }

    private void DeleteInput()
    {
        if(isTaskComplete)
        {
            return;
        }

        int random = Random.Range(0, beepSound.Count);
        SoundFXManager.instance.PlaySound(beepSound[random], transform, 0.5f);

        if(pressedButtons.Count-1>=0)
        {
            int lastIndex = pressedButtons.Count-1;
            string deletedInput = pressedButtons[lastIndex].ToString();

            int indexToDelete = displayText.text.LastIndexOf(deletedInput);
            if (indexToDelete >= 0)
            {
                displayText.text = displayText.text.Remove(indexToDelete, deletedInput.Length);

                if(pressedButtons.Count <= password.Count && pressedButtons[lastIndex] == password[lastIndex])
                {
                    index--;
                }
                
                buttonsPressed--;

                pressedButtons.RemoveAt(lastIndex);

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
        pressedButtons.Clear();
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
