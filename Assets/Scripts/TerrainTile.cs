using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class TerrainTile : Tile
{
    [Header("Terrain Settings")]
    public int hp;
    public string resource;
    public Tile[] topDecorations;
    public Tile[] bottomDecorations;

    [CustomEditor(typeof(TerrainTile))]
    [CanEditMultipleObjects]
    public class TerrainTileEditor : Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            TerrainTile tile = AssetDatabase.LoadAssetAtPath<TerrainTile>(assetPath);
            if (tile && tile.sprite != null)
            {
                Texture2D spritePreview = AssetPreview.GetAssetPreview(tile.sprite); // Get sprite texture

                Color[] pixels = spritePreview.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = pixels[i] * tile.color; // Tint
                }
                spritePreview.SetPixels(pixels);
                spritePreview.Apply();

                Texture2D preview = new Texture2D(width, height);
                EditorUtility.CopySerialized(spritePreview, preview); // Returning the original texture causes an editor crash
                return preview;
            }
            return null;
        }
    }
}
