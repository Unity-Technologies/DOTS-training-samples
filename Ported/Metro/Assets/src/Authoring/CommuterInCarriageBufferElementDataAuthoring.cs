using Unity.Entities;
using UnityEngine;
public class CommuterInCarriageBufferElementDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Capacity = 27;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var commuterBuffer = dstManager.AddBuffer<CommuterInCarriageBufferElementData>(entity);

        for (int i = 0; i < Capacity; ++i)
        {
            commuterBuffer.Add(new CommuterInCarriageBufferElementData { Value = Entity.Null });
        }
    }
}

public struct CommuterInCarriageBufferElementData : IBufferElementData
{
    public Entity Value;
}
