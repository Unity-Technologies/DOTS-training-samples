using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{

  static float3 MapToRoundedCorners(float t, float radius)
    {
        float R = CarMovementSystem.RoundedCorner;
        float straight = 1.0f - 2.0f * R;
        float curved = (2.0f * math.PI * R) * 0.25f;
        float total = straight + curved;
        float tls = math.saturate(straight/total);
        float tlr = math.saturate(curved/total);

        int q = (int)(t * 4.0f);

        float x = 0;
        float y = 0;
        float a = 0;

        if(q == 0)
        {
            float n = t * 4.0f;
            x = R;
            y = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x -= math.cos(a) * R;
            y += math.sin(a) * R;
        }
        else if(q == 1)
        {
            float n = (t - 0.25f) * 4.0f;
            y = 1.0f - R;
            x = math.lerp(R, 1.0f - R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y += math.cos(a) * R;
            x += math.sin(a) * R;
            a += math.PI/2.0f;
        }
        else if(q == 2)
        {
            float n = (t - 0.5f) * 4.0f;
            x = 1.0f - R;
            y = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            x += math.cos(a) * R;
            y -= math.sin(a) * R;
            a -= math.PI;
        }
        else
        {
            float n = (t - 0.75f) * 4.0f;
            y = R;
            x = math.lerp(1.0f - R, R, math.saturate(n/tls));

            a = 0.5f * math.PI * math.saturate((n - tls)/tlr);
            y -= math.cos(a) * R;
            x -= math.sin(a) * R;
            a -= math.PI/2.0f;
        }

        x -= 0.5f;
        y -= 0.5f;
        x *= radius;
        y *= radius;
        return new float3(x,y,a);
    }

    private EntityQuery RequirePropagation;
    private TrackOccupancySystem m_TrackOccupancySystem;

    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);
        uint laneCount = m_TrackOccupancySystem.LaneCount;
        uint tilesPerLane = TrackOccupancySystem.TilesPerLane;

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                for (uint i = 0; i < tilesPerLane; ++i)
                {
                    var tile = ecb.Instantiate(spawner.TilePrefab);

                    float t = (float)i/(float)tilesPerLane;
                    float3 spawnPosition = MapToRoundedCorners(t, CarMovementSystem.TrackRadius);
                    var translation = new Translation {Value = new float3(spawnPosition.x, 0, spawnPosition.y)};
                    ecb.SetComponent(tile, translation);

                    ecb.SetComponent(tile, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(0.5f, 0.5f, 0.5f, 1.0f)
                    });

                    ecb.SetComponent(tile, new TileDebugColor
                    {
                        tileId = i
                    });

                }
            }).Run();

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (uint i = 0; i < spawner.CarCount; ++i)
                {
                    var vehicle = ecb.Instantiate(spawner.CarPrefab);
                    var translation = new Translation {Value = new float3(0, 0, 0)};
                    ecb.SetComponent(vehicle, translation);

                    ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
                    {
                        Value = random.NextFloat4()
                    });

                    ecb.SetComponent(vehicle, new CarMovement
                    {
                        Offset = (float)i / spawner.CarCount,
                        Lane = i % laneCount,
                        Velocity = random.NextFloat(0.035f, 0.075f),
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        
        // Propagate color from parent to child entities
        // We do this every frame because we change the color of the cars

        // A "ComponentDataFromEntity" allows random access to a component type from a job.
        // This much slower than accessing the components from the current entity via the
        // lambda parameters.
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel();
    }
}
