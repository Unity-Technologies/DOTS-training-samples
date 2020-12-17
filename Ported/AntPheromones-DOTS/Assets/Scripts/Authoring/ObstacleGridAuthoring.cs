using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public class ObstacleGridAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int gridX;
    public int gridY;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var obstacleBuffer = dstManager.AddBuffer<ObstacleBufferElement>(entity);
        obstacleBuffer.EnsureCapacity(gridX * gridY);
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                obstacleBuffer.Add(new ObstacleBufferElement {present = false});
            }
        }
        // dstManager.AddComponentData(entity, new Board {BoardHeight = gridY, BoardWidth = gridX});
    }
}