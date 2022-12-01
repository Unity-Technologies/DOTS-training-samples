using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

[UpdateAfter(typeof(SpawnerSystem))]
[BurstCompile]
partial struct ConnectPathsSystem : ISystem
{
    private EntityQuery m_BaseColorQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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
        if (SystemAPI.Time.ElapsedTime < 0.5f) return;
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
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        
        for (int i = 0; i < pathsCount; i++)
        {
            if (i >= (config.PlatformCountPerStation - 1) * config.NumberOfStations)
                break;
            
            var entryWaypointLeft = paths[i].EntryLeft;
            var exitWaypointLeft = paths[i + config.NumberOfStations].ExitLeft;
            
            var entryWaypointRight = paths[i].EntryRight;
            var exitWaypointRight = paths[i + config.NumberOfStations].ExitRight;
            
            Connect(ref state, entryWaypointLeft, exitWaypointLeft, ecb);
            Connect(ref state, entryWaypointRight, exitWaypointRight, ecb);
        }

        paths.Dispose();
    }
    
    private void Connect(ref SystemState state, Entity entry, Entity exit, EntityCommandBuffer ecb)
    {
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        // entry green
        ecb.SetComponentForLinkedEntityGroup(entry, queryMask, new URPMaterialPropertyBaseColor { Value = new float4(0f,1f,0f,1f) });
        // exit red
        ecb.SetComponentForLinkedEntityGroup(exit, queryMask, new URPMaterialPropertyBaseColor { Value = new float4(1f,0f,0f,1f) });
        
        // forward connection
        var entryWaypoint = SystemAPI.GetComponent<Waypoint>(entry);
        entryWaypoint.Connection = exit;
        SystemAPI.SetComponent(entry, entryWaypoint);
        
        // backward connection
        var exitWaypoint = SystemAPI.GetComponent<Waypoint>(exit);
        exitWaypoint.Connection = entry;
        SystemAPI.SetComponent(exit, exitWaypoint);
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