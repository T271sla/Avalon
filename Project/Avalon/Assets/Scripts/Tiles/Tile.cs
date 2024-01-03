using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public GameObject highlight, attackHighlight, target;
    [SerializeField] private bool isWalkable;
    public string tileName;
    public float attackBonus, defenseBonus;

    public BaseUnit OccupiedUnit;
    public bool Walkable => isWalkable && OccupiedUnit == null;
    public float movementCost = 1;
    public AudioClip walkingSound;

    public virtual void Init(int x, int y)
    {
         
    }

    private void OnMouseEnter()
    {
        if(GameManager.Instance.GameState == GameState.Pause) 
        {
            return;
        }
        if(!MenuManager.instance.battleUIActive) 
        {
            target.SetActive(true);
        }
        MenuManager.instance.showTileInfo(this);
    }

    private void OnMouseExit()
    {
        target.SetActive(false);
        MenuManager.instance.showTileInfo(null);
        if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Enemy)
        {
            setHighlightsActivity(OccupiedUnit, false, OccupiedUnit.unitMove + OccupiedUnit.unitRange);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if(OccupiedUnit != null) 
            {
                MenuManager.instance.showSelectedUnit(OccupiedUnit);
            }
            else
            {
                MenuManager.instance.showFullTileInfo(this);
            }
        }

        if(Input.GetKey(KeyCode.R)) 
        {
            if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Enemy)
            {
                setHighlightsActivity(OccupiedUnit, true, OccupiedUnit.unitMove + OccupiedUnit.unitRange);
            }
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;

        if(OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Hero && UnitManager.Instance.selectedHero != OccupiedUnit && OccupiedUnit.active)
            {
                if(UnitManager.Instance.selectedHero != null)
                {
                    turnOffValidMovement();
                }
                UnitManager.Instance.SetSeclectedHero((BaseHero)OccupiedUnit);
                UnitManager.Instance.selectedHero.findValidMovement();
            }
            else
            {
                if (UnitManager.Instance.selectedHero != null && OccupiedUnit.Faction != UnitManager.Instance.selectedHero.Faction && MenuManager.instance.attackPressed)
                {
                    var enemy = (BaseEnemy)OccupiedUnit;
                    float distanceAttackX = Mathf.Abs(UnitManager.Instance.selectedHero.transform.position.x - enemy.transform.position.x);
                    float distanceAttackY = Mathf.Abs(UnitManager.Instance.selectedHero.transform.position.y - enemy.transform.position.y);

                    if(distanceAttackX + distanceAttackY <= UnitManager.Instance.selectedHero.unitRange)
                    {
                        UnitManager.Instance.selectedHero.Attack(enemy);
                        MenuManager.instance.attackPressed = false;
                        MenuManager.instance.subMenuActivity(false, true);
                        setHighlightsActivity(UnitManager.Instance.selectedHero, false, UnitManager.Instance.selectedHero.unitRange);
                        UnitManager.Instance.selectedHero.active = false;
                        UnitManager.Instance.SetSeclectedHero(null);
                        if(!UnitManager.Instance.enemiesRemain())
                        {
                            return;
                        }
                        UnitManager.Instance.checkUnitActivity();
                    }
                }

                if(UnitManager.Instance.selectedHero != null && UnitManager.Instance.selectedHero == OccupiedUnit && !MenuManager.instance.subMenuActive)
                {
                    prepareUI();
                }
            }
        }
        else
        {
            if (UnitManager.Instance.selectedHero != null && UnitManager.Instance.selectedHero.OccupiedTile != null && UnitManager.Instance.selectedHero.validMovementTiles.Contains(this))
            {
                setUnit(UnitManager.Instance.selectedHero);
                prepareUI();
                AudioManager.Instance.playSound(walkingSound);
            }
        }
    }

    private void prepareUI()
    {
        turnOffValidMovement();
        MenuManager.instance.buttonsActivity(true);
    }

    private void turnOffValidMovement()
    {
        UnitManager.Instance.selectedHero.OccupiedTile.highlight.SetActive(false);
        for (int i = 0; i < UnitManager.Instance.selectedHero.validMovementTiles.Count; i++)
        {
            UnitManager.Instance.selectedHero.validMovementTiles.ElementAt(i).highlight.SetActive(false);
        }
        UnitManager.Instance.selectedHero.validMovementTiles.Clear();
    }

    public void setHighlightsActivity(BaseUnit unit, bool highlightActivity, int range)
    {
        float unitX = unit.transform.position.x;
        float unitY = unit.transform.position.y;

        for (float x = unitX - range; x <= unitX + range; x++)
        {
            for (float y = unitY - range; y <= unitY + range; y++)
            {
                bool positionInRange = x >= 0 && x < GridManager.Instance.width && y >= 0 && y < GridManager.Instance.height;
                if (!positionInRange)
                {
                    continue;
                }

                Tile currentTile = GridManager.Instance.GetTileAtPoint(new Vector2(x, y));

                float distanceMoveX2 = Mathf.Abs(x - unitX);
                float distanceMoveY2 = Mathf.Abs(y - unitY);
                if (distanceMoveX2 + distanceMoveY2 <= range && distanceMoveX2 + distanceMoveY2 != 0)
                {
                    if (currentTile.OccupiedUnit != null && currentTile.OccupiedUnit.Faction != unit.Faction)
                    {
                        currentTile.attackHighlight.SetActive(highlightActivity);
                    }
                    else
                    {
                        currentTile.highlight.SetActive(highlightActivity);
                    }
                }
            }
        }
    }

    public void setUnit(BaseUnit unit)
    {
        if(unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
        unit.calculateBuffs();
    }

    public void destroyTile()
    {
        Destroy(gameObject);
    }

    public void SetColor(Color color) => spriteRenderer.color = color;
}
