using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Collections; 

[BurstCompile]
public partial struct AntMovementSystem : ISystem
{
    private uint seed;

    public void OnCreate(ref SystemState state)
    {
        seed = (uint) System.DateTime.Now.Millisecond;

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<Resource>();
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

        AntMovementJob antMovementJob = new AntMovementJob
        {
            config = config,
            seed = seed,
            deltaTime = deltaTime
        };
        

        /*
        new UpdateMapJob
        {
            mapAspect = mapAspect
        }.Schedule();
        */

        foreach ((TransformAspect transformAspect, RefRO<Ant> ant) in SystemAPI.Query<TransformAspect, RefRO<Ant>>())
        {
            mapAspect.AddStrength((int) transformAspect.Position.x, (int) transformAspect.Position.y,
                ant.ValueRO.Speed);
        }

        // ants moving towards resources -- jobified 
        var query = SystemAPI.QueryBuilder()
            .WithAll<Wall, LocalToWorldTransform>()
            .Build();
        var resource = SystemAPI.GetSingletonEntity<Resource>();
        var resourceTransform = state.EntityManager.GetComponentData<LocalToWorldTransform>(resource);
        
        var wallTransforms = query.ToComponentDataArray<LocalToWorldTransform>(state.WorldUpdateAllocator);
        AntResourceSeekingJob antResourceJob = new AntResourceSeekingJob
        {
            config = config,
            resourceTransform = resourceTransform,
            wallTransforms = wallTransforms,
        };
        
        
        // scheduling jobs now 
        
        
        
        // both are parallel 
        antMovementJob.ScheduleParallel();
        antResourceJob.ScheduleParallel();
        
        /*
        // schedule single-threaded job 
        antMovementJob.ScheduleParallel();
        antResourceJob.Schedule();
        
        // running on the main thread 
        antMovementJob.ScheduleParallel();
        state.Dependency.Complete();
        antResourceJob.Run(); // run on main thread (NOT a job) 
        
        // another way to schedule
         JobHandle antMovementHandle = antMovementJob.ScheduleParallel(state.Dependency);
         JobHandle antResourceHandle = antResourceJob.Schedule(antMovementHandle);
         state.Dependency = antResourceHandle; // this line makes you not need to call Complete() 
        */ 
        
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
        Random rand = Random.CreateFromIndex((uint) (seed + entityIndex));
        var angle = (0.5f + noise.cnoise(transformAspect.Position / 10f)) * 4.0f * math.PI;
        angle += angle * rand.NextFloat(-180, 180);
        if (!config.AntRandomMovementActivated) angle = 0f; // no random ant movement 
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

[BurstCompile]
public partial struct AntResourceSeekingJob : IJobEntity
{
    public Config config;
    public LocalToWorldTransform resourceTransform; 
    [ReadOnly] public NativeArray<LocalToWorldTransform> wallTransforms;
    public void Execute(ref Ant ant, in TransformAspect antTransform)
    {

        // get direction from ant to resource (end - start) 
        float3 direction = resourceTransform.Value.Position - antTransform.Position;
        direction = math.normalize(direction);
        bool hasCollision = false;
        
        // loop through each wall 
        foreach (var wallTransform in wallTransforms)
        {
            // the wall is a sphere/circle. Does the direction RAY starting at antTransform.position intersect with the wall? 
            // ant ray --> center of wall sphere vector 
            float3 antToSphere = wallTransform.Value.Position - antTransform.Position;

            // find the point of the ray which is perpendicular to the center of the wall sphere:
            // p = origin + direction * t where t = dot(antToSphere, direction)
            float3 point = antTransform.Position + direction * math.dot(antToSphere, direction);

            // is this point within WallRadius of the WallSphere center? if YES then collision
            if (math.distance(point, wallTransform.Value.Position) <= config.WallRadius)
            {
                hasCollision = true;
                break; // TODO: should this be return? 
            }
        }
        
        // does ant have collision + no food? 
        if (!hasCollision && !ant.HasFood)
        {
            // update the angle based on the direction 

            // current direction details 
            var currDirection = float3.zero;
            var currAngleInRadians = math.radians(ant.Angle);
            currDirection.x = math.cos(currAngleInRadians);
            currDirection.y = math.sin(currAngleInRadians);

            // angle between currDirection and wanted direction 
            float angleToRotate = Angle(currDirection, direction);

            if (math.dot(currDirection, direction) < 0)
            {
                angleToRotate *= -1;
            }

            ant.Angle += angleToRotate;
        }
       
    }
    
    private float Angle(float3 from, Vector3 to)
    {
        float kEpsilonNormalSqrt = 1e-15F;

        // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
        float denominator = (float) math.sqrt(math.lengthsq(from) * math.lengthsq(to));
        if (denominator < kEpsilonNormalSqrt)
            return 0F;

        float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
        return math.degrees(math.acos(dot));
    }
}