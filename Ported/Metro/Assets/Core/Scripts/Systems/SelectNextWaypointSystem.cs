using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct SelectNextWaypointSystem : ISystem
{
    private ComponentLookup<LocalToWorld> m_LocalToWorldTransformFromEntity;
    private ComponentLookup<Waypoint> m_WaypointFromEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
        m_WaypointFromEntity = state.GetComponentLookup<Waypoint>(true);
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
        
        foreach (var (transform, agent, target, waypointtag) in
                 SystemAPI.Query <
                     TransformAspect,
                     RefRW<Agent>,
                     RefRW<TargetPosition>,
                     WaypointMovementTag
                     > ())
        {
            var distance = math.distancesq(target.ValueRO.Value, transform.WorldPosition.xz);
            if (distance <= Utility.kStopDistance)
            {
                var waypointEntity = agent.ValueRO.CurrentWaypoint;
                var waypointCom = m_WaypointFromEntity[waypointEntity];
                var waypointTransf = m_LocalToWorldTransformFromEntity[waypointEntity];
                
                agent.ValueRW.CurrentWaypoint = waypointCom.NextWaypointEntity;
                target.ValueRW.Value = waypointTransf.Position.xz;
            }
        }
    }
  /*  
    [WithAll(typeof(Waypoint))]
    [BurstCompile]
    public partial struct TraverseTreeJob : IJobEntity
    {
        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Waypoint transf)
        {
            
        }
    }
    */
}