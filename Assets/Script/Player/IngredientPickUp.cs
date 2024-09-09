using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPickUp : MonoBehaviour
{
    public Transform ingredientPlacement;
    public LayerMask pickUpMask;
    public Vector3 dir {get; set;}
    private GameObject ingredientHeld;
    // Start is called before the first frame update
    void Start()
    {
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
            else //pickup ingredient
            {
                Collider2D pickupIngredient = Physics2D.OverlapCircle(transform.position + dir, 1f, pickUpMask);
                if(pickupIngredient)
                {
                    ingredientHeld = pickupIngredient.gameObject;
                    ingredientHeld.transform.position = ingredientPlacement.position;
                    ingredientHeld.transform.parent = transform;

                    if(ingredientHeld.GetComponent<Rigidbody2D>())
                    {
                        ingredientHeld.GetComponent<Rigidbody2D>().simulated = false;
                    }
                }
                Debug.Log(pickupIngredient);
            }
        }
    }

    private void OnDrawGizmos() ///debugging purposes --> shows piickup range for player
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + dir, 1f);
    }
}
