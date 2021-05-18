using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FoodSourceAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        var random = new Random((uint) System.DateTime.Now.Ticks);
        var direction = random.NextFloat(0, 2.0f * Mathf.PI);
        var position = new float3(Mathf.Cos(direction), Mathf.Sin(direction), 0) * 40f;

        dstManager.AddComponent<FoodSource>(entity);
        dstManager.SetComponentData(entity, new Translation {Value = position});
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
        dstManager.SetComponentData(entity, new URPMaterialPropertyBaseColor {Value = new float4(0, 1, 0, 0)});
    }
}