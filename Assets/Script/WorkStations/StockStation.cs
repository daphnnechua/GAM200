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
            IngredientManager ingredientManager = newObj.GetComponent<IngredientManager>();
            ingredientManager.SetImage(ingredientManager.ingredientSO.imageName);

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
        else if(stockSO.objType == " Plate")
        {
            stockCount = 999;
        }
    }



}
