using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GravityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ResourceGravity = 0.5f;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GravityConstants
        {
            ResourceGravity = ResourceGravity
        });
    }
}
