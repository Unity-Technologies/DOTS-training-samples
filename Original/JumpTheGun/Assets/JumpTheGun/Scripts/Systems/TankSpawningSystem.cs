using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
[UpdateAfter(typeof(BoxPositioningSystem))]
public partial class TankSpawningSystem : SystemBase
{
    private EntityQuery _tankQuery;
    private EntityQuery _boxQuery;
    protected override void OnCreate()
    {
        _tankQuery = GetEntityQuery(typeof(Tank));
        _boxQuery = GetEntityQuery(typeof(Boxes), typeof(Translation), typeof(NonUniformScale));
    }
    protected override void OnUpdate()
    {
        CreateTanks();

    }
    public void CreateTanks()
    {

        UnityEngine.Debug.Log("Inside CreateTanks");
        // Create query for EntityPrefabHolder
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var boxexEntities = _boxQuery.ToEntityArray(Allocator.Temp);
        var boxes = _boxQuery.ToComponentDataArray<Boxes>(boxexEntities, Allocator.Temp);
        var tanks = _tankQuery.ToEntityArray(Allocator.Temp);
        var boxPositions = _boxQuery.ToComponentDataArray<Translation>(boxexEntities, Allocator.Temp);
        var boxScales = _boxQuery.ToComponentDataArray<NonUniformScale>(boxexEntities, Allocator.Temp);
        Config config = GetSingleton<Config>();
        if (tanks.Length == 0 && boxes.Length > 0)
        {
            int maxBrickCount = (int)config.terrainWidth * config.terrainLength;
            int tankCount = math.min(boxes.Length - 1, config.tankCount);
            for (int i = 0; i < tankCount; i++)
            {
                // UnityEngine.Debug.Log("boxes "+ boxPositions[i].Value);
                var boxesTranslation = boxPositions[i];
                var boxScale = boxScales[i];
                float3 scale = boxScale.Value;
                var instance = ecb.Instantiate(config.tankPrefab);
                //check if Tank is on player box
                /*if (boxesTranslation.Value.x == targetTransform.Value.x ) { 
                }*/
                float3 position = boxesTranslation.Value;

                position.y = scale.y;

                position.y *= 0.6f;
                UnityEngine.Debug.Log("Position " + position.x);
                ecb.SetComponent(instance, new Translation
                {
                    Value = position
                });

            }
            ecb.Playback(World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>().EntityManager);
        }
    }
}