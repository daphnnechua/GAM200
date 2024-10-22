using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class IngredientSO : ScriptableObject
{
    //for assigning respective values to the ingredients
    public string causeActionID;
    public string prevStateID;
    public string ingredientID;
    public string ingredientName;
    public bool canCut;
    public bool canFry;
    public bool canBoil;
    public bool isReady;
    public string imageName;


}
