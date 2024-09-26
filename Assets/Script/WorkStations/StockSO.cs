using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StockSO : ScriptableObject
{
    public string stationName;

    public string objType;
    public GameObject prefab;  
    
    public string ingredientID;
}
