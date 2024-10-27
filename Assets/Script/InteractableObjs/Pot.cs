using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pot : MonoBehaviour
{
    [SerializeField] private float cookingTime = 5f;
    private float prepProgress;
    public bool startedPrep = false;

    private bool hasUndergonePrep = false; 

    public GameObject progressBar;

    public List<string> ingredientIDs = new List<string>(); //store ingredient ids on pot
    private List<GameObject> ingredientsInPot = new List<GameObject>(); 
    public bool isReadyToCook = false;

    public bool isDoneCooking = false;
    public Recipe currentRecipe;
    private GameController gameController;
    private OrderManager orderManager;

    private StockStationManager stockStationManager;
    private GameObject potUI;
    [SerializeField] private bool onStove = false;

    public bool isHoldingPot = false;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        orderManager = FindObjectOfType<OrderManager>();
        stockStationManager = FindObjectOfType<StockStationManager>();

        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(onStove && isReadyToCook)
        {
            if(!hasUndergonePrep && ingredientIDs.Count == 3) //only start cooking when number of ingredients in pot is 3
            {
                SpawnProgressBar();
            }
        }

        if(progressBar!=null)
        {
            progressBar.transform.position = ProgressBarPos();
            if(onStove)
            {
                UpdateCookingProgressBar();
            }
        }


        if(potUI !=null)
        {
            potUI.transform.position = PotUIPos();

            if(isHoldingPot)
            {
                potUI.SetActive(false);
            }
            else
            {
                potUI.SetActive(true);
            }
        }
    }

    public void PlaceIngredientInPot(GameObject ingredient)
    {
        if(ingredientIDs.Count<3) 
        {
            
            if(!ingredientsInPot.Contains(ingredient) && ingredient.GetComponent<IngredientManager>().ingredientSO.canBoil)
            {
                
                ingredientIDs.Add(ingredient.GetComponent<IngredientManager>().ingredientSO.ingredientID);
                // LoadPotGraphics();
                SpawnPotUI();
                isReadyToCook = true;

                Destroy(ingredient); //no need for the ingredient anynmore --> destroy (prevent player from interacting with it again)
                Debug.Log($"placed ingredient. ingredients in pot: {ingredientsInPot.Count}");
                
            }
        }
    }

    public void CookingComplete()
    {
        if(ingredientIDs.Count==3)
        {
            Ingredient cookedIngredient = Game.GetIngredientByID(ingredientIDs[0]);

            ingredientIDs[0] = cookedIngredient.id;
            isReadyToCook = false;
            isDoneCooking = true;
            Debug.Log("Soup is cooked!");
        }

    }

    public void PlaceSoupInPlate(Plate plateScript)
    {
        Debug.Log("Placing soup...");
        if(plateScript.ingredientsOnPlateIDs.Count ==0)
        {
            for(int i =0; i<ingredientIDs.Count;i++)
            {
                plateScript.ingredientsOnPlateIDs.Add(ingredientIDs[i]);
            }
            ingredientIDs.Clear();
            Debug.Log("soup placed in plate");
            Debug.Log(ingredientIDs.Count);

            isDoneCooking = false;
            isReadyToCook = false;
            hasUndergonePrep = false;

            // plateScript.LoadPlateGraphics();
            plateScript.SpawnPlateUI();
            // LoadPotGraphics();
            SpawnPotUI();
        }
    }

    public void TrashFoodInPot()
    {
        ingredientsInPot.Clear();
        ingredientIDs.Clear();
        // LoadPotGraphics();
        Destroy(potUI);

        Debug.Log($"pot reset! ingredients on plate: {ingredientsInPot.Count}, ids: {ingredientIDs.Count}");
    }


    //display cooking status
    public void SpawnProgressBar()
    {
        if(progressBar == null && !hasUndergonePrep && !isDoneCooking)
        {
            AssetManager.LoadPrefab("UI/Progress bar", (GameObject prefab) =>
            {
                progressBar= Instantiate(prefab, GameObject.Find("Canvas").transform);
                progressBar.transform.SetAsFirstSibling();

                Slider slider = progressBar.GetComponent<Slider>();
                prepProgress = cookingTime;
                slider.value = 1;

            });
            hasUndergonePrep = true;
        }
    }

    private Vector3 ProgressBarPos()
    {
        return transform.position + new Vector3(0, -0.75f, 0); //below the pot
    }

    public void UpdateCookingProgressBar()
    {
        if(progressBar!=null)
        {
            Slider slider = progressBar.GetComponent<Slider>();
            prepProgress -= Time.deltaTime;
            slider.value = prepProgress/cookingTime;

            if(prepProgress<=0) //finish cooking
            {
                CookingComplete();
                Debug.Log(ingredientIDs[0]);
                Destroy(progressBar);
                hasUndergonePrep = false;
            }
        }
    }


    // private void LoadPotGraphics()
    // {
    //     List<PotGraphics> potGraphicsList = Game.GetPotGraphicsList();

    //     if(ingredientIDs.Count <=0)
    //     {
    //         PotGraphics emptyPlate = Game.GetPotGraphicsByIngredientIDs("null");
    //         string filePath = emptyPlate.imageFilePath;
    //         AssetManager.LoadSprite(filePath, (Sprite sp) =>
    //         {
    //             this.GetComponent<SpriteRenderer>().sprite = sp;
    //         });
    //         return;
    //     }

    //     foreach (var p in potGraphicsList)
    //     {
    //         List<string> ingredientsNeeded = new List<string>(p.ingredientIDs);
    //         List<string> currentIDs = new List<string>();

    //         foreach(string ids in ingredientIDs)
    //         {
    //             currentIDs.Add(ids);
    //         }
    //         ingredientsNeeded.Sort();
    //         currentIDs.Sort();

    //         if (ingredientsNeeded.SequenceEqual(currentIDs))
    //         {
    //             string filePath = p.imageFilePath;

    //             AssetManager.LoadSprite(filePath, (Sprite sp) =>
    //             {
    //                 this.GetComponent<SpriteRenderer>().sprite = sp;
    //             });
    //             break;
    //         }
    //     }    
    // }

    private void SpawnPotUI()
    {
        Destroy(potUI);

        if(ingredientIDs.Count>0)
        {
            AssetManager.LoadPrefab("UI/Plate_Ingredients", (GameObject prefab) =>
            {
                potUI = Instantiate(prefab, GameObject.Find("Canvas").transform);
                potUI.transform.SetAsFirstSibling();
                GameObject ingredientImages = potUI.transform.Find("Image").gameObject;
                
                if(ingredientIDs.Count>1)
                {
                    List<Image> images = new List<Image>();
                    images.Add(ingredientImages.GetComponent<Image>());
                    for(int i =0; i<ingredientIDs.Count-1;i++)
                    {
                        GameObject image = Instantiate(ingredientImages, potUI.transform);
                        images.Add(image.GetComponent<Image>());
                        Debug.Log(images[i]);
                    }
                    for(int i =0; i<images.Count;i++)
                    {
                        SetPotUIImage(Game.GetIngredientByID(ingredientIDs[i]).imageFilePath, images[i]);
                    }
                }
                else if(ingredientIDs.Count == 1)
                {
                    SetPotUIImage(Game.GetIngredientByID(ingredientIDs[0]).imageFilePath, ingredientImages.GetComponent<Image>());
                }
            });
        }
    }

    private void SetPotUIImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    private Vector3 PotUIPos()
    {
        return transform.position + new Vector3(0, 0.75f, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stove"))
        {
            onStove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stove"))
        {
            onStove = false;
        }
    }
}
