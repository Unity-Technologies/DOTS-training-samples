using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
public partial struct AntMovementSystem : ISystem
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

        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach ((TransformAspect transformAspect, RefRO<Ant> ant) in SystemAPI.Query<TransformAspect,RefRO<Ant>>())
        {
            var dir = float3.zero;
            math.sincos(ant.ValueRO.Angle, out dir.x, out dir.y);
            transformAspect.Position += dir * deltaTime * ant.ValueRO.Speed;
            transformAspect.Rotation = quaternion.RotateY(ant.ValueRO.Angle);
        }
    }
}