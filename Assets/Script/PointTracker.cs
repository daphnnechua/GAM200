using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText;

    private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
       pointsText = GetComponentInChildren<TextMeshProUGUI>(); 
       gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePointsUI(int points)
    {
        if(gameController.points<0)
        {
            pointsText.text = $"Points: <color=red>{points}</color>";
        }
        else
        {
            pointsText.text = $"Points: {points}";
        }
    }
}
