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

    public bool canFry {get; set;}

    public bool canBoil {get; set;}

    public bool isReady {get; set;}

    public string prefabPath {get;}
    public string imageFilePath {get;}

    public Ingredient(string causeID, string prevStateID, string id, string name, bool canCut, bool canFry, bool canBoil, bool isReady, string prefabPath, string imageFilePath)
    {
        this.causeID = causeID;
        this.prevStateID = prevStateID;
        this.id = id;
        this.name = name;
        this.canCut = canCut;
        this.canFry = canFry;
        this.canBoil = canBoil;
        this.isReady = isReady;
        this.prefabPath = prefabPath;
        this.imageFilePath = imageFilePath;
    }

}
