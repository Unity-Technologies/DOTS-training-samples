using System.Diagnostics;
using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct PlatformSpawnSystem : ISystem
{
    private EntityQuery m_BaseColorQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var track in SystemAPI.Query<TrackAspect>())
        {
            var platforms = track.Platforms.AsNativeArray();

            for (int i = 0; i < platforms.Length; i++)
            {
                var rotation = Quaternion.LookRotation(math.normalize(platforms[i].endWorldPosition - platforms[i].startWorldPosition)) * Quaternion.Euler(0f,90f,0f);

                var platform = ecb.Instantiate(config.PlatformPrefab);
                ecb.SetComponent(platform, new Translation { Value = (platforms[i].startWorldPosition + platforms[i].endWorldPosition)*0.5f });
                ecb.SetComponent(platform, new Rotation { Value = rotation });
                SetColor(platform, ecb, track.BaseColor.ValueRO);
            }
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }

    private void SetColor(Entity entity, EntityCommandBuffer ecb, URPMaterialPropertyBaseColor color)
    {
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        ecb.SetComponentForLinkedEntityGroup(entity, queryMask, color);
    }
}