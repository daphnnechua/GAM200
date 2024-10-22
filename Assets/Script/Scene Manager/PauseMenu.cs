using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private MasterController masterController;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();

        resumeButton.onClick.AddListener(() => masterController.UnpauseGame());
        restartButton.onClick.AddListener(() => masterController.RestartLevel());
        quitButton.onClick.AddListener(() => masterController.QuitToStart());
        closeButton.onClick.AddListener(() => masterController.UnpauseGame());

    }
}
