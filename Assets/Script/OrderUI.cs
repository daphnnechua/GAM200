using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    private OrderManager orderManager;
    private List<Orders> trackOrders = new List<Orders>();
    public List<GameObject> trackOrderUI = new List<GameObject>();
    [SerializeField] private GameObject activeOrderUIPrefab;
    [SerializeField] private GameObject orderUIRoot;
    private GameObject recipeImage;
    private GameObject requiredIngredientsRoot;
    private GameObject newOrderUI;

    // Start is called before the first frame update
    void Start()
    {
        orderManager = FindObjectOfType<OrderManager>();
        orderUIRoot = GameObject.FindWithTag("OrderUI");
    }

    // Update is called once per frame
    void Update()
    {
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

        foreach(var obj in trackOrderUI)
        {
            Destroy(obj);
        }
        trackOrderUI.Clear();
        trackOrders.Clear();

        for(int i =0; i<currentOrders.Count; i++)
        {
            newOrderUI = Instantiate(activeOrderUIPrefab, orderUIRoot.transform);
            trackOrderUI.Add(newOrderUI);
            trackOrders.Add(currentOrders[i]);
            recipeImage = newOrderUI.transform.Find("Recipe Image").gameObject;
            requiredIngredientsRoot = newOrderUI.transform.Find("Ingredient Images").gameObject;

            UpdateOrderUI(currentOrders[i].Recipe);
        }
    }

    private void UpdateOrderUI(Recipe recipe)
    {
        string imagePath = recipe.imageFilePath;

        Image recipeImg = recipeImage.GetComponentInChildren<Image>();
        SetImage(imagePath, recipeImg);

        TextMeshProUGUI recipeName = newOrderUI.GetComponentInChildren<TextMeshProUGUI>();
        recipeName.text = $"{recipe.recipeName}";
        
        if(recipe.ingredientIDs.Length >1)
        {
            List<Image> ingredientImgs = new List<Image>();
            GameObject ingredients = requiredIngredientsRoot.transform.Find("Ingredient Image").gameObject;
            ingredientImgs.Add(ingredients.GetComponentInChildren<Image>());

            for(int i =0; i<recipe.ingredientIDs.Length-1;i++)
            {
                GameObject ingredientImg = Instantiate(ingredients, requiredIngredientsRoot.transform);
                ingredientImgs.Add(ingredientImg.GetComponent<Image>());
            }

            List<string> imageFilePaths = new List<string>();
            foreach(string id in recipe.ingredientIDs)
            {
                Ingredient ingredient = Game.GetIngredientByID(id);
                string filePath = ingredient.imageFilePath;
                imageFilePaths.Add(filePath);
            }

            for(int i =0; i<ingredientImgs.Count; i++)
            {
                SetImage(imageFilePaths[i], ingredientImgs[i]);
            }
        }
    }

    public void UpdateUIStatus(int index, Color change)
    {
        GameObject orderUI = trackOrderUI[index];
        StartCoroutine(ShowColorChange(orderUI, change));
    }

    private IEnumerator ShowColorChange(GameObject orderUI, Color change)
    {
        orderUI.GetComponent<Image>().color = change;

        yield return new WaitForSeconds(1f); // Wait for a short duration
        orderManager.toUpdateOrderUI = true;

    }
    public void SetImage(string spritePath, Image image)
    {
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
                Slider timer  = trackOrderUI[i].transform.Find("Timer").GetComponent<Slider>(); 
                float refBaseTimer = orderManager.baseExpiryTime;
                timer.value = timeLeft/(refBaseTimer += i*5);

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
                }
            }
        }
    }
}
