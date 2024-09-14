using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class IngredientSO : ScriptableObject
{
    //for assigning respective values to the ingredients
    public string causeActionID;
    public string prevStateID;
    public string ingredientID;
    public string ingredientName;
    public bool canCut;
    public bool canCook;
    public bool isReady;
    public GameObject ingredientPrefab;
}
