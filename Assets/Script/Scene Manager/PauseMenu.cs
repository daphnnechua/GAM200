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

    [SerializeField] private GameObject fadeOverlay;

    private float fadeDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        masterController = FindObjectOfType<MasterController>();

        resumeButton.onClick.AddListener(() => masterController.UnpauseGame());
        restartButton.onClick.AddListener(() => Restart());
        quitButton.onClick.AddListener(() => StartMenu());
        closeButton.onClick.AddListener(() => masterController.UnpauseGame());

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
        Time.timeScale = 1f;
        StartCoroutine(StartMenuFadeToBlack());
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
        Time.timeScale = 1f;
        StartCoroutine(RestartFadeToBlack());
    }
    
}
