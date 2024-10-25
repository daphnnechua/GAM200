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
    public bool isStockStation = false;
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
        dir = playerMovement.FacingDirection();

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

    private void PutDownIngredient()
    {
        if(!IsDroneStation())
        {
            objHeld.GetComponent<Rigidbody2D>().isKinematic = false;

            IngredientsAssembly();
            
            Vector2 placePosition = transform.position + dir; ///calculation --> area around player

            Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.25f, pickUpMask); // Check within a small area

            if (colliderAtPosition != null) //if there is something at where the obj is supposed to be placed
            {
                // Debug.Log(colliderAtPosition);
                return; // Do not place the obj
            }

            if(!accessTable && !isTrashCan || isServingStation && objHeld.CompareTag("Ingredient")) //not accessing any tables, stations or trash can --> place on floor. if holding ingredient and is accessing serving station, place on floor
            {
                // dir = new Vector2(0, -1f);
                objHeld.transform.position = transform.position + dir;
                objHeld.transform.parent = null;


                objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;


                if(objHeld.CompareTag("Plate"))
                {
                    plateScript = objHeld.GetComponent<Plate>();
                    plateScript.isHoldingPlate = false;
                }

                if(objHeld.CompareTag("FryingPan"))
                {
                    fryingPan = objHeld.GetComponent<FryingPan>();
                    fryingPan.isHoldingFryingPan = false;
                }
                if(objHeld.CompareTag("Pot"))
                {
                    pot = objHeld.GetComponent<Pot>();
                    pot.isHoldingPot = false;
                }

                if(objHeld.GetComponent<Rigidbody2D>())
                {
                    objHeld.GetComponent<Rigidbody2D>().simulated = true;
                }
                objHeld = null;
                isHoldingObj = false;
            }
            else if(isServingStation && objHeld.CompareTag("Plate"))
            {
                plateScript = objHeld.GetComponent<Plate>();
                if(plateScript.readyToServe)
                {
                    plateScript.isHoldingPlate = false;
                    plateScript.ServePlate();
                }
                else //put plate down if there are no ingredients
                {
                    // dir = new Vector2(0, -1f);
                    objHeld.transform.position = transform.position + dir;
                    objHeld.transform.parent = null;
                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    plateScript.isHoldingPlate = false;

                    if(objHeld.GetComponent<Rigidbody2D>())
                    {
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
                    objHeld.transform.position = transform.position + dir;
                    objHeld.transform.parent = null;


                    objHeld.GetComponent<SpriteRenderer>().sortingOrder = 1;


                    if(objHeld.CompareTag("Plate"))
                    {
                        plateScript = objHeld.GetComponent<Plate>();
                        plateScript.isHoldingPlate = false;
                    }

                    if(objHeld.CompareTag("FryingPan"))
                    {
                        fryingPan = objHeld.GetComponent<FryingPan>();
                        fryingPan.isHoldingFryingPan = false;
                    }
                    if(objHeld.CompareTag("Pot"))
                    {
                        pot = objHeld.GetComponent<Pot>();
                        pot.isHoldingPot = false;
                    }
                    if(objHeld.GetComponent<Rigidbody2D>())
                    {
                        objHeld.GetComponent<Rigidbody2D>().simulated = true;
                    }
                    objHeld = null;
                    isHoldingObj = false;
                }
                else if(IsCounterOccupied() || isStockStation)
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
                        }
                        if(objHeld.CompareTag("FryingPan"))
                        {
                            fryingPan = objHeld.GetComponent <FryingPan>();
                            fryingPan.isHoldingFryingPan=false;
                        }
                        if(objHeld.CompareTag("Pot"))
                        {
                            pot = objHeld.GetComponent<Pot>();
                            pot.isHoldingPot = false;
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
                else if(isTrashCan && playerMovement.IsObjectInteractable(closestObj.transform)) //accessing trash can
                {
                    trashScript.TrashIngredient(objHeld);
                    isHoldingObj = false;
                }   
            }
            

        }
    }

    private void IngredientsAssembly()
    {
        Vector2 infrontPlayer = transform.position + dir; ///calculation --> area around player
        Collider2D nearestObj = Physics2D.OverlapCircle(infrontPlayer, 0.1f); //check the place where player is facing


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
                        }
                        else if(collider.CompareTag("FryingPan") && objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(objHeld);
                        }
                        else if(collider.CompareTag("Pot") && objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = currentPlacementObj.GetComponent<Pot>();
                            pot.PlaceIngredientInPot(objHeld);
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
                            fryingPan.PlaceIngredientInPan(ingredient);
                        }
                    }
                    else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                    {
                        plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                        if(fryingPan.isDoneCooking)
                        {
                            fryingPan.PlaceFoodInPlate(plateScript);
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
                        }
                    }
                    else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                    {
                        plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                        if(pot.isDoneCooking)
                        {
                            pot.PlaceSoupInPlate(plateScript);
                        }
                    }
                }

            }
        }
        else if(closestObj.CompareTag("Stove") && isStove) //facing stove
        {
            Debug.Log("facing stove");
            if(objHeld.CompareTag("Ingredient"))
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position, 1f, pickUpMask);
                if(collider!=null && collider.CompareTag("FryingPan")) //frying pan on stove
                {
                    Debug.Log("interacting stove with frying pan");
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                        {
                            fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                            fryingPan.PlaceIngredientInPan(objHeld);
                        }
                            
                    }

                }
                else if(collider!=null && collider.CompareTag("Pot"))
                {
                    Debug.Log("interacting stove with pot");
                    GameObject currentPlacementObj = collider.gameObject;
                    if(playerMovement.IsObjectInteractable(currentPlacementObj.transform))
                    {
                        if(objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                        {
                            pot = currentPlacementObj.GetComponent<Pot>();
                             pot.PlaceIngredientInPot(objHeld);
                            
                        }
                }

            }
            
        }
        else
        {
            Collider2D colliderAtPos = Physics2D.OverlapCircle(infrontPlayer, 0.25f, pickUpMask); // Check within a small area
            if(colliderAtPos==null && nearestObj == null)
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
                            }
                            else if(collider.CompareTag("FryingPan") && objHeld.GetComponent<IngredientManager>().ingredientSO.canFry)
                            {
                                fryingPan = currentPlacementObj.GetComponent<FryingPan>();
                                fryingPan.PlaceIngredientInPan(objHeld);
                            }
                            else if(collider.CompareTag("Pot") && objHeld.GetComponent<IngredientManager>().ingredientSO.canBoil)
                            {
                                pot = currentPlacementObj.GetComponent<Pot>();
                                pot.PlaceIngredientInPot(objHeld);
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
                                fryingPan.PlaceIngredientInPan(ingredient);
                            }
                        }
                        else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                        {
                            plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                            if(fryingPan.isDoneCooking)
                            {
                                fryingPan.PlaceFoodInPlate(plateScript);
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
                            }
                        }
                        else if(colliderAtPosition.gameObject.CompareTag("Plate"))
                        {
                            plateScript = colliderAtPosition.gameObject.GetComponent<Plate>();
                            if(pot.isDoneCooking)
                            {
                                pot.PlaceSoupInPlate(plateScript);
                            }
                        }
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
            if (isStockStation && closestObj.gameObject.CompareTag("StockStation") && !objHeld && playerMovement.IsObjectInteractable(closestObj.transform)) // If player is interacting with a StockStation
            {
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
                    
                }
                else if(objHeld.CompareTag("FryingPan"))
                {
                    fryingPan = objHeld.GetComponent<FryingPan>();
                    fryingPan.isHoldingFryingPan = true;
                }
                else if(objHeld.CompareTag("Pot"))
                {
                    pot = objHeld.GetComponent<Pot>();
                    pot.isHoldingPot = true;
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
            isStockStation = false;
            isServingStation = false;
            isTrashCan = false;


            if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation") ||other.CompareTag("StockStation") || other.CompareTag("ServingStation") || other.CompareTag("Stove"))
            {
                accessTable = true;
                // tableTopPos = other.transform;

                if(other.CompareTag("CuttingStation"))
                {
                    isCuttingStation = true;
                }

                if(other.CompareTag("StockStation"))
                {
                    isStockStation = true;
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
            isStockStation = false;
            isServingStation = false;
            isStove = false;
        }

        closestObj = null;
    }

}
