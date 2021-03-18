using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class BloodDropletAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Force>(entity);
        dstManager.AddComponent<Velocity>(entity);
        dstManager.AddComponentData(entity, new NonUniformScale() { Value = transform.localScale } );
        dstManager.AddComponent<VelocityScale>(entity);
        dstManager.AddComponentData(entity, new InitialScale() {Value = transform.localScale });
    }
}