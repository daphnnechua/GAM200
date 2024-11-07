using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockStation
{
    public string stockStationID {get;}
    public string stockStationName {get;}
    public string ingredientID {get;}
    public string imageFilePath {get;}

    public string emptyImagePath;
    public string indicatorImage;


    public StockStation(string stockStationID, string stockStationName, string ingredientID, string imageFilePath, string emptyImagePath, string indicatorImage)
    {
        this.stockStationID = stockStationID;
        this.stockStationName = stockStationName;
        this.ingredientID = ingredientID;
        this.imageFilePath = imageFilePath;
        this.emptyImagePath = emptyImagePath;
        this.indicatorImage = indicatorImage;
    }
}
