using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManagerMemento
{
    public List<BaseUnitMemento> heroes = new List<BaseUnitMemento>();
    public List<BaseUnitMemento> enemies = new List<BaseUnitMemento>();

    public int aggroDistance;
    public bool sideWeaker = false;

    public GridManagerMemento gridManagerMemento;

    public UnitManagerMemento(UnitManager unitManager, GridManagerMemento gridManagerMemento)
    {
        this.gridManagerMemento = gridManagerMemento;

        foreach (BaseHero hero in unitManager.heroes)
        {
            this.heroes.Add(new BaseUnitMemento(hero, this, gridManagerMemento));
        }

        foreach (BaseEnemy enemy in unitManager.enemies)
        {
            this.enemies.Add(new BaseUnitMemento(enemy, this, gridManagerMemento));
        }

        this.aggroDistance = unitManager.aggroDistance;
        this.sideWeaker = unitManager.enemiesWeaker;
    }

    public UnitManagerMemento(UnitManagerMemento unitManagerMemento, GridManagerMemento gridManagerMemento)
    {
        this.gridManagerMemento = gridManagerMemento;

        foreach (BaseUnitMemento hero in unitManagerMemento.heroes)
        {
            this.heroes.Add(new BaseUnitMemento(hero, this, gridManagerMemento));
        }

        foreach (BaseUnitMemento enemy in unitManagerMemento.enemies)
        {
            this.enemies.Add(new BaseUnitMemento(enemy, this, gridManagerMemento));
        }

        this.aggroDistance = unitManagerMemento.aggroDistance;
        this.sideWeaker = unitManagerMemento.sideWeaker;
    }

    public List<List<MoveMemento>> getChildStates(bool enemyTurn)
    {
        float enemyStrength = calculateEnemyStrength();
        float heroStrength = calculateHeroStrength();
        sideWeaker = enemyStrength < heroStrength;

        List<BaseUnitMemento> units = enemyTurn ? this.enemies : this.heroes;
        List<BaseUnitMemento> rivalUnits = enemyTurn ? this.heroes : this.enemies;
        if (!enemyTurn) sideWeaker = !sideWeaker;

        foreach (BaseUnitMemento unit in units)
        {
            unit.AIv2(rivalUnits, sideWeaker);
        }

        List<List<MoveMemento>> allStates = findAllStates(units);

        filterMoves(ref allStates, rivalUnits);

        return allStates;
    }

    public List<List<MoveMemento>> findAllStates(List<BaseUnitMemento> units)
    {
        var moveMementos = units
            .Where(u => u.possibleMoves != null && u.possibleMoves.Count > 0)
            .Select(u => u.possibleMoves)
            .ToList();

        if (moveMementos.Count == 0 || moveMementos.Any(l => l == null || l.Count == 0))
            return new List<List<MoveMemento>>();

        return GenerateCombinations(moveMementos);
    }

    public void filterMoves(ref List<List<MoveMemento>> allStates, List<BaseUnitMemento> rivalUnits)
    {
        var filteredStates = allStates.Where(list =>
        {
            if (list.GroupBy(m => m.Tile).Any(g => g.Count() > 1))
                return false;

            foreach (var rival in rivalUnits)
            {
                var heroMoves = list.Where(m => m.Target?.unitId == rival.unitId);

                float damage = 0;
                bool killFound = false;

                foreach (var move in heroMoves)
                {
                    damage += move.damage;
                    if (damage >= rival.unitHealth && !killFound)
                    {
                        move.targetKilled = true;
                        killFound = true;
                    }
                    else if (killFound)
                    {
                        return false;
                    }
                }
            }
            return true;
        }).ToList();

        if (filteredStates.Count > 0)
        {
            allStates = filteredStates;
        }
    }


    public static List<List<T>> GenerateCombinations<T>(List<List<T>> lists)
    {
        if (lists == null || lists.Count == 0 || lists.Any(l => l == null || l.Count == 0))
            return new List<List<T>>();

        var result = new List<List<T>> { new List<T>() };

        foreach (var currentList in lists)
        {
            var temp = new List<List<T>>();

            foreach (var combination in result)
            {
                foreach (var item in currentList)
                {
                    var newCombo = new List<T>(combination) { item };
                    temp.Add(newCombo);

                    if (temp.Count > 1000) break;
                }
            }
            result = temp;
            if (result.Count > 1000) break;
        }

        return result;
    }

    public float calculateHeroStrength()
    {
        float sideStrength = 0;

        foreach (BaseUnitMemento hero in heroes)
        {
            sideStrength += hero.getUnitStrength();
        }

        return sideStrength;
    }

    public float calculateEnemyStrength()
    {
        float sideStrength = 0;

        foreach (BaseUnitMemento enemy in enemies)
        {
            sideStrength += enemy.getUnitStrength();
        }

        return sideStrength;
    }

    public void checkUnitActivity()
    {
        foreach (BaseUnitMemento hero in heroes)
        {
            if (hero.active)
            {
                return;
            }
        }
    }

    public bool enemiesRemain()
    {
        if (enemies.Count == 0)
        {
            return false;
        }

        return true;
    }

    public bool heroesRemain()
    {
        if (heroes.Count == 0)
        {
            return false;
        }

        return true;
    }
}
