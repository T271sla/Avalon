using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class BaseUnitMemento
{
    public TileMemento OccupiedTile;
    public Faction Faction;
    public Class Class;
    public string unitName;
    public int unitId = 1;
    public int unitMove;
    public int unitRange;
    public float unitMaxHealth;
    public float unitHealth;
    public float unitAttack;
    public float attackBuff = 1;
    public float defenseBuff = 1;
    public List<TileMemento> validMovementTiles = new List<TileMemento>();
    public List<MoveMemento> possibleMoves = new List<MoveMemento>();
    public List<BaseUnitMemento> possibleTargets = new List<BaseUnitMemento>();
    public bool active = true;
    public Vector2 position;
    public UnitManagerMemento unitManagerMemento;
    public GridManagerMemento gridManagerMemento;
    public bool engaged = false;
    System.Random random = new System.Random();

    public BaseUnitMemento(BaseUnit unit, UnitManagerMemento unitManagerMemento, GridManagerMemento gridManagerMemento)
    {
        this.Faction = unit.Faction;
        this.Class = unit.Class;
        this.unitName = unit.unitName;
        this.unitId = unit.unitId;
        this.unitMove = unit.unitMove;
        this.unitRange = unit.unitRange;
        this.unitMaxHealth = unit.unitMaxHealth;
        this.unitHealth = unit.unitHealth;
        this.unitAttack = unit.unitAttack;
        this.attackBuff = unit.attackBuff;
        this.defenseBuff = unit.defenseBuff;
        this.position = unit.transform.position;
        this.unitManagerMemento = unitManagerMemento;
        this.gridManagerMemento = gridManagerMemento;
    }

    public BaseUnitMemento(BaseUnitMemento unit, UnitManagerMemento unitManagerMemento, GridManagerMemento gridManagerMemento)
    {
        this.Faction = unit.Faction;
        this.Class = unit.Class;
        this.unitName = unit.unitName;
        this.unitId = unit.unitId;
        this.unitMove = unit.unitMove;
        this.unitRange = unit.unitRange;
        this.unitMaxHealth = unit.unitMaxHealth;
        this.unitHealth = unit.unitHealth;
        this.unitAttack = unit.unitAttack;
        this.attackBuff = unit.attackBuff;
        this.defenseBuff = unit.defenseBuff;
        this.position = unit.position;
        this.unitManagerMemento = unitManagerMemento;
        this.gridManagerMemento = gridManagerMemento;
    }

    public virtual void Attack(BaseUnitMemento enemy)
    {
        float attack = attackValue(enemy, OccupiedTile);
        enemy.modifyHealth(attack);
    }

    public float attackValue(BaseUnitMemento enemy, TileMemento tile)
    {
        float attackValue = unitAttack * (attackBuff + tile.attackBonus) + random.Next(-9, 9);
        if ((Class == Class.Strength && enemy.Class == Class.Agility) || (Class == Class.Agility && enemy.Class == Class.Magic) || (Class == Class.Magic && enemy.Class == Class.Strength))
        {
            attackValue *= 1.2f;
        }
        float defenseValue = enemy.defenseBuff + enemy.OccupiedTile.defenseBonus;
        return attackValue / defenseValue;
    }

    public virtual void modifyHealth(float healthToChange)
    {
        unitHealth -= healthToChange;
        if (unitHealth <= 0)
        {
            if (Faction.Equals(Faction.Enemy))
            {
                unitManagerMemento.enemies.Remove(this);
            }
            else
            {
                unitManagerMemento.heroes.Remove(this);
            }
        }

        if (unitHealth > unitMaxHealth)
        {
            unitHealth = unitMaxHealth;
        }
    }

    public List<TileMemento> findValidMovement(TileMemento tile, int distanceBonus)
    {
        List<TileMemento> validTiles = new List<TileMemento>();

        for (float x = tile.position.x - unitMove - distanceBonus; x <= tile.position.x + unitMove + distanceBonus; x++)
        {
            for (float y = tile.position.y - unitMove - distanceBonus; y <= tile.position.y + unitMove + distanceBonus; y++)
            {
                if (!(x >= 0 && x < gridManagerMemento.width && y >= 0 && y < gridManagerMemento.height))
                {
                    continue;
                }

                var currentTile = gridManagerMemento.GetTileAtPoint(new Vector2(x, y));
                if (!currentTile.Walkable)
                {
                    continue;
                }

                List<TileMemento> path = new PathfindingMemento(gridManagerMemento).FindPath(tile, currentTile);

                if (path == null)
                {
                    path = new List<TileMemento>();
                    path.Add(tile);
                }

                if (path.Count - 1 <= unitMove)
                {
                    validTiles.Add(path.ElementAt(path.Count - 1));
                }
            }
        }

        return validTiles;
    }

    public float getUnitStrength()
    {
        return (unitAttack * attackBuff) + (unitHealth * defenseBuff);
    }

    public void generatePossibleMoves(TileMemento startingTile, List<BaseUnitMemento> rivalUnits, bool sideWeaker)
    {
        this.validMovementTiles = findValidMovement(startingTile, 0);

        foreach (TileMemento tile in validMovementTiles)
        {
            MoveMemento move = new MoveMemento(this, tile, null);

            if (sideWeaker)
            {
                move.Score = evaluateMoveDefense(move, rivalUnits);
            }
            else
            {
                move.Score = evaluateMoveAttack(move, rivalUnits);
            }

            possibleMoves.Add(move);

            foreach (BaseUnitMemento rival in rivalUnits)
            {
                if (Vector2.Distance(rival.position, tile.position) > unitRange)
                {
                    continue;
                }

                move = new MoveMemento(this, tile, rival);
                move.damage = attackValue(move.Target, move.Tile);

                if (sideWeaker)
                {
                    move.Score = evaluateMoveDefense(move, rivalUnits);
                }
                else
                {
                    move.Score = evaluateMoveAttack(move, rivalUnits);
                }

                possibleMoves.Add(move);
            }
        }
    }

    int evaluateMoveAttack(MoveMemento move, List<BaseUnitMemento> rivalUnits)
    {
        float score = 0;

        if (move.Target != null)
        {
            float distance = Vector2.Distance(move.Unit.position, move.Target.position);
            score += 100 - distance;
            score += move.damage * 5;

            if (move.Target.unitHealth - move.damage <= 0)
            {
                score += 700;
                move.targetKilled = true;
            }
        }
        else
        {
            float closestTargetDistance = rivalUnits
                .Where(r => PathfindingMemento.GetDistanceBetween(r.OccupiedTile, move.Tile) <= unitManagerMemento.aggroDistance)
                .Select(r => PathfindingMemento.GetDistanceBetween(r.OccupiedTile, move.Tile))
                .DefaultIfEmpty(100)
                .Min();

            score += 100 - closestTargetDistance;
        }

        score += (move.Tile.attackBonus + move.Tile.defenseBonus) * 50;
        return (int)score;
    }

    int evaluateMoveDefense(MoveMemento move, List<BaseUnitMemento> rivalUnits)
    {
        float score = 0;
        int threatCount = 0;

        foreach (var rival in rivalUnits)
        {
            float distanceToHero = PathfindingMemento.GetDistanceBetween(move.Tile, rival.OccupiedTile);
            score += distanceToHero;
            if (distanceToHero <= rival.unitRange + rival.unitMove)
            {
                threatCount++;
            }
        }

        score -= threatCount * 25;

        foreach (var enemy in unitManagerMemento.enemies)
        {
            float distanceToAlly = PathfindingMemento.GetDistanceBetween(move.Tile, enemy.OccupiedTile);
            score += (1 / (distanceToAlly + 1f)) * 50f;
        }

        score += 100 * move.Tile.defenseBonus;

        if (move.Target != null)
        {
            float distance = Vector2.Distance(move.Unit.position, move.Target.position);
            score += 100 - distance;
            score += move.damage * 5;

            if (move.Target.unitHealth - move.damage <= 0)
            {
                score += 500;
            }
        }

        return (int)score;
    }

    public bool classAdvantage(BaseUnit target)
    {
        return (this.Class == Class.Strength && target.Class == Class.Agility) ||
           (this.Class == Class.Agility && target.Class == Class.Magic) ||
           (this.Class == Class.Magic && target.Class == Class.Strength);
    }

    public void AIv2(List<BaseUnitMemento> rivalUnits, bool sideWeaker)
    {
        validMovementTiles.Clear();
        possibleMoves.Clear();
        possibleTargets.Clear();

        if(!engaged)
        {
            foreach (BaseUnitMemento rival in rivalUnits)
            {
                if (Vector2.Distance(rival.position, position) <= unitManagerMemento.aggroDistance)
                {
                    possibleTargets.Add(rival);
                }
            }

            if(possibleTargets.Count > 0) 
            {
                engaged = true;
            }
        }
        else
        {
            foreach (BaseUnitMemento rival in rivalUnits)
            {
                possibleTargets.Add(rival);
            }
        }

        generatePossibleMoves(OccupiedTile, rivalUnits, sideWeaker);

        int maxScore = possibleMoves.ElementAt(0).Score;

        foreach (MoveMemento move in possibleMoves)
        {
            if (move.Score > maxScore)
            {
                maxScore = move.Score;
            }
        }

        if (maxScore > 100)
        {
            possibleMoves.RemoveAll(p => p.Score <= 100);
        }
        else
        {
            float targetScore = possibleMoves.ElementAt(0).Score - 2;
            possibleMoves.RemoveAll(p => p.Score < targetScore);
        }
    }
}
