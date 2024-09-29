using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigames
{
    public string minigameID {get;}
    public string minigameName {get;}
    public string filePath {get;}

    public Minigames(string minigameID, string minigameName, string filePath)
    {
        this.minigameID = minigameID;
        this.minigameName = minigameName;
        this.filePath = filePath;
    }
}
