using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WriteGroup(typeof(LocalToWorld))]
public struct Size : IComponentData
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
        Debug.LogWarning("converting scale " + value);
        dstManager.AddComponentData(entity, new Size()
        {
            Value = value
        });
        
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }
}