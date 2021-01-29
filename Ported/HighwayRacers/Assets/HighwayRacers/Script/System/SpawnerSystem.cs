using NUnit.Framework.Constraints;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public class SpawnerSystem : SystemBase
{
    public static readonly float MinimumVelocity = 0.035f;
    private EntityQuery RequirePropagation;
    private TrackOccupancySystem m_TrackOccupancySystem;
    private TrackSettings CurrentTrackSettings;
    private int CurrentSpawnCount;

    protected override void OnCreate()
    {
        m_TrackOccupancySystem = World.GetExistingSystem<TrackOccupancySystem>();
    }

    protected override void OnUpdate()
    {
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


        // The code below only runs when the UI updates


        if (UIValues.GetModified() == CurrentTrackSettings.Iteration && CurrentTrackSettings.Iteration > 0)
        {
            return;
        }

        CurrentTrackSettings = UIValues.GetTrackSettings();
        if (CurrentSpawnCount == CurrentTrackSettings.CarCount)
        {
            return;
        }


        var random = new Random(1234);
        uint laneCount = m_TrackOccupancySystem.LaneCount;
        uint tilesPerLane = TrackOccupancySystem.TilesPerLane;
        float minimumVelocity = MinimumVelocity;

        int carCount = CurrentTrackSettings.CarCount;

        var ecb = new EntityCommandBuffer(Allocator.Temp);


        Entities.ForEach((Entity ent, in PropagateColor c) => { ecb.DestroyEntity(ent); }
        ).Run();

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.


                for (uint i = 0; i < carCount; ++i)
                {
                    var vehicle = ecb.Instantiate(spawner.CarPrefab);
                    var translation = new Translation {Value = new float3(0, 0, 0)};
                    ecb.SetComponent(vehicle, translation);

                    ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
                    {
                        Value = random.NextFloat4()
                    });

                    // If the driver profile is set to random, pick one of
                    // the profiles for the new car.
                    DriverProfile profile = spawner.DriverProfile;
                    if (profile == DriverProfile.Random)
                    {
                        profile = random.NextBool() ? DriverProfile.American : DriverProfile.European;
                    }

                    uint currentLane = i % laneCount;
                    ecb.SetComponent(vehicle, new CarMovement
                    {
                        // todo here we ar enot smart enough. Two cars might end up in the same tile.
                        // This means they can drive through each other.
                        Offset = (float) i / spawner.CarCount,
                        Lane = currentLane,
                        LaneOffset = (float) currentLane,
                        Velocity = random.NextFloat(minimumVelocity, 0.075f),
                        LaneSwitchCounter = 0,
                        Profile = profile
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}