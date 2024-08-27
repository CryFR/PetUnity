using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;
    public TileState[] tileStates;

    private TileGrid grid;
    private List<Tile> tiles;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private bool waiting;

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

    private void Update()
    {
        if (waiting) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Store the position where the touch began
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    // Store the position where the touch ended
                    endTouchPosition = touch.position;

                    // Calculate the difference between start and end positions
                    Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                    // Check if the swipe is significant enough to consider
                    if (swipeDelta.magnitude > 100) // You can adjust the swipe sensitivity here
                    {
                        // Determine the swipe direction
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            // Horizontal swipe
                            if (swipeDelta.x > 0)
                            {
                                // Swipe right
                                MoveTiles(Vector2Int.right, grid.Width - 2, -1, 0, 1);
                            }
                            else
                            {
                                // Swipe left
                                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                            }
                        }
                        else
                        {
                            // Vertical swipe
                            if (swipeDelta.y > 0)
                            {
                                // Swipe up
                                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                            }
                            else
                            {
                                // Swipe down
                                MoveTiles(Vector2Int.down, 0, 1, grid.Height - 2, -1);
                            }
                        }
                    }
                    break;
            }

        }

    }


    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied)
                {
                   changed |= MoveTile(cell.Tile, direction);
                }
            }

        }
        if (changed){
            StartCoroutine(WaitForChanges());
        }
    }


    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = IndexOf(b.state);
        int number = a.number;
        if (index == 1)
        {
            number = a.number - b.number;
        }

        b.SetState(tileStates[0], number);
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                Merge(tile, adjacent.Tile);
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        if (tiles.Count != grid.Size)
        {
            CreateTile();
        }
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }
}
