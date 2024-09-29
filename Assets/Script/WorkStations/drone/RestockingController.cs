using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class RestockingController : MonoBehaviour
{
    public List<StockStation> allStockStations;
    public int maxRestockLimit;
    public float restockTime = 2f;

    [SerializeField] private Dictionary<string, Button> ingredientRestockButtons = new Dictionary<string, Button>();

    private Transform restockPos;
    private Transform displayPos;
    private List<GameObject> restockButtons = new List<GameObject>();
    private List<GameObject> displayButtons = new List<GameObject>();
    [SerializeField] private Button confirmationButton;

    public List<string> selectedIngredientID = new List<string>();
    private bool restockButtonsActive = false;
    private bool toUpdateDisplayButtons = false;
    public bool droneAvailable = true;

    [SerializeField] private OverloadBar overloadBar;
    [SerializeField] private TabController tabController;
    private DroneStation droneStation;

    // Start is called before the first frame update
    void Start()
    {
        droneStation = FindObjectOfType<DroneStation>();
        tabController = FindObjectOfType<TabController>();

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
    }

    public void InitializeRestockingController()
    {
        tabController = FindObjectOfType<TabController>();
        overloadBar = FindObjectOfType<OverloadBar>();
        displayPos = GameObject.FindWithTag("IngredientsDisplay").transform;
        restockPos = GameObject.FindWithTag("IngredientRestockButton").transform;
        allStockStations = FindObjectsOfType<StockStation>().ToList();
        for(int i =0; i<allStockStations.Count;i++)
        {
            if(allStockStations[i].stockSO.objType == "Plate")
            {
                allStockStations.RemoveAt(i);
            }
        }

        UpdateButtons();

        

        confirmationButton = GameObject.FindGameObjectWithTag("RestockConfirmationButton").GetComponent<Button>();
        confirmationButton.onClick.AddListener(()=>SendDroneForRestock());
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
        string prefabPath = "RestockBtn";

        if(!restockButtonsActive)
        {
             AssetManager.LoadPrefab(prefabPath, (prefab) =>
            {
                if (prefab != null)
                {
                    foreach (var e in allStockStations)
                    {
                        GameObject restockButton = Instantiate(prefab, restockPos);
                        restockButtons.Add(restockButton);
                    }
                    restockButtonsActive = true;
                    InitializeRestockButtons();
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

            AssetManager.LoadPrefab(prefabPath, (prefab) =>
            {
                if (prefab != null)
                {
                    for (int i = 0; i < selectedIngredientID.Count; i++)
                    {
                        GameObject displayButton = Instantiate(prefab, displayPos);
                        displayButtons.Add(displayButton);

                        int currentIndex = i;
                        displayButton.GetComponent<Button>().onClick.AddListener(() => RemoveSelectedIngredients(currentIndex));
                    }
                    toUpdateDisplayButtons = false;
                }
            });

        }
    }

    private void UpdateButtonVisuals()
    {
        for(int i =0; i<allStockStations.Count;i++) //update button text for restock buttons
        {
            if(allStockStations[i].stockSO.objType == "Ingredient")
            {
                string id = allStockStations[i].stockSO.ingredientID;
                string ingredientName = Game.GetIngredientByID(id).name;
                TextMeshProUGUI restockButtonText = restockButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                restockButtonText.text = ingredientName;
            }
        }

        if(displayButtons.Count>0)
        {
            for(int i =0; i< selectedIngredientID.Count;i++)
            {
                string name = Game.GetIngredientByID(selectedIngredientID[i]).name;
                TextMeshProUGUI displayButtonText = displayButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                displayButtonText.text = name;
            }

        }


        TextMeshProUGUI confirm = confirmationButton.GetComponentInChildren<TextMeshProUGUI>();
        if(selectedIngredientID.Count>0 && droneAvailable)
        {
            confirm.text = $"Confirm ({2*selectedIngredientID.Count}s)";
        }
        else if(selectedIngredientID.Count<=0)
        {
            confirm.text = "Confirm";
        }
        else if(!droneAvailable)
        {
            confirm.text = "Retrieving Ingredients";
        }

        if(selectedIngredientID.Count >0 && droneAvailable)
        {
            GameObject.FindWithTag("RestockConfirmationButton").GetComponent<Image>().color = Color.green;
        }
        else
        {
            GameObject.FindWithTag("RestockConfirmationButton").GetComponent<Image>().color = Color.gray;
        }
    }

    public void InitializeRestockButtons()
    {
        for(int i =0; i<allStockStations.Count;i++)
        {
            if(allStockStations[i].stockSO.objType == "Ingredient")
            {
                string id = allStockStations[i].stockSO.ingredientID;
                if(!ingredientRestockButtons.ContainsKey(id))
                {
                    Button button = restockButtons[i].GetComponent<Button>();
                    ingredientRestockButtons.Add(id, button);

                    button.onClick.AddListener(()=>SelectIngredients(id));
                }
            }
        }
    }

    public void SendDroneForRestock()
    {
        if(selectedIngredientID.Count>0 && droneAvailable)
        {
            droneAvailable = false;
            StartCoroutine(RestockTimer());
        }
        else if(!droneAvailable)
        {
            Debug.Log("drone is busy!");
        }
    }

    private IEnumerator RestockTimer()
    {
        yield return new WaitForSeconds(2f*selectedIngredientID.Count);
        RestockIngredients();
        droneAvailable = true;
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
            Debug.Log("replenishing: " + allStockStations[i].name + " number replenished: " + allStockStations[i].stockCount);
        }
        foreach (var displayButton in displayButtons)
        {
            Destroy(displayButton);
        }
        displayButtons.Clear(); 

        Debug.Log($"Adding to overload count! Adding: {selectedIngredientID.Count}");
        overloadBar.IncreaseOverloadValue(selectedIngredientID.Count);
        
        Debug.Log($"Clearing ingredients soon! currentt count: {selectedIngredientID.Count}");

        selectedIngredientID.Clear();
        Debug.Log($"ingredients clears! currentt count: {selectedIngredientID.Count}");

    }

}
