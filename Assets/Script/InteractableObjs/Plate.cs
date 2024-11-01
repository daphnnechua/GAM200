using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour
{
    public InteractableObjSO interactableObjSO;
    public List<string> ingredientsOnPlateIDs = new List<string>(); //store ingredient ids on plate
    private List<GameObject> ingredientsOnPlate = new List<GameObject>(); 
    public bool readyToServe = false;
    public Recipe currentRecipe;
    private GameController gameController;
    private OrderManager orderManager;

    private StockStationManager stockStationManager;
    private GameObject plateUI;

    public bool isHoldingPlate = false;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        orderManager = FindObjectOfType<OrderManager>();
        stockStationManager = FindObjectOfType<StockStationManager>();
    }

    void Update()
    {
        if(plateUI!=null)
        {
            plateUI.transform.position = PlateUIPos();

            if(isHoldingPlate)
            {
                plateUI.SetActive(false);
            }
            else
            {
                plateUI.SetActive(true);
            }
        }
    }
    
    public void PlaceIngredient(GameObject ingredient)
    {
        if(!ingredientsOnPlate.Contains(ingredient) && ingredient.GetComponent<IngredientManager>().ingredientSO.isReady && ingredientsOnPlateIDs.Count < 3)
        {
            if(interactableObjSO.objType == ingredient.GetComponent<IngredientManager>().ingredientSO.plateTyping)
            {
                ingredientsOnPlateIDs.Add(ingredient.GetComponent<IngredientManager>().ingredientSO.ingredientID);
                LoadPlateGraphics();
                SpawnPlateUI();
                readyToServe = true;

                Destroy(ingredient); //no need for the ingredient anynmore --> destroy (prevent player from interacting with it again)
                CheckRecipe();
            }
            else
            {
                Debug.Log("plate typing does not match!");
            }
            
        }
    }

    public void CheckRecipe()
    {
        if(Game.GetRecipeList()!=null)
        {
            foreach(Recipe recipe in Game.GetRecipeList())
            {
                if(MatchRecipe(recipe.ingredientIDs))
                {
                    currentRecipe = recipe;
                    return;
                }
            }
        }

    }

    public bool MatchRecipe(string[] recipe)
    {
        Dictionary<string, int> plateIngredients = ingredientsOnPlateIDs.GroupBy(id => id).ToDictionary(id => id.Key, id => id.Count()); //count how many of the same ingredients are on the plate

        Dictionary<string, int> recipeIngredients = recipe.GroupBy(id => id).ToDictionary(id => id.Key, id => id.Count()); //how many of the same ingredient is required for the recipe

        // Compare between dictionaries
        // first compare the counts of separate ingredientID groups in both dictionaries 
        // after which compare the values key by key

        if(plateIngredients.Count != recipeIngredients.Count)
        {
            return false;
        }
        else
        {
            if(plateIngredients.All(pair => recipeIngredients.ContainsKey(pair.Key) && recipeIngredients[pair.Key] == pair.Value))
            {
                return true;
            }
        }
        return false;

    }

    public void ServePlate()
    {
        if(readyToServe)
        {
            Recipe orderOfInterest = orderManager.GetCurrentOrder().Recipe;

            if(currentRecipe!=null && orderOfInterest.recipeID == currentRecipe.recipeID)
            {
                //add reward
                // Debug.Log("Recipe matches! Submitted:" + currentRecipe.recipeName + " Order:" + orderOfInterest.recipeName);

                //play successful order sfx

                orderManager.RemoveOrder();
                orderManager.AddBonusTime();
                gameController.AddPoints(currentRecipe.reward);
            }
            else
            {
                //deduct points
                // Debug.Log("Submitted is not matching!");

                //play failed order sfx

                orderManager.RemoveOrder();
                gameController.DeductPoints(5);
            }
        }
        Destroy(plateUI);
        Destroy(gameObject);
        if(stockStationManager.stockSO.stationName == "Plate_Stocking_Station")
        {
            stockStationManager.stockCount++;
            Debug.Log("Plate has returned! Count: " + stockStationManager.stockCount);
        }
    }

    public void LoadPlateGraphics()
    {
        List<PlateGraphics> plateGraphicsList = Game.GetPlateGraphicsList();

        if(ingredientsOnPlateIDs.Count <=0)
        {
            List<PlateGraphics> currentPlateTypeGraphics = Game.GetPlateGraphicsByPlateType(interactableObjSO.objType);

            PlateGraphics emptyPlate = Game.GetPlateGraphicsByIngredientIDs(currentPlateTypeGraphics, "null");

            string filePath = emptyPlate.imageFilePath;
            AssetManager.LoadSprite(filePath, (Sprite sp) =>
            {
                this.GetComponent<SpriteRenderer>().sprite = sp;
            });
            return;
        }

        foreach (var p in plateGraphicsList)
        {
            List<string> ingredientsNeeded = new List<string>(p.ingredientIDs);
            List<string> currentIDs = new List<string>();

            foreach(string ids in ingredientsOnPlateIDs)
            {
                currentIDs.Add(ids);
            }
            ingredientsNeeded.Sort();
            currentIDs.Sort();

            if (ingredientsNeeded.SequenceEqual(currentIDs))
            {
                string filePath = p.imageFilePath;

                AssetManager.LoadSprite(filePath, (Sprite sp) =>
                {
                    this.GetComponent<SpriteRenderer>().sprite = sp;
                });
                break;
            }
        }    
    }

    public void TrashPlate()
    {
        ingredientsOnPlate.Clear();
        ingredientsOnPlateIDs.Clear();
        LoadPlateGraphics();
        Destroy(plateUI);

        Debug.Log($"plate reset! ingredients on plate: {ingredientsOnPlate.Count}, ids: {ingredientsOnPlateIDs.Count}");
    }

    public void SpawnPlateUI()
    {
        Destroy(plateUI);

        if(ingredientsOnPlateIDs.Count>0)
        {
            AssetManager.LoadPrefab("UI/Plate_Ingredients", (GameObject prefab) =>
            {
                plateUI = Instantiate(prefab, GameObject.Find("Canvas").transform);
                plateUI.transform.SetAsFirstSibling();
                GameObject ingredientImages = plateUI.transform.Find("Image").gameObject;
                
                if(ingredientsOnPlateIDs.Count>1)
                {
                    List<Image> images = new List<Image>();
                    images.Add(ingredientImages.GetComponent<Image>());
                    for(int i =0; i<ingredientsOnPlateIDs.Count-1;i++)
                    {
                        GameObject image = Instantiate(ingredientImages, plateUI.transform);
                        images.Add(image.GetComponent<Image>());
                        Debug.Log(images[i]);
                    }
                    for(int i =0; i<images.Count;i++)
                    {
                        Ingredient currentIngredient = Game.GetIngredientByID(ingredientsOnPlateIDs[i]);

                        Ingredient originalIngredient = Game.GetIngredientByOriginalID(currentIngredient.originalStateID);

                        SetPlateUIImage(originalIngredient.imageFilePath, images[i]);
                    }
                }
                else if(ingredientsOnPlateIDs.Count == 1)
                {
                    Ingredient currentIngredient = Game.GetIngredientByID(ingredientsOnPlateIDs[0]);

                    Ingredient originalIngredient = Game.GetIngredientByOriginalID(currentIngredient.originalStateID);

                    SetPlateUIImage(originalIngredient.imageFilePath, ingredientImages.GetComponent<Image>());
                }
            });
        }
    }

    private void SetPlateUIImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    private Vector3 PlateUIPos()
    {
        return transform.position + new Vector3(0, 0.75f, 0);
    }

}
