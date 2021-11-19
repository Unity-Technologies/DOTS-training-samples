using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

public class BeeAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, 
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Bee>(entity);
        dstManager.AddComponent<Flutter>(entity);
        dstManager.AddComponent<BeeIdleMode>(entity);
        dstManager.AddComponent<TargetedBy>(entity);
        dstManager.AddComponentData(entity, new NonUniformScale { Value = new float3(1.0f) });
    }
}