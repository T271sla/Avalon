using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider, effectsSlider;
    [SerializeField] private Toggle fullscreenToggleButton;
    [SerializeField] private TMP_Dropdown resolutionDropdow;

    private Resolution[] allResolutions;
    private int selectedResolution;
    List<Resolution> selectedResolutionsList = new List<Resolution>();

    public void Awake()
    {
        if(PlayerPrefs.HasKey("musicVolume"))
        {
            loadMusicVolume();
        }

        if(PlayerPrefs.HasKey("effectVolume"))
        {
            loadEffectsVolume();
        }

        if(PlayerPrefs.HasKey("fullscreen") && PlayerPrefs.GetInt("fullscreen") == 0)
        {
            fullscreenToggleButton.isOn = false;
        }

        allResolutions = Screen.resolutions;

        List<string> resolutionsStringList = new List<string>();
        string newResolution;

        foreach (Resolution resolution in allResolutions) 
        {
            newResolution = resolution.width.ToString() + " x " + resolution.height.ToString();
            if(!resolutionsStringList.Contains(newResolution))
            {
                resolutionsStringList.Add(newResolution);
                selectedResolutionsList.Add(resolution);
            }
        }

        resolutionDropdow.AddOptions(resolutionsStringList);

        if (PlayerPrefs.HasKey("resolution"))
        {
            resolutionDropdow.value = PlayerPrefs.GetInt("resolution");
        }
    }

    public void returnButton()
    {
        SceneManager.UnloadSceneAsync("Options Menu");
    }

    public void changeResolution()
    {
        selectedResolution = resolutionDropdow.value;
        Screen.SetResolution(selectedResolutionsList[selectedResolution].width, selectedResolutionsList[selectedResolution].height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolution", selectedResolution);
    }

    public void fullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void musicSliderChange()
    {
        float volume = musicSlider.value;
        AudioManager.Instance.setMusicVolume(volume);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void effectsSliderChange()
    {
        float volume = musicSlider.value;
        AudioManager.Instance.setEffectsVolume(volume);
        PlayerPrefs.SetFloat("effectsVolume", volume);
    }

    public void loadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        musicSliderChange();
    }

    public void loadEffectsVolume()
    {
        effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume");
        effectsSliderChange();
    }
}
