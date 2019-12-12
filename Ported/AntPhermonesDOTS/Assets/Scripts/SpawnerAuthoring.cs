using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity Prefab;
    public int AntCount;
}

[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/SpawnFromEntity/Spawner")]
[ConverterVersion("joe", 1)]
public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject Prefab;
    public int antCount = 100;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new Spawner
        {
            Prefab = conversionSystem.GetPrimaryEntity(Prefab),
            AntCount = antCount
        };
        dstManager.AddComponentData(entity, spawnerData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
}
