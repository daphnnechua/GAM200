using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient
{
    public string causeID {get;}

    public string prevStateID {get;}
    public string id {get;}

    public string name {get; set;}

    public bool canCut {get; set;}

    public bool canCook {get; set;}

    public bool isReady {get; set;}

    public string prefabPath {get;}
    public string imageFilePath {get;}

    public Ingredient(string causeID, string prevStateID, string id, string name, bool canCut, bool canCook, bool isReady, string prefabPath, string imageFilePath)
    {
        this.causeID = causeID;
        this.prevStateID = prevStateID;
        this.id = id;
        this.name = name;
        this.canCut = canCut;
        this.canCook = canCook;
        this.isReady = isReady;
        this.prefabPath = prefabPath;
        this.imageFilePath = imageFilePath;
    }

}
