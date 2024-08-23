using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;
    public TileState[] tileStates;

    private TileGrid grid;
    private List<Tile> tiles;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(9);
    }

    private void Start()
    {
        CreateTile();
        CreateTile();
    }

    private void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        if (!tiles.Any())
        {
            tile.SetState(tileStates[0], 20);
        }
        else
        {
            tile.SetState(tileStates[1], 10);
        }
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);

    }
}
