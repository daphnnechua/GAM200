using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpObjs : MonoBehaviour
{
    public Transform objPlacement;
    public LayerMask pickUpMask;
    public Vector3 dir;
    private GameObject objHeld;

    private GameObject player;
    private GameObject trashCan;
    public bool accessTable = false;
    public bool isCuttingStation = false;
    // public bool isStockStation = false;
    public bool isServingStation = false; 

    public bool isTrashCan = false;
    public bool isStove = false;


    private Trash trashScript;
    private Plate plateScript;

    private FryingPan fryingPan;
    private Pot pot;
    private PlayerMovement playerMovement;

    private Transform closestObj;

    public bool isHoldingObj = false;
    [SerializeField] private List<AudioClip> putdownIngredients;

    [SerializeField] private List<AudioClip> potSfx;

    [SerializeField] private List<AudioClip> plateSfx;

    [SerializeField] private List<AudioClip> fryingPanSfx;


    private AudioClip currentSoundFx;

    // Start is called before the first frame update
    void Start()
    {        
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        objPlacement = GameObject.FindGameObjectWithTag("IngredientPlacement").transform; 
        pickUpMask = LayerMask.GetMask("KitchenObjs");
        trashCan = GameObject.FindGameObjectWithTag("TrashCan");
        trashScript = trashCan.GetComponent<Trash>();
    }

    // Update is called once per frame
    void Update()
    {
        dir = playerMovement.FacingDirection()/1.5f;

        if(isHoldingObj && objHeld != null)
        {
            objHeld.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        
        // Debug.Log(closestObj);
        // Collider2D[] nearbyIngredients = Physics2D.OverlapCircleAll(transform.position + dir, 0.75f, pickUpMask); //all nearby ingredients

        // if (nearbyIngredients.Length > 0) //if there are ingredient nearby
        // {
        //     GameObject closestIngredient = null;
        //     float closestDist = Mathf.Infinity;

        //     foreach (Collider2D ingredient in nearbyIngredients)
        //     {
        //         float distance = Vector3.Distance(transform.position, ingredient.transform.position); //relative distance between player and ingredient
        //         if (distance < closestDist) //finding the closest distance
        //         {
        //             closestDist = distance;
        //             closestIngredient = ingredient.gameObject;
        //         }
        //         Debug.Log(ingredient);
        //     }
        //     // Debug.Log(closestIngredient);
        // }

        // Debug.Log(playerMovement.IsObjectInteractable(closestObj.transform));

        
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(objHeld)
            {
                PutDownIngredient();

            }
            else //pickup ingredient
            {
                PickUpAllObjs();
            }
        }
    }

    private void PlayRandomSfx(List<AudioClip> sfxList)
    {
        if(currentSoundFx!=null)
        {
            StartCoroutine(SoundFXManager.instance.RemoveSfx(currentSoundFx.name, 0));
        }
        int random = Random.Range(0, sfxList.Count);
        SoundFXManager.instance.PlaySound(sfxList[random], transform, 0.5f);
        currentSoundFx = sfxList[random];

    }

    private void PutDownIngredient()
    {

        if(!IsDroneStation())
        {
            objHeld.GetComponent<Rigidbody2D>().isKinematic = false;

            IngredientsAssembly();
            
            Vector2 placePosition = transform.position + dir; ///calculation --> area around player

            Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.25f, pickUpMask);
            Collider2D infrontOfPlayer = Physics2D.OverlapCircle(placePosition, 0.25f);

            // if(infrontOfPlayer!=null)
            // {
            //     Debug.Log(infrontOfPlayer.gameObject.name);
            // }

            if (colliderAtPosition != null) //if there is something at where the obj is supposed to be placed
            {
                return; // Do not place the obj
            }

            if(infrontOfPlayer==null) //not accessing any tables, stations or trash can --> place on floor. if holding ingredient and is accessing serving station, place on floor
            {
                if(objHeld.CompareTag("Ingredient"))
                {
                    objHeld.transform.position = transform.position + dir*1.5f;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;

                    PlayRandomSfx(putdownIngredients);
                }
                
                if(objHeld.CompareTag("Plate"))
                {
                    objHeld.transform.position = transform.position + dir*1.5f;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    plateScript = objHeld.GetComponent<Plate>();
                    plateScript.isHoldingPlate = false;

                    PlayRandomSfx(plateSfx);
                }

                if(objHeld.CompareTag("FryingPan"))
                {
                    objHeld.transform.position = transform.position + dir*1.5f;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    fryingPan = objHeld.GetComponent<FryingPan>();
                    fryingPan.isHoldingFryingPan = false;

                    PlayRandomSfx(fryingPanSfx);
                }
                if(objHeld.CompareTag("Pot"))
                {
                    objHeld.transform.position = transform.position + dir*1.5f;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    pot = objHeld.GetComponent<Pot>();
                    pot.isHoldingPot = false;

                    PlayRandomSfx(potSfx);
                }

                if(objHeld.GetComponent<Rigidbody2D>())
                {
                    objHeld.GetComponent<Rigidbody2D>().simulated = true;
                }
                objHeld = null;
                isHoldingObj = false;
            }
            else if(infrontOfPlayer!= null && infrontOfPlayer.CompareTag("ServingStation"))
            {
                if(objHeld.CompareTag("Ingredient") || objHeld.CompareTag("Pot") || objHeld.CompareTag("FryingPan"))
                {
                    Debug.Log("cannot place on serving station");
                    return;
                }
                else if(objHeld.CompareTag("Plate"))
                {
                    plateScript = objHeld.GetComponent<Plate>();
                    if(plateScript.readyToServe)
                    {
                        plateScript.isHoldingPlate = false;
                        plateScript.ServePlate();
                    }
                }
                
            }
            else if(infrontOfPlayer!= null && infrontOfPlayer.CompareTag("Stove"))
            {
                GameObject objInFront = infrontOfPlayer.gameObject;

                if(objHeld.CompareTag("Ingredient") || objHeld.CompareTag("Plate"))
                {

                    Debug.Log("cannot place ingredient or plate down on stove");
                    return;
                }
                else if(objHeld.CompareTag("FryingPan"))
                {
                    objHeld.transform.position = objInFront.transform.position;
                    objHeld.transform.parent = null;
                    fryingPan = objHeld.GetComponent<FryingPan>();
                    fryingPan.isHoldingFryingPan = false;

                    PlayRandomSfx(fryingPanSfx);

                    if(objHeld.GetComponent<Rigidbody2D>())
                    {
                        objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        objHeld.GetComponent<Rigidbody2D>().simulated = true;
                    }
                    objHeld = null;
                    isHoldingObj = false;
                }
                else if(objHeld.CompareTag("Pot"))
                {
                    objHeld.transform.position = objInFront.transform.position;
                    objHeld.transform.parent = null;

                    pot = objHeld.GetComponent<Pot>();
                    pot.isHoldingPot = false;

                    PlayRandomSfx(potSfx);

                    if(objHeld.GetComponent<Rigidbody2D>())
                    {
                        objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        objHeld.GetComponent<Rigidbody2D>().simulated = true;
                    }
                    objHeld = null;
                    isHoldingObj = false;
                }
            }
            else if(accessTable)
            {
                Collider2D nearestObj = Physics2D.OverlapCircle(placePosition, 0.1f); //check the place where player is facing
                if(colliderAtPosition==null && nearestObj == null) //right beside the table, and facing away from it, and there is nothing at where playeer is facing
                {
                    objHeld.transform.position = transform.position + dir*1.5f;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;

                    if(objHeld.CompareTag("Plate"))
                    {
                        plateScript = objHeld.GetComponent<Plate>();
                        plateScript.isHoldingPlate = false;
                        PlayRandomSfx(plateSfx);
                    }

                    if(objHeld.CompareTag("Ingredient"))
                    {
                        PlayRandomSfx(putdownIngredients);
                    }

                    if(objHeld.CompareTag("FryingPan"))
                    {
                        fryingPan = objHeld.GetComponent<FryingPan>();
                        fryingPan.isHoldingFryingPan = false;
                        PlayRandomSfx(fryingPanSfx);
                    }
                    if(objHeld.CompareTag("Pot"))
                    {
                        pot = objHeld.GetComponent<Pot>();
                        pot.isHoldingPot = false;
                        PlayRandomSfx(potSfx);
                    }
                    if(objHeld.GetComponent<Rigidbody2D>())
                    {
                        objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        objHeld.GetComponent<Rigidbody2D>().simulated = true;
                    }
                    objHeld = null;
                    isHoldingObj = false;
                }
                else if(IsCounterOccupied() || infrontOfPlayer.CompareTag("StockStation"))
                {
                    return;
                }
                else if(!IsCounterOccupied())
                {
                    if(isCuttingStation && objHeld.CompareTag("Plate") || isCuttingStation && objHeld.CompareTag("FryingPan") || isCuttingStation && objHeld.CompareTag("Pot"))
                    {
                        return; //do not place plate down on cutting station
                    }
                    if(closestObj && playerMovement.IsObjectInteractable(closestObj.transform))
                    {
                        if(objHeld.CompareTag("Plate")) //if holding plate and table is empty, place plate down
                        {
                            plateScript = objHeld.GetComponent<Plate>();
                            plateScript.isHoldingPlate = false;
                            PlayRandomSfx(plateSfx);
                        }
                        if(objHeld.CompareTag("FryingPan"))
                        {
                            fryingPan = objHeld.GetComponent <FryingPan>();
                            fryingPan.isHoldingFryingPan=false;
                            PlayRandomSfx(fryingPanSfx);
                        }
                        if(objHeld.CompareTag("Pot"))
                        {
                            pot = objHeld.GetComponent<Pot>();
                            pot.isHoldingPot = false;
                            PlayRandomSfx(potSfx);
                        }
                        if(objHeld.CompareTag("Ingredient"))
                        {
                            PlayRandomSfx(putdownIngredients);
                        }
                        objHeld.transform.position = closestObj.position;
                        objHeld.transform.parent = null;
                        objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;

                        if(objHeld.GetComponent<Rigidbody2D>())
                        {
                            objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                            objHeld.GetComponent<Rigidbody2D>().simulated = true;

                        }

                        objHeld = null;
                        isHoldingObj = false;
                    }
                
                }
                   
            }
            else if(isTrashCan && playerMovement.IsObjectInteractable(closestObj.transform)) //accessing trash can
            {
                trashScript.TrashIngredient(objHeld);
                if(objHeld.CompareTag("Ingredient"))
                {
                    isHoldingObj = false;
                }
                
            }

        }
    }

    private void IngredientsAssembly()
    {

        if(!isCuttingStation && !isStove) //empty space
        {
            if(objHeld.CompareTag("Ingredient"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position, 1f, pickUpMask);
                if(collider != null)
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(collider!=null && playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(collider.CompareTag("Plate") && objHeld.GetComponent<IngredientManager>().ingredientSO.isReady)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            plateScript.PlaceIngredient(objHeld);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                        else if(collider.CompareTag("FryingPan") && objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                        else if(collider.CompareTag("Pot") && objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = currentPlacementObj.GetComponent<Pot>();
                            pot.PlaceIngredientInPot(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                    }

                }

            }
            else if(objHeld.CompareTag("Plate"))
            {
                Vector2 placePosition = transform.position + dir; ///calculation --> area below the player

                Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.1f, pickUpMask); // Check within a small radius
                
                if(colliderAtPosition !=null && playerMovement.IsObjectInteractable(colliderAtPosition.transform))
                {
                    if(colliderAtPosition.gameObject.CompareTag("Ingredient"))
                    {
                        GameObject ingredient = colliderAtPosition.gameObject;
                        IngredientManager ingredientManager = ingredient.GetComponent<IngredientManager>();
                        if(ingredientManager.ingredientSO.isReady)
                        {
                            plateScript = objHeld.GetComponent<Plate>();
                            objHeld.transform.position = ingredient.transform.position;
                            plateScript.isHoldingPlate = false;
                            plateScript.PlaceIngredient(ingredient);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(plateSfx);
                        }
                    }
                }
            }
            else if(objHeld.CompareTag("FryingPan"))
            {
                fryingPan = objHeld.GetComponent<FryingPan>();
                
                Vector2 placePosition = transform.position + dir; ///calculation --> area below the player

                Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.1f, pickUpMask); // Check within a small radius

                if(colliderAtPosition !=null && playerMovement.IsObjectInteractable(colliderAtPosition.transform))
                {
                    if(colliderAtPosition.gameObject.CompareTag("Ingredient"))
                    {
                        GameObject ingredient = colliderAtPosition.gameObject;
                        IngredientManager ingredientManager = ingredient.GetComponent<IngredientManager>();
                        if(ingredientManager.ingredientSO.canFry)
                        {
                            objHeld.transform.position = ingredient.transform.position;
                            PlayRandomSfx(fryingPanSfx);
                        }
                    }
                    else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                    {
                        plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                        if(fryingPan.isDoneCooking)
                        {
                            fryingPan.PlaceFoodInPlate(plateScript);
                            plateScript.readyToServe = true;
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }

            }
            else if(objHeld.CompareTag("Pot"))
            {
                pot = objHeld.GetComponent<Pot>();
                
                Vector2 placePosition = transform.position + dir; ///calculation --> area below the player

                Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.1f, pickUpMask); // Check within a small radius

                if(colliderAtPosition !=null && playerMovement.IsObjectInteractable(colliderAtPosition.transform))
                {
                    if(colliderAtPosition.gameObject.CompareTag("Ingredient"))
                    {
                        GameObject ingredient = colliderAtPosition.gameObject;
                        IngredientManager ingredientManager = ingredient.GetComponent<IngredientManager>();
                        if(ingredientManager.ingredientSO.canBoil)
                        {
                            objHeld.transform.position = ingredient.transform.position;
                            pot.PlaceIngredientInPot(ingredient);
                            PlayRandomSfx(potSfx);
                        }
                    }
                    else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                    {
                        plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                        if(pot.isDoneCooking)
                        {
                            pot.PlaceSoupInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }

            }
        }
        else if(closestObj.CompareTag("Stove") && isStove) //at stove
        {
            if(objHeld.CompareTag("Ingredient"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask); //infront of player, check if there is ingredient, pot, frying pan or plate
                if(collider!=null && collider.CompareTag("FryingPan")) //frying pan on stove
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Pot"))
                {
                    Debug.Log("placing ingredient in pot");
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = currentPlacementObj.GetComponent<Pot>();
                            pot.PlaceIngredientInPot(objHeld);
                            PlayRandomSfx(potSfx);
                            
                        }
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.isReady)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            plateScript.PlaceIngredient(objHeld);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            
            }
            else if(objHeld.CompareTag("Plate"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask); //infront of player, check if there is ingredient, pot, frying pan or plate
                if(collider!=null && collider.CompareTag("Ingredient"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        plateScript = objHeld.GetComponent<Plate>();
                        plateScript.PlaceIngredient(currentPlacementObj);
                        plateScript.readyToServe = true;
                        PlayRandomSfx(putdownIngredients);
                    }

                }
                else if(collider!=null && collider.CompareTag("Pot"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        pot = currentPlacementObj.GetComponent<Pot>();
                        if(pot.isDoneCooking)
                        {
                            plateScript = objHeld.GetComponent<Plate>();
                            pot.PlaceSoupInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(potSfx);
                            
                        }
                    }

                }
                else if(collider!=null && collider.CompareTag("FryingPan"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                        if(fryingPan.isDoneCooking)
                        {
                            plateScript = objHeld.GetComponent<Plate>();
                            fryingPan.PlaceFoodInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(potSfx);
                            
                        }
                    }

                }            
            }
            else if(objHeld.CompareTag("FryingPan"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask);
                if(collider!=null && collider.CompareTag("Ingredient"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(currentPlacementObj.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = objHeld.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(currentPlacementObj);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        fryingPan = objHeld.GetComponent<FryingPan>();

                        if(fryingPan.isDoneCooking)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            fryingPan.PlaceFoodInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            }
            else if(objHeld.CompareTag("Pot"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask);
                if(collider!=null && collider.CompareTag("Ingredient"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(currentPlacementObj.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = objHeld.GetComponent<Pot>();
                            pot.PlaceIngredientInPot(currentPlacementObj);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        pot = objHeld.GetComponent<Pot>();

                        if(pot.isDoneCooking)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            pot.PlaceSoupInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            }
        }
        else if(closestObj.CompareTag("CuttingStation") && isCuttingStation) //at cutting station
        {
            if(objHeld.CompareTag("Ingredient"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask); //infront of player, check if there is ingredient, pot, frying pan or plate
                if(collider!=null && collider.CompareTag("FryingPan")) //frying pan on stove
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Pot"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = currentPlacementObj.GetComponent<Pot>();
                             pot.PlaceIngredientInPot(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.isReady)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            plateScript.PlaceIngredient(objHeld);
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            
            }
            else if(objHeld.CompareTag("Plate"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask); //infront of player, check if there is ingredient, pot, frying pan or plate
                if(collider!=null && collider.CompareTag("Ingredient")) //frying pan on stove
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        plateScript = objHeld.GetComponent<Plate>();
                        plateScript.PlaceIngredient(currentPlacementObj);
                        PlayRandomSfx(putdownIngredients);    
                    }

                }            
            }
            else if(objHeld.CompareTag("FryingPan"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask);
                if(collider!=null && collider.CompareTag("Ingredient"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(currentPlacementObj.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = objHeld.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(currentPlacementObj);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        fryingPan = objHeld.GetComponent<FryingPan>();

                        if(fryingPan.isDoneCooking)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            fryingPan.PlaceFoodInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            }
            else if(objHeld.CompareTag("Pot"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 0.5f, pickUpMask);
                if(collider!=null && collider.CompareTag("Ingredient"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(currentPlacementObj.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = objHeld.GetComponent<Pot>();
                            pot.PlaceIngredientInPot(currentPlacementObj);
                            PlayRandomSfx(putdownIngredients);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Plate"))
                {
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        pot = objHeld.GetComponent<Pot>();

                        if(pot.isDoneCooking)
                        {
                            plateScript = currentPlacementObj.GetComponent<Plate>();
                            pot.PlaceSoupInPlate(plateScript);
                            plateScript.readyToServe = true;
                            PlayRandomSfx(putdownIngredients);
                        }
                    }
                }
            }
        }
    }

    private bool IsCounterOccupied()
    {
        Collider2D collider = Physics2D.OverlapCircle(closestObj.position, 0.2f, pickUpMask);
        if(collider)
        {
            return true;
        }

        return false;
    }

    private void PickUpAllObjs()
    {
        if(!IsDroneStation())
        {
            Collider2D infrontOfPlayer = Physics2D.OverlapCircle(transform.position+dir, 0.25f); 

            if (infrontOfPlayer!=null && infrontOfPlayer.CompareTag("StockStation") && !objHeld && playerMovement.IsObjectInteractable(infrontOfPlayer.transform)) // If player is interacting with a StockStation
            {
                closestObj = infrontOfPlayer.transform;
                Collider2D objOnStation = Physics2D.OverlapCircle(closestObj.position, 0.1f, pickUpMask);
                if(objOnStation && !objHeld)
                {
                    PickUpIngredient();
                }
                else if(!objOnStation && !objHeld)
                {
                    GameObject newObj = closestObj.gameObject.GetComponent<StockStationManager>().GetNewObj(); // Get new object from StockStation
                    if (newObj != null)
                    {
                        PickUpNewObj(newObj); // Handle picking up the new object
                    }

                }
            }
            else
            {
                PickUpIngredient(); // Otherwise, pick up the ingredient normally
            }
            isHoldingObj = true;
        }
    }

    private void PickUpNewObj(GameObject gameObject)
    {
        objHeld = gameObject;
        // if(playerMovement.IsObjectInteractable(closestObj.transform))
        // {
            if(objHeld.CompareTag("Plate"))
            {
                plateScript = objHeld.GetComponent<Plate>();
                plateScript.isHoldingPlate = true;
            }
            objHeld.transform.position = objPlacement.position;

            objHeld.transform.parent = transform;

            if (objHeld.GetComponent<Rigidbody2D>())
            {
                objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                objHeld.GetComponent<Rigidbody2D>().simulated = false;
            }
        // }
    }

    private void PickUpIngredient()
    {
        //find the closest ingredient
        Collider2D[] nearbyObjs = Physics2D.OverlapCircleAll(transform.position, 1f, pickUpMask); //all nearby ingredients

        if (nearbyObjs.Length > 0) //if there are ingredient nearby
        {
            GameObject closestObj = null;
            float closestDist = Mathf.Infinity;

            foreach (Collider2D objs in nearbyObjs)
            {
                if(playerMovement.IsObjectInteractable(objs.transform))
                {
                    float distance = Vector3.Distance(transform.position, objs.transform.position); //relative distance between player and ingredient
                    if (distance < closestDist) //finding the closest distance
                    {
                        closestDist = distance;
                        closestObj = objs.gameObject;
                    }
                }
            }
            if(closestObj!=null && playerMovement.IsObjectInteractable(closestObj.transform))
            {
                
                objHeld = closestObj;
                if(objHeld.CompareTag("Plate"))
                {
                    plateScript = objHeld.GetComponent<Plate>();
                    plateScript.isHoldingPlate = true;
                    PlayRandomSfx(plateSfx);
                    
                }
                else if(objHeld.CompareTag("FryingPan"))
                {
                    fryingPan = objHeld.GetComponent<FryingPan>();
                    fryingPan.isHoldingFryingPan = true;
                    PlayRandomSfx(fryingPanSfx);
                }
                else if(objHeld.CompareTag("Pot"))
                {
                    pot = objHeld.GetComponent<Pot>();
                    pot.isHoldingPot = true;
                    PlayRandomSfx(potSfx);
                }
                else if(objHeld.CompareTag("Ingredient"))
                {
                    PlayRandomSfx(putdownIngredients);
                }

                objHeld.transform.position = objPlacement.position;
                objHeld.transform.parent = transform;

                if(objHeld.GetComponent<Rigidbody2D>())
                {
                    objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    objHeld.GetComponent<Rigidbody2D>().simulated = false;
                }
            }
        }
    }

    private void OnDrawGizmos() ///debugging purposes --> shows piickup range for player
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + dir, 0.25f);

    }

    void OnTriggerStay2D(Collider2D other)
    {
        float distance = Vector3.Distance(transform.position, other.transform.position); //distance between player and counters
        if (closestObj == null || distance < Vector3.Distance(transform.position, closestObj.position)) //check if current counter is the nearest
        {
            closestObj = other.transform; //set the placing down pos of ingredients to the nearest counter

            accessTable = false;
            isCuttingStation = false;
            isServingStation = false;
            isTrashCan = false;


            if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation") || other.CompareTag("ServingStation") || other.CompareTag("Stove"))
            {
                accessTable = true;
                // tableTopPos = other.transform;

                if(other.CompareTag("CuttingStation"))
                {
                    isCuttingStation = true;
                }

                if(other.CompareTag("ServingStation"))
                {
                    isServingStation = true;
                }

                if(other.CompareTag("Stove"))
                {
                    isStove = true;
                }


            }

            if(other.CompareTag("TrashCan"))
            {
                isTrashCan = true;
            }
        }

    }

    public bool IsDroneStation()
    {
        Vector2 interactionArea = transform.position + dir;
        Collider2D interact = Physics2D.OverlapCircle(interactionArea, 0.25f);
        if(interact ==null)
        {
            return false;
        }
        else if(interact!=null && interact.gameObject.CompareTag("droneStation"))
        {
            return true;
        }
        
        return false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation") || other.CompareTag("TrashCan") ||other.CompareTag("StockStation") || other.CompareTag("ServingStation")||other.CompareTag("Stove"))
        {
            accessTable = false;
            isCuttingStation = false;
            isTrashCan = false;
            isServingStation = false;
            isStove = false;
        }

        closestObj = null;
    }

}
