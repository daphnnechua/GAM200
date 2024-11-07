using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    private OrderManager orderManager;
    private List<Orders> trackOrders = new List<Orders>();
    public List<GameObject> trackOrderUI = new List<GameObject>();

    [SerializeField] private List<GameObject> orderUIs;
    [SerializeField] private GameObject orderUIRoot;
    // private GameObject recipeImage;
    // private GameObject requiredIngredientsRoot;

    private Dictionary<Guid, Coroutine> shakeEffects = new Dictionary<Guid, Coroutine>();

    private DialogueController dialogueController;

    // private GameObject newOrderUI;

    // Start is called before the first frame update
    void Start()
    {
        dialogueController = FindObjectOfType<DialogueController>();
        orderManager = FindObjectOfType<OrderManager>();
        orderUIRoot = GameObject.FindWithTag("OrderUI");

        foreach(var obj in orderUIs)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(orderManager.toUpdateOrderUI);

        if(orderManager.toUpdateOrderUI)
        {
            SpawnOrderUI(orderManager.activeOrders);
            orderManager.toUpdateOrderUI = false;
        }

        for (int i = 0; i < trackOrders.Count; i++)
        {
            UpdateTimerUI(trackOrders[i], trackOrders[i].RemainingTime);
        }
    }

    public void SpawnOrderUI(List<Orders> currentOrders)
    {
        // trackOrders = orderManager.activeRecipe;
        foreach(var obj in orderUIs)
        {
            obj.SetActive(false);
        }
        trackOrderUI.Clear();
        trackOrders.Clear();

        foreach (var e in shakeEffects.Keys.ToList())
        {
            StopCoroutine(shakeEffects[e]);
            shakeEffects.Remove(e);
        }

        for(int i =0; i<currentOrders.Count; i++)
        {
            orderUIs[i].SetActive(true);
            // newOrderUI = Instantiate(activeOrderUIPrefab, orderUIRoot.transform);
            trackOrderUI.Add(orderUIs[i]);
            trackOrders.Add(currentOrders[i]);

            GameObject childObj = orderUIs[i].transform.Find("child").gameObject;
            GameObject recipeImage = childObj.transform.Find("Recipe Image").gameObject;
            GameObject requiredIngredientsRoot = childObj.transform.Find("Ingredient Images").gameObject;

            UpdateOrderUI(recipeImage, requiredIngredientsRoot, currentOrders[i].Recipe);
        }
    }

    private void UpdateOrderUI(GameObject parentRecipeImage, GameObject ingredientParentObj, Recipe recipe)
    {
        string imagePath = recipe.imageFilePath;

        Image recipeImg = parentRecipeImage.GetComponentInChildren<Image>();
        SetImage(imagePath, recipeImg);

        List<Image> ingredientImgs = ingredientParentObj.GetComponentsInChildren<Image>(true).ToList();

        // Debug.Log($"image placeholders found: {ingredientImgs.Count}"); //3

        foreach(var e in ingredientImgs)
        {
            e.gameObject.SetActive(false);
        }
        
        if(recipe.ingredientIDs.Length >0)
        {
            for (int i = 0; i < recipe.ingredientIDs.Length; i++)
            {
                ingredientImgs[i].gameObject.SetActive(true);
                Ingredient ingredient = Game.GetIngredientByID(recipe.ingredientIDs[i]);
                Ingredient originalIngredient = Game.GetIngredientByOriginalID(ingredient.originalStateID);
                string filePath = originalIngredient.imageFilePath;
                SetImage(filePath, ingredientImgs[i]);
            }
        }
    }

    // public void UpdateUIStatus(int index, Color change)
    // {
    //     GameObject orderUI = trackOrderUI[index];
    //     StartCoroutine(ShowColorChange(orderUI, change));
    // }

    // private IEnumerator ShowColorChange(GameObject orderUI, Color change)
    // {
    //     orderUI.GetComponent<Image>().color = change;

    //     yield return new WaitForSeconds(1f); // Wait for a short duration
    //     orderManager.toUpdateOrderUI = true;

    // }
    public void SetImage(string spritePath, Image image)
    {
        if (image == null) 
        {
            return;
        }

        AssetManager.LoadSprite(spritePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });


    }

    public void UpdateTimerUI(Orders order, float timeLeft)
    {
        for(int i =0; i<trackOrders.Count; i++)
        {
            if(trackOrders[i] == order)
            {
                GameObject childObj  = trackOrderUI[i].transform.Find("child").gameObject;  
                Slider timer  = childObj.transform.Find("Timer").GetComponent<Slider>(); 
                // float refBaseTimer = orderManager.baseExpiryTime;
                timer.value = timeLeft/order.ExpiryTime;

                if(timer.value>0.5f)
                {
                    timer.fillRect.GetComponent<Image>().color = Color.green;
                }
                else if(timer.value<=0.5f && timer.value > 0.25f)
                {
                    timer.fillRect.GetComponent<Image>().color = Color.yellow;
                }
                else if(timer.value<=0.25f)
                {
                    timer.fillRect.GetComponent<Image>().color = Color.red;
                    if (!shakeEffects.ContainsKey(order.OrderId))
                    {
                        Coroutine shakeCoroutine = StartCoroutine(ShakeEffect(trackOrderUI[i], order.OrderId));
                        shakeEffects[order.OrderId] = shakeCoroutine;
                    }
                }
            }
        }
    }

    private IEnumerator ShakeEffect(GameObject orderUI, Guid orderID)
    {
        if (orderUI == null) 
        {
            yield break;
        }

        RectTransform rt = orderUI.GetComponent<RectTransform>();
        if (rt == null) 
        {
            yield break;
        }

        Vector3 originalPosition = rt.localPosition;
        float duration = 1.5f;
        float magnitude = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if(dialogueController.dialogueOpen)
            {
                yield return null;
            }
            else
            {
                float xOffset = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float yOffset = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                rt.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

                elapsed += Time.deltaTime;
                yield return new WaitForSeconds(0.05f);
            }
        }

        rt.localPosition = originalPosition;
        shakeEffects.Remove(orderID);
    }
    
}
