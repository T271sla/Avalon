using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> units;
    public List<BaseHero> heroes = new List<BaseHero>();
    public List<BaseEnemy> enemies = new List<BaseEnemy>();

    public BaseHero selectedHero;
    public BaseEnemy selectedEnemy;

    public GameObject grid;
    public int aggroDistance;

    private void Awake()
    {
        Instance = this;
        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void spawnHeroes()
    {
        foreach (Transform child in grid.transform.GetChild(1))
        {
            var spawnedHero = child;
            var healthBar = spawnedHero.GetComponent<BaseUnit>().bar.GetComponent<Healthbar>();
            healthBar.setColor(Color.cyan);
            healthBar.setMaxHealth(spawnedHero.GetComponent<BaseUnit>().unitMaxHealth);
            healthBar.setValue(spawnedHero.GetComponent<BaseUnit>().unitHealth);
            GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).setUnit(spawnedHero.GetComponent<BaseUnit>());
            Instance.heroes.Add((BaseHero)GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).OccupiedUnit);
        }

        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void spawnEnemies()
    {
        foreach (Transform child in grid.transform.GetChild(2))
        {
            var spawnedHero = child;
            Vector2 pos = new Vector2(0, 0);
            pos.x = Mathf.Round(spawnedHero.transform.position.x);
            pos.y = Mathf.Round(spawnedHero.transform.position.y);
            spawnedHero.transform.position = pos;
            var healthBar = spawnedHero.GetComponent<BaseUnit>().bar.GetComponent<Healthbar>();
            healthBar.setColor(Color.red);
            healthBar.setMaxHealth(spawnedHero.GetComponent <BaseUnit>().unitMaxHealth);
            healthBar.setValue(spawnedHero.GetComponent<BaseUnit>().unitHealth);
            GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).setUnit(spawnedHero.GetComponent<BaseUnit>());
            Instance.enemies.Add((BaseEnemy)GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).OccupiedUnit);
        }

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        var factionUnits = units.Where(u => u.Faction == faction).ToList();

        if (factionUnits.Count == 0)
        {
            Debug.LogError($"No units found for faction {faction}");
            return null;
        }

        return (T)factionUnits.OrderBy(o => Random.value).First().unitPrefab;
    }

    public void SetSeclectedHero(BaseHero hero)
    {
        selectedHero = hero;
    }

    public void SetSelectedEnemy(BaseEnemy enemy)
    {
        selectedEnemy = enemy;
    }

    public IEnumerator EnemyContoller()
    {
        yield return new WaitForSeconds(2.5f);
        foreach (BaseEnemy enemy in Instance.enemies)
        {
            if(enemy.active)
            {
                StartCoroutine(enemy.newAI());
            }
            enemy.active = false;
            if (!Instance.heroesRemain())
            {
                yield break;
            }
        }
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.TurnChange);
    }

    public void checkUnitActivity()
    {
        foreach (BaseHero hero in Instance.heroes)
        {
            if (hero.active)
            {
                return; // If any hero is active, return early.
            }
        }
        GameManager.Instance.ChangeState(GameState.EnemiesTurn);
    }

    public void endUnitTurn()
    {
        Instance.selectedHero.active = false;
        Instance.SetSeclectedHero(null);
        Instance.checkUnitActivity();
    }

    public bool enemiesRemain()
    {
        if(Instance.enemies.Count == 0)
        {
            GameManager.Instance.ChangeState(GameState.Victory);
            return false;
        }

        return true;
    }

    public bool heroesRemain()
    {
        if (Instance.heroes.Count == 0)
        {
            GameManager.Instance.ChangeState(GameState.Lose);
            return false;
        }

        return true;
    }

    public void upgradeWarrior()
    {
        foreach(BaseHero hero in Instance.heroes) 
        {
            if (hero.unitName == "Skeleton Warrior")
            {
                Rage skillToChange = (Rage)hero.skill;
                skillToChange.attackBonus = 1.25f;
            }
        }
    }
    public void upgradeLancer()
    {
        foreach (BaseHero hero in Instance.heroes)
        {
            if(hero.unitName == "Skeleton Lancer")
            {
                hero.unitRange++;
            }
        }
    }
}
