using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverloadBar : MonoBehaviour
{
    private RestockingController restockingController;

    public int currentOverloadCount = 0;
    public int maxOverloadCount = 10;

    public int minigamesToComplete;
    public int completedMinigames=0;

    private int routineOverloadReductionVal = 2;
    [SerializeField] private List<Slider> slider = new List<Slider>();
    [SerializeField] private List<Image> sliderFill = new List<Image>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOverloadBarVisuals();
    }

    public void IncreaseOverloadValue(int ingredientsRestocked)
    {
        // currentOverloadCount += ingredientsRestocked;

        currentOverloadCount = maxOverloadCount;

        // Debug.Log($"overload count increased! current: {currentOverloadCount}");

        if(currentOverloadCount<maxOverloadCount)
        {
            completedMinigames=0;
            minigamesToComplete=1;
        }
        else
        {
            completedMinigames=0;
            minigamesToComplete=3;
        }
    }

    public void DecreaseOverloadValue()
    {
        if(currentOverloadCount < routineOverloadReductionVal)
        {
            currentOverloadCount = 0;
        }
        else if(currentOverloadCount<maxOverloadCount)
        {
            currentOverloadCount -= routineOverloadReductionVal;
        }
        else if(currentOverloadCount>=maxOverloadCount)
        {
            currentOverloadCount = maxOverloadCount/2; //decrease by half if itt enters overload state
        }

        if(currentOverloadCount<0)
        {
            Debug.LogWarning("overload count is less than 0!");
        }
    }

    public bool IsDroneOverloaded()
    {
        if(currentOverloadCount >= maxOverloadCount)
        {
            return true;
        }
        return false;
    }

    private void UpdateOverloadBarVisuals()
    {
        for(int i =0; i<slider.Count;i++)
        {
            slider[i].value = currentOverloadCount;
            slider[i].maxValue = maxOverloadCount;

            if(currentOverloadCount <= 3)
            {
                //set fill colour to green
                sliderFill[i].color = Color.green;
            }
            else if (currentOverloadCount > 3 && currentOverloadCount <= 8)
            {
                //set fill colour to yellow
                sliderFill[i].color = Color.yellow;
            }
            else if(currentOverloadCount > 8)
            {
                //set fill colour to red
                sliderFill[i].color = Color.red;
            }
        }
    }
}
