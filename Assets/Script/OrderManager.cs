using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Orders> activeOrders = new List<Orders>();
    public float spawnInterval = 10f;
    public float expiryInterval = 35f;

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
        
     }

    private void GenerateNewOrder()
    {
        if(activeOrders.Count < 5)
        {
            int random = Random.Range(0, Game.GetRecipeList().Count);
            Recipe newOrderRecipe = Game.GetRecipeList()[random];
            Orders newOrder = new Orders(newOrderRecipe, expiryInterval);
            activeOrders.Add(newOrder);
            toUpdateOrderUI = true;
            StartCoroutine(ExpiryTimer(newOrder));

        }
    }

    public void RemoveOrder(Color uiColor)
    {
        orderUI.UpdateUIStatus(0, uiColor);
        activeOrders.RemoveAt(0); //always remove he first order!
        // toUpdateOrderUI = true;
    }

    public Orders GetCurrentOrder()
    {
        return activeOrders[0];
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

    IEnumerator ExpiryTimer(Orders order)
    {
        float expiryTimer = expiryInterval;
        order.RemainingTime = expiryTimer;

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
            RemoveOrder(Color.red);
            gameController.DeductPoints(5);
        }
    }

    
}
