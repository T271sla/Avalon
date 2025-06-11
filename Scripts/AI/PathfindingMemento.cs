using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathfindingMemento
{
    public GridManagerMemento gridManagerMemento;

    public PathfindingMemento(GridManagerMemento gridManagerMemento)
    {
        this.gridManagerMemento = gridManagerMemento;
    }

    public List<TileMemento> FindPath(TileMemento startTile, TileMemento targetTile)
    {
        List<TileMemento> openSet = new List<TileMemento>();
        HashSet<TileMemento> closedSet = new HashSet<TileMemento>();
        Dictionary<TileMemento, TileMemento> cameFrom = new Dictionary<TileMemento, TileMemento>();
        Dictionary<TileMemento, float> gScore = new Dictionary<TileMemento, float>();
        Dictionary<TileMemento, float> fScore = new Dictionary<TileMemento, float>();

        openSet.Add(startTile);
        gScore[startTile] = 0;
        fScore[startTile] = HeuristicCostEstimate(startTile, targetTile);

        while (openSet.Count > 0)
        {
            TileMemento currentTile = GetTileWithLowestFScore(openSet, fScore);

            if (currentTile == targetTile)
            {
                return ReconstructPath(cameFrom, currentTile);
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (TileMemento neighbor in GetNeighbors(currentTile))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeGScore = gScore[currentTile] + neighbor.movementCost;

                if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentTile;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, targetTile);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private static float HeuristicCostEstimate(TileMemento startTile, TileMemento targetTile)
    {
        return Vector2.Distance(startTile.position, targetTile.position);
    }

    private static TileMemento GetTileWithLowestFScore(List<TileMemento> openSet, Dictionary<TileMemento, float> fScore)
    {
        float lowestFScore = float.MaxValue;
        TileMemento result = null;

        foreach (TileMemento tile in openSet)
        {
            if (fScore.TryGetValue(tile, out float score) && score < lowestFScore)
            {
                lowestFScore = score;
                result = tile;
            }
        }

        return result;
    }

    private static List<TileMemento> ReconstructPath(Dictionary<TileMemento, TileMemento> cameFrom, TileMemento currentTile)
    {
        List<TileMemento> path = new List<TileMemento> { currentTile };

        while (cameFrom.TryGetValue(currentTile, out TileMemento cameFromTile))
        {
            for (int i = 1; i < currentTile.movementCost; i++)
            {
                path.Insert(0, currentTile);
            }
            currentTile = cameFromTile;
            path.Insert(0, currentTile);
        }

        return path;
    }

    private List<TileMemento> GetNeighbors(TileMemento tile)
    {
        List<TileMemento> neighbors = new List<TileMemento>();

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 direction in directions)
        {
            Vector2 neighborPosition = tile.position + direction;
            TileMemento neighbor = this.gridManagerMemento.GetTileAtPoint(neighborPosition);

            if (neighbor != null && neighbor.Walkable)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public static float GetDistanceBetween(TileMemento tileA, TileMemento tileB)
    {
        return Math.Abs(tileA.position.x - tileB.position.x) + Math.Abs(tileA.position.y - tileB.position.y);
    }
}
