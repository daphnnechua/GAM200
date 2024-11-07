using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class AnimatedSceneController : MonoBehaviour
{
    [SerializeField] private float sceneTime;
    [SerializeField] private GameObject dialogueCanvas;

    [SerializeField] private GameObject skipButton;

    [SerializeField] private GameObject skipCutscenePrompt;

    [SerializeField] private Button confirmSkip;
    [SerializeField] private Button declineSkip;

    [SerializeField] private Image fadeOverlay;

    [SerializeField] private PlayableDirector playableDirector;

    private DialogueController dialogueController;

    private bool hasStartedDialogue = false;

    private bool cutsceneSkipped = false;

    private float fadeDuration = 1f;

    [SerializeField] private List<AudioClip> clickButtonSound;

    // Start is called before the first frame update
    void Start()
    {
        skipButton.GetComponent<Button>().onClick.AddListener(() => OpenSkipPrompt());
        declineSkip.onClick.AddListener(() => CloseSkipPrompt());
        confirmSkip.onClick.AddListener(() => SkipCutscene());

        skipCutscenePrompt.SetActive(false);

        dialogueController = FindObjectOfType<DialogueController>();
        if(sceneTime > 0)
        {
            dialogueCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        sceneTime -= Time.deltaTime;
        if(sceneTime<=0 && !hasStartedDialogue && !cutsceneSkipped)
        {
            StartCoroutine(SkipAnimatedCutscene());
        }
    }

    private void OpenSkipPrompt()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);

        skipCutscenePrompt.SetActive(true);
        playableDirector.Pause();
    }

    private void CloseSkipPrompt()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);
        
        skipCutscenePrompt.SetActive(false);
        playableDirector.Resume();
    }

    private void SkipCutscene()
    {
        int random = Random.Range(0, clickButtonSound.Count);
        SoundFXManager.instance.PlaySound(clickButtonSound[random], transform, 1f);
        
        StartCoroutine(SkipAnimatedCutscene());
    }
    private IEnumerator SkipAnimatedCutscene()
    {
        yield return StartCoroutine(FadeOut());

        playableDirector.time = playableDirector.duration;
        playableDirector.Evaluate(); 
        playableDirector.Stop();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeIn());

        dialogueCanvas.SetActive(true);
        dialogueController.OpenDialogue();
        hasStartedDialogue = true;
        cutsceneSkipped = true;
    }

    private IEnumerator FadeOut()
    {
        skipButton.SetActive(false);
        skipCutscenePrompt.SetActive(false);

        fadeOverlay.gameObject.transform.SetAsLastSibling();
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.color = color;
    }

    private IEnumerator FadeIn()
    {
        
        float elapsedTime = 0f;
        Color color = fadeOverlay.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            fadeOverlay.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeOverlay.color = color;
    }
}
