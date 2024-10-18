using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BatterySlots : MonoBehaviour
{
    public BatterySlots currentBatterySlot;
    public  Battery currentBattery; 

    public bool CanPlaceBattery(Battery battery)
    {
        return currentBattery == null; //check for batteries in slot
    }

    public void PlaceBattery(Battery battery)
    {
        if (CanPlaceBattery(battery)) //empty slot
        {
            currentBattery = battery; //replay battery
            battery.gameObject.transform.position = gameObject.transform.position; 

        }
    }

    public void RemoveBattery()
    {
        currentBattery = null;
    }

}