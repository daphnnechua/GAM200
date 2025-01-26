using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MasterController : MonoBehaviour
{
    private SceneController currentController;
    public DataManager dataManager;

    private GameController gameController;

    private string currentSceneName;

    public string firstScene = "cutscene_01";
    public string endLevelScene = "End_Level";

    public string startMenuScene = "Start_Menu";

    [SerializeField] private AudioClip cutsceneBGM;

    [SerializeField] private AudioClip startMenuBGM;

    [SerializeField] private List<AudioClip> ambientSfx;

    [SerializeField] private AudioClip openPauseMenu;
    [SerializeField]private AudioClip closePauseMenu;

    private List<string> allOpenSceneNames = new List<string>();

    public bool pauseMenuOpen = false;

    public bool canPause = true;

    // Start is called before the first frame update
    void Start()
    {
        dataManager = FindObjectOfType<DataManager>();
        dataManager.LoadAllData();

        currentSceneName = startMenuScene;
        LoadScene(startMenuScene);

        SoundFXManager.instance.PlayBackgroundMusic(startMenuBGM, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //debug purposes
        // if(Input.GetKeyDown(KeyCode.R))
        // {
        //     RestartLevel();
        // }
        if(Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            gameController = FindObjectOfType<GameController>();
            if(gameController!=null)
            {
                if(!gameController.isPaused)
                {
                    PauseGame();
                }
                else if(gameController.isPaused && allOpenSceneNames.Count==2) //only the level and pause screen is open
                {
                    UnpauseGame();
                }
            }
        }

    }

    #region scene loading/ unloading
    public void LoadScene(string aScene)
    {
        SoundFXManager.instance.StopBackgroundMusic();
        SoundFXManager.instance.StopAmbientSFX();
        
        // string firstLevel = Game.GetLevelList()[0].levelName; //this is tutorial
        Scene sceneToLoad = SceneManager.GetSceneByName(aScene);
        if (sceneToLoad.isLoaded)
        {
            return;
        }
        RemoveScene(currentSceneName);
        
        RemoveScene(endLevelScene);

        currentSceneName = aScene;

        if(aScene!= startMenuScene)
        {
            Levels levelToLoad = Game.GetLevelByName(currentSceneName);
            string levelType = levelToLoad.levelType;

            if(levelType == "Cutscene")
            {
                SoundFXManager.instance.PlayBackgroundMusic(cutsceneBGM, 1);
            }

            int random = Random.Range(0, ambientSfx.Count);
            SoundFXManager.instance.PlayAmbientSFX(ambientSfx[random], 0.05f);
        }

        AsyncOperation loadSceneOp = SceneManager.LoadSceneAsync(aScene, LoadSceneMode.Additive);
        loadSceneOp.completed += (result) =>
        {
            Scene scene = SceneManager.GetSceneByName(aScene);
            GameObject[] rootObjs = scene.GetRootGameObjects();


            foreach (var e in rootObjs)
            {
                    
                SceneController sceneController = e.GetComponentInChildren<SceneController>();
                if (sceneController != null)
                {
                        currentController = sceneController;
                        currentController.Initialize(this);
                        break;
                }
            }
        };
        allOpenSceneNames.Add(aScene);

        
    }
    public void RemoveScene(string aScene)
    {
        Scene sceneToRemove = SceneManager.GetSceneByName(aScene);
    
        if (sceneToRemove.IsValid() && sceneToRemove.isLoaded)
        {
            SceneManager.UnloadSceneAsync(aScene);
            allOpenSceneNames.Remove(aScene);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (currentController != null)
        {   
            pauseMenuOpen = false;
            List<string> scenesToClose = new List<string>();
            foreach(string name in allOpenSceneNames)
            {
                scenesToClose.Add(name);
            }
            foreach(string name in scenesToClose)
            {
                RemoveScene(name);
            }

            LoadScene(currentSceneName);
            
        }
        
    }

    public void LoadEndOfLevelScene()
    {
        pauseMenuOpen = false;
        SceneManager.LoadSceneAsync(endLevelScene, LoadSceneMode.Additive);
        allOpenSceneNames.Add(endLevelScene);
        
    }

    public void QuitToStart()
    {
        Time.timeScale = 1f;
        
        pauseMenuOpen = false;
        List<string> scenesToClose = new List<string>();
        foreach(string name in allOpenSceneNames)
        {
            scenesToClose.Add(name);
        }
        foreach(string name in scenesToClose)
        {
            RemoveScene(name);
        }

        LoadScene(startMenuScene);
        
        SoundFXManager.instance.PlayBackgroundMusic(startMenuBGM,1);
        SoundFXManager.instance.StopAmbientSFX();
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        List<Levels> allLevels = Game.GetLevelList();
        foreach(var level in allLevels)
        {
            if(level.levelName == currentSceneName)
            {
                SceneManager.LoadSceneAsync("Pause_Menu", LoadSceneMode.Additive);
                SoundFXManager.instance.PlaySound(openPauseMenu, transform, 1);
                allOpenSceneNames.Add("Pause_Menu");
                if(gameController!=null)
                {
                    gameController.isPaused = true;
                }
                break;
            }
        }
        pauseMenuOpen = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale =1f;
        RemoveScene("Pause_Menu");
        if(gameController!=null)
        {
            gameController.isPaused = false;
            SoundFXManager.instance.PlaySound(closePauseMenu, transform, 1);
        }
        pauseMenuOpen = false;
    }
    #endregion scene loading/ unloading

    
}
