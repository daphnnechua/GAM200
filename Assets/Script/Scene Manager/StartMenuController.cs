using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    private MasterController masterController;

    [SerializeField] private Button startButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button levelLoadout;
    [SerializeField] private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();

        startButton.onClick.AddListener(() => masterController.LoadScene(masterController.firstScene));
        settingButton.onClick.AddListener(() => masterController.LoadScene("StartMenu_Settings"));
        levelLoadout.onClick.AddListener(()=> masterController.LoadScene("Level_LoadOut"));
        quitButton.onClick.AddListener(() => Application.Quit());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
