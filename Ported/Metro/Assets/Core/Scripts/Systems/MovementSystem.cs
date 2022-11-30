using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct MovementSystem : ISystem
{
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
        foreach (var (transform, /*position, direction,*/ speed, target) in 
                 SystemAPI.Query<
                     TransformAspect,
                     //Direction, 
                     
                     //Position,
                     RefRO<Speed>,
                     RefRO<TargetPosition>> ())
        {
            var dt = SystemAPI.Time.DeltaTime;
            var vectorToTarget = target.ValueRO.Value - transform.WorldPosition.xz;
            

            //if (math.lengthsq(vectorToTarget) > Utility.kStopDistance)
            {
                var direction = math.normalize(vectorToTarget);
                
                var look = new float3(direction.x, 0.0f, direction.y);
                transform.WorldRotation = quaternion.LookRotation(look, math.up());
                transform.WorldPosition += look * speed.ValueRO.Value * dt;
            }
        }
    }
}