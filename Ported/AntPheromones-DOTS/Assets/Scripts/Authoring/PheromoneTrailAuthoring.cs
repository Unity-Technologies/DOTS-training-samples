using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PheromoneTrailAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Material material;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Texture2D texturePheromone = new Texture2D(128,128);
        texturePheromone.wrapMode = TextureWrapMode.Mirror;

        material.mainTexture = texturePheromone;

        TrailTexture trailTexture = new TrailTexture
        {
            texture = texturePheromone
        };
        
        dstManager.AddComponent<Pheromone>(entity);
        dstManager.AddComponentData(entity, trailTexture);
    }
}
