using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

public class CannonballBoxCollisionSystem : SystemBase
{
    private NativeList<int> _DecrementChangesList;

    private EndSimulationEntityCommandBufferSystem _ECBSys;
    
    protected override void OnCreate()
    {
        _ECBSys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        
        _DecrementChangesList = new NativeList<int>(8096, Allocator.TempJob);
        
        RequireForUpdate(GetEntityQuery(typeof(Cannonball)));
        RequireSingletonForUpdate<GameObjectRefs>();
        RequireSingletonForUpdate<HeightBufferElement>();
    }

    protected override void OnDestroy()
    {
        _DecrementChangesList.Dispose();
    }

    protected override void OnUpdate()
    {
        var ecb = _ECBSys.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();

        var boxMapEntity = GetSingletonEntity<HeightBufferElement>();

        var decrementChangesList = _DecrementChangesList;
        decrementChangesList.Clear();
        var changelistParallelWriter = _DecrementChangesList.AsParallelWriter();

        var config = this.GetSingleton<GameObjectRefs>().Config.Data;

        Entities
            .WithName("cannonball_box_collision_check")
            .WithAll<Cannonball>()
            .ForEach((Entity cannonball, int entityInQueryIndex, in Translation translation, in ParabolaTValue tValue) =>
            {
                if (tValue.Value < 0.97f) // TODO: fix movement system so it actually gets to 1.0
                    return;
                
                parallelWriter.DestroyEntity(entityInQueryIndex, cannonball);
                    
                // Apply tile damage
                var x = (int) translation.Value.x;
                var y = (int) translation.Value.z;
                changelistParallelWriter.AddNoResize(y * config.TerrainLength + x);
                
            }).ScheduleParallel();

        var heightMap = GetBuffer<HeightBufferElement>(boxMapEntity);
        
        Job
            .WithName("cannonball_box_collision_apply_damage")
            .WithCode(() =>
            {
                foreach (var index in decrementChangesList)
                {
                    var tile = heightMap[index];
                    tile.Value = Mathf.Max(config.MinTerrainHeight, tile.Value - config.HeightDamage);
                    heightMap[index] = tile;
                }
            }).Schedule();
        
        _ECBSys.AddJobHandleForProducer(Dependency);
    }
}
