using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct MovementSystem : ISystem
{
    private EntityQuery myQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<LocalTransform, Speed, TargetPosition, WaypointMovementTag>();
        builder.WithNone<PassengerInfo>();
        myQuery = state.GetEntityQuery(builder);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        
        /*
        foreach (var (transform, speed, target, waypointMovement) in
                 SystemAPI.Query<
                     TransformAspect,
                     RefRO<Speed>,
                     RefRO<TargetPosition>,
                     WaypointMovementTag>())
        {       
            var vectorToTarget = target.ValueRO.Value - transform.WorldPosition;
            var direction = math.normalize(vectorToTarget);
            var look = new float3(direction.x, 0.0f, direction.y);
            transform.WorldRotation = quaternion.LookRotation(look, math.up());
            transform.WorldPosition += direction * speed.ValueRO.Value * dt;
        }
        */
        
        // entity job version
        state.Dependency = new MovementEntityJob { DT = dt }.ScheduleParallel(state.Dependency);
        
        // chunk job version
        //state.Dependency = new MovementChunkJob
        //{
        //    DT = dt,
        //    TransformHandle = state.GetComponentTypeHandle<LocalTransform>(),
        //    SpeedHandle = state.GetComponentTypeHandle<Speed>(true),
        //    TargetHandle = state.GetComponentTypeHandle<TargetPosition>(true),
        //}.ScheduleParallel(myQuery, state.Dependency);
    }
    
    [BurstCompile][WithNone(typeof(PassengerInfo))]
    partial struct MovementEntityJob : IJobEntity
    {
        public float DT;
        
        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref TransformAspect transform, in Speed speed, in TargetPosition target)
        {
            var vectorToTarget = target.Value - transform.WorldPosition;
            var direction = math.normalize(vectorToTarget);
            var look = new float3(direction.x, 0.0f, direction.y);
            transform.WorldRotation = quaternion.LookRotation(look, math.up());
            transform.WorldPosition += direction * speed.Value * DT;
        }
    }
    
    [WithAll(typeof(WaypointMovementTag))][WithNone(typeof(PassengerInfo))]
    [BurstCompile]
    public partial struct MovementChunkJob : IJobChunk
    {
        public float DT;
        
        public ComponentTypeHandle<LocalTransform> TransformHandle;
        [ReadOnly] public ComponentTypeHandle<Speed> SpeedHandle;
        [ReadOnly] public ComponentTypeHandle<TargetPosition> TargetHandle;

        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnableMask,
                in v128 chunkEnabledMask)
        {
            var transforms =
                chunk.GetNativeArray(ref TransformHandle);
            var speeds =
                    chunk.GetNativeArray(ref SpeedHandle);
            var targets =
                    chunk.GetNativeArray(ref TargetHandle);
            
            var enumerator = new ChunkEntityEnumerator(
                    useEnableMask,
                    chunkEnabledMask,
                    chunk.Count);
            
            while (enumerator.NextEntityIndex(out var i))
            {
                var transform = transforms[i];
                var speed = speeds[i];
                var target = targets[i];
                
                var vectorToTarget = target.Value - transform.Position;
                var direction = math.normalize(vectorToTarget);
                var look = new float3(direction.x, 0.0f, direction.y);
                transform.Rotation = quaternion.LookRotation(look, math.up());
                transform.Position += direction * speed.Value * DT;
                
                transforms[i] = transform;
            }
        }
    }
}