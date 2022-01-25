using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;

[DisallowMultipleComponent]
public class FireRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<FireRenderer>(entity);
        dstManager.AddComponent<NonUniformScale>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
    }
}
