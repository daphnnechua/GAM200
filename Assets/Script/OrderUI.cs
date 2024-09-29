using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    private OrderManager orderManager;
    private List<Recipe> trackOrders = new List<Recipe>();
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
            SpawnOrderUI(orderManager.activeRecipe);
            orderManager.toUpdateOrderUI = false;
        }
    }

    public void SpawnOrderUI(List<Recipe> currentOrders)
    {
        // trackOrders = orderManager.activeRecipe;

        foreach(var obj in trackOrderUI)
        {
            Destroy(obj);
        }
        trackOrderUI.Clear();

        foreach(var orders in currentOrders)
        {
            newOrderUI = Instantiate(activeOrderUIPrefab, orderUIRoot.transform);
            trackOrderUI.Add(newOrderUI);
            recipeImage = newOrderUI.transform.Find("Recipe Image").gameObject;
            requiredIngredientsRoot = newOrderUI.transform.Find("Ingredient Images").gameObject;

            UpdateOrderUI(orders);
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

        yield return new WaitForSeconds(0.5f); // Wait for a short duration
        orderManager.toUpdateOrderUI = true;

    }
    public void SetImage(string spritePath, Image image)
    {
        AssetManager.LoadSprite(spritePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });


    }
}
