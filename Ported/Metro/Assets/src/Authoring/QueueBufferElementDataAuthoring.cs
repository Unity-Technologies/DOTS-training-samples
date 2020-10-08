using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class QueueBufferElementDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> Items;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<QueueBufferElementData>(entity);

        foreach (var item in Items)
        {
            Entity childEntity = conversionSystem.GetPrimaryEntity(item);
            //dstManager.AddBuffer<CommuterInQueueBufferElementData>(childEntity);
            buf.Add(new QueueBufferElementData { Value = childEntity });
        }
    }
}

public struct QueueBufferElementData : IBufferElementData
{
    public Entity Value;
}