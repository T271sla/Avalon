using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SaveManager saveManager;
    public ProgressData progressData;
    public BookmarkSaveManager bookmarkSaveManager;
    public BookmarkSave bookmarkSave;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            AudioManager.Instance.setMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        }

        if (PlayerPrefs.HasKey("effectVolume"))
        {
            AudioManager.Instance.setEffectsVolume(PlayerPrefs.GetFloat("effectsVolume"));
        }

        if (PlayerPrefs.HasKey("fullscreen") && PlayerPrefs.GetInt("fullscreen") == 0)
        {
            Screen.fullScreen = false;
        }

        Instance.saveManager = new SaveManager();
        Instance.bookmarkSaveManager = new BookmarkSaveManager();
    }

    public void createProgressSave()
    {
        saveManager.createSave();
    }

    public void loadProgressSave()
    {
        progressData = saveManager.loadSave();
    }

    public void createBookmark()
    {
        bookmarkSaveManager.createSave();
    }

    public void loadBookmark()
    {
        bookmarkSave = Instance.bookmarkSaveManager.loadSave();
        this.progressData = bookmarkSave.progressData;
    }

    public void deleteBookmark()
    {
        if(this.bookmarkSave == null)
        {
            return;
        }

        Instance.bookmarkSaveManager.deleteSave();
    }
}

public class SaveManager
{
    public void createSave()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        ProgressSave tempSave = new ProgressSave();
        ProgressData state = new ProgressData(tempSave);

        formatter.Serialize(stream, state);
        stream.Close();
    }

    public ProgressData loadSave()
    {
        string path = Application.persistentDataPath + "/save.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ProgressData progress = formatter.Deserialize(stream) as ProgressData;
            stream.Close();

            return progress;
        }
        else
        {
            Debug.LogError("Save not found in " + path);
            return null;
        }
    }
}

public class ProgressSave
{
    public List<LevelStatistics> levels = new List<LevelStatistics>();
    public int completedLevels = 0;

    public bool warriorUpdgrade1 = false;
    public bool warriorUpdgrade2 = false;

    public bool lancerUpdgrade1 = false;
    public bool lancerUpdgrade2 = false;

    public bool ghostUpdgrade1 = false;
    public bool ghostUpdgrade2 = false;

    public ProgressSave()
    {
        completedLevels = ProgressManager.Instance.completedLevels;
        warriorUpdgrade1 = ProgressManager.Instance.warriorUpdgrade1;
        warriorUpdgrade2 = ProgressManager.Instance.warriorUpdgrade2;
        lancerUpdgrade1 = ProgressManager.Instance.lancerUpdgrade1;
        lancerUpdgrade2 = ProgressManager.Instance.lancerUpdgrade2;
        ghostUpdgrade1 = ProgressManager.Instance.ghostUpdgrade1;
        ghostUpdgrade2 = ProgressManager.Instance.ghostUpdgrade2;
        levels = ProgressManager.Instance.levels;
    }
}

public class BookmarkSaveManager
{
    public void createSave()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/bookmark.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        BookmarkSave state = new BookmarkSave(UnitManager.Instance.heroes, UnitManager.Instance.enemies);

        formatter.Serialize(stream, state);
        stream.Close();
    }

    public BookmarkSave loadSave()
    {
        string path = Application.persistentDataPath + "/bookmark.bin";
        if(File.Exists(path)) 
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            BookmarkSave state = formatter.Deserialize(stream) as BookmarkSave;
            stream.Close();

            return state;
        }
        else
        {
            Debug.LogError("Bookmark not found in " + path);
            return null;
        }
    }

    public void deleteSave()
    {
        string path = Application.persistentDataPath + "/bookmark.bin";
        File.Delete(path);
    }
}

[System.Serializable]
public class BookmarkSave
{
    public List<BaseUnitData> heroDatas = new List<BaseUnitData>();
    public List<BaseUnitData> enemyDatas = new List<BaseUnitData>();
    public int gameTurnSaved;
    public string sceneName;
    public ProgressData progressData;

    public BookmarkSave()
    {
        
    }

    public BookmarkSave(List<BaseHero> heroes, List<BaseEnemy> enemies)
    {
        foreach (BaseHero hero in heroes)
        {
            BaseUnitData heroData = new BaseUnitData((BaseUnit)hero);
            heroDatas.Add(heroData);
        }

        foreach (BaseEnemy enemy in enemies)
        {
            BaseUnitData enemyData = new BaseUnitData((BaseUnit)enemy);
            enemyDatas.Add(enemyData);
        }

        this.gameTurnSaved = GameManager.Instance.gameTurn;
        this.sceneName = SceneManager.GetActiveScene().name;

        ProgressSave tempSave = new ProgressSave();
        this.progressData = new ProgressData(tempSave);
    }
}
