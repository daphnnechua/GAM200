using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Recipe> activeRecipe = new List<Recipe>();
    public float spawnInterval = 10f;
    public float expiryInterval = 25f;

    private WaitForSeconds generationTimer;
    private WaitForSeconds expiryTimer;
    private GameController gameController;

    private bool hasBeenInitialized = false;
    private bool hasCoroutineBeenStarted = false;

    public bool toUpdateOrderUI = false;

    public OrderUI orderUI;
    
    // Start is called before the first frame update
    void Start()
    {
        generationTimer = new WaitForSeconds(spawnInterval);
        expiryTimer = new WaitForSeconds(expiryInterval);
        gameController = FindObjectOfType<GameController>();
        orderUI = FindObjectOfType<OrderUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.gameStart && !hasBeenInitialized)
        {
            GenerateNewOrder();
            hasBeenInitialized = true;

            if(!hasCoroutineBeenStarted)
            {
                StartCoroutine(orderGenerationTimer());
                hasCoroutineBeenStarted = true;
            }
        }
        
 
        for(int i =0; i<activeRecipe.Count; i++)
        {
            Debug.Log("Count:" + i + " Recipe Name:" + activeRecipe[i].recipeName);
        }
    }

    private void GenerateNewOrder()
    {
        if(activeRecipe.Count < 5)
        {
            int random = Random.Range(0, Game.GetRecipeList().Count);
            Recipe newOrderRecipe = Game.GetRecipeList()[random];
            activeRecipe.Add(newOrderRecipe);
            toUpdateOrderUI = true;
            StartCoroutine(ExpiryTimer(newOrderRecipe));

        }
    }

    public void RemoveOrder(int index, Color uiColor)
    {
        orderUI.UpdateUIStatus(index, uiColor);
        activeRecipe.RemoveAt(index);
        // toUpdateOrderUI = true;
    }

    public Recipe GetCurrentOrder()
    {
        return activeRecipe[0];
    }

    IEnumerator orderGenerationTimer()
    {
        while(gameController.isGameLoopActive)
        {
            if(!gameController.isPaused)
            {
                yield return generationTimer;
                GenerateNewOrder();
            }
            else
            {
                yield return null;
            }

        }
    }

    IEnumerator ExpiryTimer(Recipe recipe)
    {
        if(!gameController.isPaused)
        {
            yield return expiryTimer;
            if(activeRecipe.Contains(recipe))
            {
                int index = activeRecipe.IndexOf(recipe);
                RemoveOrder(index, Color.red);
                gameController.DeductPoints(5);
            }
        }
        else
        {
            yield return null;
        }
    }

    
}
