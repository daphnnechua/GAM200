using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    private MasterController masterController;

    [SerializeField] private Button startButton;
    // [SerializeField] private Button settingButton;
    [SerializeField] private Button levelLoadout;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject levelLoadoutInterface;

    [SerializeField] private List<LevelButtons> levelLoadoutButtons;

    [SerializeField] private Button levelLoadOutCloseButton;

    // Start is called before the first frame update
    void Start()
    {
        levelLoadoutInterface.SetActive(false);

        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = false; //dont pause in start menu

        startButton.onClick.AddListener(() => masterController.LoadScene(masterController.firstScene));
        // settingButton.onClick.AddListener(() => masterController.LoadScene("StartMenu_Settings"));
        levelLoadout.onClick.AddListener(()=> OpenLevelLoadOut());
        quitButton.onClick.AddListener(() => Application.Quit());

        levelLoadOutCloseButton.onClick.AddListener(() => CloseLevelLoadOut());

        foreach(var e in levelLoadoutButtons)
        {
            string levelName = e.levelToLoad;
            e.button.onClick.AddListener(() => masterController.LoadScene(levelName));

            // e.button.onClick.AddListener(() => DebugLevelLoadout(levelName));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(levelLoadoutInterface.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CloseLevelLoadOut();
            }
        }
    }

    //debug purpose
    // private void DebugLevelLoadout(string levelName)
    // {
    //     Debug.Log($"level to load: {levelName}");
    // }

    private void OpenLevelLoadOut()
    {
        levelLoadoutInterface.SetActive(true);
    }

    private void CloseLevelLoadOut()
    {
        levelLoadoutInterface.SetActive(false);
    }

    [System.Serializable]
    public class LevelButtons
    {
        public Button button;
        public string levelToLoad;
    }
}
