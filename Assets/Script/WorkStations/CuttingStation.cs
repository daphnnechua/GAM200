using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CuttingStation : MonoBehaviour
{
    [SerializeField] public float cutTimer = 1f;
    private GameObject ingredientObj;

    Rigidbody2D rb; 

    private GameObject player;

    private PickUpObjs pickUpObjs;

    private IngredientManager ingredientManager;
    private PlayerMovement playerMovement;
    private GameController gameController;
    public bool ingredientOnStation = true;

    private bool startedCutting = false;

    private bool isChoppingSoundPlaying = false;

    [SerializeField] private AudioClip chopppingSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb  = player.GetComponent<Rigidbody2D>();
        pickUpObjs = player.GetComponent<PickUpObjs>();
        gameController = FindObjectOfType<GameController>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pickUpObjs.isCuttingStation && ingredientObj && ingredientObj.GetComponent<IngredientManager>().ingredientSO.canCut && !pickUpObjs.isHoldingObj)
        {
            if(Input.GetKey(KeyCode.K))
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                playerMovement.canMove = false;
                CutIngredient();
            }
            
        }
        else
        {
            playerMovement.isCutting = false;
        }
        if(!gameController.levelEnded && Input.GetKeyUp(KeyCode.K))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerMovement.canMove = true;
            playerMovement.isCutting = false;
            startedCutting = false;
            isChoppingSoundPlaying = false;
        }
        // if(ingredientObj && !ingredientObj.GetComponent<IngredientManager>().ingredientSO.canCut)
        // {
        //     Debug.Log(ingredientObj.name + " cannot be cut!");
        // }
    }

    private void CutIngredient()
    {
        playerMovement.isCutting = true;
        if(!ingredientManager.startedPrep)
        {
            //instantiate progress bar here
            ingredientManager.SpawnProgressBar();
            ingredientManager.startedPrep = true;
        }
        if (ingredientObj != null)
        {
            
            ingredientManager.prepProgress += Time.deltaTime;
            ingredientManager.UpdateCuttingProgressBar(this, cutTimer);

            if(!startedCutting && !isChoppingSoundPlaying)
            {
                SoundFXManager.instance.PlaySound(chopppingSound, transform, 0.5f);
                startedCutting = true;
                isChoppingSoundPlaying = true;
            }

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
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerMovement.canMove = true;
        playerMovement.isCutting = false;
        
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
        startedCutting = false;
        isChoppingSoundPlaying = true;
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

            GameObject cutIngredient = Instantiate(cutPrefab, ingredientObj.transform.position, ingredientObj.transform.rotation, transform);
                
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
