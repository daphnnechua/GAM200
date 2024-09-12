using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPickUp : MonoBehaviour
{
    public Transform ingredientPlacement;
    public LayerMask pickUpMask;
    public Vector3 dir {get; set;}
    private GameObject ingredientHeld;
    private GameObject player;

    private Transform tableTopPos;
    public bool accessTable = false;
    public bool isCuttingStation = false;

    // Start is called before the first frame update
    void Start()
    {        player = GameObject.FindWithTag("Player");
        ingredientPlacement = GameObject.FindGameObjectWithTag("IngredientPlacement").transform; 
        pickUpMask = LayerMask.GetMask("Ingredient");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(ingredientHeld)
            {
                PutDownIngredient();

            }
            else //pickup ingredient
            {
                PickUpIngredient();
                
            }
        }
    }

    private void PutDownIngredient()
    {
        if(!accessTable)
        {
            dir = new Vector2(0, -1f);
            ingredientHeld.transform.position = transform.position + dir;
            ingredientHeld.transform.parent = null;

            if(ingredientHeld.GetComponent<Rigidbody2D>())
            {
                 ingredientHeld.GetComponent<Rigidbody2D>().simulated = true;
            }
            ingredientHeld = null;
            dir = new Vector2(0, 0);
        }
        else
        {

            ingredientHeld.transform.position = tableTopPos.position;
            ingredientHeld.transform.parent = null;

            if(ingredientHeld.GetComponent<Rigidbody2D>())
            {
                ingredientHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                ingredientHeld.GetComponent<Rigidbody2D>().simulated = true;

            }

            ingredientHeld = null;
        }   
    }

    private void PickUpIngredient()
    {

        Collider2D pickupIngredient = Physics2D.OverlapCircle(transform.position + dir, 0.75f, pickUpMask);
        if(pickupIngredient)
        {
            ingredientHeld = pickupIngredient.gameObject;
            ingredientHeld.transform.position = ingredientPlacement.position;
            ingredientHeld.transform.parent = transform;
            
            if(ingredientHeld.GetComponent<Rigidbody2D>())
            {
                ingredientHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                ingredientHeld.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                ingredientHeld.GetComponent<Rigidbody2D>().simulated = false;
            }
        }
    }

    private void OnDrawGizmos() ///debugging purposes --> shows piickup range for player
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + dir, 0.75f);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation"))
        {
            accessTable = true;
            tableTopPos = other.transform;

            if(other.CompareTag("CuttingStation"))
            {
                isCuttingStation = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("TableTop") || other.CompareTag("CuttingStation"))
        {
            accessTable = false;
            isCuttingStation = false;
        }
    }

}
