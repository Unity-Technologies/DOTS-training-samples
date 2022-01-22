using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ResourceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<ResourceTag>(entity);

        dstManager.AddComponentData(entity, new Holder
        {
            Value = Entity.Null
        });

        dstManager.AddComponentData(entity, new Velocity
        {
            Value = float3.zero
        });

        dstManager.AddComponentData(entity, new Targeted
        {
            Value = false
        });
    }
}
