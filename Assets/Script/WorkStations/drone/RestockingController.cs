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
        }
        else if(!droneAvailable)
        {
            Debug.Log("drone is busy");
        }
    }

    private void RemoveSelectedIngredients(int index)
    {
        if(droneAvailable)
        {
            selectedIngredientID.RemoveAt(index);
            toUpdateDisplayButtons = true;
        }
    }

    public void UpdateButtons()
    {
        string restockBtnPath = "UI/RestockBtn";
        
        List<string> ingredientIDs = new List<string>();

        if(!restockButtonsActive)
        {
             AssetManager.LoadPrefab(restockBtnPath, (prefab) =>
            {
                if (prefab != null)
                {
                    for(int i =0; i<allStockStations.Count;i++)
                    {
                        if(allStockStations[i].stockSO.objType == "Ingredient")
                        {
                            GameObject restockButton = Instantiate(prefab, restockPos);
                            restockButtons.Add(restockButton);

                            ingredientIDs.Add(allStockStations[i].stockSO.ingredientID);

                        }
                    }

                    // Debug.Log($"count for ingredient ids: {ingredientIDs.Count}");
                    // Debug.Log($"count for restock buttons: {restockButtons.Count}");

                    for(int i =0; i<ingredientIDs.Count;i++)
                    {
                        string currentIngredientID = ingredientIDs[i]; // Capture the current value
                        // Debug.Log(currentIngredientID);

                        if (!ingredientRestockButtons.ContainsKey(currentIngredientID))
                        {
                            Button button = restockButtons[i].GetComponent<Button>();
                            ingredientRestockButtons.Add(currentIngredientID, button);

                            button.onClick.AddListener(() => SelectIngredients(currentIngredientID));
                        }

                    }
                    restockButtonsActive = true;
                }
            });
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

                int currentIndex = i;
                displayButton.GetComponent<Button>().onClick.AddListener(() => RemoveSelectedIngredients(currentIndex));
            }
            toUpdateDisplayButtons = false;

        }
    }

    private void UpdateButtonVisuals()
    {
        int restockBtnIndex = 0;

        for(int i =0; i<allStockStations.Count;i++) //update button text for restock buttons
        {
            if(allStockStations[i].stockSO.objType == "Ingredient")
            {
                string id = allStockStations[i].stockSO.ingredientID;
                string ingredientName = Game.GetIngredientByID(id).name;
                // Debug.Log($"{i} index, {ingredientName}");
                
                TextMeshProUGUI restockButtonText = restockButtons[restockBtnIndex].GetComponentInChildren<TextMeshProUGUI>();
                restockButtonText.text = ingredientName;

                Image restockButtonImage = restockButtons[restockBtnIndex].transform.Find("Ingredient image").GetComponent<Image>();
                string imagePath = Game.GetIngredientByID(id).imageFilePath;
                SetButtonImage(imagePath, restockButtonImage);
                
                restockBtnIndex++;
            }
        }

        if(displayButtons.Count>0)
        {
            for(int i =0; i< selectedIngredientID.Count;i++)
            {
                string name = Game.GetIngredientByID(selectedIngredientID[i]).name;
                TextMeshProUGUI displayButtonText = displayButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                displayButtonText.text = name;

                Image displayButtonImage = displayButtons[i].transform.Find("Ingredient image").GetComponent<Image>();
                string imagePath = Game.GetIngredientByID(selectedIngredientID[i]).imageFilePath;
                SetButtonImage(imagePath, displayButtonImage);

            }

        }


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
            
            SetButtonImage("UI/confirm_button_green", confirmBtnImage);
        }
        else
        {
            SetButtonImage("UI/confirm_button 1", confirmBtnImage);
        }

        if(restockCompleted)
        {
            SetButtonImage("UI/confirm_button_green", confirmBtnImage);

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
        }
        else if(!droneAvailable)
        {
            Debug.Log("drone is busy!");
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
