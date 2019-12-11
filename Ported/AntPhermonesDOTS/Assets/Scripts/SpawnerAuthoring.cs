using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Spawner : IComponentData
{
    public int Dummy;
}


[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/SpawnFromEntity/Spawner")]
[ConverterVersion("joe", 1)]
public class SpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int dummy = 42;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // TODO: Add prefab support
        var spawnerData = new Spawner
        {
            Dummy = dummy
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
