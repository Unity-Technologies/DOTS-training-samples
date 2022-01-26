using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class BucketAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bucket>(entity);
        dstManager.AddComponent<NonUniformScale>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
