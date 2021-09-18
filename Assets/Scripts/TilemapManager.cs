using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileLayer
{
    TERRAIN,
    TILEFX,
    RAILS,
    BIOME
}

public class TilemapManager : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] Tilemap terrain;
    [SerializeField] Tilemap cracks;
    [SerializeField] Tilemap rails;
    [SerializeField] Tilemap biome;

    private static Dictionary<TileLayer, Tilemap> layers;
    private static Grid staticGrid;

    private void Awake()
    {
        layers = new Dictionary<TileLayer, Tilemap>();
        layers.Add(TileLayer.TERRAIN, terrain);
        layers.Add(TileLayer.TILEFX, cracks);
        layers.Add(TileLayer.RAILS, rails);
        layers.Add(TileLayer.BIOME, biome);
        staticGrid = grid;
    }

    public static Tile GetTile(TileLayer layer, Vector3 position)
    {
        Tilemap map = layers[layer];
        
        Tile returnVal = map.GetTile<Tile>(map.WorldToCell(position));

        return returnVal;
    }

    public static void SetTile(TileLayer layer, TileBase tile, Vector3 position)
    {
        SetTile(layer, tile, layers[layer].WorldToCell(position));
    }

    public static void SetTile(TileLayer layer, TileBase tile, Vector3Int position)
    {
        Tilemap map = layers[layer];

        TileBase toReplace = map.GetTile(position);

        switch(layer)
        {
            case TileLayer.TERRAIN:
            {
                if (toReplace && tile is null)
                {
                    EventBus.BlockEvents.OnBlockBreak?.Invoke(null, layer, tile, toReplace, position);
                }
                else if (toReplace && tile)
                {
                    EventBus.BlockEvents.OnBlockReplace?.Invoke(null, layer, tile, toReplace, position);
                    }
                else if (toReplace is null && tile)
                {
                    EventBus.BlockEvents.OnBlockPlace?.Invoke(null, layer, tile, toReplace, position);
                }
                else throw new System.Exception("Unsupported block opperation: old: " + toReplace + ", new: " + tile);
            }
            break;
        }

        map.SetTile(position, tile);
    }

    public static Vector3Int WorldToCell(Vector3 pos)
    {
        return staticGrid.WorldToCell(pos);
    }

    public static Vector3 CellToWorld(Vector3Int pos)
    {
        return staticGrid.CellToWorld(pos);
    }
}
