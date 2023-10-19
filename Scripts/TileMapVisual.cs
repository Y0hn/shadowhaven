using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapVisual : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTileMap;
    [SerializeField]
    private TileBase floorTile;

    public void PaintFloorTitles(IEnumerable<Vector2Int> floorPos)
    {
        PaintTiles(floorPos, floorTileMap, floorTile);
    }
    private void PaintTiles(IEnumerable<Vector2Int> pos, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in pos) 
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePos = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePos, tile);
    }
}
