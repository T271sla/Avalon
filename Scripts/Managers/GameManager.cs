using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    private GameState pausedState;
    public int gameTurn = 1;
    [SerializeField] AudioManager audioManager;
    [SerializeField] AudioClip victorySound, gameOverSound;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenarateGrid);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(GameState == GameState.Pause)
            {
                unpauseGame();
            }
            else
            {
                pausedState = GameState;
                ChangeState(GameState.Pause);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        if (GameState == GameState.Victory && GameState == GameState.Lose)
        {
            Debug.LogWarning("Cannot change state after Victory state is set.");
            return;
        }

        GameState = newState;

        switch (newState)
        {
            case GameState.GenarateGrid:
                GridManager.Instance.generateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.spawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.spawnEnemies();
                break;
            case GameState.HeroesTurn:
                StartCoroutine(MenuManager.Instance.playerPhase());
                break;
            case GameState.EnemiesTurn:
                StartCoroutine(MenuManager.Instance.enemyPhase());
                StartCoroutine(UnitManager.Instance.EnemyContoller());
                break;
            case GameState.TurnChange:
                turnChange();
                break;
            case GameState.Victory:
                victory();
                break;
            case GameState.Lose:
                gameOver();
                break;
            case GameState.Pause:
                pauseGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void turnChange()
    {
        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            hero.active = true;
        }
        foreach (BaseEnemy enemy in UnitManager.Instance.enemies)
        {
            enemy.active = true;
        }

        gameTurn++;
        
        SkillManager.Instance.checkSkillsAvailable();
        UnitManager.Instance.checkStatus();

        ChangeState(GameState.HeroesTurn);
    }

    private void victory()
    {
        AudioManager.Instance.pauseMusic();
        AudioManager.Instance.playSound(victorySound);
        MenuManager.Instance.victoryScreen();

        int levelNumber = SceneManager.GetActiveScene().buildIndex;

        ProgressManager.Instance.levels.ElementAt(levelNumber).completion();

        DataManager.Instance.createProgressSave();
    }

    private void gameOver()
    {
        AudioManager.Instance.pauseMusic();
        AudioManager.Instance.playSound(gameOverSound);
        MenuManager.Instance.loseScreen();
    }

    private void pauseGame()
    {
        MenuManager.Instance.pauseMenuActivity(true);
    }

    public void unpauseGame()
    {
        MenuManager.Instance.pauseMenuActivity(false);
        ChangeState(pausedState);
    }
}

public enum GameState
{
    GenarateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4,
    TurnChange = 5,
    Victory = 6,
    Lose = 7,
    Pause = 8,
}
