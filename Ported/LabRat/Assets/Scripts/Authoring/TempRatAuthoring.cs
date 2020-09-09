using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class TempRatAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager mgr, GameObjectConversionSystem conversionSystem)
    {
        var pseudoRndDirection = 1 << Mathf.RoundToInt(Mathf.Abs(Vector3.Dot(transform.position, Vector3.one) % 4));
        var pseudoRndSpeed = 2.5f + Mathf.Abs(Vector3.Dot(transform.position, Vector3.one * 0.39f)) % 1.5f;
        mgr.AddComponent<RatTag>(entity);
        mgr.AddComponentData(entity, new Direction {Value = (Direction.Attributes)pseudoRndDirection});
        mgr.AddComponentData(entity, new Speed { Value = pseudoRndSpeed });
        mgr.AddComponentData(entity, new Size { Value = transform.localScale.x });
        mgr.AddComponentData(entity, new PositionXZ {Value = new float2(transform.position.x, transform.position.z)});
    }
}
