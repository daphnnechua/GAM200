using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private GameObject fadeOverlay;

    private float fadeDuration = 1f;

    [SerializeField] private List<AudioClip> clickButtonSfx;

    // Start is called before the first frame update
    void Start()
    {
        levelLoadoutInterface.SetActive(false);

        masterController = FindObjectOfType<MasterController>();
        masterController.canPause = false; //dont pause in start menu

        startButton.onClick.AddListener(() => StartButton());
        // settingButton.onClick.AddListener(() => masterController.LoadScene("StartMenu_Settings"));
        levelLoadout.onClick.AddListener(()=> OpenLevelLoadOut());
        quitButton.onClick.AddListener(() => Application.Quit());

        levelLoadOutCloseButton.onClick.AddListener(() => CloseLevelLoadOut());

        foreach(var e in levelLoadoutButtons)
        {
            string levelName = e.levelToLoad;
            e.button.onClick.AddListener(() => LoadLevel(levelName));

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
                levelLoadoutInterface.SetActive(false);
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
        int index = Random.Range(0, clickButtonSfx.Count);

        SoundFXManager.instance.PlaySound(clickButtonSfx[index], transform, 0.5f);

        levelLoadoutInterface.SetActive(true);

    }

    private void CloseLevelLoadOut()
    {
        int index = Random.Range(0, clickButtonSfx.Count);

        SoundFXManager.instance.PlaySound(clickButtonSfx[index], transform, 0.5f);

        levelLoadoutInterface.SetActive(false);
    }

    private IEnumerator FadeToBlackFromStart()
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

        masterController.LoadScene(masterController.firstScene);
    }

    private IEnumerator FadeToBlackFromLevelLoadOut(string levelName)
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

        masterController.LoadScene(levelName);
    }

    private void LoadLevel(string levelName)
    {
        int index = Random.Range(0, clickButtonSfx.Count);

        SoundFXManager.instance.PlaySound(clickButtonSfx[index], transform, 0.5f);

        StartCoroutine(FadeToBlackFromLevelLoadOut(levelName));
    }

    private void StartButton()
    {
        int index = Random.Range(0, clickButtonSfx.Count);

        SoundFXManager.instance.PlaySound(clickButtonSfx[index], transform, 0.5f);
        
        StartCoroutine(FadeToBlackFromStart());
    }


    [System.Serializable]
    public class LevelButtons
    {
        public Button button;
        public string levelToLoad;
    }


}
