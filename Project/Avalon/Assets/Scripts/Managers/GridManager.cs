using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public float width, height;
    [SerializeField] private Tile grassTile, mountainTile;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    private Dictionary<Vector2, Tile> tilesForAiPath;
    public List<Tile> enemyTiles = new List<Tile>();
    public GameObject grid;
    public GameObject healthBar;

    private void Awake()
    {
        Instance = this;
    }

    public void generateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        tilesForAiPath = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(grassTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                tiles[new Vector2(x, y)] = spawnedTile;
                tilesForAiPath[new Vector2(x, y)] = spawnedTile;
            }
        }

        foreach (Transform child in grid.transform.GetChild(0))
        {
            var spawnedTile = Instantiate(child, child.position, Quaternion.identity);
            Tile tile = spawnedTile.GetComponent<Tile>();
            child.GetComponent<Tile>().destroyTile();
            Vector2 pos = new Vector2(0, 0);
            pos.x = Mathf.Round(tile.transform.position.x);
            pos.y = Mathf.Round(tile.transform.position.y);
            tile.transform.position = pos;
            tiles[tile.transform.position].destroyTile();
            tiles[tile.transform.position] = tile;
            tilesForAiPath[tile.transform.position].destroyTile();
            tilesForAiPath[tile.transform.position] = tile;
        }

        Debug.Log(tiles.Count());

        GameManager.Instance.ChangeState(GameState.SpawnHeroes);
    }

    public Tile GetTileAtPoint(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    public Tile GetAiPathTileAtPoint(Vector2 pos)
    {
        if (tilesForAiPath.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }
}
