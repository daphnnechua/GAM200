using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    public string recipeID {get;}
    public string recipeName {get;}
    public string[] ingredientIDs {get;}
    public int reward {get;}
    public float expiryTimer {get;}

    public Recipe(string recipeID, string recipeName, string[] ingredientIDs, int reward, float expiryTimer)
    {
        this.recipeID = recipeID;
        this.recipeName = recipeName;
        this.recipeID = recipeID;
        this.ingredientIDs = ingredientIDs;
        this.reward = reward;
        this.expiryTimer = expiryTimer;
    }
}
