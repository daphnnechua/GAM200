using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Orders
{
    public Guid OrderId { get; private set; }
    public Recipe Recipe { get; private set; }
    public float RemainingTime { get; set; }

    public float ExpiryTime {get; set;}

    public Orders(Recipe recipe, float remainingTime, float expiryTime)
    {
        OrderId = Guid.NewGuid();
        Recipe = recipe;
        RemainingTime = remainingTime;
        ExpiryTime = expiryTime;
    }
}
