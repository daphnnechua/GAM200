using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    public string actionID {get;}
    public string actionName {get;}
    public float timeRequired {get;}
    public string workstationID {get;}

    public Actions(string actionID, string actionName, float timeRequired, string workstationID)
    {
        this.actionID = actionID;
        this.actionName = actionName;
        this.timeRequired = timeRequired;
        this.workstationID = workstationID;
    }
}
