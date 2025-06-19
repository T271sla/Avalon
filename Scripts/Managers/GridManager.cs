using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public float width, height;
    [SerializeField] private Tile baseGridTile;

    private Dictionary<Vector2, Tile> tiles;

    public GameObject grid;

    private void Awake()
    {
        Instance = this;
    }

    public void generateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(baseGridTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                Vector2 pos = new Vector2(x, y);
                tiles[pos] = spawnedTile;
            }
        }

        var childLayer = grid.transform.GetChild(0);

        foreach (Transform child in childLayer)
        {
            var spawnedTile = Instantiate(child, child.position, Quaternion.identity);
            Tile newTile = spawnedTile.GetComponent<Tile>();

            Vector2 pos = new Vector2(Mathf.Round(newTile.transform.position.x), Mathf.Round(newTile.transform.position.y));
            newTile.transform.position = pos;

            if (tiles.ContainsKey(pos) && tiles[pos] != null)
            {
                Destroy(tiles[pos].gameObject);
            }

            tiles[pos] = newTile;
            Destroy(child.gameObject);
        }

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

    public Dictionary<Vector2, Tile> getTiles()
    {
        return tiles;
    }
}
