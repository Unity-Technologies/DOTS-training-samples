using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Capacity = 50.0f;
    [Range(0, 1)]
    public float InitialVolumeRatio = 1.0f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WaterTagComponent());
        dstManager.AddComponentData(entity, new WaterVolumeComponent(){Volume = Capacity * InitialVolumeRatio});
        dstManager.AddComponentData(entity, new WaterCapacityComponent(){Capacity = Capacity});
    }
}
