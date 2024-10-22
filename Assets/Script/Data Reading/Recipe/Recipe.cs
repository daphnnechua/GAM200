using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    public string recipeID {get;}
    public string recipeName {get;}
    public string[] ingredientIDs {get;}
    public int reward {get;}

    public string[] unlockedInScenes {get;}
    public string imageFilePath{get;}

    public Recipe(string recipeID, string recipeName, string[] ingredientIDs, int reward, string[] unlockedInScenes, string imageFilePath)
    {
        this.recipeID = recipeID;
        this.recipeName = recipeName;
        this.recipeID = recipeID;
        this.ingredientIDs = ingredientIDs;
        this.reward = reward;
        this.unlockedInScenes = unlockedInScenes;
        this.imageFilePath = imageFilePath;
    }
}
