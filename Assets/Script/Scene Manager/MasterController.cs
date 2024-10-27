using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    private SceneController currentController;
    public DataManager dataManager;

    private GameController gameController;

    private string currentSceneName;

    public string firstScene = "cutscene_01";
    public string endLevelScene = "End_Level";

    public string startMenuScene = "Start_Menu";

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
    }

    // Update is called once per frame
    void Update()
    {
        //debug purposes
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
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

        if(pauseMenuOpen)
        {
            Time.timeScale = 0f;
        }
        else if(!pauseMenuOpen)
        {
            Time.timeScale = 1f;
        }
    }

    #region scene loading/ unloading
    public void LoadScene(string aScene)
    {
        // string firstLevel = Game.GetLevelList()[0].levelName; //this is tutorial

        RemoveScene(currentSceneName);

        currentSceneName = aScene;
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

    public void LoadNextLevel()
    {
        pauseMenuOpen = false;
        RemoveScene(endLevelScene);
        RemoveScene(currentSceneName);

        Levels currentLevel = Game.GetLevelByName(currentSceneName);
        int levelIndex = Game.GetLevelList().IndexOf(currentLevel);
        levelIndex++;
        
        LoadScene(Game.GetLevelList()[levelIndex].levelName);
        
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
        
    }

    public void PauseGame()
    {
        List<Levels> allLevels = Game.GetLevelList();
        foreach(var level in allLevels)
        {
            if(level.levelName == currentSceneName)
            {
                SceneManager.LoadSceneAsync("Pause_Menu", LoadSceneMode.Additive);
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
        RemoveScene("Pause_Menu");
        if(gameController!=null)
        {
            gameController.isPaused = false;
        }
        pauseMenuOpen = false;
    }
    #endregion scene loading/ unloading

    
}
