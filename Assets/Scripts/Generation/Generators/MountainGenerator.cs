using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class MountainGenerator : AbstractGenerator
{
    public AnimationCurve mountainCurve;
    public TileBase mainTile;
    public TileBase biome;
    public int height = 100;
    public float slope;
    public float preMultiply;
    public float postMultiply;
    public int fadeDepth = 0;
    public AnimationCurve fade;

    public override void Generate(WorldGeneration generator, System.Random rng, int width)
    {
        base.Generate(generator, rng, width);

        for (pos.y = startingDepth; pos.y <= startingDepth + height; pos.y++)
        {
            for (pos.x = -width / 2; pos.x <= width / 2; pos.x++)
            {
                float noise = postMultiply * Mathf.PerlinNoise(0, (pos.x + offset) * preMultiply).Remap(0,1, -1, 1);
                //float gauss = Gaussian(height, slope, pos.x);
                float gauss = mountainCurve.Evaluate(((float)pos.x).Remap(-(width/2), width/2, 0, 1));
                //Debug.Log(noise);


                if (pos.y - startingDepth <= gauss.Remap(0,1, startingDepth, startingDepth + height) + noise)
                {
                    float p = NextFloat(rng, 0, 1);

                    float t = fade.Evaluate(((float)pos.y).Remap(fadeDepth, fadeDepth + startingDepth, 0, 1));

                    if(pos.y <= fadeDepth)
                    {
                        if(p < t)TilemapManager.SetTile(TileLayer.TERRAIN, mainTile, pos);
                    }
                    else
                    {
                        TilemapManager.SetTile(TileLayer.TERRAIN, mainTile, pos);
                    }

                    TilemapManager.SetTile(TileLayer.BIOME, biome, pos);
                }
            }
        }
    }
}
