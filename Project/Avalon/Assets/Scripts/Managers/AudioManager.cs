using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicSource, effectSource;

    private void Awake()
    {
        Instance = this;
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

    public void setEffectsVolume(float volume)
    {
        effectSource.volume = volume;
    }
}
