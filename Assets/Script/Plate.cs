using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private List<string> ingredientsOnPlateIDs = new List<string>(); //store ingredient ids on plate
    private List<GameObject> ingredientsOnPlate = new List<GameObject>(); 
    public bool readyToServe = false;
    public Recipe currentRecipe;
    
    public void PlaceIngredient(GameObject ingredient)
    {
        if(!ingredientsOnPlate.Contains(ingredient) && ingredient.GetComponent<IngredientManager>().ingredientSO.isReady)
        {
            ingredientsOnPlateIDs.Add(ingredient.GetComponent<IngredientManager>().ingredientSO.ingredientID);

            Destroy(ingredient); //no need for the ingredient anynmore --> destroy (prevent player from interacting with it again)
            CheckRecipe();
            
        }
    }

    private void CheckRecipe()
    {
        if(Game.GetRecipeList()!=null)
        {
            foreach(Recipe recipe in Game.GetRecipeList())
            {
                if(MatchRecipe(recipe.ingredientIDs))
                {
                    currentRecipe = recipe;
                    readyToServe = true;
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Recipe list is null");
        }


    }

    private bool MatchRecipe(string[] recipe)
    {
        Dictionary<string, int> plateIngredients = ingredientsOnPlateIDs.GroupBy(id => id).ToDictionary(id => id.Key, id => id.Count()); //count how many of the same ingredients are on the plate

        Dictionary<string, int> recipeIngredients = recipe.GroupBy(id => id).ToDictionary(id => id.Key, id => id.Count()); //how many of the same ingredient is required for the recipe

        // Compare between dictionaries
        // first compare the counts of separate ingredientID groups in both dictionaries 
        // after which compare the values key by key

        if(plateIngredients.Count != recipeIngredients.Count)
        {
            return false;
        }
        else
        {
            if(plateIngredients.All(pair => recipeIngredients.ContainsKey(pair.Key) && recipeIngredients[pair.Key] == pair.Value))
            {
                return true;
            }
        }
        return false;

    }
}
