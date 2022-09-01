using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class OverridableMaterialPropertiesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new OverridableMaterial_Color{ Value = new float4(1f) });
        dstManager.AddComponentData(entity, new OverridableMaterial_Smoothness { Value = 0.5f });
    }
}
