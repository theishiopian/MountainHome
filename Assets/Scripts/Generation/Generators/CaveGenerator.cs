using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu]
public class CaveGenerator : AbstractGenerator
{
    [Serializable]
    public class Speckle
    {
        [Tooltip("The tile to use")]
        public TileBase tile;

        [Tooltip("The distribution over the layer's depth of this speckle")]
        public AnimationCurve distribution;
    }

    [Serializable]
    public class Blob
    {
        public TileBase tile;
        [Tooltip("Multiplies the scale of the blob noisemap. Higher values lead to smaller more dense blobs")]
        public float multiplier = 10;
        [Tooltip("The actual size of the individual blobs. higher values lead to bigger blobs")]
        [Range(0, 1)]
        public float size = 0.5f;
        [Tooltip("Per axis scale multiplication. Smaller values lead to larger stretch")]
        public Vector2 scale;
    }

    [Serializable]
    public class CavernLayer
    {
        [Tooltip("The tile this layer is made of")]
        public TileBase primaryTile;

        [Tooltip("The depth of this layer")]
        public int depth = 10;

        [Tooltip("Multiplies the scale of the cave noisemap, higher values lead to smaller, more dense caves")]
        public float multiplier = 10;

        [Range(0, 1)]
        [Tooltip("The size of the actual caverns. 0 means solid earth and 1 means open air")]
        public float size = 0.5f;

        [Tooltip("Per axis scale multiplication. Smaller values lead to larger stretch")]
        public Vector2 scale;

        [Tooltip("One block speckles to add in the level, useful for rare ores")]
        public Speckle[] speckles;

        [Tooltip("Large chunks of blocks to add in the level, useful for large ore veins")]
        public Blob[] blobs;
    }

    public CavernLayer[] layers;
    public BiomeTile biome;//temporary

    float noise = 0.5f;
    int depth;

    public override void Generate(WorldGeneration generator, System.Random rng, int width)
    {
        base.Generate(generator, rng, width);
        depth = startingDepth;

        CavernLayer layer;

        //pass 1: caves and ores
        for (int i = 0; i < layers.Length; i++)
        {
            layer = layers[i];

            for (pos.y = depth; pos.y >= depth - layer.depth; pos.y--)
            {
                for (pos.x = -width / 2; pos.x <= width / 2; pos.x++)
                {
                    TilemapManager.SetTile(TileLayer.BIOME, biome, pos);
                    noise = GetNoise(pos, layer.multiplier, offset, layer.scale, depth, layer.depth);

                    if (noise >= layer.size)
                    {
                        //outside cave
                        TileBase toPlace = layer.primaryTile;

                        float top = depth;
                        float bottom = depth - layer.depth;

                        foreach (Speckle speckle in layer.speckles)
                        {
                            float p = NextFloat(rng, 0, 1);

                            float b = speckle.distribution.Evaluate(((float)pos.y).Remap(top, bottom, 0, 1));

                            if (p < b)
                            {
                                toPlace = speckle.tile;
                            }
                        }

                        foreach (Blob blob in layer.blobs)
                        {
                            noise = GetNoise(pos, blob.multiplier, offset, blob.scale, depth, layer.depth);

                            if (noise < blob.size)
                            {
                                toPlace = blob.tile;
                            }
                        }

                        TilemapManager.SetTile(TileLayer.TERRAIN, toPlace, pos);
                    }
                    else
                    {
                        //inside cave
                        //place cave air for decorator and monster passes
                    }
                }
            }

            depth -= layer.depth;
        }

        depth = 0;

        //pass 2: decorators
        for (int i = 0; i < layers.Length; i++)
        {
            layer = layers[i];

            for (pos.y = depth; pos.y >= depth - layer.depth; pos.y--)
            {
                for (pos.x = -width / 2; pos.x <= width / 2; pos.x++)
                {
                    TileBase tile = TilemapManager.GetTile(TileLayer.TERRAIN, pos);

                    if (tile && tile is TerrainTile)
                    {
                        //decorators
                        float p = NextFloat(rng, 0, 1);

                        if (((TerrainTile)tile).topDecorations.Length > 0 && p > 0.75f)
                        {
                            TileBase up = TilemapManager.GetTile(TileLayer.TERRAIN, pos + Vector3Int.up);

                            if (up == null)
                            {
                                TilemapManager.SetTile(TileLayer.TERRAIN, ((TerrainTile)tile).topDecorations[rng.Next(0, ((TerrainTile)tile).topDecorations.Length)], pos + Vector3Int.up);
                            }
                        }

                        p = NextFloat(rng, 0, 1);

                        if (((TerrainTile)tile).bottomDecorations.Length > 0 && p > 0.75f)
                        {
                            TileBase up = TilemapManager.GetTile(TileLayer.TERRAIN, pos + Vector3Int.down);

                            if (up == null)
                            {
                                TilemapManager.SetTile(TileLayer.TERRAIN, ((TerrainTile)tile).bottomDecorations[rng.Next(0, ((TerrainTile)tile).bottomDecorations.Length)], pos + Vector3Int.down);
                            }
                        }
                    }
                }
            }

            depth -= layer.depth;
        }
    }

    private float GetNoise(Vector3Int pos, float multiplier, float r, Vector2 scale, int currentDepth, int depth)
    {
        float x, y;
        x = multiplier * scale.x * ((float)(pos.x)).Remap((-width / 2f) + r, (width / 2f) + r, 0f, 1f);
        y = multiplier * scale.y * ((float)(pos.y)).Remap((currentDepth - depth) + r, currentDepth + r, 0f, 1f);

        return Mathf.PerlinNoise(x, y);//add to coords for fuzz
    }
}
