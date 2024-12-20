using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private Plate plate;
    private FryingPan fryingPan;
    private Pot pot;

    [SerializeField] private List<AudioClip> trashSound;
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
        else if(Obj!=null && Obj.CompareTag("FryingPan"))
        {
            fryingPan = Obj.GetComponent<FryingPan>();
            fryingPan.TrashFoodInPan();
        }
        else if(Obj!= null && Obj.CompareTag("Pot"))
        {
            pot = Obj.GetComponent<Pot>();
            pot.TrashFoodInPot();
        }

        int random = Random.Range(0, trashSound.Count);
        SoundFXManager.instance.PlaySound(trashSound[random], transform, 0.5f);
    }

}
