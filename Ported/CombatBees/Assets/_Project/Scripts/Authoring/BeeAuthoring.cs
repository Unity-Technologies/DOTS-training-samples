using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class BeeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Bee Bee;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, Bee);
        dstManager.AddComponentData(entity, new NonUniformScale { Value = new float3(1f)});
        dstManager.AddComponentData(entity, new ConstrainToLevelBounds());
    }
}
