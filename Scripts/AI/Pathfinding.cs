using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{

    public static List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();

        openSet.Add(startTile);
        gScore[startTile] = 0;
        fScore[startTile] = HeuristicCostEstimate(startTile, targetTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = GetTileWithLowestFScore(openSet, fScore);

            if (currentTile == targetTile)
            {
                return ReconstructPath(cameFrom, currentTile);
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            foreach (Tile neighbor in GetNeighbors(currentTile))
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

    private static float HeuristicCostEstimate(Tile startTile, Tile targetTile)
    {
        return Vector2.Distance(startTile.transform.position, targetTile.transform.position);
    }

    private static Tile GetTileWithLowestFScore(List<Tile> openSet, Dictionary<Tile, float> fScore)
    {
        float lowestFScore = float.MaxValue;
        Tile result = null;

        foreach (Tile tile in openSet)
        {
            if (fScore.TryGetValue(tile, out float score) && score < lowestFScore)
            {
                lowestFScore = score;
                result = tile;
            }
        }

        return result;
    }

    private static List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile currentTile)
    {
        List<Tile> path = new List<Tile> { currentTile };

        while (cameFrom.TryGetValue(currentTile, out Tile cameFromTile))
        {
            for(int i = 1; i < currentTile.movementCost; i++)
            {
                path.Insert(0, currentTile);
            }
            currentTile = cameFromTile;
            path.Insert(0, currentTile);
        }

        return path;
    }

    private static List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 direction in directions)
        {
            Vector2 neighborPosition = tile.transform.position + (Vector3)direction;
            Tile neighbor = GridManager.Instance.GetTileAtPoint(neighborPosition);

            if (neighbor != null && neighbor.Walkable)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public static float GetDistanceBetween(Tile tileA, Tile tileB)
    {
        return Math.Abs(tileA.transform.position.x - tileB.transform.position.x) +  Math.Abs(tileA.transform.position.y - tileB.transform.position.y);
    }
}

