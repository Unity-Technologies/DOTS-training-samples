using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public int Dummy;
    public Entity Prefab;
}

[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/SpawnFromEntity/Spawner")]
[ConverterVersion("joe", 1)]
public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public int dummy = 42;
    public GameObject Prefab;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new Spawner
        {
            Dummy = dummy,
            Prefab = conversionSystem.GetPrimaryEntity(Prefab)
        };
        dstManager.AddComponentData(entity, spawnerData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
}
