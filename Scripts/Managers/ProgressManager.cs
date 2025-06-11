using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;
    public List<LevelStatistics> levels = new List<LevelStatistics>();
    [SerializeField] private int numberOfLevels;
    public int completedLevels = 0;
    public int upgradeCoin;

    public bool warriorUpdgrade1 = false;
    public bool warriorUpdgrade2 = false;

    public bool lancerUpdgrade1 = false;
    public bool lancerUpdgrade2 = false;

    public bool ghostUpdgrade1 = false;
    public bool ghostUpdgrade2 = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        List<string> scenes = new List<string>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            scenes.Add(scenePath);
        }

        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].Contains("Level"))
            {
                levels.Add(new LevelStatistics(i));
            }
            else
            {
                break;
            }
        }

        numberOfLevels = levels.Count;

        this.upgradeCoin = this.completedLevels;

        DontDestroyOnLoad(gameObject);
    }

    public void setProgress(ProgressData progress)
    {
        completedLevels = progress.completedLevels;
        this.upgradeCoin = completedLevels;
        warriorUpdgrade1 = progress.warriorUpdgrade1;
        warriorUpdgrade2 = progress.warriorUpdgrade2;
        lancerUpdgrade1 = progress.lancerUpdgrade1;
        lancerUpdgrade2 = progress.lancerUpdgrade2;
        ghostUpdgrade1 = progress.ghostUpdgrade1;
        ghostUpdgrade2 = progress.ghostUpdgrade2;

        for(int i = 0; i < progress.levels.Count; i++)
        {
            this.levels[i].completed = progress.levels[i].completed;
        }
    }

    public void resetProgress()
    {
        completedLevels = 0;
        this.upgradeCoin = completedLevels;
        warriorUpdgrade1 = false;
        warriorUpdgrade2 = false;
        lancerUpdgrade1 = false;
        lancerUpdgrade2 = false;
        ghostUpdgrade1 = false;
        ghostUpdgrade2 = false;

        foreach(LevelStatistics level in this.levels) 
        {
            level.completed = false;
        }
    }
}

public class LevelStatistics
{
    public int levelNumber;
    public bool completed = false;

    public LevelStatistics(int levelNumber)
    {
        this.levelNumber = levelNumber;
    }

    public void completion()
    {
        if(completed) 
        {
            return;
        }

        completed = true;
        ProgressManager.Instance.completedLevels++;
        ProgressManager.Instance.upgradeCoin++;
    }
}
