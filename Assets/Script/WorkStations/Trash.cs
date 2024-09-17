using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{

    public void TrashIngredient(GameObject Obj)
    {
        if(Obj!=null && Obj.CompareTag("Ingredient"))
        {
            Destroy(Obj);
            Debug.Log("ingredient is thrown away");
        }
        else
        {
            Debug.Log("no ingredient to be found");
        }
    }

}
