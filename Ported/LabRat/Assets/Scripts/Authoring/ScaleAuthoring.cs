using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[MaterialProperty("_UniformScale", MaterialPropertyFormat.Float)]
public struct Scale : IComponentData
{
    public float Value;
}

public class ScaleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Scale;

    [Header("Random Scale")]
    public bool Enable = false;
    public float MinScale;
    public float MaxScale;
    


    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        var rnd = new Random(((uint)UnityEngine.Random.Range(1, 100000)));
        var value = Enable ? rnd.NextFloat(MinScale, MaxScale) : Scale;
        var f3 = new float3(value, value, value);
        dstManager.RemoveComponent<NonUniformScale>(entity);
        dstManager.AddComponentData(entity, new NonUniformScale()
        {
            Value = f3
        });
    }
}