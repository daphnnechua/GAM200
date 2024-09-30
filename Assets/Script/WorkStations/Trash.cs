using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private Plate plate;
    public void TrashIngredient(GameObject Obj)
    {
        if(Obj!=null && Obj.CompareTag("Ingredient"))
        {
            Destroy(Obj);
            Debug.Log("ingredient is thrown away");
        }
        else if(Obj!=null && Obj.CompareTag("Plate"))
        {
            plate = Obj.GetComponent<Plate>();
            plate.TrashPlate();
        }
        else
        {
            Debug.Log("no ingredient to be found");
        }
    }

}
