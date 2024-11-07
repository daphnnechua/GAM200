using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FryingPan : MonoBehaviour
{
    public InteractableObjSO interactableObjSO;
    [SerializeField] private float cookingTime = 7f;
    private float prepProgress;
    public bool startedPrep = false;

    private bool hasUndergonePrep = false; 

    public GameObject progressBar;

    public List<string> ingredientIDs = new List<string>(); //store ingredient ids on pan
    private List<GameObject> ingredientsInPan = new List<GameObject>(); 
    public bool isReadyToCook = false;

    public bool isDoneCooking = false;
    public Recipe currentRecipe;
    private GameController gameController;
    private OrderManager orderManager;

    private StockStationManager stockStationManager;
    private GameObject panUI;

    public bool isHoldingFryingPan = false;

    [SerializeField] private bool onStove = false;

    [SerializeField] private List<AudioClip> cookingSound;

    private bool isCookingSoundPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        orderManager = FindObjectOfType<OrderManager>();
        stockStationManager = FindObjectOfType<StockStationManager>();

        GetComponent<Rigidbody2D>().isKinematic = true;

        LoadPanGraphics();
    }

    // Update is called once per frame
    void Update()
    {
        if(onStove && isReadyToCook)
        {
            if(!hasUndergonePrep)
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


        if(panUI !=null)
        {
            panUI.transform.position = PanUIPos();

            if(isHoldingFryingPan)
            {
                panUI.SetActive(false);
            }
            else
            {
                panUI.SetActive(true);
            }
        }

        PlayCookingSound();
    }

    public void PlaceIngredientInPan(GameObject ingredient)
    {
        if(ingredientIDs.Count<1) //only 1 ingredient allow on pan
        {
            if(!ingredientsInPan.Contains(ingredient) && ingredient.GetComponent<IngredientManager>().ingredientSO.canFry)
            {
                ingredientIDs.Add(ingredient.GetComponent<IngredientManager>().ingredientSO.ingredientID);
                LoadPanGraphics();
                SpawnPanUI();

                if(ingredientIDs.Count == 1)
                {
                    isReadyToCook = true;
                }

                Destroy(ingredient); //no need for the ingredient anynmore --> destroy (prevent player from interacting with it again)
                
            }
        }
    }

    public void CookingComplete()
    {
        //replace the ingredient in pan with the cooked ingredient
        if(ingredientIDs.Count==1)
        {
            Ingredient cookedIngredient = Game.GetIngredientByPrevStateID(ingredientIDs[0]);

            ingredientIDs[0] = cookedIngredient.id;
            isReadyToCook = false;
            isDoneCooking = true;
            isCookingSoundPlayed = false;
        }

    }

    public void PlaceFoodInPlate(Plate plateScript)
    {
        if(plateScript.ingredientsOnPlateIDs.Count ==0 && plateScript.interactableObjSO.objType == "burger_plate")
        {
            for(int i =0; i<ingredientIDs.Count;i++)
            {
                plateScript.ingredientsOnPlateIDs.Add(ingredientIDs[i]);
            }
            ingredientIDs.Clear();

            isDoneCooking = false;
            isReadyToCook = false;
            hasUndergonePrep = false;

            plateScript.LoadPlateGraphics();
            plateScript.SpawnPlateUI();
            
            LoadPanGraphics();
            SpawnPanUI();
        }
    }
    public void TrashFoodInPan()
    {
        ingredientsInPan.Clear();
        ingredientIDs.Clear();
        LoadPanGraphics();
        Destroy(panUI);

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
        return transform.position + new Vector3(0, -0.75f, 0); //below the pan
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

    public void LoadPanGraphics()
    {
        List<PlateGraphics> panGraphicsList = Game.GetPlateGraphicsByPlateType(interactableObjSO.objType);

        if(ingredientIDs.Count <=0)
        {
            PlateGraphics emptyPot = Game.GetPlateGraphicsByIngredientIDs(panGraphicsList, "null");

            string filePath = emptyPot.imageFilePath;
            AssetManager.LoadSprite(filePath, (Sprite sp) =>
            {
                this.GetComponent<SpriteRenderer>().sprite = sp;
            });
            return;
        }

        foreach (var p in panGraphicsList)
        {
            List<string> ingredientsNeeded = new List<string>(p.ingredientIDs);
            List<string> currentIDs = new List<string>();

            foreach(string ids in ingredientIDs)
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

    private void SpawnPanUI()
    {
        Destroy(panUI);

        if(ingredientIDs.Count>0)
        {
            AssetManager.LoadPrefab("UI/Plate_Ingredients", (GameObject prefab) =>
            {
                panUI = Instantiate(prefab, GameObject.Find("Canvas").transform);
                panUI.transform.SetAsFirstSibling();
                GameObject ingredientImages = panUI.transform.Find("Image").gameObject;
                
                if(ingredientIDs.Count>1)
                {
                    List<Image> images = new List<Image>();
                    images.Add(ingredientImages.GetComponent<Image>());
                    for(int i =0; i<ingredientIDs.Count-1;i++)
                    {
                        GameObject image = Instantiate(ingredientImages, panUI.transform);
                        images.Add(image.GetComponent<Image>());
                        Debug.Log(images[i]);
                    }
                    for(int i =0; i<images.Count;i++)
                    {
                        Ingredient currentIngredient = Game.GetIngredientByID(ingredientIDs[i]);

                        Ingredient originalIngredient = Game.GetIngredientByOriginalID(currentIngredient.originalStateID);

                        SetPanUIImage(originalIngredient.imageFilePath, images[i]);
                    }
                }
                else if(ingredientIDs.Count == 1)
                {
                    Ingredient currentIngredient = Game.GetIngredientByID(ingredientIDs[0]);

                    Ingredient originalIngredient = Game.GetIngredientByOriginalID(currentIngredient.originalStateID);

                    SetPanUIImage(originalIngredient.imageFilePath, ingredientImages.GetComponent<Image>());
                }
            });
        }
    }

    private void SetPanUIImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    private Vector3 PanUIPos()
    {
        return transform.position + new Vector3(0, 0.75f, 0);
    }

    private bool isOnStove()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Stove"))
            {
                return true;
            }
        }

        return false;
    }

    private void PlayCookingSound()
    {
        if(isOnStove() && !isCookingSoundPlayed && !isDoneCooking && isReadyToCook)
        {
            int random = Random.Range(0, cookingSound.Count);

            SoundFXManager.instance.PlaySound(cookingSound[random], transform, 0.5f);

            isCookingSoundPlayed = true;
        }
        else if(!isOnStove())
        {
            isCookingSoundPlayed = false;
        }
    }

}
