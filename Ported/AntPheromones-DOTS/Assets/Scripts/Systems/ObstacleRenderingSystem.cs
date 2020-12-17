using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ObstacleRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity obstacleEntity = GetSingletonEntity<ObstacleBufferElement>();
        DynamicBuffer<ObstacleBufferElement> obstacleGrid = EntityManager.GetBuffer<ObstacleBufferElement>(obstacleEntity);
        
        Entity trailTextureEntity = GetSingletonEntity<TrailTexture>();
        TrailTexture trailTexture = EntityManager.GetComponentData<TrailTexture>(trailTextureEntity);
        
        Color[] pixels = trailTexture.texture.GetPixels();
        
        for (int i = 0; i < obstacleGrid.Length; i++)
        {
            if (!obstacleGrid[i].present)
                continue;
            pixels[i] = new Color(0f, 0f, 0f);
        }
        
        trailTexture.texture.SetPixels(pixels);
        trailTexture.texture.Apply();
    }
}