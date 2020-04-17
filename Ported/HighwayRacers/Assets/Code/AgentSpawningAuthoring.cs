using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AgentSpawningAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject PrefabAgent;

    [Range(1, 100)]
    public int AgentNumber;
    [Range(0.01f, 0.08f)]
    public float MaxSpeedPercentage;
    [Range(0.01f, 0.08f)]
    public float MinSpeedPercentage;
    [Range(0.02f, 0.05f)]
    public float OvertakePercentIncrement;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AgentSpawner()
        {
            NumAgents = AgentNumber,
            Prefab = conversionSystem.GetPrimaryEntity(PrefabAgent),
            MaxSpeed = MaxSpeedPercentage,
            MinSpeed = MinSpeedPercentage,
            OvertakeIncrement = OvertakePercentIncrement
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PrefabAgent);
    }
}
//
