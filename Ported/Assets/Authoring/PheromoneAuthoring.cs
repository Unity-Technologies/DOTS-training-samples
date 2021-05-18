using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class PheromoneAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public int pheromoneGridSize = 128;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PheromoneMap()
        {
            gridSize = pheromoneGridSize
        });

        dstManager.AddComponent<Respawn>(entity);
    }
}