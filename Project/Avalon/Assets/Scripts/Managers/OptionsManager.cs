using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public void returnButton()
    {
        SceneManager.UnloadSceneAsync("Options Menu");
    }

    public void fullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
