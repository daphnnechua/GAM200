using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public List<Ingredient> ingredients= new List<Ingredient>();
    public List<Actions> cookingActions = new List<Actions>();
    public List<Stations> workstations = new List<Stations>();

    public List<Recipe> recipes = new List<Recipe>();

    public void LoadAllData() //called at the start of game 
    {
        LoadIngredients();
        LoadCookingActions();
        LoadWorkstations();
        LoadRecipes();
    }

    #region Ingredients

    public void LoadIngredients()
    {
        string filePath = Application.streamingAssetsPath + "/Ingredients.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {','});

            refIngredient refData = new refIngredient();
            refData.causeID = columnData[0];
            refData.prevStateID = columnData[1];
            refData.IngredientID = columnData[2];
            refData.name = columnData[3];
            refData.canCUt = columnData[4].ToLower() == "true";
            refData.canCook = columnData[5].ToLower() == "true";
            refData.isReady = columnData[6].ToLower() == "true";
            refData.prefabPath = columnData[7];

            Ingredient ingredient = new Ingredient(refData.causeID, refData.prevStateID, refData.IngredientID, refData.name, refData.canCUt, refData.canCook, refData.isReady, refData.prefabPath);

            ingredients.Add(ingredient);

            Game.SetIngredientList(ingredients);

        }

    }


    #endregion Ingredients

    #region cooking actions
    public void LoadCookingActions()
    {
        string filePath = Application.streamingAssetsPath + "/Actions.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {','});

            RefActions refData = new RefActions();
            refData.actionID = columnData[0];
            refData.actionName = columnData[1];
            refData.timeRequired = float.Parse(columnData[2]);
            refData.workstationID = columnData[3];

            Actions action = new Actions(refData.actionID, refData.actionName, refData.timeRequired, refData.workstationID);

            cookingActions.Add(action);

            Game.SetCookingActionList(cookingActions);

        }
    }

    #endregion cooking actions

    #region workstations

    public void LoadWorkstations()
    {
        string filePath = Application.streamingAssetsPath + "/Stations.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {','});

            RefStations refData = new RefStations();
            refData.stationID = columnData[0];
            refData.stationName = columnData[1];
            refData.actionID = columnData[2];
            refData.requiredIngredientNumber = int.Parse(columnData[3]);

            Stations station = new Stations(refData.stationID, refData.stationName, refData.actionID, refData.requiredIngredientNumber);

            workstations.Add(station);

            Game.SetWorkStationList(workstations);

        }
    }

    #endregion workstations

    #region recipes

    public void LoadRecipes()
    {
        string filePath = Application.streamingAssetsPath + "/Recipe.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {','});

            RefRecipe refData = new RefRecipe();
            refData.recipeID = columnData[0];
            refData.recipeName = columnData[1];
            refData.ingredientIDs = columnData[2].Split('@');
            refData.reward = int.Parse(columnData[3]);
            refData.expiryTimer = float.Parse(columnData[4]);

            Recipe recipe = new Recipe(refData.recipeID, refData.recipeName, refData.ingredientIDs, refData.reward, refData.expiryTimer);

            recipes.Add(recipe);

            Game.SetRecipeList(recipes);

        }
    }

    #endregion recipes
}
