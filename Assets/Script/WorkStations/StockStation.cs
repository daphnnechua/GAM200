using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockStationManager : MonoBehaviour
{
    public StockSO stockSO;
    public int stockCount;
    private int checkCount;

    [SerializeField] private GameObject stockStationUIPrefab;

    private GameObject uiPrefab;

    void Start()
    {
        InitializeCount();
    }

    void Update()
    {
        if(checkCount !=stockCount && stockSO.objType == "Ingredient")
        {
            string ingredientID = stockSO.ingredientID;
            StockStation thisStation = Game.GetStockStationByIngredientID(ingredientID);
            LoadStockStationImage(thisStation.imageFilePath);

            if(stockCount <= 0)
            {
                LoadStockStationImage(thisStation.emptyImagePath);
            }


            checkCount = stockCount;
        }



        if(stockSO.objType == "Ingredient")
        {
            //reflect stock count number
            TextMeshProUGUI text  = uiPrefab.GetComponentInChildren<TextMeshProUGUI>(true);

            Image background = uiPrefab.GetComponentInChildren<Image>();

            if(stockCount<=0)
            {
                string ingredientID = stockSO.ingredientID;
                StockStation thisStation = Game.GetStockStationByIngredientID(ingredientID);
                LoadImage(thisStation.indicatorImage, background);
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                string count = $"{stockCount}";
                text.text = count;
                LoadImage("Tables/foodstash stuff/board", background);
            }

        }
    }



    public GameObject GetNewObj()
    {
        GameObject newObj = null;
        if(stockCount>0)
        {
            //instantiate new obj which will be picked up by the player
            newObj = Instantiate(stockSO.prefab);

            if(stockSO.objType == "Ingredient")
            {
                IngredientManager ingredientManager = newObj.GetComponent<IngredientManager>();
                ingredientManager.SetImage(ingredientManager.ingredientSO.imageName);

                int random = Random.Range(0, stockSO.soundSfx.Count);

                SoundFXManager.instance.PlaySound(stockSO.soundSfx[random], transform, 0.5f);
            }
            else
            {
                //load plate image here

                List<PlateGraphics> currentPlateTypeGraphics = Game.GetPlateGraphicsByPlateType(stockSO.typing);

                PlateGraphics emptyPlate = Game.GetPlateGraphicsByIngredientIDs(currentPlateTypeGraphics, "null");
                LoadPlateImage(emptyPlate.imageFilePath, newObj);

                int random = Random.Range(0, stockSO.soundSfx.Count);

                SoundFXManager.instance.PlaySound(stockSO.soundSfx[random], transform, 0.5f);
            }

            stockCount--;
            // Debug.Log("Remaining:"+ stockCount);
        }
        else
        {
            // Debug.Log(stockCount + " No more ingredients!");
        }
        return newObj;

    }

    private void InitializeCount()
    {
        if(stockSO.objType == "Ingredient")
        {
            stockCount = 3;
            checkCount = stockCount;

            //spawn stock station ui
            uiPrefab = Instantiate(stockStationUIPrefab, GameObject.Find("StockStationUI").transform);            

            uiPrefab.transform.position = UIPos();
        }
        else if(stockSO.objType == "Plate")
        {
            stockCount = 999;
            checkCount = stockCount;
        }
    }

    private Vector3 UIPos()
    {
        return transform.position + new Vector3(0, 0.75f, 0);
    }
    private void LoadPlateImage(string imagePath, GameObject newObj)
    {
        AssetManager.LoadSprite(imagePath, (Sprite sp) =>
        {
            newObj.GetComponent<SpriteRenderer>().sprite = sp;
        });
    }

    private void LoadStockStationImage(string imagePath)
    {
        AssetManager.LoadSprite(imagePath, (Sprite sp) =>
        {
            GetComponent<SpriteRenderer>().sprite = sp;
        });
    }

    private void LoadImage(string path, Image image)
    {
        AssetManager.LoadSprite(path, (Sprite sp) =>
        {
            image.sprite = sp;
        });

    }



}
