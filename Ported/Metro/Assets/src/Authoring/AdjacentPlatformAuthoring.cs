using System.Collections.Generic;
using Unity.Entities;

public struct AdjacentPlatform : IComponentData
{
    public Entity Value;
}

public class AdjacentPlatformAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityEngine.GameObject AdjacentPlatform;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AdjacentPlatform
        {
            Value = conversionSystem.GetPrimaryEntity(AdjacentPlatform),
        });
    }

    public void DeclareReferencedPrefabs(List<UnityEngine.GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(AdjacentPlatform);
    }
}
