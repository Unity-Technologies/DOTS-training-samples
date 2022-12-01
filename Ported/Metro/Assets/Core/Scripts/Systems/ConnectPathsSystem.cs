using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SpawnerSystem))]
[BurstCompile]
partial struct ConnectPathsSystem : ISystem
{
    private EntityQuery myQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<Path>();
        myQuery = state.GetEntityQuery(builder);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {     
        /*
        var ecbSingleton =
            SystemAPI.GetSingleton<
                BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var job = new MyIJobChunk
        {
            FooHandle = state.GetComponentTypeHandle<Path>(true),
            Ecb = ecb.AsParallelWriter()
        };
        
        state.Dependency = job.ScheduleParallel(myQuery, state.Dependency);
        */
        
        // wait until all dynamically spawned entities are created
        if (SystemAPI.Time.ElapsedTime < 0.1f) return;
        if (!SystemAPI.HasSingleton<Config>()) return;
        var config = SystemAPI.GetSingleton<Config>();
        state.Enabled = false;

        var pathsCount = config.PlatformCountPerStation * config.NumberOfStations;
        var paths = CollectionHelper.CreateNativeArray<Path>(pathsCount, Allocator.Temp);
        
        foreach (var (path, pathID) in SystemAPI.Query<
                     Path,
                     PathID
                 >())
        {
            paths[pathID.Value] = path;
        }
        
        for (int i = 0; i < pathsCount; i++)
        {
            if (i >= (config.PlatformCountPerStation - 1) * config.NumberOfStations)
                break;
            
            var entryWaypointLeft = paths[i].Entry;
            var entryWaypointRight = paths[i + config.NumberOfStations].Entry;
            
            var exitWaypointLeft = paths[i].Exit;
            var exitWaypointRight = paths[i + config.NumberOfStations].Exit;

            Connect(ref state, entryWaypointLeft, exitWaypointRight);
            Connect(ref state, entryWaypointRight, exitWaypointLeft);
        }

        paths.Dispose();
    }
    
    private void Connect(ref SystemState state, Entity entry, Entity exit)
    {
        var entryWaypoint = SystemAPI.GetComponent<Waypoint>(entry);
        entryWaypoint.Connection = exit;
        SystemAPI.SetComponent(entry, entryWaypoint);
    }

 /* 
  [BurstCompile]
public partial struct MyIJobChunk : IJobChunk
{
    public ComponentTypeHandle<Path> FooHandle;
    public EntityTypeHandle EntityHandle;
    
    public EntityCommandBuffer.ParallelWriter Ecb;

    [BurstCompile]
    public void Execute(in ArchetypeChunk chunk,
            int unfilteredChunkIndex,
            bool useEnableMask,
            in v128 chunkEnabledMask)
    {
        NativeArray<Entity> entities = chunk.GetNativeArray(EntityHandle);
        NativeArray<Path> foos = chunk.GetNativeArray(ref FooHandle);

        var enumerator = new ChunkEntityEnumerator(
                useEnableMask,
                chunkEnabledMask,
                chunk.Count);
        
        while (enumerator.NextEntityIndex(out var i))
        {
            //var entity = entities[i];
            var foo = foos[i];
            
            
        }
    }
}
*/
}