using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TempRatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        var psuedoRndDirection = 1 << Mathf.RoundToInt(Mathf.Clamp(Mathf.Abs(Vector3.Dot(transform.position, Vector3.one)), 0, 3)); 
        mgr.AddComponent<RatTag>(entity);
        mgr.AddComponentData(entity, new Direction {Value = (byte)psuedoRndDirection});
        mgr.AddComponentData(entity, new Speed { Value = 5f });
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
