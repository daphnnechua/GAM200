using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class StockSO : ScriptableObject
{
    public string stationName;

    public string objType;

    public string typing;
    public GameObject prefab;  
    
    public string ingredientID;

    public string prefabName;

    public List<AudioClip> soundSfx;
}
