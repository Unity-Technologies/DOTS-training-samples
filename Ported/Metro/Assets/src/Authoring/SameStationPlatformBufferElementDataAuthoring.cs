using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SameStationPlatformBufferElementDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> Platforms;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<SameStationPlatformBufferElementData>(entity);

        foreach (var platform in Platforms)
        {
            buf.Add(new SameStationPlatformBufferElementData { Value = conversionSystem.GetPrimaryEntity(platform) });
        }
    }


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(Platforms);
    }
}

public struct SameStationPlatformBufferElementData : IBufferElementData
{
    public Entity Value;
}