using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelController : MonoBehaviour
{
    private GameController gameController;
    private MasterController masterController;

    [SerializeField] private TextMeshProUGUI levelPoints;


    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();
        gameController = FindObjectOfType<GameController>();
        SetLevelEndText();

        restartButton.onClick.AddListener(() => masterController.RestartLevel());
        nextLevelButton.onClick.AddListener(() => masterController.LoadNextLevel());
        quitButton.onClick.AddListener(()=> masterController.QuitToStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetLevelEndText()
    {
        levelPoints.text = $"Points: {gameController.points}";
    }
}
