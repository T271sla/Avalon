using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.GraphicsBuffer;

public class Move
{
    public BaseUnit Unit;
    public Tile Tile;
    public BaseUnit Target;
    public float damage = 0;
    public bool targetKilled = false;
    public int Score = 0;

    public Move(BaseUnit unit, Tile tile, BaseUnit target, int score = 0)
    {
        Unit = unit;
        Tile = tile;
        Target = target;
        targetKilled = false;
        Score = score;
        if(Target != null)
        {
            damage = Unit.attackValue(Target, Tile); 
        }
    }

    public Move(MoveMemento moveMemento)
    {
        foreach(BaseEnemy enemy in UnitManager.Instance.enemies)
        {
            if(enemy.unitId == moveMemento.Unit.unitId)
            {
                this.Unit = enemy;
            }
        }

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            if (hero.unitId == moveMemento.Target.unitId)
            {
                this.Target = hero;
            }
        }

        Tile = GridManager.Instance.GetTileAtPoint(moveMemento.Tile.position);
        this.targetKilled = moveMemento.targetKilled;
        this.Score = moveMemento.Score;

        if (Target != null)
        {
            damage = Unit.attackValue(Target, Tile);
        }
    }
}

public class MoveMemento
{
    public BaseUnitMemento Unit;
    public TileMemento Tile;
    public BaseUnitMemento Target;
    public float damage = 0;
    public bool targetKilled = false;
    public int Score = 0;

    public MoveMemento(BaseUnitMemento unit, TileMemento tile, BaseUnitMemento target, int score = 0)
    {
        Unit = unit;
        Tile = tile;
        Target = target;
        Score = score;
    }
}