using Unity.Entities;
using UnityEngine;
using Unity.Rendering;

[DisallowMultipleComponent]
public class BucketAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{

    public float Capacity = 3.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WaterVolumeComponent());
        dstManager.AddComponentData(entity, new WaterCapacityComponent(){Capacity = Capacity});
        dstManager.AddComponentData(entity, new BucketActiveComponent());
        dstManager.AddComponentData(entity, new BucketFullComponent());
        dstManager.AddComponentData(entity, new URPMaterialPropertyBaseColor());
    }
}
