using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CuttingStation : MonoBehaviour
{
    [SerializeField] public float cutTimer = 1f;
    private GameObject ingredientObj;

    private GameObject player;

    private PickUpObjs pickUpObjs;

    private IngredientManager ingredientManager;
    public bool ingredientOnStation = true;

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
        if(!ingredientManager.startedPrep)
        {
            //instantiate progress bar here
            ingredientManager.SpawnProgressBar();
            ingredientManager.startedPrep = true;
        }
        if (ingredientObj != null)
        {
            Debug.Log(cutTimer);
            ingredientManager.prepProgress += Time.deltaTime;
            ingredientManager.UpdateCuttingProgressBar(this, cutTimer);

            // Check if cutting is complete
            if (ingredientManager.prepProgress >= cutTimer)
            {
                ingredientManager.DestroyProgressBar(cutTimer);

                CompleteCutting();
            }
        }

    }

    
    private void CompleteCutting()
    {

        if (ingredientObj != null)
        {
            var ingredientData = ingredientObj.GetComponent<IngredientManager>().ingredientSO;

            if(ingredientData!=null)
            {
                var resultingIngredient = Game.GetIngredientByPrevStateID(ingredientData.ingredientID);

                if(resultingIngredient != null)
                {
                    ReplaceIngredient(resultingIngredient.prefabPath);
                }

            }
        }
        ingredientManager.prepProgress = 0f; //reset progress
        ingredientManager.startedPrep = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ingredient"))
        {
            ingredientObj = other.gameObject;
            ingredientManager = ingredientObj.GetComponent<IngredientManager>();
            ingredientOnStation = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ingredient"))
        {
            ingredientObj = null;
            ingredientManager = null;
            ingredientOnStation = false;
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
