using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockStation : MonoBehaviour
{
    public StockSO stockSO;
    public int stockCount;

    void Start()
    {
        InitializeCount();
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
            }
            else if(stockSO.objType == "Plate")
            {
                //load plate image here
                PlateGraphics emptyPlate = Game.GetPlateGraphicsByIngredientIDs("null");
                LoadPlateImage(emptyPlate.imageFilePath, newObj);
            }

            stockCount--;
            Debug.Log("Remaining:"+ stockCount);
        }
        else
        {
            Debug.Log(stockCount + " No more ingredients!");
        }
        return newObj;

    }

    private void InitializeCount()
    {
        if(stockSO.objType == "Ingredient")
        {
            stockCount = 5;
        }
        else if(stockSO.objType == "Plate")
        {
            stockCount = 999;
        }
    }

    private void LoadPlateImage(string imagePath, GameObject newObj)
    {
        AssetManager.LoadSprite(imagePath, (Sprite sp) =>
        {
            newObj.GetComponent<SpriteRenderer>().sprite = sp;
        });
    }



}
