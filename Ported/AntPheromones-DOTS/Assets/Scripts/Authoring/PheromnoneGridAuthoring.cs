using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public class PheromnoneGridAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int gridX;
    public int gridY;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var pheromoneBuffer = dstManager.AddBuffer<Pheromones>(entity);
        pheromoneBuffer.EnsureCapacity(gridX * gridY);
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                pheromoneBuffer.Add(new Pheromones {pheromoneStrength = 0});
            }
        }
    }
}
