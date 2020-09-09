using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TempRatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        mgr.AddComponent<RatTag>(entity);
        mgr.AddComponent<Direction>(entity);
        mgr.AddComponentData(entity, new Speed { Value = 1f });
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
