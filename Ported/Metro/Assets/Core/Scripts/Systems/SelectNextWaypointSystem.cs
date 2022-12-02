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
    private ComponentLookup<PathID> mPathIDFromEntity;
    
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Random.CreateFromIndex(1234);
        
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
        m_WaypointFromEntity = state.GetComponentLookup<Waypoint>(true);
        mPathIDFromEntity = state.GetComponentLookup<PathID>(true);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<Config>()) return;
            
        m_LocalToWorldTransformFromEntity.Update(ref state);
        m_WaypointFromEntity.Update(ref state);
        mPathIDFromEntity.Update(ref state);
        
        var config = SystemAPI.GetSingleton<Config>();
        var platformsCount = config.NumberOfStations * config.PlatformCountPerStation;
        
        var selectJob = new SelectNextWaypointJob
        {
            LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity, 
            WaypointFromEntity = m_WaypointFromEntity,
            PathIDFromEntity = mPathIDFromEntity,
            random = random,
            PlatformsCount = platformsCount
        };
        selectJob.ScheduleParallel();
        //state.Dependency = job.Schedule(state.Dependency);
    }
    
    [BurstCompile]
    public partial struct SelectNextWaypointJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldTransformFromEntity;
        [ReadOnly] public ComponentLookup<Waypoint> WaypointFromEntity;
        [ReadOnly] public ComponentLookup<PathID> PathIDFromEntity;

        [ReadOnly] public Random random;
        [ReadOnly] public int PlatformsCount;
        
        public void Execute(
            TransformAspect transform,
            ref Agent agent,
            ref TargetPosition target,
            ref DestinationPlatform destination 
            )
        {
            var distance = math.distancesq(target.Value, transform.WorldPosition);
            
            if (distance <= Utility.kStopDistance)
            {
                var waypointEntity = agent.CurrentWaypoint;
                var waypointCom = WaypointFromEntity[waypointEntity];
                var waypointTransf = LocalToWorldTransformFromEntity[waypointEntity];

                var pathEntity = waypointCom.PathEntity;
                var pathID = PathIDFromEntity[pathEntity];
                
                // randomly decide on target platform
                {
                    destination.Value =
                        random.NextInt(PlatformsCount);
                }

                if(pathID.Value <= destination.Value && waypointCom.Connection != Entity.Null) 
                {
                    agent.CurrentWaypoint = waypointCom.Connection;
                    target.Value = waypointTransf.Position;
                }
                else
                {
                    agent.CurrentWaypoint = waypointCom.NextWaypointEntity;
                    target.Value = waypointTransf.Position;
                }
            }
        }
    }
}