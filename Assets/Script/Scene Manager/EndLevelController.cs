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

    [SerializeField] private TextMeshProUGUI ordersServed;

    [SerializeField] private TextMeshProUGUI ordersFailed;

    [SerializeField] private List<Image> starImages;

    [SerializeField] private List<TextMeshProUGUI> starPoints;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject fadeOverlay;

    [SerializeField] private Image endLevelBackground;

    private float fadeDuration = 1f;
    private bool canProceedToNextLevel = false;

    private int starsAccumulated = 0;
    
    [SerializeField] private List<AudioClip> clickButtonSounds;

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

        if(!canProceedToNextLevel)
        {
            nextLevelButton.gameObject.SetActive(false);

            ordersServed.text  = $"Orders Delivered: {gameController.ordersDelivered}";
            ordersFailed.text = $"Orders Failed: {gameController.ordersFailed}";

            levelPoints.text = $"Total: {gameController.points}";

            for(int i =0; i<pointsRequired.Count;i++)
            {
                starPoints[i].text = pointsRequired[i].ToString();
            }

            string filePath = "end screens/DAYFAILED";
            SetImage(filePath, endLevelBackground);
            
        }
        else
        {
            ordersServed.text  = $"Orders Delivered: {gameController.ordersDelivered}";
            ordersFailed.text = $"Orders Failed: {gameController.ordersFailed}";

            levelPoints.text = $"TOTAL: {gameController.points}";

            if(gameController.sceneType == "Tutorial")
            {
                string filePath = "end screens/TUTORIAL";
                SetImage(filePath, endLevelBackground);

                string acquiredStar = "end screens/star";
                for(int i =0; i<numberOfStars;i++)
                {
                    SetImage(acquiredStar, starImages[i]);
                }
                
            }
            else if(gameController.sceneType == "Normal")
            {
                if(gameController.sceneName == "Level_1")
                {
                    string filePath = "end screens/DAY1";
                    SetImage(filePath, endLevelBackground);
                }
                else if(gameController.sceneName == "Level_2")
                {
                    string filePath = "end screens/DAY2";
                    SetImage(filePath, endLevelBackground);
                }
                else if(gameController.sceneName == "Level_3")
                {
                    string filePath = "end screens/DAY1";
                    SetImage(filePath, endLevelBackground);
                }

                if(starsAccumulated > 0)
                {
                    string acquiredStar = "end screens/star";
                    for(int i =0; i<starsAccumulated;i++)
                    {
                        SetImage(acquiredStar, starImages[i]);
                    }
                }
            }

            for(int i =0; i<pointsRequired.Count;i++)
            {
                starPoints[i].text = pointsRequired[i].ToString();
            }
        }


    }

    private void SetImage(string filePath, Image image)
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
        int random = Random.Range(0, clickButtonSounds.Count);
        SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
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

        Levels currentLevel = Game.GetLevelByName(gameController.sceneName);
        int current = Game.GetLevelList().IndexOf(currentLevel);

        Levels nextLevel = Game.GetLevelList()[current + 1];

        masterController.LoadScene(nextLevel.levelName);

    }

    private void NextLevel()
    {
        int random = Random.Range(0, clickButtonSounds.Count);
        SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
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
        int random = Random.Range(0, clickButtonSounds.Count);
        SoundFXManager.instance.PlaySound(clickButtonSounds[random], transform, 1f);
        StartCoroutine(StartMenuFadeToBlack());
    }

}
