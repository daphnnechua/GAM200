using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Battery : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BatteryMinigame batteryMinigame;
    private Vector3 resetPos;
    private RectTransform rectTransform;
    [SerializeField] private string batteryType;
    private BatterySlots currentSlot;

    private Canvas canvas;

    private bool isFalling = false;
    private float dropSpeed = 1000f;
    
    void Start()
    {
        batteryMinigame = FindObjectOfType<BatteryMinigame>();
        rectTransform = GetComponent<RectTransform>();

        resetPos = gameObject.transform.position;


    }

    void Update()
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>(); // Attempt to find a new canvas instance
            if (canvas == null) 
            {
                Debug.Log("NO canvas found");
                return; // Exit if no canvas is found
            }
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(batteryMinigame.isTaskComplete) {return;}

        if(currentSlot!=null)
        {
            currentSlot.RemoveBattery(); //reset battery state when player starts dragging
            currentSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(batteryMinigame.isTaskComplete) {return;}

        rectTransform.anchoredPosition += eventData.delta; //move the battery
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(batteryMinigame.isTaskComplete) {return;}
        
        BatterySlots[] slots = FindObjectsOfType<BatterySlots>();
        bool batteryReplaced = false;

        foreach (BatterySlots slot in slots)
        {
            if (IsOverlapping(slot.gameObject))
            {
                if (slot.CanPlaceBattery(this) && batteryType == "New")
                {
                    slot.PlaceBattery(this);
                    currentSlot = slot;
                    batteryReplaced = true;
                    break;
                }
            }
        }

        if (!batteryReplaced)
        {
            ResetPos();
        }
    }

    public void ResetPos()
    {
        if(batteryType == "New")
        {
            transform.position = resetPos;
        }
        else //old batteries fall off the screen after taking them out
        {
            if (!isFalling)
            {
                StartCoroutine(DropOldBatteries());
            }
        }
    }

    private IEnumerator DropOldBatteries()
    {
        isFalling = true;

        while (rectTransform.anchoredPosition.y > -Screen.height)
        {
            rectTransform.anchoredPosition += new Vector2(0, -dropSpeed * Time.deltaTime);
            yield return null; 
        }
        Destroy(gameObject);
    }

    private bool IsOverlapping(GameObject batterySlot)
    {
        Vector2 battPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, transform.position);

        return RectTransformUtility.RectangleContainsScreenPoint(batterySlot.transform as RectTransform, battPos, canvas.worldCamera) && batteryType == "New";
    }
}
