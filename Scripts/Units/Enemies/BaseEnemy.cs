using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public List<BaseHero> possibleTargets = new List<BaseHero>();

    public IEnumerator AIv1()
    {
        findValidMovement(OccupiedTile, 0);

        List<List<Tile>> validTargets = new List<List<Tile>>();

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            if (Vector2.Distance(hero.transform.position, transform.position) > UnitManager.Instance.aggroDistance)
            {
                continue;
            }

            UnitManager.Instance.SetSelectedHero(hero);
            UnitManager.Instance.selectedHero.OccupiedTile.OccupiedUnit = null;

            List<Tile> path = Pathfinding.FindPath(OccupiedTile, hero.OccupiedTile);

            UnitManager.Instance.selectedHero.OccupiedTile.OccupiedUnit = UnitManager.Instance.selectedHero;
            UnitManager.Instance.SetSelectedHero(null);

            if (path != null)
            {
                validTargets.Add(path);
            }
        }

        if (validTargets.Count == 0)
        {
            yield break;
        }

        int targetIndex = 0;
        int distance = validTargets[0].Count;

        for (int i = 0; i < validTargets.Count; i++)
        {
            if (validTargets[i].Count < distance)
            {
                targetIndex = i;
                distance = validTargets[i].Count;
            }
        }

        Tile destination = OccupiedTile;

        foreach (Tile tile in validTargets[targetIndex])
        {
            if (validMovementTiles.Contains(tile))
            {
                destination = tile;
            }
        }

        destination.setUnit(this);
        AudioManager.Instance.playSound(OccupiedTile.walkingSound);

        BaseHero target = null;
        int targetHealth = 999;

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            if (Vector2.Distance(hero.transform.position, transform.position) <= unitRange && hero.unitHealth < targetHealth)
            {
                target = hero;
            }
        }

        if (target != null)
        {
            yield return new WaitForSeconds(0.5f);
            Attack(target);
            if (!UnitManager.Instance.heroesRemain())
            {
                yield break;
            }
        }
    }

    public IEnumerator AIv2()
    {
        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            if (Vector2.Distance(hero.transform.position, transform.position) <= UnitManager.Instance.aggroDistance)
            {
                possibleTargets.Add(hero);
            }
        }

        if(possibleTargets.Count == 0) 
        {
            yield break;
        }

        generatePossibleMoves(OccupiedTile);

        Move maxScoreMove = possibleMoves.OrderByDescending(p => p.Score).FirstOrDefault();

        /*
        maxScoreMove.Tile.setUnit(this);
        AudioManager.Instance.playSound(OccupiedTile.walkingSound);

        if (maxScoreMove.Target != null)
        {
            yield return new WaitForSeconds(0.5f);
            Attack(maxScoreMove.Target);
            if (!UnitManager.Instance.heroesRemain())
            {
                yield break;
            }
        }*/
    }
}