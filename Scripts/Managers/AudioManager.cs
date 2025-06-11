using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicSource, effectSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.musicSource.clip = musicSource.clip;
            Instance.playMusic();
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void playSound(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void pauseMusic()
    {
        musicSource.Pause();
    }

    public void playMusic()
    {
        musicSource.Play();
    }

    public void setMusic(AudioSource musicSource)
    {
        this.musicSource.Stop();
        this.musicSource = musicSource;
        this.musicSource.Play();
    }

    public void setMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void setEffectsVolume(float volume)
    {
        effectSource.volume = volume;
    }
}
