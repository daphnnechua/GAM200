using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    private SceneController currentController;
    public DataManager dataManager;


    private string currentSceneName;

    public string firstScene = "SampleScene";
    public string endLevelScene = "End_Level";

    public string startMenuScene = "Start_Menu";

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
    }

    public void LoadScene(string aScene)
    {
        // string firstLevel = Game.GetLevelList()[0].levelName; //this is tutorial

        if(currentSceneName == startMenuScene && aScene == firstScene)
        {
            RemoveScene(startMenuScene);
        }
        
        if(currentSceneName == aScene)
        {
            RemoveScene(currentSceneName);
        }

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
    }

    public void LoadNextLevel()
    {
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
            }
    }

    public void RestartLevel()
    {
        if (currentController != null)
        {            
            RemoveScene(endLevelScene);

            LoadScene(currentSceneName);
        }
    }

    public void LoadEndOfLevelScene()
    {
        SceneManager.LoadSceneAsync(endLevelScene, LoadSceneMode.Additive);
    }

    public void QuitToStart()
    {
        RemoveScene(endLevelScene);
        RemoveScene(currentSceneName);

        LoadScene(startMenuScene);
    }

}
