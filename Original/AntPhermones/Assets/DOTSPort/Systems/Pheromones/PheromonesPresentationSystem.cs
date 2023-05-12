using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public partial struct PheromonesPresentationInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();

        var map = MonoBehaviour.FindObjectOfType<PheromoneMap>();
        
        map.CreateTexture(globalSettings.MapSizeX, globalSettings.MapSizeY);

        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(entity, new PheromoneTextureManaged()
        {
            texture = map.PheromoneMapTexture
        });

#if UNITY_EDITOR
        state.EntityManager.SetName(entity, $"__Pheromone Texture Reference");
#endif
    }
}

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct PheromonesPresentationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var pheromoneBufferElement = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
        var PheromoneTexture = SystemAPI.ManagedAPI.GetSingleton<PheromoneTextureManaged>();
        
        var colorArray = PheromoneTexture.texture.GetPixels();
        //  PheromoneTexture.texture.SetPixelData(pheromoneBufferElement.AsNativeArray().Reinterpret<Color>(), 0, 0);

        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i].r = pheromoneBufferElement[i].Value.x;
            colorArray[i].g = pheromoneBufferElement[i].Value.y;
            colorArray[i].b = pheromoneBufferElement[i].Value.z;
        }
       
        PheromoneTexture.texture.SetPixels(colorArray);
        PheromoneTexture.texture.Apply();
    }
}


public class PheromoneTextureManaged: IComponentData
{
    public Texture2D texture;

    // Every IComponentData class must have a no-arg constructor.
    public PheromoneTextureManaged()
    {
    }
}



