using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class WorldGeneration : MonoBehaviour
{
    [Header("Settings")]
    public int width = 100;
    public int seed = 0;
    public bool useSeed = false;

    [Header("Generators")]
    public AbstractGenerator[] generators;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        yield return new WaitForSeconds(0.01f);
        
        System.Random rng = useSeed ? new System.Random(seed) : new System.Random();


        EventBus.GenerationEvents.OnGenerationStart?.Invoke(this, useSeed, seed);

        foreach(AbstractGenerator generator in generators)
        {
            generator.Generate(this, rng, width);
        }

        EventBus.GenerationEvents.OnGenerationEnd?.Invoke(this);

        yield return null;
    }

    
}
