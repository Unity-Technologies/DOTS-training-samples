using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct AntMovementSystem : ISystem
{
    
    private uint time;
    
    public void OnCreate(ref SystemState state)
    {
        time = (uint)System.DateTime.Now.Millisecond;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var config = SystemAPI.GetSingleton<Config>();
        
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
            Random rand = Random.CreateFromIndex(time); // todo: figure out how to make this a random number 
            var angle = (0.5f + noise.cnoise(transformAspect.Position / 10f)) * 4.0f * math.PI;
            angle += angle * rand.NextFloat(-180, 180);
            var angleInRadians = math.radians(ant.ValueRO.Angle + angle);//ant.ValueRO.Angle

            var resourceDir = float3.zero;
            
            // realtive
            var relative = 
            
            var lookat = quaternion.LookRotationSafe(transformAspect.Position, )

            dir.x += math.cos(angleInRadians);// + rand.NextFloat(1f, 5f));
            dir.y += math.sin(angleInRadians + );

            transformAspect.Position += dir * deltaTime * ant.ValueRO.Speed;
            transformAspect.Rotation = quaternion.RotateZ(angleInRadians);
            
            mapAspect.AddStrength((int)transformAspect.Position.x, (int)transformAspect.Position.y, ant.ValueRO.Speed);
        }
    }
}