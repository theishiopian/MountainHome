using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SurfaceGenerator : AbstractGenerator
{
    [Serializable]
    public class SurfaceLayer
    {
        public TileBase tile;
        public int depth;
    }

    public BiomeTile biome;
    public SurfaceLayer[] layers;

    int depth;

    public override void Generate(WorldGeneration generator, System.Random rng, int width)
    {
        base.Generate(generator, rng, width);

        depth = startingDepth;

        foreach (SurfaceLayer layer in layers)
        {
            for (pos.y = depth; pos.y >= -layer.depth; pos.y--)
            {
                for (pos.x = -width/2; pos.x <= width/2; pos.x++)
                {
                    TilemapManager.SetTile(TileLayer.TERRAIN, layer.tile, pos);
                    TilemapManager.SetTile(TileLayer.BIOME, biome, pos);
                }  
            }
            depth -= layer.depth;
        }
    }
}
