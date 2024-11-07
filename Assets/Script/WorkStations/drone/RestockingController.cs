using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestockingController : MonoBehaviour
{
    public List<StockStationManager> allStockStations;
    public int maxRestockLimit;
    public float restockTime = 2f;

    [SerializeField] private Dictionary<string, Button> ingredientRestockButtons = new Dictionary<string, Button>();

    private Transform restockPos;
    private Transform displayPos;
    [SerializeField] private List<GameObject> restockButtons = new List<GameObject>();
    [SerializeField] public List<GameObject> displayButtons = new List<GameObject>();
    [SerializeField] private Button confirmationButton;
    [SerializeField] private List<Image> ingredientImages;
    [SerializeField] private List<TextMeshProUGUI> ingredientNames;

    public List<string> selectedIngredientID = new List<string>();
    private bool restockButtonsActive = false;
    private bool toUpdateDisplayButtons = false;
    public bool droneAvailable = true;

    [SerializeField] private OverloadBar overloadBar;
    [SerializeField] private TabController tabController;
    private DroneStation droneStation;

    [SerializeField] private GameObject displayBtnPrefab;
    [SerializeField] private GameObject timerCountdown;

    [SerializeField] private GameObject exclaimationPrefab;

    private Coroutine textCoroutine;
    private DroneMenuController droneMenuController;

    public bool restockCompleted = false;
    [SerializeField] private List<AudioClip> clickButtonSounds;

    // Start is called before the first frame update
    void Start()
    {
        droneStation = FindObjectOfType<DroneStation>();
        tabController = FindObjectOfType<TabController>();
        droneMenuController = FindObjectOfType<DroneMenuController>();

    }

    // Update is called once per frame
    void Update()
    {
        if(tabController.isRestockingPage && droneStation.isinteracting)
        {
            if(restockButtonsActive && toUpdateDisplayButtons)
            {
                UpdateButtons();
            }
            UpdateButtonVisuals();

        }
        if(!restockCompleted && droneAvailable)
        {
            confirmationButton.onClick.RemoveAllListeners();
            confirmationButton.onClick.AddListener(()=>SendDroneForRestock());
        }
        else if(restockCompleted)
        {
            confirmationButton.onClick.RemoveAllListeners();
            confirmationButton.onClick.AddListener(() => RestockIngredients());
        }
    }

    public void InitializeRestockingController()
    {
        tabController = FindObjectOfType<TabController>();
        overloadBar = FindObjectOfType<OverloadBar>();
        displayPos = GameObject.FindWithTag("IngredientsDisplay").transform;
        restockPos = GameObject.FindWithTag("IngredientRestockButton").transform;
        allStockStations = FindObjectsOfType<StockStationManager>().ToList();
        List<int> indexToRemove = new List<int>();
        // Debug.Log($"Checking how many stations are there: {allStockStations.Count}");
        for(int i =0; i<allStockStations.Count;i++)
        {
            if(allStockStations[i].stockSO.objType == "Plate")
            {
                indexToRemove.Add(i);
                // Debug.Log($"index number: {i} is aa plate stock");
            }

        }
        for (int i = allStockStations.Count - 1; i >= 0; i--)
        {
            if (allStockStations[i].stockSO.objType == "Plate")
            {
                allStockStations.RemoveAt(i);
                // Debug.Log($"Removed index number: {i} as it is a plate stock");
            }
        }
        // foreach(var e in allStockStations)
        // {
        //     Debug.Log(e.stockSO.ingredientID);
        // }
        UpdateButtons();

        confirmationButton = GameObject.FindGameObjectWithTag("RestockConfirmationButton").GetComponent<Button>();
        
    }

    public void SelectIngredients(string ingredientID)
    {
        if(overloadBar.maxOverloadCount - overloadBar.currentOverloadCount < 4)
        {
            maxRestockLimit = overloadBar.maxOverloadCount - overloadBar.currentOverloadCount;
        }
        else
        {
            maxRestockLimit = 4;
        }

        if(selectedIngredientID.Count < maxRestockLimit && droneAvailable)
        {
            selectedIngredientID.Add(ingredientID);
            // Debug.Log(ingredientID + " selected!");
            toUpdateDisplayButtons = true;

            int random = Random.Range(0, clickButtonSounds.Count);
            SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
        }
        else if(!droneAvailable || selectedIngredientID.Count < maxRestockLimit)
        {
            Debug.Log("drone is busy");

            int random = Random.Range(0, clickButtonSounds.Count);
            SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
        }
    }

    private void RemoveSelectedIngredients(int index)
    {
        if(droneAvailable)
        {
            selectedIngredientID.RemoveAt(index);
            toUpdateDisplayButtons = true;

        }

        int random = Random.Range(0, clickButtonSounds.Count);
        SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);

    }

    public void UpdateButtons()
    {
        List<string> ingredientIDs = new List<string>();

        if(!restockButtonsActive)
        {
            for(int i =0; i<restockButtons.Count;i++)
            { 
                ingredientImages[i].gameObject.SetActive(false);
                ingredientNames[i].gameObject.SetActive(false);

                Image buttonImage = restockButtons[i].GetComponent<Image>();
                SetButtonImage("drone menu/Drone Menu/blackbutton", buttonImage);
            }

            for(int i =0; i<allStockStations.Count;i++)
            {
                if(allStockStations[i].stockSO.objType == "Ingredient")
                {
                    ingredientIDs.Add(allStockStations[i].stockSO.ingredientID);

                }
            }
            for(int i =0; i<ingredientIDs.Count;i++)
            {
                string currentIngredientID = ingredientIDs[i]; // Capture the current value

                Ingredient ingredient = Game.GetIngredientByID(currentIngredientID);
                string ingredientFilePath = ingredient.imageFilePath;

                string name = ingredient.name;

                Button button = restockButtons[i].GetComponent<Button>();

                button.onClick.AddListener(() => SelectIngredients(currentIngredientID));

                Image buttonImgae = restockButtons[i].GetComponent<Image>();
                SetButtonImage("drone menu/Drone Menu/blueButton", buttonImgae);

                ingredientImages[i].gameObject.SetActive(true);
                ingredientNames[i].gameObject.SetActive(true);

                SetButtonImage(ingredientFilePath, ingredientImages[i]);
                
                ingredientNames[i].text = name;
                // Debug.Log(currentIngredientID);

            }
            restockButtonsActive = true;
        }

        if(toUpdateDisplayButtons)
        {
            foreach (var displayButton in displayButtons)
            {
                Destroy(displayButton);
            }
            displayButtons.Clear(); 
            for (int i = 0; i < selectedIngredientID.Count; i++)
            {
                GameObject displayButton = Instantiate(displayBtnPrefab, displayPos);
                displayButtons.Add(displayButton);

                string name = Game.GetIngredientByID(selectedIngredientID[i]).name;

                Image displayButtonImage = displayButtons[i].GetComponentInChildren<Image>();
                string imagePath = Game.GetIngredientByID(selectedIngredientID[i]).imageFilePath;
                SetButtonImage(imagePath, displayButtonImage);

                int currentIndex = i;
                displayButton.GetComponent<Button>().onClick.AddListener(() => RemoveSelectedIngredients(currentIndex));
            }
            toUpdateDisplayButtons = false;

        }
    }

    private void UpdateButtonVisuals()
    {
        TextMeshProUGUI confirm = confirmationButton.GetComponentInChildren<TextMeshProUGUI>();
        if(selectedIngredientID.Count>0 && droneAvailable)
        {
            if(textCoroutine!=null)
            {
                textCoroutine = null;
            }

            confirm.text = $"Confirm ({2*selectedIngredientID.Count}s)";
        }
        else if(selectedIngredientID.Count<=0)
        {
            if(textCoroutine!=null)
            {
                textCoroutine = null;
            }
            confirm.text = "Confirm";
        }
        else if(!droneAvailable)
        {
            if(textCoroutine==null)
            {
                textCoroutine = droneMenuController.UITextForRestock(this, confirm);
            }
        }
        
        Image confirmBtnImage = GameObject.FindWithTag("RestockConfirmationButton").GetComponent<Image>();
        if(selectedIngredientID.Count >0 && droneAvailable)
        {
            
            SetButtonImage("drone menu/Drone Menu/orangebutton", confirmBtnImage);
        }
        else
        {
            SetButtonImage("drone menu/Drone Menu/greybutton", confirmBtnImage);
        }

        if(restockCompleted)
        {
            SetButtonImage("drone menu/Drone Menu/orangebutton", confirmBtnImage);

            TextMeshProUGUI receiveText = confirmationButton.GetComponentInChildren<TextMeshProUGUI>();
            receiveText.text = $"Receive Stock ({selectedIngredientID.Count})";
        }
    }

    public void SendDroneForRestock()
    {
        if(selectedIngredientID.Count>0 && droneAvailable)
        {
            droneAvailable = false;
            droneMenuController.SendDroneOut(this, selectedIngredientID, timerCountdown, exclaimationPrefab);
            int random = Random.Range(0, clickButtonSounds.Count);
            SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
        }
        else if(!droneAvailable)
        {
            Debug.Log("drone is busy!");
            int random = Random.Range(0, clickButtonSounds.Count);
            SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
        }
    }


    public Vector3 TimerPos()
    {
        return droneStation.gameObject.transform.position;
    }

    public Vector3 ExclaimationPos()
    {
        return TimerPos() + new Vector3(0, 1.25f, 0);
    }

    public void RestockIngredients()
    {
        for(int i =0; i<allStockStations.Count;i++)
        {
            string id = null;
            if(allStockStations[i].stockSO.objType == "Ingredient")
            {
                id = allStockStations[i].stockSO.ingredientID;
            }

            for(int j =0; j<selectedIngredientID.Count;j++)
            {
                if(selectedIngredientID[j] == id)
                {
                    allStockStations[i].stockCount++;
                }
            }
            // Debug.Log("replenishing: " + allStockStations[i].name + " number replenished: " + allStockStations[i].stockCount);
        }
        foreach (var displayButton in displayButtons)
        {
            Destroy(displayButton);
        }
        displayButtons.Clear(); 

        // Debug.Log($"Adding to overload count! Adding: {selectedIngredientID.Count}");
        overloadBar.IncreaseOverloadValue(selectedIngredientID.Count);
        
        // Debug.Log($"Clearing ingredients soon! currentt count: {selectedIngredientID.Count}");

        selectedIngredientID.Clear();
        // Debug.Log($"ingredients clears! currentt count: {selectedIngredientID.Count}");
        restockCompleted = false;
        droneAvailable = true;

        Image droneExclaimation = GameObject.Find("droneUI").GetComponentInChildren<Image>();
        Destroy(droneExclaimation.gameObject);

        int random = Random.Range(0, clickButtonSounds.Count);
        SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
    }

    private void SetButtonImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    public void ClearDisplayButtons()
    {
        if(droneAvailable)
        {
            selectedIngredientID.Clear();
            foreach(var button in displayButtons)
            {
                Destroy(button);
            }
            displayButtons.Clear();
        }
    }


}
