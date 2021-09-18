using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGenerator : ScriptableObject
{
    public int startingDepth = 0;
    protected int width;
    protected float offset;

    protected Vector3Int pos = new Vector3Int();

    public virtual void Generate(WorldGeneration generator, System.Random rng, int width)
    {
        this.width = width;
        offset = NextFloat(rng, -99999, 99999);
    }

    protected float NextFloat(System.Random rng, float min, float max)
    {
        double val = (rng.NextDouble() * (max - min) + min);
        return (float)val;
    }
}
