using System;
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
    public List<Minigames> minigamesList = new List<Minigames>();

    public List<PlateGraphics> plateGrahpicsList = new List<PlateGraphics>();
    public List<StockStation> stockStationsList = new List<StockStation>();

    public List<Levels> levelsList = new List<Levels>();

    public List<GeneralDialogue> generalDialogueList = new List<GeneralDialogue>();

    public List<PlayerResponse> playerResponseList = new List<PlayerResponse>();

    public List<Stars> starsList = new List<Stars>();

    public void LoadAllData() //called at the start of game 
    {
        LoadIngredients();
        LoadCookingActions();
        LoadWorkstations();
        LoadRecipes();
        LoadMinigames();
        LoadPlateGraphics();
        LoadStockStations();
        LoadLevels();
        LoadGeneralDialogue();
        LoadPlayerResponse();
        LoadStars();
    }

    #region Ingredients

    public void LoadIngredients() 
    {
        string filePath = Application.streamingAssetsPath + "/Ingredients.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            refIngredient refData = new refIngredient();
            refData.causeID = columnData[0];
            refData.originalStateID = columnData[1];
            refData.prevStateID = columnData[2];
            refData.IngredientID = columnData[3];
            refData.name = columnData[4];
            refData.canCUt = columnData[5].ToLower() == "true";
            refData.canFry = columnData[6].ToLower() == "true";
            refData.canBoil = columnData[7].ToLower() == "true";
            refData.isReady = columnData[8].ToLower() == "true";
            refData.prefabPath = columnData[9];
            refData.imageFilePath = columnData[10];
            refData.plateTyping = columnData[11];

            Ingredient ingredient = new Ingredient(refData.causeID, refData.originalStateID, refData.prevStateID, refData.IngredientID, refData.name, refData.canCUt, refData.canFry, refData.canBoil, refData.isReady, refData.prefabPath, refData.imageFilePath, refData.plateTyping);

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
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefRecipe refData = new RefRecipe();
            refData.recipeID = columnData[0];
            refData.recipeName = columnData[1];
            refData.ingredientIDs = columnData[2].Split('@');
            refData.reward = int.Parse(columnData[3]);
            refData.unlockedInScenes = columnData[4].Split('@');
            refData.imageFilePath = columnData[5];
            refData.penalty = int.Parse(columnData[6]);

            Recipe recipe = new Recipe(refData.recipeID, refData.recipeName, refData.ingredientIDs, refData.reward, refData.unlockedInScenes, refData.imageFilePath, refData.penalty);

            recipes.Add(recipe);

            Game.SetRecipeList(recipes);

        }
    }

    #endregion recipes

    #region minigames

    public void LoadMinigames()
    {
        string filePath = Application.streamingAssetsPath + "/Minigame.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefMinigames refData = new RefMinigames();
            refData.minigameID = columnData[0];
            refData.minigameName = columnData[1];
            refData.filePath = columnData[2];

            Minigames minigames = new Minigames(refData.minigameID, refData.minigameName, refData.filePath);

            minigamesList.Add(minigames);

            Game.SetMinigameList(minigamesList);

        }
    }

    #endregion minigames
    #region plate graphics

    public void LoadPlateGraphics()
    {
        string filePath = Application.streamingAssetsPath + "/Plate_Graphics.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefPlateGraphics refData = new RefPlateGraphics();
            refData.recipeID = columnData[0];
            refData.ingredientIDs = columnData[1].Split('@');
            refData.imageFilePath = columnData[2];
            refData.plateTyping = columnData[3];

            PlateGraphics graphics = new PlateGraphics(refData.recipeID, refData.ingredientIDs, refData.imageFilePath, refData.plateTyping);

            plateGrahpicsList.Add(graphics);

            Game.SetPlateGraphicsList(plateGrahpicsList);

        }
    }
    #endregion plate graphics

    #region stock station
    public void LoadStockStations()
    {
        string filePath = Application.streamingAssetsPath + "/StockStation.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefStockStation refData = new RefStockStation();
            refData.stockStationID = columnData[0];
            refData.stockStationName = columnData[1];
            refData.ingredientID = columnData[2];
            refData.imageFilePath = columnData[3];
            refData.emptyImagePath = columnData[4];
            refData.indicatorImage = columnData[5];

            StockStation stockStations = new StockStation(refData.stockStationID, refData.stockStationName, refData.ingredientID, refData.imageFilePath, refData.emptyImagePath, refData.indicatorImage);

            stockStationsList.Add(stockStations);

            Game.SetStockStationList(stockStationsList);

        }
    }


    #endregion stock station

    #region Levels
    public void LoadLevels()
    {
        string filePath = Application.streamingAssetsPath + "/Levels.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefLevels refData = new RefLevels();
            refData.levelNumber = columnData[0];
            refData.levelName = columnData[1];
            refData.description = columnData[2];
            refData.levelType = columnData[3];

            Levels level = new Levels(refData.levelNumber, refData.levelName, refData.description, refData.levelType);

            levelsList.Add(level);

            Game.SetLevelList(levelsList);

        }
    }

    #endregion Levels

    #region dialogue
    public void LoadGeneralDialogue()
    {
        string filePath = Application.streamingAssetsPath + "/General Dialogue.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefGeneralDialogue refData = new RefGeneralDialogue();
            refData.dialogueID = columnData[0];
            refData.dialogue   = columnData[1];
            refData.dialogueBy = columnData[2];
            refData.isDialogueSelection = columnData[3].ToLower() == "true";
            refData.optionResponseID = columnData[4];
            refData.sceneName = columnData[5];
            refData.leftSpriteFilePath = columnData[6];
            refData.rightSpriteFilePath = columnData[7];
            refData.tutorialImage = columnData[8];
            refData.repeatDialogue = columnData[9].ToLower() == "true";
            refData.isLeftSpeaker = columnData[10].ToLower() == "true";
            refData.toCloseDialogue = columnData[11].ToLower() == "true";


            GeneralDialogue dialogue = new GeneralDialogue(refData.dialogueID, refData.dialogue, refData.dialogueBy, refData.isDialogueSelection, refData.optionResponseID, refData.sceneName, refData.leftSpriteFilePath, refData.rightSpriteFilePath, refData.tutorialImage, refData.repeatDialogue, refData.isLeftSpeaker, refData.toCloseDialogue);

            generalDialogueList.Add(dialogue);

            Game.SetGeneralDialogueList(generalDialogueList);

        }
    }

    public void LoadPlayerResponse()
    {
        string filePath = Application.streamingAssetsPath + "/Player Dialogue Response.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefPlayerResponse refData = new RefPlayerResponse();
            refData.triggerID = columnData[0].Split('@');
            refData.playerDialogueID = columnData[1];
            refData.dialogue = columnData[2];
            refData.dialogueType = columnData[3];
            refData.nextSceneName = columnData[4];
            refData.currentSceneName = columnData[5].Split('@');


            PlayerResponse response = new PlayerResponse(refData.triggerID, refData.playerDialogueID, refData.dialogue, refData.dialogueType, refData.nextSceneName, refData.currentSceneName);

            playerResponseList.Add(response);

            Game.SetPlayerResponseList(playerResponseList);

        }
    }


    #endregion dialogue

    #region star scoring

    public void LoadStars()
    {
        string filePath = Application.streamingAssetsPath + "/Stars.csv";
        string [] fileData =  File.ReadAllLines(filePath);

        for(int i =1 ; i<fileData.Length; i++)
        {
            string[] columnData = fileData[i].Split(new char[] {';'});

            RefStars refData = new RefStars();
            refData.levelName = columnData[0];
            refData.levelType = columnData[1];
            refData.availableStars = int.Parse(columnData[2]);
            refData.pointsRequired = Array.ConvertAll(columnData[3].Split('@'), int.Parse);


            Stars stars = new Stars(refData.levelName, refData.levelType, refData.availableStars, refData.pointsRequired);

            starsList.Add(stars);

            Game.SetStarsList(starsList);

        }
    }
    #endregion star scoring
}
