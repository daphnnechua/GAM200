using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wires : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image image;
    private LineRenderer lineRenderer;
    private Canvas canvas;
    private bool isDragging = false; 
    public bool isLeftWire;

    [SerializeField] private WireTask wireTask;
    public Color currentColor;

    public bool isCorrectMatch = false;
    private void Awake()
    {
        image = GetComponent<Image>();
        lineRenderer = GetComponent<LineRenderer>();
        canvas = FindObjectOfType<Canvas>();
        wireTask = GetComponentInParent<WireTask>();
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
        if(isDragging)
        {
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out movePos
            );
            
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, canvas.transform.TransformPoint(movePos));
        }
        else
        {
            if(!isCorrectMatch)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
            }

        }

        bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition, canvas.worldCamera);
        if(isHovered)
        {
            wireTask.currentHovered = this;
            // Debug.Log("hovering: " + wireTask.currentHovered.currentColor);
        }
    }

    public void SetImageColour(Color color)
    {
        image.color = color;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        currentColor = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isCorrectMatch)
        {
            return;
        }
        isDragging = true;
        wireTask.currentDragged = this;
        Debug.Log(wireTask.currentDragged);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(wireTask.currentHovered!=null && wireTask.currentDragged!=null)
        {
            RectTransform hoveredRect = wireTask.currentHovered.GetComponent<RectTransform>();

            // Check if the hovered wire is within the bounds of the dragged wire
            if (RectTransformUtility.RectangleContainsScreenPoint(hoveredRect, Input.mousePosition, canvas.worldCamera))
            {
                if(wireTask.currentHovered.currentColor == currentColor && wireTask.currentDragged.isLeftWire != wireTask.currentHovered.isLeftWire)
                {
                    wireTask.currentDragged.isCorrectMatch = true;
                    wireTask.currentHovered.isCorrectMatch = true;

                    // Fix the line between the matched wires
                    wireTask.currentDragged.lineRenderer.SetPosition(0, wireTask.currentDragged.transform.position);
                    wireTask.currentDragged.lineRenderer.SetPosition(1, wireTask.currentHovered.transform.position);                
                    // Debug.Log($"successful match! {wireTask.currentHovered}");
                }
            }

        isDragging = false;
        wireTask.currentDragged = null;
        }
    }

    public void ResetWires()
    {
        isCorrectMatch = false;
    }
}
