using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct SelectNextWaypointSystem : ISystem
{
    private ComponentLookup<LocalToWorld> m_LocalToWorldTransformFromEntity;
    private ComponentLookup<Waypoint> m_WaypointFromEntity;
    
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Random.CreateFromIndex(1234);
        
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
        m_WaypointFromEntity = state.GetComponentLookup<Waypoint>(true);

        foreach (var path in
                 SystemAPI.Query<
                     Path
                 >())
        {
            
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_LocalToWorldTransformFromEntity.Update(ref state);
        m_WaypointFromEntity.Update(ref state);
        
        //var job = new TraverseTreeJob();
        //state.Dependency = job.Schedule(state.Dependency);
        
        foreach (var (transform, agent, target, destination/*, waypointtag*/) in
                 SystemAPI.Query <
                     TransformAspect,
                     RefRW<Agent>,
                     RefRW<TargetPosition>,
                     RefRW<DestinationPlatform>
                     //WaypointMovementTag
                     > ())
        {
            var distance = math.distancesq(target.ValueRO.Value, transform.WorldPosition);
            
            if (distance <= Utility.kStopDistance)
            {
                var waypointEntity = agent.ValueRO.CurrentWaypoint;
                var waypointCom = m_WaypointFromEntity[waypointEntity];
                var waypointTransf = m_LocalToWorldTransformFromEntity[waypointEntity];

                var pathEntity = waypointCom.PathEntity;
                var pathID = SystemAPI.GetComponent<PathID>(pathEntity);
                
                // randomly decide on target platform
                if (SystemAPI.HasSingleton<Config>())
                {
                    var config = SystemAPI.GetSingleton<Config>();
                    destination.ValueRW.Value =
                        random.NextInt(config.NumberOfStations * config.PlatformCountPerStation);
                }

                if(pathID.Value <= destination.ValueRO.Value && waypointCom.Connection != Entity.Null) 
                {
                    agent.ValueRW.CurrentWaypoint = waypointCom.Connection;
                    target.ValueRW.Value = waypointTransf.Position;
                }
                else
                {
                    agent.ValueRW.CurrentWaypoint = waypointCom.NextWaypointEntity;
                    target.ValueRW.Value = waypointTransf.Position;
                }
            }
        }
    }
}