using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingStation : MonoBehaviour
{
    [SerializeField] private float cutTimer = 5f;
    [SerializeField] GameObject cutObjPrefab;
    private GameObject ingredientObj;

    private GameObject player;

    private float cutProgress = 0;

    private IngredientPickUp ingredientPickUp;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        ingredientPickUp = player.GetComponent<IngredientPickUp>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ingredientPickUp.isCuttingStation && ingredientObj)
        {
            if(Input.GetKey(KeyCode.E))
            {
                CutIngredient();
            }
            
        }
    }

    private void CutIngredient()
    {

        if (ingredientObj != null)
        {
            cutProgress += Time.deltaTime;

            Debug.Log(cutProgress/cutTimer);

            // Check if cutting is complete
            if (cutProgress >= cutTimer)
            {
                CompleteCutting();
            }
        }

    }

    
    private void CompleteCutting()
    {
        if (ingredientObj != null)
        {
            Instantiate(cutObjPrefab, transform.position, ingredientObj.transform.rotation); // Instantiate the cut object
            Destroy(ingredientObj); // Destroy the original ingredient
        }
        cutProgress = 0f; //reset progress

        ingredientObj = null; //cannot cut anynmore --> do scriptable obj to set cancut bool to false
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ingredient"))
        {
            ingredientObj = other.gameObject;
        }
    }


}
