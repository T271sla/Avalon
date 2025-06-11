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
    public bool enemiesWeaker = false;

    public StateAI stateToApply;

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
            BaseHero hero = (BaseHero)GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).OccupiedUnit;
            Instance.heroes.Add(hero);
            hero.unitId = Instance.heroes.Count();
            checkForUpgrades(hero);
        }

        if(DataManager.Instance.bookmarkSave.heroDatas.Count() != 0)
        {
            var matches = from hero in Instance.heroes
                          join heroData in DataManager.Instance.bookmarkSave.heroDatas
                          on hero.unitId equals heroData.unitId
                          select new { Hero = hero, Data = heroData };

            foreach (var pair in matches)
            {
                pair.Hero.modifyHealth(pair.Data.unitMaxHealth - pair.Data.unitHealth);
                pair.Hero.bar.setValue(pair.Hero.unitHealth);
                GridManager.Instance.GetTileAtPoint(new Vector2(pair.Data.unitX, pair.Data.unitY)).setUnit(pair.Hero);
                pair.Hero.active = pair.Data.active;

                if(pair.Data.skillData.isActive) 
                {
                    continue;
                }

                pair.Hero.skill.isActive = pair.Data.skillData.isActive;
                pair.Hero.skill.turnCasted = pair.Data.skillData.turnCasted;
                pair.Hero.skill.turnAvailable = pair.Data.skillData.turnAvailable;
                SkillManager.Instance.addSkill(pair.Hero.skill);

                if(pair.Data.unitStatusesData == null || pair.Data.unitStatusesData.Count() == 0) 
                {
                    continue;
                }

                foreach(StatusData statusData in pair.Data.unitStatusesData)
                {
                    Status statusToAdd = StatusManager.Instance.getStatus(statusData.statusName);
                    if (statusToAdd != null)
                    {
                        pair.Hero.addStatus(statusToAdd);
                        pair.Hero.unitStatuses.Last().turnCasted = statusData.turnCasted;
                        pair.Hero.unitStatuses.Last().turnLifted = statusData.turnLifted;
                    }
                }

                DataManager.Instance.bookmarkSave = null;
            }
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
            BaseEnemy enemy = (BaseEnemy)GridManager.Instance.GetTileAtPoint(spawnedHero.transform.position).OccupiedUnit;
            Instance.enemies.Add(enemy);
            enemy.unitId = Instance.heroes.Count() + Instance.enemies.Count();
        }

        if (DataManager.Instance.bookmarkSave.enemyDatas.Count() != 0)
        {
            var matches = from enemy in Instance.enemies
                          join enemyData in DataManager.Instance.bookmarkSave.enemyDatas
                          on enemy.unitId equals enemyData.unitId
                          select new { Enemy = enemy, Data = enemyData };

            foreach (var pair in matches)
            {
                pair.Enemy.modifyHealth(pair.Data.unitMaxHealth - pair.Data.unitHealth);
                pair.Enemy.bar.setValue(pair.Enemy.unitHealth);
                GridManager.Instance.GetTileAtPoint(new Vector2(pair.Data.unitX, pair.Data.unitY)).setUnit(pair.Enemy);

                if (pair.Data.unitStatusesData == null || pair.Data.unitStatusesData.Count() == 0)
                {
                    continue;
                }

                foreach (StatusData statusData in pair.Data.unitStatusesData)
                {
                    Status statusToAdd = StatusManager.Instance.getStatus(statusData.statusName);
                    if(statusToAdd != null)
                    {
                        pair.Enemy.addStatus(statusToAdd);
                        pair.Enemy.unitStatuses.Last().turnCasted = statusData.turnCasted;
                        pair.Enemy.unitStatuses.Last().turnLifted = statusData.turnLifted;
                    }
                }
            }
        }

        enemies.Sort((x, y) => x.Class.CompareTo(y.Class));

        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    public void SetSelectedHero(BaseHero hero)
    {
        selectedHero = hero;
    }

    public void SetSelectedEnemy(BaseEnemy enemy)
    {
        selectedEnemy = enemy;
    }

    public IEnumerator EnemyContoller()
    {
        createStateAI();

        yield return new WaitForSeconds(2.5f);

        foreach (MoveMemento move in stateToApply.stateMoves)
        {
            BaseEnemy enemy = enemies.FirstOrDefault(e => e.unitId == move.Unit.unitId);

            if(enemy == null)
            {
                continue;
            }

            GridManager.Instance.GetTileAtPoint(move.Tile.position).setUnit(enemy);

            yield return new WaitForSeconds(0.5f);

            if (move.Target != null)
            {
                BaseHero hero = heroes.FirstOrDefault(e => e.unitId == move.Target.unitId);
                enemy.Attack(hero);
                yield return new WaitForSeconds(0.5f);
            }

            if (!Instance.heroesRemain())
            {
                yield break;
            }
        }

        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(GameState.TurnChange);
    }

    public async void createStateAI()
    {
        StateAI state = new StateAI(UnitManager.Instance, GridManager.Instance, false);
        AIController controller = new AIController();

        StateAI resultState = await controller.RunSearchAsync(state);

        stateToApply = resultState;
    }

    public float calculateHeroStrength()
    {
        float sideStrength = 0;

        foreach (BaseHero hero in Instance.heroes) 
        {
            sideStrength += hero.getUnitStrength();
        }

        return sideStrength;
    }

    public float calculateEnemyStrength()
    {
        float sideStrength = 0;

        foreach (BaseEnemy enemy in Instance.enemies)
        {
            sideStrength += enemy.getUnitStrength();
        }

        return sideStrength;
    }

    public void checkUnitActivity()
    {
        foreach (BaseHero hero in Instance.heroes)
        {
            if (hero.active)
            {
                return; 
            }
        }
        GameManager.Instance.ChangeState(GameState.EnemiesTurn);
    }

    public void endUnitTurn()
    {
        Instance.selectedHero.active = false;
        Instance.SetSelectedHero(null);
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

    public void checkStatus()
    {
        foreach (BaseHero hero in Instance.heroes)
        {
            if (hero.unitStatuses.Count() == 0 || hero.unitStatuses == null)
            {
                continue;
            }

            List<Status> statusesToRemove = new List<Status>();

            foreach (Status status in hero.unitStatuses)
            {
                if (GameManager.Instance.gameTurn >= status.turnLifted)
                {
                    statusesToRemove.Add(status);
                }
            }

            foreach (Status status in statusesToRemove)
            {
                status.Lift(hero);
                hero.unitStatuses.Remove(status);
                Destroy(status);
            }
        }

        foreach (BaseEnemy enemy in Instance.enemies)
        {
            if (enemy.unitStatuses.Count() == 0 || enemy.unitStatuses == null)
            {
                continue;
            }

            List<Status> statusesToRemove = new List<Status>();

            foreach (Status status in enemy.unitStatuses)
            {
                if (GameManager.Instance.gameTurn >= status.turnLifted)
                {
                    statusesToRemove.Add(status);
                }
            }

            foreach (Status status in statusesToRemove)
            {
                status.Lift(enemy);
                enemy.unitStatuses.Remove(status);
                Destroy(status);
            }
        }
    }

    public void checkForUpgrades(BaseHero hero)
    {
        if(hero.unitName == "Skeleton Warrior" && (ProgressManager.Instance.warriorUpdgrade1 || ProgressManager.Instance.warriorUpdgrade2))
        {
            upgradeWarrior(hero);
            return;
        }

        if (hero.unitName == "Skeleton Lancer" && (ProgressManager.Instance.lancerUpdgrade1 || ProgressManager.Instance.lancerUpdgrade2))
        {
            upgradeLancer(hero);
            return;
        }
    }

    public void upgradeWarrior(BaseHero hero)
    {
        if(ProgressManager.Instance.warriorUpdgrade1)
        {
            Rage skillToChange = (Rage)hero.skill;
            skillToChange.attackBonus = 0.25f;
        }

        if(ProgressManager.Instance.warriorUpdgrade2)
        {
            hero.unitMaxHealth = 150;
            hero.unitHealth = 150;
        }
    }

    public void upgradeLancer(BaseHero hero)
    {
        if (ProgressManager.Instance.lancerUpdgrade1)
        {
            hero.unitRange++;
        }

        if (ProgressManager.Instance.lancerUpdgrade2)
        {
            hero.unitAttack = 65;
        }
    }

    public void upgradeGhost(BaseHero hero)
    {
        Heal skillToChange = (Heal)hero.skill;

        if (ProgressManager.Instance.ghostUpdgrade1)
        {
            skillToChange.healDistance += 2;
        }

        if (ProgressManager.Instance.ghostUpdgrade2)
        {
            skillToChange.healAmmount += 20;
        }
    }
}
