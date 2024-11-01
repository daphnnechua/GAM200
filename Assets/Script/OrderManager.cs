using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Orders> activeOrders = new List<Orders>();
    public float spawnInterval = 10f;
    public float baseExpiryTime = 45f;
    private float balancedTimer;

    private WaitForSeconds generationTimer;
    private GameController gameController;

    private bool hasBeenInitialized = false;
    private bool hasCoroutineBeenStarted = false;

    public bool toUpdateOrderUI = false;

    public OrderUI orderUI;
    
    // Start is called before the first frame update
    void Start()
    {
        generationTimer = new WaitForSeconds(spawnInterval);
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
                StartCoroutine(OrderGenerationTimer());
                hasCoroutineBeenStarted = true;
            }
        }
        
     }

    private void GenerateNewOrder()
    {
        if(activeOrders.Count < 5)
        {
            int random = Random.Range(0, Game.GetUnlockedRecipeListByScenes(gameController.sceneName).Count);
            // foreach(var e in Game.GetUnlockedRecipeListByScenes(gameController.sceneName))
            // {
            //     Debug.Log($"available recipe in {gameController.sceneName}: {e.recipeName}");
            // }
            Recipe newOrderRecipe = Game.GetUnlockedRecipeListByScenes(gameController.sceneName)[random];

            float refBaseTimer = baseExpiryTime; 

            balancedTimer = refBaseTimer += activeOrders.Count*5; //setting timer for new order (+5s for each active order)
            // Debug.Log(balancedTimer);


            Orders newOrder = new Orders(newOrderRecipe, balancedTimer);
            activeOrders.Add(newOrder);


            toUpdateOrderUI = true;
            StartCoroutine(ExpiryTimer(newOrder));

        }
    }

    public void RemoveOrder()
    {
        // orderUI.UpdateUIStatus(0, uiColor); //to remove! testing purposes only 

        activeOrders.RemoveAt(0); //always remove he first order!
        // toUpdateOrderUI = true;
    }

    public Orders GetCurrentOrder()
    {
        return activeOrders[0];
    }

    IEnumerator OrderGenerationTimer()
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

    IEnumerator ExpiryTimer(Orders order)
    {
        order.RemainingTime = balancedTimer;

        while (order.RemainingTime > 0)
        {
            if (!gameController.isPaused)
            {
                order.RemainingTime -= Time.deltaTime;
                yield return null;
            }
            else
            {
                yield return null;
            }
        }

        if (activeOrders.Contains(order))
        {
            //play order failed sfx
            
            RemoveOrder();
            gameController.DeductPoints(5);
        }
    }

    public void AddBonusTime()
    {
        for(int i =0; i<activeOrders.Count; i++)
        {
            activeOrders[i].RemainingTime += 5f; //add bonus time to all remaining orders

            float maxOrderTime = baseExpiryTime += i*5;

            if(activeOrders[i].RemainingTime>maxOrderTime)
            {
                activeOrders[i].RemainingTime = maxOrderTime;
            }

        }
    }

    public void StopOrders() //called when level ends
    {
        StopAllCoroutines();
        
    }

    
}
