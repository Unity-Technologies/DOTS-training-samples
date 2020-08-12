using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/SpawnFromEntity/Spawner")]
[ConverterVersion("joe", 1)]
public class LineSpawnerAuthoring_FromEntity : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject Prefab;
    public int Count;
    public int CountOfFullPassBots;
    public int CountOfEmptyPassBots;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new LineSpawner_FromEntity
        {
            // The referenced prefab will be converted due to DeclareReferencedPrefabs.
            // So here we simply map the game object to an entity reference to that prefab.
            LinePrefab = conversionSystem.GetPrimaryEntity(Prefab),
            BotPrefab = conversionSystem.GetPrimaryEntity(Prefab),
            Count = Count,
            CountOfFullPassBots = CountOfFullPassBots,
            CountOfEmptyPassBots = CountOfEmptyPassBots
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
