using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Game
{
    private static Ingredient ingredient;
    private static List<Ingredient> ingredientList;

    private static Actions cookingAction;
    private static List<Actions> cookingActionList;


    private static Stations workstation;
    private static List<Stations> workstationList;

    private static Recipe recipe;
    private static List<Recipe> recipeList;

    private static Minigames minigames;
    private static List<Minigames> minigameList;

    private static PlateGraphics plateGraphics;
    private static List<PlateGraphics> plateGraphicsList;

    private static StockStation stockStation;
    private static List<StockStation> stockStationList;

    #region ingredient related
    public static Ingredient GetIngredient()
    {
        return ingredient;
    }

    public static Ingredient GetIngredientByID(string ingredientID)
    {
        return ingredientList.Find(i => i.id == ingredientID);
    }

    public static void SetIngredient(Ingredient aIngredient)
    {
        ingredient = aIngredient;
    }

    public static void SetIngredientList(List<Ingredient> aList)
    {
        ingredientList = aList;
    }

    public static List<Ingredient> GetIngredientList()
    {
        return ingredientList;
    }

    public static Ingredient GetIngredientByPrevStateID(string prevID)
    {
        return ingredientList.Find(i => i.prevStateID == prevID);
    }

    #endregion ingredient related

    #region cooking actions

    public static Actions GetCookingAction()
    {
        return cookingAction;
    }

    public static void SetCookingAction(Actions aAction)
    {
        cookingAction = aAction;
    }

    public static void SetCookingActionList(List<Actions> aList)
    {
        cookingActionList = aList;
    }

    public static List<Actions> GetCookingActionList()
    {
        return cookingActionList;
    }

    public static Actions GetActionByStationID(string stationID)
    {
        return cookingActionList.Find(i => i.workstationID == stationID);
    }

    #endregion cooking actions

    #region workstations

    public static Stations GetWorkStation()
    {
        return workstation;
    }

    public static void SetWorkStation(Stations aStation)
    {
        workstation = aStation;
    }

    public static void SetWorkStationList(List<Stations> aList)
    {
        workstationList = aList;
    }

    public static List<Stations> GetWorkStationList()
    {
        return workstationList;
    }

    public static Stations GetStationByActionID(string actionID)
    {
        return workstationList.Find(i => i.actionID == actionID);
    }

    #endregion workstations

    #region recipes

    public static Recipe GetRecipe()
    {
        return recipe;
    }

    public static void SetRecipe(Recipe aRecipe)
    {
        recipe = aRecipe;
    }

    public static void SetRecipeList(List<Recipe> aList)
    {
        recipeList = aList;
    }

    public static List<Recipe> GetRecipeList()
    {
        return recipeList;
    }

    public static Recipe GetRecipeByID(string ID)
    {
        return recipeList.Find(i => i.recipeID == ID);
    }

    #endregion recipes

    #region minigame

    public static Minigames GetMinigames()
    {
        return minigames;
    }

    public static void SetMinigame(Minigames aMinigame)
    {
        minigames = aMinigame;
    }

    public static void SetMinigameList(List<Minigames> aList)
    {
        minigameList = aList;
    }

    public static List<Minigames> GetMinigameList()
    {
        return minigameList;
    }

    public static Minigames GetMinigameByID(string ID)
    {
        return minigameList.Find(i => i.minigameID == ID);
    }

    #endregion minigame
    #region  plate graphics
    public static PlateGraphics GetPlateGraphics()
    {
        return plateGraphics;
    }

    public static void SetPlateGraphics(PlateGraphics aGraphics)
    {
        plateGraphics = aGraphics;
    }

    public static void SetPlateGraphicsList(List<PlateGraphics> aList)
    {
        plateGraphicsList = aList;
    }

    public static List<PlateGraphics> GetPlateGraphicsList()
    {
        return plateGraphicsList;
    }

    public static PlateGraphics GePlateGraphicsByRecipeID(string ID)
    {
        return plateGraphicsList.Find(i => i.recipeID == ID);
    }

    public static PlateGraphics GetPlateGraphicsByIngredientIDs(string id)
    {
        return plateGraphicsList.Find(i => i.ingredientIDs.Contains(id));
    }

    #endregion plate graphics

    #region stock station 

    public static StockStation GetStockStation()
    {
        return stockStation;
    }

    public static void SetStockStation(StockStation aStockStation)
    {
        stockStation = aStockStation;
    }

    public static void SetStockStationList(List<StockStation> aList)
    {
        stockStationList = aList;
    }

    public static List<StockStation> GetStockStationList()
    {
        return stockStationList;
    }

    public static StockStation GetStockStationByID(string ID)
    {
        return stockStationList.Find(i => i.stockStationID == ID);
    }

    public static StockStation GetStockStationByIngredientID(string id)
    {
        return stockStationList.Find(i => i.ingredientID.Contains(id));
    }


    #endregion stock station
}
