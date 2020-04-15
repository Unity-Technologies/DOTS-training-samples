using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Bootstrap : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int GridWidth;
    public int GridHeight;
    public float FireSpreadProbabilityMultiplier = 1f;

    public GameObject FirePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var init = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitWorldStateSystem>();
        init.GridWidth = GridWidth;
        init.GridHeight = GridHeight;
        init.FirePrefab = conversionSystem.GetPrimaryEntity(FirePrefab);

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<FireSimulationSystem>().FireSpreadProbabilityMultiplier = FireSpreadProbabilityMultiplier;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(FirePrefab);
    }
}