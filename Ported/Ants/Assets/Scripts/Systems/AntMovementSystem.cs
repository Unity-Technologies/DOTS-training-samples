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

        foreach ((TransformAspect transformAspect,  Ant ant) in SystemAPI.Query<TransformAspect,Ant>())
        {
            var dir = float3.zero;
            math.sincos(ant.Angle, out dir.x, out dir.z);
            transformAspect.Position += dir * deltaTime * ant.Speed;
            transformAspect.Rotation = quaternion.RotateZ(ant.Angle);
        }
    }
}