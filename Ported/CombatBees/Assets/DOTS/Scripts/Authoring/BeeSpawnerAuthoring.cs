using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BeeSpawnerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BeePrefab;
    [UnityRange(0, 1000)] public int BeeCount;
    [UnityRange(0, 100)] public int BeeCountFromResource = 10;

    [UnityRange(0, 100)] public int MaxSpeed = 1;
    [UnityRange(0, 100)] public int MaxSize = 1;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BeeSpawner
        {
            BeePrefab = conversionSystem.GetPrimaryEntity(BeePrefab),
            BeeCount = BeeCount,
            BeeCountFromResource = BeeCountFromResource,
            MaxSpeed = MaxSpeed,
            MaxSize = MaxSize
        });
    }
}
