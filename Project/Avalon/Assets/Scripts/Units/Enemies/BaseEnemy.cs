using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemy : BaseUnit
{
    /*public IEnumerator AI()
    {
        findValidMovement();
        float distance = 999;
        List<BaseHero> validTargets = new List<BaseHero>();
        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            float distanceFromHero = Mathf.Abs(hero.transform.position.x - transform.position.x) + Mathf.Abs(hero.transform.position.y - transform.position.y);
            if (distanceFromHero < distance)
            {
                distance = distanceFromHero;
            }
        }

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            float distanceFromHero = Mathf.Abs(hero.transform.position.x - transform.position.x) + Mathf.Abs(hero.transform.position.y - transform.position.y);
            if (distanceFromHero <= distance && distanceFromHero <= UnitManager.Instance.aggroDistance)
            {
                validTargets.Add(hero);
            }
        }

        if(validTargets.Count == 0) 
        {
            yield break;
        }

        UnitManager.Instance.SetSeclectedHero(validTargets[0]);

        foreach (BaseHero hero in validTargets)
        {
            if(hero.unitHealth < UnitManager.Instance.selectedHero.unitHealth)
            {
                UnitManager.Instance.SetSeclectedHero(hero);
            }
        }

        Vector2 destinationPosition = UnitManager.Instance.selectedHero.transform.position;

        foreach(BaseHero hero in UnitManager.Instance.heroes) 
        {
            hero.OccupiedTile.OccupiedUnit = null;
        }

        List<Tile> path = Pathfinding.FindPath(OccupiedTile, GridManager.Instance.GetTileAtPoint(destinationPosition));

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            hero.OccupiedTile.OccupiedUnit = hero;
        }

        Tile destinationTile = OccupiedTile;

        foreach (Tile tile in path)
        {
            if(validMovementTiles.Contains(tile))
            {
                destinationTile = tile;
            }
        }

        destinationTile.setUnit(this);
        AudioManager.Instance.playSound(OccupiedTile.walkingSound);

        float distanceAttackX = Mathf.Abs(UnitManager.Instance.selectedHero.transform.position.x - destinationTile.transform.position.x);
        float distanceAttackY = Mathf.Abs(UnitManager.Instance.selectedHero.transform.position.y - destinationTile.transform.position.y);

        if (distanceAttackX + distanceAttackY == unitRange)
        {
            yield return new WaitForSeconds(0.5f);
            Attack(UnitManager.Instance.selectedHero);
        }

        UnitManager.Instance.SetSeclectedHero(null);
        UnitManager.Instance.SetSelectedEnemy(null);
    }*/

    public IEnumerator newAI()
    {
        findValidMovement();

        List<List<Tile>> validTargets = new List<List<Tile>>();

        foreach (BaseHero hero in UnitManager.Instance.heroes)
        {
            if (Vector2.Distance(hero.transform.position, transform.position) > UnitManager.Instance.aggroDistance)
            {
                continue;
            }

            UnitManager.Instance.SetSeclectedHero(hero);
            UnitManager.Instance.selectedHero.OccupiedTile.OccupiedUnit = null;

            List<Tile> path = Pathfinding.FindPath(OccupiedTile, hero.OccupiedTile);

            UnitManager.Instance.selectedHero.OccupiedTile.OccupiedUnit = UnitManager.Instance.selectedHero;
            UnitManager.Instance.SetSeclectedHero(null);

            if (path != null)  // Check if path is not null before adding
            {
                validTargets.Add(path);
            }
        }

        if (validTargets.Count == 0)  // Check if the collection is empty
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

        Debug.Log(destination);

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

}
