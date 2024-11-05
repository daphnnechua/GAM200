using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars
{
    public string levelName {get;}
    public string levelType{get;}
    public int availableStars {get;}
    public int[] pointsRequired {get;}

    public Stars(string levelName, string levelType, int availableStars, int[] pointsRequired)
    {
        this.levelName = levelName;
        this.levelType = levelType;
        this.availableStars = availableStars;
        this.pointsRequired = pointsRequired;
    }
}

public class RefStars
{
    public string levelName;
    public string levelType;
    public int availableStars;
    public int[] pointsRequired;
}
