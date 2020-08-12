using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Water : IComponentData
{
    public float volume;
    public float capacity;
    public float3 fullScale;
    // the transform will come from the prefab - automagically converted for us
    public Entity prefab;
}

/*
public class WaterAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Volume = 5f;
    public float Capacity;
    public Vector3 FullScale;
    public Entity Prefab;

    public void Convert(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Water
        {
            volume = Volume,
            capacity = Capacity,
            fullScale = FullScale,
            prefab = Prefab
        });
    }
}
*/