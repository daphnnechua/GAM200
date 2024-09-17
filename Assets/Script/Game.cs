using System.Collections;
using System.Collections.Generic;
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

    #region ingredient related
    public static Ingredient GetIngredient()
    {
        return ingredient;
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
}
