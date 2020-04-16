using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Bootstrap : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Header("FIRE")]
    public int GridWidth;
    public int GridHeight;
    public GameObject FirePrefab;
    public float PropagationChance = 0.3f;
    public float GrowSpeed = 0.01f;
    public float UpdateFrequency = 0.01f;
    public float UpdatePropagationFrequency = 0.5f;
    [Tooltip("Random seed to use, or 0 to allow random per-run")]
    public int RandomSeed = 0;
    [Tooltip("How many fires to start on initialization")]
    public int StartingFireCount = 1;

    [Header("Colours")]
    // cell colours
    public Color colour_fireCell_neutral;
    public Color colour_fireCell_cool;
    public Color colour_fireCell_hot;
    // bot colours
    public Color colour_bot_SCOOP;
    public Color colour_bot_PASS_FULL;
    public Color colour_bot_PASS_EMPTY;
    public Color colour_bot_THROW;
    public Color colour_bot_OMNIBOT;
    // bucket Colours
    public Color colour_bucket_empty;
    public Color colour_bucket_full;

    [System.Serializable]
    public class BrigadeLineInfo
    {
        [Range(1, 1000)]
        public int WorkerCount;
    }

    [Tooltip("Brigade Line Setup")]
    public List<BrigadeLineInfo> BrigadeLines = new List<BrigadeLineInfo>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var init = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitWorldStateSystem>();
        init.GridWidth = GridWidth;
        init.GridHeight = GridHeight;
        init.StartingFireCount = StartingFireCount;
        init.RandomSeed = RandomSeed;
        init.FirePrefab = conversionSystem.GetPrimaryEntity(FirePrefab);

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<FireSimulationSystem>().PropagationChance = PropagationChance;

        foreach (var br in BrigadeLines)
        {
            var bInfo = dstManager.CreateEntity(ComponentType.ReadOnly<BrigadeInitInfo>());
            dstManager.SetName(bInfo, "BrigadeLine");
            dstManager.SetComponentData(bInfo, new BrigadeInitInfo() {WorkerCount = br.WorkerCount});
        }

        var fireSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<FireSimulationSystem>();
        fireSystem.PropagationChance = PropagationChance;
        fireSystem.GrowSpeed = GrowSpeed;
        fireSystem.UpdateFrequency = UpdateFrequency;
        fireSystem.UpdatePropagationFrequency = UpdatePropagationFrequency;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(FirePrefab);
    }
}