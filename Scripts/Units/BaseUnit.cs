using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.ParticleSystem;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
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
    public List<Tile> validMovementTiles = new List<Tile>();
    public List<Move> possibleMoves = new List<Move>();
    public bool active = true;
    [SerializeField] protected Skill skillObject;
    public Skill skill;
    public List<Status> unitStatuses = new List<Status>();
    public Healthbar bar;
    [SerializeField] protected AudioClip attackSound;
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if(skillObject != null ) 
        {
            skill = Instantiate(skillObject);
        }
    }

    public virtual void Attack(BaseUnit enemy)
    {
        float attack = attackValue(enemy, OccupiedTile);
        enemy.modifyHealth(attack);
        enemy.animator.SetTrigger("Damage");
        AudioManager.Instance.playSound(attackSound);
    }

    public float attackValue(BaseUnit enemy, Tile unitTile)
    {
        float randomValue = UnityEngine.Random.Range(-9, 9);
        float attackValue = unitAttack * (attackBuff + unitTile.attackBonus) + randomValue;
        if((Class == Class.Strength && enemy.Class == Class.Agility) || (Class == Class.Agility && enemy.Class == Class.Magic) || (Class == Class.Magic && enemy.Class == Class.Strength)) 
        {
            attackValue *= 1.2f;
        }
        float defenseValue = enemy.defenseBuff + enemy.OccupiedTile.defenseBonus;
        return attackValue / defenseValue;
    }

    public virtual void modifyHealth(float healthToChange)
    {
        unitHealth -= healthToChange;
        if(unitHealth <= 0)
        {
            if(Faction.Equals(Faction.Enemy))
            {
                UnitManager.Instance.enemies.Remove((BaseEnemy)this);
            }
            else
            {
                UnitManager.Instance.heroes.Remove((BaseHero)this);
            }
            Destroy(gameObject);
        }

        if(unitHealth > unitMaxHealth) 
        {
            unitHealth = unitMaxHealth;
        }

        bar.setValue(unitHealth);
    }

    public List<Tile> findValidMovement(Tile tile, int distanceBonus)
    {
        List<Tile> validTiles = new List<Tile>();

        for (float x = tile.transform.position.x - unitMove - distanceBonus; x <= tile.transform.position.x + unitMove + distanceBonus; x++)
        {
            for (float y = tile.transform.position.y - unitMove - distanceBonus; y <= tile.transform.position.y + unitMove + distanceBonus; y++)
            {
                if (!(x >= 0 && x < GridManager.Instance.width && y >= 0 && y < GridManager.Instance.height))
                {
                    continue;
                }

                var currentTile = GridManager.Instance.GetTileAtPoint(new Vector2(x, y));
                if (!currentTile.Walkable)
                {
                    continue;
                }

                List<Tile> path = Pathfinding.FindPath(tile, currentTile);

                if (path == null)
                {
                    path = new List<Tile>();
                    path.Add(tile);
                }

                if (path.Count - 1 <= unitMove)
                {
                    if (this.Faction == Faction.Hero)
                    {
                        path.ElementAt(path.Count - 1).highlight.SetActive(true);
                    }
                    validTiles.Add(path.ElementAt(path.Count - 1));
                }

                if (this.Faction == Faction.Hero && GameManager.Instance.GameState == GameState.HeroesTurn)
                {
                    setUnitMovementHighlight(true);
                }
            }
        }

        return validTiles;
    }

    public void setUnitMovementHighlight(bool highlightActivity)
    {
        foreach (Tile tile in UnitManager.Instance.selectedHero.validMovementTiles)
        {
            tile.highlight.SetActive(highlightActivity);
        }
    }

    public float getUnitStrength()
    {
        return (unitAttack * attackBuff) + (unitHealth * defenseBuff);
    }

    public void generatePossibleMoves(Tile startingTile)
    {
        this.validMovementTiles = findValidMovement(startingTile, 0);

        foreach (Tile tile in validMovementTiles)
        {
            Move move = new Move(this, tile, null, 0);

            if (UnitManager.Instance.enemiesWeaker)
            {
                move.Score = evaluateMoveDefense(move);
            }
            else
            {
                move.Score = evaluateMoveAttack(move);
            }

            possibleMoves.Add(move);

            foreach (BaseHero hero in UnitManager.Instance.heroes)
            {
                if (Vector2.Distance(hero.transform.position, tile.transform.position) > unitRange)
                {
                    continue;
                }

                move = new Move(this, tile, hero, 0);

                if (UnitManager.Instance.enemiesWeaker)
                {
                    move.Score = evaluateMoveDefense(move);
                }
                else
                {
                    move.Score = evaluateMoveAttack(move);
                }

                possibleMoves.Add(move);
            }
        }
    }

    int evaluateMoveAttack(Move move)
    {
        float score = 0;

        if(move.Target != null) 
        {
            float distance = Vector2.Distance(move.Unit.transform.position, move.Target.transform.position);
            score += 100 - distance;

            float damage = move.Unit.attackValue(move.Target, OccupiedTile);
            score += damage * 5;

            if (move.Target.unitHealth - damage <= 0)
            {
                score += 500;
                move.targetKilled = true;
            }
        }
        else
        {
            float closestTargetDistance = Pathfinding.GetDistanceBetween(UnitManager.Instance.heroes.ElementAt(0).OccupiedTile, move.Tile);
            float targetDistance = closestTargetDistance;

            foreach (BaseHero hero in UnitManager.Instance.heroes)
            {
                if (Pathfinding.GetDistanceBetween(hero.OccupiedTile, move.Tile) > UnitManager.Instance.aggroDistance)
                {
                    continue;
                }

                targetDistance = Pathfinding.GetDistanceBetween(hero.OccupiedTile, move.Tile);

                if (targetDistance < closestTargetDistance) 
                {
                    closestTargetDistance = targetDistance;
                }
            }

            score += 100 - closestTargetDistance;
        }

        score += (move.Tile.attackBonus + move.Tile.defenseBonus) * 100;

        return (int)score;
    }

    int evaluateMoveDefense(Move move)
    {
        float score = 0;

        int threatCount = 0;

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            float distanceToHero = Pathfinding.GetDistanceBetween(move.Tile, hero.OccupiedTile);
            score += distanceToHero;
            if (distanceToHero <= hero.unitRange + hero.unitMove)
            {
                threatCount++;
            }
        }

        score -= threatCount * 200;

        foreach (BaseEnemy enemy in UnitManager.Instance.enemies)
        {
            float distanceToAlly = Pathfinding.GetDistanceBetween(move.Tile, enemy.OccupiedTile);
            score += (1 / (distanceToAlly + 1)) * 50;
        }

        score += 100 * move.Tile.defenseBonus;

        if (move.Target != null)
        {
            float distance = Vector2.Distance(move.Unit.transform.position, move.Target.transform.position);
            score += 100 - distance;

            float damage = move.Unit.attackValue(move.Target, OccupiedTile);
            score += damage;

            if (move.Target.unitHealth - damage <= 0)
            {
                score += 100;
            }
        }

        return (int)score;
    }

    public void addStatus(Status status)
    {
        bool statusExists = unitStatuses.Any(s => s.statusName == status.statusName);

        if (statusExists && !status.stackable)
        {
            return;
        }

        this.unitStatuses.Add(Instantiate(status));
        this.unitStatuses.ElementAt(this.unitStatuses.Count() - 1).Activate(this);
    }
}
