using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AgentSpawningAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject PrefabAgent;
    public float CarLength;

    [Range(1, 100)]
    public int AgentNumber;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AgentSpawner()
        {
            NumAgents = AgentNumber,
            Prefab = conversionSystem.GetPrimaryEntity(PrefabAgent),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PrefabAgent);
    }
}
//
