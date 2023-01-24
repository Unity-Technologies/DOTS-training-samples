using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using Unity.VisualScripting;

[BurstCompile]
public partial struct SiloSystem : ISystem
{
    ComponentLookup<WorldTransform> m_WorldTransformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_WorldTransformLookup = state.GetComponentLookup<WorldTransform>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookup.Update(ref state);

        float radius = 1f;
        const float debugRenderStepInDegrees = 20;
        // Debug rendering (the white circle).
        for (float angle = 0; angle < 360; angle += debugRenderStepInDegrees)
        {
            var a = new float3 (5f,5f,5f);
            var b = new float3(5f, 5f, 5f);
            math.sincos(math.radians(angle), out a.x, out a.z);
            math.sincos(math.radians(angle + debugRenderStepInDegrees), out b.x, out b.z);
            UnityEngine.Debug.DrawLine(a * radius, b * radius);
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var farmerSpawnJob = new SpawnWorker
        {
            WorldTransformLookup = m_WorldTransformLookup,
            ECB = ecb
        };

        farmerSpawnJob.Schedule();
    }

    [BurstCompile]
    partial struct SpawnWorker : IJobEntity
    {
        [ReadOnly] public ComponentLookup<WorldTransform> WorldTransformLookup;
        public EntityCommandBuffer ECB;

        void Execute(in SiloAspect silo)
        {
            var instance = ECB.Instantiate(silo.FarmerPrefab);
            var spawnLocalToWorld = WorldTransformLookup[silo.FarmerSpawn];
            var farmerTransform = LocalTransform.FromPosition(spawnLocalToWorld.Position);

            farmerTransform.Scale = WorldTransformLookup[silo.FarmerPrefab].Scale;
            ECB.SetComponent(instance, farmerTransform);
            ECB.SetComponent(instance, new Farmer
            {
                moveSpeed = 5.0f,
                moveTarget = new float3(15, 0, 0)
            });
        }
    }
}
