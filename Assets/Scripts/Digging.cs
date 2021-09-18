using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Digging : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 2;
    public Tile[] breakTiles;

    private Dictionary<Vector3Int, int> breakProgress = new Dictionary<Vector3Int, int>();

    private void Start()
    {
        InputManager.instance.click.started += _ => Dig();
    }

    void Dig()
    {
        Vector2 point = InputManager.instance.GetClickPos();

        if (Vector2.Distance(point, transform.position) <= radius)
        {
            Vector3Int tilePos = TilemapManager.WorldToCell(point);
            TerrainTile tile = TilemapManager.GetTile(TileLayer.TERRAIN, tilePos) as TerrainTile;

            if (tile != null)
            {
                int hp;

                if (!breakProgress.ContainsKey(tilePos))
                {
                    hp = tile.hp;
                    breakProgress.Add(tilePos, hp);
                }
                else
                {
                    hp = breakProgress[tilePos];
                }

                if (hp > 0)
                {
                    TilemapManager.SetTile(TileLayer.TILEFX, breakTiles[(hp - 1).Remap(0, tile.hp, 0, breakTiles.Length - 1)], tilePos);
                    breakProgress[tilePos]--;
                    EventBus.PlayerEvents.OnPlayerDamageBlock?.Invoke(this, tile, tilePos);
                }
                else
                {
                    TilemapManager.SetTile(TileLayer.TERRAIN, null, tilePos);
                    TilemapManager.SetTile(TileLayer.TILEFX, null, tilePos);
                    EventBus.PlayerEvents.OnPlayerDestroyBlock?.Invoke(this, tile, tilePos);
                }
            }
        }
    }
}
