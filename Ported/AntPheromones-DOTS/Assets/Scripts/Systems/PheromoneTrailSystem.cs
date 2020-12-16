using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PheromoneTrailSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<Pheromones>();
        DynamicBuffer<Pheromones> pheromoneGrid = EntityManager.GetBuffer<Pheromones>(pheromoneEntity);
        
        Entity trailTextureEntity = GetSingletonEntity<TrailTexture>();
        TrailTexture trailTexture = EntityManager.GetComponentData<TrailTexture>(trailTextureEntity);

        
        Color[] pixels = trailTexture.texture.GetPixels();
        
        for (int i = 0; i < pheromoneGrid.Length; i++)
        {
           pixels[i] = new Color(pheromoneGrid[i].pheromoneStrength, 0.5f, 0.5f);
        }
        
        trailTexture.texture.SetPixels(pixels);
        trailTexture.texture.Apply();
    }
}
