using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource sfxPrefab;
    [SerializeField] private AudioSource bgmPrefab;

    [SerializeField] private AudioSource ambientPrefab;

    private AudioSource bgm;
    private AudioSource ambientSFX;

    private Dictionary<string, AudioSource> activeSFX = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySound(AudioClip audioClip, Transform transform, float volume)
    {
        if (activeSFX.ContainsKey(audioClip.name))
        {
            return;
        }


        AudioSource audioSource = Instantiate(sfxPrefab, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        activeSFX[audioClip.name] = audioSource;

        Destroy(audioSource.gameObject, audioClip.length);

        StartCoroutine(RemoveSfx(audioClip.name, audioClip.length));    
    }

    public AudioSource PlayStoppableSound(AudioClip audioClip, Transform transform, float volume)
    {
        if (activeSFX.ContainsKey(audioClip.name))
        {
            return activeSFX[audioClip.name];
        }

        AudioSource audioSource = Instantiate(sfxPrefab, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        activeSFX[audioClip.name] = audioSource;

        Destroy(audioSource.gameObject, audioClip.length);
        StartCoroutine(RemoveSfx(audioClip.name, audioClip.length));

        return audioSource;    
    }

    public IEnumerator RemoveSfx(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (activeSFX.ContainsKey(soundName))
        {
            activeSFX.Remove(soundName);
        }
    }

    public void PlayBackgroundMusic(AudioClip audioClip, float volume)
    {
        if (bgm == null)
        {
            bgm = Instantiate(bgmPrefab, transform.position, Quaternion.identity);  // Instantiate from prefab

            bgm.clip = audioClip;
            bgm.volume = volume;
            bgm.loop = true;
            bgm.Play();

        }

        bgm.clip = audioClip;
        bgm.volume = volume;
        bgm.loop = true;
        bgm.Play();

    }
    public void StopBackgroundMusic()
    {
        if (bgm !=null && bgm.isPlaying)
        {
            bgm.Stop();
        }

    }

    public void PlayAmbientSFX(AudioClip audioClip, float volume)
    {
        if (ambientSFX == null)
        {
            ambientSFX = Instantiate(ambientPrefab, transform.position, Quaternion.identity);  // Instantiate from prefab

            ambientSFX.clip = audioClip;
            ambientSFX.volume = volume;
            ambientSFX.loop = true;
            ambientSFX.Play();

        }

        ambientSFX.clip = audioClip;
        ambientSFX.volume = volume;
        ambientSFX.loop = true;
        ambientSFX.Play();

    }

    public void StopAmbientSFX()
    {
        if(ambientSFX!=null && ambientSFX.isPlaying)
        {
            ambientSFX.Stop();
        }
    }

    public IEnumerator FadeOutMusic(float duration)
    {
        if (bgm.isPlaying)
        {
            float startVolume = bgm.volume;

            while (bgm.volume > 0)
            {
                bgm.volume -= startVolume * Time.deltaTime / duration;
                yield return null;
            }

            StopBackgroundMusic();
            bgm.volume = startVolume; // Reset volume for future playback
        }
    }

}
