using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelController : MonoBehaviour
{
    private GameController gameController;
    private MasterController masterController;

    [SerializeField] private TextMeshProUGUI levelPoints;

    [SerializeField] private List<Image> starImages;

    [SerializeField] private List<TextMeshProUGUI> starPoints;

    [SerializeField] private TextMeshProUGUI levelEndHeader;


    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject fadeOverlay;

    private float fadeDuration = 1f;
    private bool canProceedToNextLevel = false;

    private int starsAccumulated = 0;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();
        gameController = FindObjectOfType<GameController>();
        // SetLevelEndText();

        restartButton.onClick.AddListener(() => Restart());
        nextLevelButton.onClick.AddListener(() => NextLevel());
        quitButton.onClick.AddListener(()=> StartMenu());

        CheckLevelCompletion();
        SetLevelCompletionStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CheckLevelCompletion()
    {
        string levelName = gameController.sceneName;

        Stars currentLevelRequirements = Game.GetLevelStarsByLevelName(levelName);

        if(currentLevelRequirements.levelType == "Tutorial")
        {
            canProceedToNextLevel = true;
        }
        else
        {
            List<int> pointsRequired = currentLevelRequirements.pointsRequired.ToList();
            for(int i =0; i<pointsRequired.Count; i++)
            {
                if(gameController.points >= pointsRequired[i])
                {
                    canProceedToNextLevel = true;
                    starsAccumulated++;
                }
            }
        }
    }

    private void SetLevelCompletionStatus()
    {
        string levelName = gameController.sceneName;

        Stars currentLevelRequirements = Game.GetLevelStarsByLevelName(levelName);

        int numberOfStars = currentLevelRequirements.availableStars;
        List<int> pointsRequired = currentLevelRequirements.pointsRequired.ToList();

        foreach(var e in starImages)
        {
            e.gameObject.SetActive(false);
        }

        for(int i = 0; i < numberOfStars;i++)
        {
            starImages[i].gameObject.SetActive(true);
        }

        foreach(var e in starImages)
        {
            e.color = Color.gray; //change to star image later
        }

        if(!canProceedToNextLevel)
        {
            nextLevelButton.gameObject.SetActive(false);
            levelEndHeader.text = "Level Failed!";

            levelPoints.text = $"Total: {gameController.points}";

            for(int i =0; i<pointsRequired.Count;i++)
            {
                starPoints[i].text = pointsRequired[i].ToString();
            }
            
        }
        else
        {
            

            levelPoints.text = $"Total: {gameController.points}";

            if(gameController.sceneType == "Tutorial")
            {
                levelEndHeader.text = "Tutorial Complete!";
                for(int i =0; i<numberOfStars;i++)
                {
                    starImages[i].color = Color.white; //change to star image later
                }
                
            }
            else if(gameController.sceneType == "Normal")
            {
                levelEndHeader.text = "Level Complete!";
                if(starsAccumulated > 0)
                {
                    for(int i =0; i<starsAccumulated;i++)
                    {
                        starImages[i].color = Color.white; //change to star image later
                    }
                }
            }

            for(int i =0; i<pointsRequired.Count;i++)
            {
                starPoints[i].text = pointsRequired[i].ToString();
            }
        }


    }

    private void SetStarImage(string filePath, Image image)
    {
        AssetManager.LoadSprite(filePath, (Sprite sp) =>
        {
            image.sprite = sp;
        });
    }

    private IEnumerator RestartFadeToBlack()
    {
        fadeOverlay.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeOverlay.GetComponent<Image>().color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.GetComponent<Image>().color = color;

        StartCoroutine(SoundFXManager.instance.FadeOutMusic(fadeDuration));
        yield return new WaitForSeconds(fadeDuration);
        masterController.RestartLevel();

    }

    private void Restart()
    {
        StartCoroutine(RestartFadeToBlack());
    }

    private IEnumerator NextLevelFadeToBlack()
    {
        fadeOverlay.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeOverlay.GetComponent<Image>().color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.GetComponent<Image>().color = color;

        StartCoroutine(SoundFXManager.instance.FadeOutMusic(fadeDuration));
        yield return new WaitForSeconds(fadeDuration);
        masterController.LoadNextLevel();

    }

    private void NextLevel()
    {
        StartCoroutine(NextLevelFadeToBlack());
    }

    private IEnumerator StartMenuFadeToBlack()
    {
        fadeOverlay.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeOverlay.GetComponent<Image>().color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.GetComponent<Image>().color = color;

        StartCoroutine(SoundFXManager.instance.FadeOutMusic(fadeDuration));
        yield return new WaitForSeconds(fadeDuration);
        masterController.QuitToStart();

    }

    private void StartMenu()
    {
        StartCoroutine(StartMenuFadeToBlack());
    }

}
