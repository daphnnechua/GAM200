using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WireTask : MonoBehaviour, IMinigame
{
    public List<Color> wireColors = new List<Color>();
    public List<Wires> leftWires = new List<Wires>();
    public List<Wires> rightWires = new List<Wires>();

    private List<Color> availableColor;
    private List<int> availableLeftWireIndex;
    private List<int> availableRightWireIndex;
    public Wires currentDragged;
    public Wires currentHovered;

    private bool isTaskComplete =  false;

    private GameObject wireTaskInterface;
    [SerializeField] private GameObject droneMenu;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private GameObject completionWindow;
    private OverloadBar overloadBar;
    private DroneStation droneStation;
    private bool isOpen = true;

    void Start()
    {
        // wireTaskInterface = GameObject.FindWithTag("WireTask");
        overloadBar = FindObjectOfType<OverloadBar>();
        droneStation = FindObjectOfType<DroneStation>();
        // wireTaskInterface.SetActive(false);
    }

    public void StartMinigame()
    {
        InitializeMinigame();

        droneMenu = GameObject.FindWithTag("DroneMenu");
        droneMenu.SetActive(false);

        wireTaskInterface.SetActive(true);
        completionWindow.SetActive(false);
    }
    
    public void InitializeMinigame()
    {
        wireTaskInterface = GameObject.FindWithTag("WireTask");
        availableColor = new List<Color>(wireColors);
        availableLeftWireIndex = new List<int>();
        availableRightWireIndex = new List<int>();

        for(int i=0; i<leftWires.Count; i++)
        {
            availableLeftWireIndex.Add(i);
        }
        for(int i =0; i<rightWires.Count;i++)
        {
            availableRightWireIndex.Add(i);
        }

        while(availableColor.Count> 0 && availableLeftWireIndex.Count> 0 && availableRightWireIndex.Count>0)
        {
            int randomColorIndex = Random.Range(0, availableColor.Count);
            Color randomColor = availableColor[randomColorIndex];
            randomColor.a = 1;
            int leftWire = Random.Range(0, availableLeftWireIndex.Count);
            int rightWire = Random.Range(0, availableRightWireIndex.Count);

            leftWires[availableLeftWireIndex[leftWire]].SetImageColour(randomColor);
            rightWires[availableRightWireIndex[rightWire]].SetImageColour(randomColor);

            availableColor.RemoveAt(randomColorIndex);
            availableLeftWireIndex.RemoveAt(leftWire);
            availableRightWireIndex.RemoveAt(rightWire);
        }
        StartCoroutine(CheckTaskStatus());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            CloseWindow();
            droneMenu.SetActive(true);
        }
    }

    private IEnumerator CheckTaskStatus()
    {
        while(!isTaskComplete)
        {
            int successfulMatches = 0;
            for(int i =0; i<rightWires.Count;i++)
            {
                if(rightWires[i].isCorrectMatch)
                {
                    successfulMatches++;
                }
            } 
            if(successfulMatches >= leftWires.Count)
            {
                Debug.Log("Task complete");
                completionWindow.SetActive(true);
                completionText.text = "Task Complete!";
                isTaskComplete = true;
                StartCoroutine(CloseTimer());
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CloseTimer()
    {
        // Debug.Log("taask complete. closing in 3");
        yield return new WaitForSeconds(3f);
        // Debug.Log("closing...");
        //close task window after 3 seconds
        CloseWindow();
    }

    private void CloseWindow()
    {
        foreach (var wire in leftWires)
        {
            wire.ResetWires(); // Ensure this method exists in your Wires class
        }
        
        foreach (var wire in rightWires)
        {
            wire.ResetWires(); // Ensure this method exists in your Wires class
        }
        wireTaskInterface.SetActive(false);
        isOpen = false;

        if(isTaskComplete)
        {
            overloadBar.DecreaseOverloadValue();
        }

        isTaskComplete = false;
        droneMenu.SetActive(true);
        droneStation.isinteracting = true;
    }
}
