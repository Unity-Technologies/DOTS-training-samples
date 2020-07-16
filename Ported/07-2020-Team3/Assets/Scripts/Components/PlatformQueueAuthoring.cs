using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlatformQueue : IBufferElementData
{
    public Entity Value;
}

public class PlatformQueueAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] QueueWaypointGameObjects;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        for (var i = 0; i < QueueWaypointGameObjects.Length; ++i)
        {
            var waypointGameObject = QueueWaypointGameObjects[i];
            conversionSystem.DeclareDependency(gameObject, waypointGameObject);
            if (!conversionSystem.HasPrimaryEntity(waypointGameObject))
                return;
        }

        var queuesBuffer = dstManager.AddBuffer<PlatformQueue>(entity);
        queuesBuffer.Capacity = QueueWaypointGameObjects.Length;
        for (var i = 0; i < QueueWaypointGameObjects.Length; ++i)
        {
            var waypointGameObject = QueueWaypointGameObjects[i];
            queuesBuffer.Add(new PlatformQueue { Value = conversionSystem.GetPrimaryEntity(waypointGameObject) });
        }
    }
}
