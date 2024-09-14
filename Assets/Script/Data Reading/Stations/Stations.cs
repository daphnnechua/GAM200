using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stations
{
    public string stationID {get;}
    public string stationName {get;}
    public string actionID {get;}
    public int requiredIngredientNumber {get;}

    public Stations(string stationID, string stationName, string actionID, int requiredIngredientNumber)
    {
        this.stationID = stationID;
        this.stationName = stationName;
        this.actionID = actionID;
        this.requiredIngredientNumber = requiredIngredientNumber;
    }
}
