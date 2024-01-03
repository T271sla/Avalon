using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public Class Class;
    public string unitName;
    public int unitMove;
    public int unitRange;
    public float unitMaxHealth;
    public float unitHealth;
    public float unitAttack;
    public float attackBuff = 1;
    public float defenseBuff = 1;
    public List<Tile> validMovementTiles = new List<Tile>();
    public bool active = true;
    [SerializeField] protected Skill skillObject;
    public Skill skill;
    public Healthbar bar;
    [SerializeField] protected AudioClip attackSound;
    public Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        if(skillObject != null ) 
        {
            skill = Instantiate(skillObject);
        }
    }

    public virtual void Attack(BaseUnit enemy)
    {
        float attack = attackValue(enemy);
        enemy.modifyHealth(attack);
        enemy.bar.setValue(enemy.bar.getValue() - attack);
        enemy.animator.SetTrigger("Damage");
        AudioManager.Instance.playSound(attackSound);
    }

    public float attackValue(BaseUnit enemy)
    {
        float attackValue = unitAttack * attackBuff + Random.Range(-9, 9);
        if((Class == Class.Strength && enemy.Class == Class.Agility) || (Class == Class.Agility && enemy.Class == Class.Magic) || (Class == Class.Magic && enemy.Class == Class.Strength)) 
        {
            attackValue *= 1.2f;
        }
        Debug.Log(attackValue);
        float defenseValue = 100 * enemy.defenseBuff;
        return attackValue/defenseBuff;
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
    }

    public void findValidMovement()
    {
        for (float x = OccupiedTile.transform.position.x - unitMove; x <= OccupiedTile.transform.position.x + unitMove; x++)
        {
            for (float y = OccupiedTile.transform.position.y - unitMove; y <= OccupiedTile.transform.position.y + unitMove; y++)
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

                List<Tile> path = Pathfinding.FindPath(OccupiedTile, currentTile);

                if (path == null)
                {
                    path = new List<Tile>();  // Initialize path to an empty list
                    path.Add(OccupiedTile);
                }

                if (path.Count - 1 <= unitMove)
                {
                    if (this.Faction == Faction.Hero)
                    {
                        path.ElementAt(path.Count - 1).highlight.SetActive(true);
                    }
                    validMovementTiles.Add(path.ElementAt(path.Count - 1));
                }

                if (this.Faction == Faction.Hero)
                {
                    setUnitMovementHighlight(true);
                }
            }
        }
    }

    public void setUnitMovementHighlight(bool highlightActivity)
    {
        foreach (Tile tile in UnitManager.Instance.selectedHero.validMovementTiles)
        {
            tile.highlight.SetActive(highlightActivity);
        }
    }

    public void calculateBuffs()
    {
        attackBuff += OccupiedTile.attackBonus;
        defenseBuff += OccupiedTile.defenseBonus;
    }

    public void deactivateUnit()
    {

    }
}
