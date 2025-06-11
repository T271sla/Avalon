using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField]
    private GameObject selectedHeroObject, tileObject, fullTileObject, tileUnitObject, battleMenuObject, skillButtonObject, cancelButtonObject, upgradeMenuObject,
        pauseMenuObject, victoryScreenObject, loseScreenObject, playerPhaseObject, enemyPhaseObject, levelButtonsObject;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    public bool attackPressed = false;
    public bool skillsPressed = false;
    public bool battleUIActive = false;
    public bool subMenuActive => attackPressed || skillsPressed;

    private void Awake()
    {
        Instance = this;
    }

    public void showTileInfo(Tile tile)
    {
        if (tile == null)
        {
            tileObject.SetActive(false);
            tileUnitObject.SetActive(false);
            return;
        }
        tileObject.GetComponentInChildren<Text>().text = tile.tileName;
        tileObject.SetActive(true);

        if(tile.OccupiedUnit != null) 
        {
            tileUnitObject.GetComponentInChildren<Text>().text = tile.OccupiedUnit.unitName;
            tileUnitObject.SetActive(true);
        }
    }

    public void showFullTileInfo(Tile tile)
    {
        if (tile == null)
        {
            fullTileObject.SetActive(false);
            return;
        }
        fullTileObject.transform.GetChild(0).GetComponent<Text>().text = tile.tileName;
        fullTileObject.transform.GetChild(1).GetComponent<Text>().text = (tile.attackBonus * 100).ToString() + "%";
        fullTileObject.transform.GetChild(2).GetComponent<Text>().text = (tile.defenseBonus * 100).ToString() + "%";
        fullTileObject.transform.GetChild(3).GetComponent<Text>().text = (tile.movementCost).ToString();
        fullTileObject.SetActive(true);
    }

    public void showSelectedUnit(BaseUnit unit)
    {
        if (unit == null)
        {
            selectedHeroObject.SetActive(false);
            return;
        }

        selectedHeroObject.transform.GetChild(0).GetComponent<Text>().text = unit.unitName;
        selectedHeroObject.transform.GetChild(1).GetComponent<Text>().text = unit.Class.ToString();
        selectedHeroObject.transform.GetChild(2).GetComponent<Text>().text = unit.unitHealth.ToString() + "/" + unit.unitMaxHealth.ToString();
        selectedHeroObject.transform.GetChild(3).GetComponent<Text>().text = (unit.unitAttack * unit.attackBuff).ToString();
        selectedHeroObject.transform.GetChild(21).GetComponent<Text>().text = unit.unitAttack.ToString();
        selectedHeroObject.transform.GetChild(4).GetComponent<Text>().text = unit.unitRange.ToString();
        selectedHeroObject.transform.GetChild(5).GetComponent<Text>().text = unit.unitMove.ToString();
        if (unit.skill != null)
        {
            selectedHeroObject.transform.GetChild(6).GetComponent<Text>().text = unit.skill.name;
            selectedHeroObject.transform.GetChild(7).GetComponent<Text>().text = unit.skill.skillDescription;
            selectedHeroObject.transform.GetChild(11).GetComponent<Text>().text = "Skill";
        }
        else
        {
            selectedHeroObject.transform.GetChild(6).GetComponent<Text>().text = "";
            selectedHeroObject.transform.GetChild(7).GetComponent<Text>().text = "";
            selectedHeroObject.transform.GetChild(11).GetComponent<Text>().text = "";
        }
        selectedHeroObject.transform.GetChild(8).GetComponent<Text>().text = unit.OccupiedTile.tileName;
        selectedHeroObject.transform.GetChild(9).GetComponent<Text>().text = (unit.OccupiedTile.attackBonus * 100).ToString() + "%";
        selectedHeroObject.transform.GetChild(10).GetComponent<Text>().text = (unit.OccupiedTile.defenseBonus * 100).ToString() + "%";
        selectedHeroObject.SetActive(true);
    }

    public void buttonsActivity(bool buttonsActivity)
    {
        if(UnitManager.Instance.selectedHero != null)
        {
            if (UnitManager.Instance.selectedHero.transform.position.x < GridManager.Instance.width / 2)
            {
                battleMenuObject.transform.position = new Vector2(Screen.width * 0.77f, battleMenuObject.transform.position.y);
            }
            else
            {
                battleMenuObject.transform.position = new Vector2(0, battleMenuObject.transform.position.y);
            }
        }

        if(buttonsActivity)
        {
            textMeshProUGUI.text = UnitManager.Instance.selectedHero.skill.skillName;
            skillButtonActivity(UnitManager.Instance.selectedHero.skill.isActive);
        }
        battleUIActive = buttonsActivity;
        battleMenuObject.SetActive(buttonsActivity);
    }

    public void skillButtonActivity(bool skillButtonActivity)
    {
        skillButtonObject.SetActive(skillButtonActivity);
    }

    public void subMenuActivity(bool buttonsActivity)
    {
        cancelButtonObject.SetActive(buttonsActivity);
    }

    public void attackButton()
    {
        Instance.buttonsActivity(false);
        Instance.subMenuActivity(true);
        Instance.attackPressed = true;
        UnitManager.Instance.selectedHero.OccupiedTile.setHighlightsActivity(UnitManager.Instance.selectedHero, true, UnitManager.Instance.selectedHero.unitRange);
    }
    public void skillsButton()
    {
        Instance.buttonsActivity(false);
        Instance.subMenuActivity(true);
        Instance.skillsPressed = true;
    }

    public void unitSkillButton()
    {
        UnitManager.Instance.selectedHero.skill.Activate(UnitManager.Instance.selectedHero, null);
        if(UnitManager.Instance.selectedHero != null) 
        {
            textMeshProUGUI.text = UnitManager.Instance.selectedHero.skill.skillName;
        }
    }

    public void cancelButton()
    {
        Instance.attackPressed = false;
        Instance.skillsPressed = false;
        Instance.subMenuActivity(false);
        Instance.buttonsActivity(true);
        UnitManager.Instance.selectedHero.OccupiedTile.setHighlightsActivity(UnitManager.Instance.selectedHero, false, UnitManager.Instance.selectedHero.unitRange);
    }

    public void waitButton()
    {
        UnitManager.Instance.endUnitTurn();
        Instance.buttonsActivity(false);
    }

    public IEnumerator playerPhase()
    {
        yield return new WaitForSeconds(0.5f);
        playerPhaseObject.GetComponentInChildren<Text>().text = "Turn " + GameManager.Instance.gameTurn.ToString();
        playerPhaseObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        playerPhaseObject.SetActive(false);
    }

    public IEnumerator enemyPhase()
    {
        yield return new WaitForSeconds(0.5f);
        enemyPhaseObject.GetComponentInChildren<Text>().text = "Turn " + GameManager.Instance.gameTurn.ToString();
        enemyPhaseObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        enemyPhaseObject.SetActive(false);
    }

    public void victoryScreen()
    {
        victoryScreenObject.SetActive(true);
    }

    public void loseScreen()
    {
        loseScreenObject.SetActive(true);
    }

    public void pauseMenuActivity(bool activity)
    {
        pauseMenuObject.SetActive(activity);
    }

    public void resumeButton()
    {
        GameManager.Instance.unpauseGame();
    }
    public void restartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void saveAndExitButton()
    {
        DataManager.Instance.createBookmark();
        SceneManager.LoadScene("Main Menu");
    }

    public void exitButton()
    {
        Application.Quit();
    }

    public void tileScreenButton()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void newGameButton()
    {
        ProgressManager.Instance.resetProgress();
        levelSelect();
    }

    public void loadGameButton()
    {
        DataManager.Instance.loadProgressSave();

        if (DataManager.Instance.progressData != null)
        {
            ProgressManager.Instance.setProgress(DataManager.Instance.progressData);
        }

        levelSelect();
    }

    public void levelSelect()
    {
        SceneManager.LoadScene("Level Select");

        for(int i = 1; i < levelButtonsObject.transform.childCount; i++) 
        {
            if(i <= ProgressManager.Instance.completedLevels)
            {
                continue;
            }

            levelButtonsObject.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void bookmarkButton()
    {
        DataManager.Instance.loadBookmark();

        if (DataManager.Instance.progressData != null)
        {
            ProgressManager.Instance.setProgress(DataManager.Instance.progressData);
        }

        BookmarkSave loadedBookmark = DataManager.Instance.bookmarkSaveManager.loadSave();

        SceneManager.LoadScene(loadedBookmark.sceneName);
        GameManager.Instance.gameTurn = loadedBookmark.gameTurnSaved;

        DataManager.Instance.deleteBookmark();
    }

    public void optionsButton()
    {
        SceneManager.LoadScene("Options Menu", LoadSceneMode.Additive);
    }

    public void levelOneButton()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void levelTwoButton()
    {
        SceneManager.LoadScene("Level 2");
    }

    public void levelThreeButton()
    {
        SceneManager.LoadScene("Level 3");
    }

    public void levelFourButton()
    {
        SceneManager.LoadScene("Level 4");
    }

    public void levelFiveButton()
    {
        SceneManager.LoadScene("Level 5");
    }

    public void shopButton()
    {
        SceneManager.LoadScene("Shop Menu", LoadSceneMode.Additive);
    }
}
