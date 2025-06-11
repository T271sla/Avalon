using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class StateAI
{
    public UnitManagerMemento unitManagerMemento;
    public GridManagerMemento gridManagerMemento;
    public List<MoveMemento> stateMoves;
    public bool enemyTurn = true;
    public int stateScore = 0;

    public StateAI(UnitManager unitManager, GridManager gridManager, bool enemyTurn)
    {
        gridManagerMemento = new GridManagerMemento(gridManager);
        unitManagerMemento = new UnitManagerMemento(unitManager, gridManagerMemento);
        this.enemyTurn = enemyTurn;

        setAllUnits();
    }

    public StateAI(UnitManagerMemento unitManagerMemento, GridManagerMemento gridManagerMemento, List<MoveMemento> stateMoves, bool enemyTurn, int stateScore)
    {
        this.gridManagerMemento = new GridManagerMemento(gridManagerMemento);
        this.unitManagerMemento = new UnitManagerMemento(unitManagerMemento, gridManagerMemento);
        this.stateMoves = stateMoves;
        this.enemyTurn = enemyTurn;
        this.stateScore = stateScore;

        setAllUnits();

        int movesScore = 0;

        foreach(MoveMemento move in stateMoves)
        {
            TileMemento tile = gridManagerMemento.GetTileAtPoint(move.Tile.position);
            BaseUnitMemento unit = gridManagerMemento.GetTileAtPoint(move.Unit.position).OccupiedUnit;

            if(unit == null)
            {
                unit = tile.OccupiedUnit;
            }
            tile.setUnit(unit);

            if (move.Target != null)
            {
                BaseUnitMemento target = gridManagerMemento.GetTileAtPoint(move.Target.position).OccupiedUnit;
                target.modifyHealth(move.damage);
            }

            movesScore += move.Score;
        }

        if(enemyTurn)
        {
            this.stateScore += movesScore;
        }
        else
        {
            this.stateScore -= movesScore;
        }
    }

    public void setAllUnits()
    {
        foreach(BaseUnitMemento hero in unitManagerMemento.heroes) 
        {
            if (hero == null)
            {
                Debug.Log("22");
            }
            gridManagerMemento.GetTileAtPoint(hero.position).setUnit(hero);
        }

        foreach (BaseUnitMemento enemy in unitManagerMemento.enemies)
        {
            if (enemy == null)
            {
                Debug.Log("33");
            }
            gridManagerMemento.GetTileAtPoint(enemy.position).setUnit(enemy);
        }
    }

    public bool IsTerminal()
    {
        bool enemiesRemain = unitManagerMemento.enemiesRemain();
        bool heroesRemain = unitManagerMemento.heroesRemain();

        if(enemiesRemain && heroesRemain)
        {
            return false;
        }

        if ((enemyTurn && !heroesRemain) || (!enemyTurn && heroesRemain))
        {
            stateScore += 2000;
        }
        else if ((enemyTurn && !enemiesRemain) || (!enemyTurn && enemiesRemain))
        {
            stateScore -= 2000;
        }

        return true;
    }
}

public class TileMemento
{
    public bool isWalkable;
    public string tileName;
    public float attackBonus, defenseBonus;

    public BaseUnitMemento OccupiedUnit;
    public bool Walkable => isWalkable && OccupiedUnit == null;
    public float movementCost = 1;
    public Vector2 position;

    public TileMemento(Tile tile)
    {
        this.isWalkable = tile.isWalkable;
        this.tileName = tile.tileName;
        this.attackBonus = tile.attackBonus;
        this.defenseBonus = tile.defenseBonus;
        this.movementCost = tile.movementCost;
        this.position = tile.transform.position;
    }

    public void setUnit(BaseUnitMemento unit)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.position = this.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    public BaseUnitMemento getUnit()
    {
        return this.OccupiedUnit;
    }
}

public class GridManagerMemento
{
    public float width, height;
    private Dictionary<Vector2, TileMemento> tiles = new Dictionary<Vector2, TileMemento>();

    public GridManagerMemento(GridManager gridManager)
    {
        this.width = gridManager.width;
        this.height = gridManager.height;

        Dictionary<Vector2, Tile> tiles = gridManager.getTiles();

        foreach(KeyValuePair<Vector2, Tile> pair in tiles)
        {
            this.tiles[pair.Key] = new TileMemento(pair.Value);
        }
    }

    public GridManagerMemento(GridManagerMemento gridManagerMemento)
    {
        this.width = gridManagerMemento.width;
        this.height = gridManagerMemento.height;
        this.tiles = gridManagerMemento.tiles;
    }

    public TileMemento GetTileAtPoint(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    public Dictionary<Vector2, TileMemento> getTiles()
    {
        return tiles;
    }
}