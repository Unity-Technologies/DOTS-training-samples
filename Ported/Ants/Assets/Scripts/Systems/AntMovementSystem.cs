using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct AntMovementSystem : ISystem
{
    
    private uint seed;
    
    public void OnCreate(ref SystemState state)
    {
        seed = (uint)System.DateTime.Now.Millisecond;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //seed++;
        var config = SystemAPI.GetSingleton<Config>();
        //SystemAPI.GetSingletonEntity<MapData>()
        MapDataAspect mapAspect = default;
        foreach (MapDataAspect mapDataAspect in SystemAPI.Query<MapDataAspect>())
        {
            mapAspect = mapDataAspect;
            break;
        }
        
        var deltaTime = SystemAPI.Time.DeltaTime;

        new AntMovementJob
        {
            config = config,
            seed = seed,
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency).Complete();

        /*
        new UpdateMapJob
        {
            mapAspect = mapAspect
        }.Schedule();
        */
        
        
        foreach ((TransformAspect transformAspect, RefRO<Ant> ant) in SystemAPI.Query<TransformAspect,RefRO<Ant>>())
        {
            mapAspect.AddStrength((int)transformAspect.Position.x, (int)transformAspect.Position.y, ant.ValueRO.Speed);
        }
        
    }
}

[BurstCompile]
public partial struct AntMovementJob : IJobEntity
{
    public uint seed;
    public Config config;
    public float deltaTime;
    
    public void Execute([EntityInQueryIndex] int entityIndex, TransformAspect transformAspect, in Ant ant)
    {
        var dir = float3.zero;
        Random rand = Random.CreateFromIndex((uint)(seed + entityIndex)); 
        var angle = (0.5f + noise.cnoise(transformAspect.Position / 10f)) * 4.0f * math.PI;
        angle += angle * rand.NextFloat(-180, 180);
        var angleInRadians = math.radians(ant.Angle + angle);

        dir.x += math.cos(angleInRadians + config.ResourcePoint.x);
        dir.y += math.sin(angleInRadians + config.ResourcePoint.y);

        transformAspect.Position += dir * deltaTime * ant.Speed;
        transformAspect.Rotation = quaternion.RotateZ(angleInRadians);
    }
}

/*
[BurstCompile]
public partial struct UpdateMapJob : IJobEntity
{
    public MapDataAspect mapAspect;
    public void Execute(TransformAspect transformAspect, in Ant ant)
    {
        mapAspect.AddStrength((int)transformAspect.Position.x, (int)transformAspect.Position.y, ant.Speed);
    }
}
*/