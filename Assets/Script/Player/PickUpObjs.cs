using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpObjs : MonoBehaviour
{
    public Transform objPlacement;
    public LayerMask pickUpMask;
    public Vector3 dir {get; set;}
    private GameObject objHeld;

    private GameObject player;
    private GameObject trashCan;

    private Transform tableTopPos;
    public bool accessTable = false;
    public bool isCuttingStation = false;
    public bool isStockStation = false;
    public bool isServingStation = false; 

    public bool isTrashCan = false;


    private Trash trashScript;
    private Plate plateScript;

    private Transform closestObj;

    // Start is called before the first frame update
    void Start()
    {        
        player = GameObject.FindWithTag("Player");
        objPlacement = GameObject.FindGameObjectWithTag("IngredientPlacement").transform; 
        pickUpMask = LayerMask.GetMask("KitchenObjs");
        trashCan = GameObject.FindGameObjectWithTag("TrashCan");
        trashScript = trashCan.GetComponent<Trash>();
    }

    // Update is called once per frame
    void Update()
    {
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
        //     }
        //     Debug.Log(closestIngredient);
        // }

        
        if(Input.GetKeyDown(KeyCode.F))
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
        IngredientsAssembly();
        
        Vector2 placePosition = (Vector2)transform.position + new Vector2(0, -1f); ///calculation --> area below the player

        Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.1f, pickUpMask); // Check within a small radius

        if (colliderAtPosition != null) //if there is something at where the obj is supposed to be placed
        {
            return; // Do not place the obj
        }

        if(!accessTable && !isTrashCan) //not accessing any tables, stations or trash can --> place on floor
        {
            dir = new Vector2(0, -1f);
            objHeld.transform.position = transform.position + dir;
            objHeld.transform.parent = null;
            if(objHeld.CompareTag("Plate"))
            {
                plateScript = objHeld.GetComponent<Plate>();
                plateScript.isHoldingPlate = false;
            }

            if(objHeld.GetComponent<Rigidbody2D>())
            {
                 objHeld.GetComponent<Rigidbody2D>().simulated = true;
            }
            objHeld = null;
            dir = new Vector2(0, 0);
        }
        else if(accessTable && IsCounterOccupied() || isServingStation && !plateScript.readyToServe) //station or counter top is occupied --> cannot place down
        {
            return;
        }
        else if(accessTable && !IsCounterOccupied()) //accessing empty table or station
        {
            if(isCuttingStation && objHeld.CompareTag("Plate"))
            {
                return;
            }
            if(objHeld.CompareTag("Plate"))
            {
                plateScript = objHeld.GetComponent<Plate>();
                if(isServingStation && plateScript.readyToServe && objHeld.CompareTag("Plate"))
                {
                    plateScript.isHoldingPlate = false;
                    plateScript.ServePlate();
                }
            }
            if(closestObj)
            {
                if(objHeld.CompareTag("Plate"))
                {
                    plateScript = objHeld.GetComponent<Plate>();
                    plateScript.isHoldingPlate = false;
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
            }
        }
        else if(isTrashCan) //accessing trash can
        {
            trashScript.TrashIngredient(objHeld);
        }   

    }

    private void IngredientsAssembly()
    {
        if(objHeld.CompareTag("Ingredient"))
        {
            if(objHeld.GetComponent<IngredientManager>().ingredientSO.isReady)
            {
                Collider2D collider = Physics2D.OverlapCircle(transform.position + dir, 1f, pickUpMask);
                if(collider != null)
                {
                    GameObject currentPlate = collider.gameObject;
                    if(collider!=null && collider.CompareTag("Plate"))
                    {
                        plateScript = currentPlate.GetComponent<Plate>();
                        plateScript.PlaceIngredient(objHeld);
                    }

                }
            }
        }
        else if(objHeld.CompareTag("Plate"))
        {
            Vector2 placePosition = (Vector2)transform.position + new Vector2(0, -1f); ///calculation --> area below the player

            Collider2D colliderAtPosition = Physics2D.OverlapCircle(placePosition, 0.1f, pickUpMask); // Check within a small radius
            if(colliderAtPosition !=null && colliderAtPosition.gameObject.CompareTag("Ingredient"))
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
        if (isStockStation && closestObj.gameObject.CompareTag("StockStation") && !objHeld) // If player is interacting with a StockStation
        {
            Collider2D objOnStation = Physics2D.OverlapCircle(closestObj.position, 0.1f, pickUpMask);
            if(objOnStation && !objHeld)
            {
                PickUpIngredient();
            }
            else if(!objOnStation && !objHeld)
            {
                GameObject newObj = closestObj.gameObject.GetComponent<StockStation>().GetNewObj(); // Get new object from StockStation
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
    }

    private void PickUpNewObj(GameObject gameObject)
    {
        objHeld = gameObject;
        if(objHeld.CompareTag("Plate"))
        {
            plateScript = objHeld.GetComponent<Plate>();
            plateScript.isHoldingPlate = true;
        }
        objHeld.transform.position = objPlacement.position;
        objHeld.transform.parent = transform;
        objHeld.GetComponent<SpriteRenderer>().sortingOrder = 2;

        if (objHeld.GetComponent<Rigidbody2D>())
        {
            objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            objHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            objHeld.GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    private void PickUpIngredient()
    {
        //find the closest ingredient
        Collider2D[] nearbyObjs = Physics2D.OverlapCircleAll(transform.position + dir, 1f, pickUpMask); //all nearby ingredients

        if (nearbyObjs.Length > 0) //if there are ingredient nearby
        {
            GameObject closestObj = null;
            float closestDist = Mathf.Infinity;

            foreach (Collider2D objs in nearbyObjs)
            {
                float distance = Vector3.Distance(transform.position, objs.transform.position); //relative distance between player and ingredient
                if (distance < closestDist) //finding the closest distance
                {
                    closestDist = distance;
                    closestObj = objs.gameObject;
                }
            }
        
            if(closestObj!=null)
            {
                
                objHeld = closestObj;
                if(objHeld.CompareTag("Plate"))
                {
                    plateScript = objHeld.GetComponent<Plate>();
                    plateScript.isHoldingPlate = true;
                }
                objHeld.transform.position = objPlacement.position;
                objHeld.transform.parent = transform;
                objHeld.GetComponent<SpriteRenderer>().sortingOrder = 2;
                
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
        Gizmos.DrawWireSphere(transform.position + dir, 1f);
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


            if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation") ||other.CompareTag("StockStation") || other.CompareTag("ServingStation"))
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
            }

            if(other.CompareTag("TrashCan"))
            {
                isTrashCan = true;
            }

        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation") || other.CompareTag("TrashCan") ||other.CompareTag("StockStation") || other.CompareTag("ServingStation"))
        {
            accessTable = false;
            isCuttingStation = false;
            isTrashCan = false;
            isStockStation = false;
            isServingStation = false;
        }

        closestObj = null;
    }

}
