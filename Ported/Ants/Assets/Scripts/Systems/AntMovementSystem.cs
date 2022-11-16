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
        MapDataAspect mapAspect = default;
        foreach (MapDataAspect mapDataAspect in SystemAPI.Query<MapDataAspect>())
        {
            mapAspect = mapDataAspect;
            break;
        }
        
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach ((TransformAspect transformAspect, RefRO<Ant> ant) in SystemAPI.Query<TransformAspect,RefRO<Ant>>())
        {
            var dir = float3.zero;
            var angleInRadians = math.radians(ant.ValueRO.Angle);
            dir.x = math.cos(angleInRadians);
            dir.y = math.sin(angleInRadians);

            transformAspect.Position += dir * deltaTime * ant.ValueRO.Speed;
            transformAspect.Rotation = quaternion.RotateZ(angleInRadians);
            
            mapAspect.AddStrength((int)transformAspect.Position.x, (int)transformAspect.Position.y, ant.ValueRO.Speed);
        }
    }
}