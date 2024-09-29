using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CuttingStation : MonoBehaviour
{
    [SerializeField] private float cutTimer = 5f;
    private GameObject ingredientObj;

    private GameObject player;

    private float cutProgress = 0;

    private PickUpObjs pickUpObjs;
    private IngredientSO ingredientSO;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pickUpObjs = player.GetComponent<PickUpObjs>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pickUpObjs.isCuttingStation && ingredientObj && ingredientObj.GetComponent<IngredientManager>().ingredientSO.canCut)
        {
            if(Input.GetKey(KeyCode.E))
            {
                CutIngredient();
            }
            
        }
        if(ingredientObj && !ingredientObj.GetComponent<IngredientManager>().ingredientSO.canCut)
        {
            Debug.Log(ingredientObj.name + " cannot be cut!");
        }
    }

    private void CutIngredient()
    {

        if (ingredientObj != null)
        {
            cutProgress += Time.deltaTime;

            // Debug.Log(cutProgress/cutTimer);

            // Check if cutting is complete
            if (cutProgress >= cutTimer)
            {
                CompleteCutting();
            }
        }

    }

    
    private void CompleteCutting()
    {
        Debug.Log("Cutting Complete!");
        if (ingredientObj != null)
        {
            var ingredientData = ingredientObj.GetComponent<IngredientManager>().ingredientSO;

            if(ingredientData!=null)
            {
                var resultingIngredient = Game.GetIngredientByPrevStateID(ingredientData.ingredientID);
                Debug.Log("Result is:" + resultingIngredient.name);

                if(resultingIngredient != null)
                {
                    ReplaceIngredient(resultingIngredient.prefabPath);
                }

            }
            else{
                Debug.Log("no ingredient data found!");
            }

        }
        cutProgress = 0f; //reset progress
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ingredient"))
        {
            ingredientObj = other.gameObject;
        }
    }
    private void ReplaceIngredient(string prefabPath)
    {
        AssetManager.LoadPrefab(prefabPath, (GameObject cutPrefab) =>
        {

            GameObject cutIngredient = Instantiate(cutPrefab, ingredientObj.transform.position, ingredientObj.transform.rotation);
                
            IngredientManager ingredientManager = cutIngredient.GetComponent<IngredientManager>();
            ingredientManager.SetImage(ingredientManager.ingredientSO.imageName);

            Rigidbody2D rb2D = cutIngredient.GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            Destroy(ingredientObj);
        });
    }


}
