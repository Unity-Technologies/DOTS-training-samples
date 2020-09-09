using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WriteGroup(typeof(LocalToWorld))]
public struct RotationY : IComponentData
{
    public float Value;
}

public class RotationYAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ValueDegree;
    
    
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RotationY()
        {
            Value = math.radians(ValueDegree)
        });
        dstManager.RemoveComponent<Rotation>(entity);
    }

}
