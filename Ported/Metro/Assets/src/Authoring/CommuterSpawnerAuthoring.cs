using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CommuterSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;
    public int Count;
    public Rect Rect;
    public float Height;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CommuterSpawner
        {
            Prefab = conversionSystem.GetPrimaryEntity(Prefab),
            Count = Count,
            Min = new float2(Rect.xMin, Rect.yMin),
            Max = new float2(Rect.xMax, Rect.yMax),
            Height = Height
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
}

public struct CommuterSpawner : IComponentData
{
    public Entity Prefab;
    public int Count;
    public float2 Min;
    public float2 Max;
    public float Height;
}