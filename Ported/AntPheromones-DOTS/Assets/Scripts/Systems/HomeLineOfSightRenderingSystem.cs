using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HomeLineOfSightRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entity antMovementParametersEntity = GetSingletonEntity<AntMovementParameters>();
        AntMovementParameters antMovementParameters =
            EntityManager.GetComponentData<AntMovementParameters>(antMovementParametersEntity);
        if (!antMovementParameters.debug) 
            return;
        
        Entity lineOfSightEntity = GetSingletonEntity<HomeLineOfSightBufferElement>();
        DynamicBuffer<HomeLineOfSightBufferElement> lineOfSightGrid =
            EntityManager.GetBuffer<HomeLineOfSightBufferElement>(lineOfSightEntity);
        
        Entity trailTextureEntity = GetSingletonEntity<TrailTexture>();
        TrailTexture trailTexture = EntityManager.GetComponentData<TrailTexture>(trailTextureEntity);
        
        Color[] pixels = trailTexture.texture.GetPixels();
        
        for (int i = 0; i < lineOfSightGrid.Length; i++)
        {
            if (!lineOfSightGrid[i].present)
                continue;
            pixels[i] = new Color(0f, 0f, 1f, 0.2f);
        }
        
        trailTexture.texture.SetPixels(pixels);
        trailTexture.texture.Apply();
    }
}