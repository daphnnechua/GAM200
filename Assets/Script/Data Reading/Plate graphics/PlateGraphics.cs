using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateGraphics
{
    public string recipeID {get;}
    public string[] ingredientIDs {get;}
    public string imageFilePath {get;}

    public PlateGraphics(string recipeID, string[] ingredientIDs, string imageFilePath)
    {
        this.recipeID = recipeID;
        this.ingredientIDs = ingredientIDs;
        this.imageFilePath = imageFilePath;
    }

}
