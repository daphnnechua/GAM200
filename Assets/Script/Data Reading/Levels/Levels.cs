using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels
{
    public string levelNumber {get;}
    public string levelName {get;}
    public string description {get;}

    public string levelType {get;}

    public Levels(string levelNumber, string levelName, string description, string levelType)
    {
        this.levelNumber = levelNumber;
        this.levelName = levelName;
        this.description = description;
        this.levelType = levelType;
    }

}

public class RefLevels
{
    public string levelNumber;
    public string levelName;
    public string description;

    public string levelType;
}
