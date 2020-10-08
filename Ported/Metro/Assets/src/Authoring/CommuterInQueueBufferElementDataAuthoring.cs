using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
public class CommuterInQueueBufferElementDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> Items;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<CommuterInQueueBufferElementData>(entity);

        foreach (var item in Items)
        {
            buf.Add(new CommuterInQueueBufferElementData { Value = conversionSystem.GetPrimaryEntity(item) });
        }
    }
}
public struct CommuterInQueueBufferElementData : IBufferElementData
{
    public Entity Value;
}