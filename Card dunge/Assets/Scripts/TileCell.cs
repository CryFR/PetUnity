using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public Tile Tile {get; set;}

    public bool Epty => Tile == null;
    public bool Occupied => Tile != null;
    
}
