using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct TrackGenerationSystem : ISystem
{
    public const float MID_RADIUS = 31.46f;
    public const float BASE_SCALE_Y = 6;

    private EntityQuery _entitiesToRemoveQuery;

    public bool NeedsRegenerating
    {
        get => _needsRegenerating;
        set => _needsRegenerating = value;
    }
    private bool _needsRegenerating;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TrackConfig>();
        NeedsRegenerating = true;

        _entitiesToRemoveQuery = state.GetEntityQuery(typeof(TrackSection));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (NeedsRegenerating)
        {
            // Create the new track sections
            float angle = 0.0f;
            float3 pos = new float3(0.0f, 0.0f, 0.0f);

            TrackSectionPrefabs prefabs = default;

            foreach (var trackGenerate in SystemAPI.Query<TrackGenerateAspect>())
            {
                prefabs = trackGenerate.Sections;
            }

            TrackConfig config = SystemAPI.GetSingleton<TrackConfig>();
            float straightPieceLength = TrackUtilities.GetStraightawayLength(config.highwaySize);
            float3 linearSectionScale = new float3(1.0f, 1.0f, straightPieceLength / BASE_SCALE_Y);

            for (int sectionIndex = 0; sectionIndex < 4; ++sectionIndex)
            {
                Entity linearSection = state.EntityManager.Instantiate(prefabs.LinearPrefab);
                state.EntityManager.SetComponentData<Translation>(linearSection, new Translation
                {
                    Value = pos
                });
                state.EntityManager.SetComponentData<Rotation>(linearSection, new Rotation
                {
                    Value = quaternion.RotateY(angle)
                });
                state.EntityManager.AddComponentData<NonUniformScale>(linearSection, new NonUniformScale
                {
                    Value = linearSectionScale
                });

                pos += math.mul(quaternion.RotateY(angle), new float3(0, 0, straightPieceLength));

                Entity curvedSection = state.EntityManager.Instantiate(prefabs.CurvedPrefab);
                state.EntityManager.SetComponentData<Translation>(curvedSection, new Translation
                {
                    Value = pos
                });
                state.EntityManager.SetComponentData<Rotation>(curvedSection, new Rotation
                {
                    Value = quaternion.RotateY(angle)
                });

                pos += math.mul(quaternion.RotateY(angle), new float3(MID_RADIUS, 0, MID_RADIUS));
                angle += math.PI / 2.0f;
            }

            NeedsRegenerating = false;
        }
    }

    public void RegenerateTrack(EntityManager entityManager)
    {
        // Remove the tracks that have already been created (if any)
        NativeArray<Entity> entitiesToRemove = _entitiesToRemoveQuery.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entitiesToRemove)
        {
            entityManager.DestroyEntity(entity);
        }
        entitiesToRemove.Dispose();

        NeedsRegenerating = true;
    }
}
