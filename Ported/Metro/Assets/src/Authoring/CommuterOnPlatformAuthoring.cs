using System.Collections.Generic;
using Unity.Entities;

public struct CommuterOnPlatform : IComponentData
{
    public Entity Value;
}

public class CommuterOnPlatformAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityEngine.GameObject Platform;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CommuterOnPlatform
        {
            Value = conversionSystem.GetPrimaryEntity(Platform),
        });
    }

    public void DeclareReferencedPrefabs(List<UnityEngine.GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Platform);
    }
}
