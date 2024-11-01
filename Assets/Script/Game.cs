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

    private static Levels level;
    private static List<Levels> levelList;

    private static GeneralDialogue generalDialogue;
    private static List<GeneralDialogue> generalDialogueList;    
    private static PlayerResponse playerResponse;
    private static List<PlayerResponse> playerResponseList;

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

    public static Ingredient GetIngredientByOriginalID(string originalID)
    {
        return ingredientList.Find(i => i.originalStateID == originalID);
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

    public static List<Recipe> GetUnlockedRecipeListByScenes(string sceneName)
    {
        List<Recipe> unlockedRecipes = new List<Recipe>();
        List<Recipe> allRecipes = GetRecipeList();
        for(int i =0; i<allRecipes.Count; i++)
        {
            if(allRecipes[i].unlockedInScenes.Contains(sceneName))
            {
                unlockedRecipes.Add(allRecipes[i]);
            }
        }
        return unlockedRecipes;
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

    public static PlateGraphics GetPlateGraphicsByIngredientIDs(List<PlateGraphics> graphics, string id)
    {
        return graphics.Find(i => i.ingredientIDs.Contains(id));
    }

    public static List<PlateGraphics> GetPlateGraphicsByPlateType(string plateType)
    {
        List<PlateGraphics> plateGraphics = new List<PlateGraphics>();
        List<PlateGraphics> allPlateGraphics = GetPlateGraphicsList();
        for(int i =0; i<allPlateGraphics.Count; i++)
        {
            if(allPlateGraphics[i].plateTyping == plateType)
            {
                plateGraphics.Add(allPlateGraphics[i]);
            }
        }
        return plateGraphics;
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
    #region levels

    public static Levels GetLevel()
    {
        return level;
    }

    public static void SetLevel(Levels aLevel)
    {
        level = aLevel;
    }

    public static void SetLevelList(List<Levels> aList)
    {
        levelList = aList;
    }

    public static List<Levels> GetLevelList()
    {
        return levelList;
    }

    public static Levels GetLevelByName(string name)
    {
        return levelList.Find(i => i.levelName == name);
    }

    public static Levels GetNextLevel(string currentLevelName)
    {
        int index = levelList.IndexOf(GetLevelByName(currentLevelName));
        return levelList[index + 1];
    }

    #endregion levels

    #region general dialogue

    public static GeneralDialogue GetGeneralDialogue()
    {
        return generalDialogue;
    }

    public static void SetGeneralDialogue(GeneralDialogue aDialogue)
    {
        generalDialogue = aDialogue;
    }

    public static void SetGeneralDialogueList(List<GeneralDialogue> aList)
    {
        generalDialogueList = aList;
    }

    public static List<GeneralDialogue> GetGeneralDialogueList()
    {
        return generalDialogueList;
    }

    public static List<GeneralDialogue> GetGeneralDialoguesByScene(string name)
    {
        List<GeneralDialogue> generalDialogues = new List<GeneralDialogue>();
        List<GeneralDialogue> allGeneralDialogues = GetGeneralDialogueList();
        for(int i =0; i<allGeneralDialogues.Count; i++)
        {
            if(allGeneralDialogues[i].sceneName.Contains(name))
            {
                generalDialogues.Add(allGeneralDialogues[i]);
            }
        }
        return generalDialogues;
    }

    public static List<GeneralDialogue> GetDialogueByResponseID(string id)
    {
        List<GeneralDialogue> dialogues = new List<GeneralDialogue>();
        List<GeneralDialogue> allDialogues = GetGeneralDialogueList();

        for(int i =0; i<allDialogues.Count; i++)
        {
            if(allDialogues[i].optionResponseID == id)
            {
                dialogues.Add(allDialogues[i]);
            }
        }
        // Debug.Log($"number of dialogues under {id}: {dialogues.Count}");
        return dialogues;
    }


    #endregion general dialogue

    #region player response dialogue

    public static PlayerResponse GetPlayerResponse()
    {
        return playerResponse;
    }

    public static void SetPlayerResponse(PlayerResponse aDialogue)
    {
        playerResponse = aDialogue;
    }

    public static void SetPlayerResponseList(List<PlayerResponse> aList)
    {
        playerResponseList = aList;
    }

    public static List<PlayerResponse> GetPlayerResponseList()
    {
        return playerResponseList;
    }

    public static List<PlayerResponse> GetPlayerResponsesByTriggerID(string id)
    {
        List<PlayerResponse> playerResponses = new List<PlayerResponse>();
        List<PlayerResponse> allPlayerResponses = GetPlayerResponseList();
        for(int i =0; i<allPlayerResponses.Count; i++)
        {
            if(allPlayerResponses[i].triggerID.Contains(id))
            {
                playerResponses.Add(allPlayerResponses[i]);
            }
        }
        return playerResponses;
    }

    public static List<PlayerResponse> GetPlayerResponsesInScene(string currentScene)
    {
        List<PlayerResponse> playerResponses = new List<PlayerResponse>();
        List<PlayerResponse> allPlayerResponses = GetPlayerResponseList();
        for(int i =0; i<allPlayerResponses.Count; i++)
        {
            if(allPlayerResponses[i].currentSceneName.Contains(currentScene))
            {
                playerResponses.Add(allPlayerResponses[i]);
            }
        }
        return playerResponses;
    }

    #endregion player response dialogue

}
