using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class BarSpawnerSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }
    public struct SpawnJob : IJob
    {
        public Entity entity;
        public BarSpawner spawner;
        public EntityCommandBuffer CommandBuffer;
        [DeallocateOnJobCompletion]
        public NativeArray<float3> points;
        [DeallocateOnJobCompletion]
        public NativeArray<bool> hasAnchors;
        public void Execute()
        {
            int pointIndex = 0;
            Random r = new Random(4);
            for (int i = 0; i < 35; i++)
            {
                int height =  r.NextInt(4, 12);
                Vector3 pos = new Vector3(r.NextFloat(-45f, 45f), 0f, r.NextFloat(-45f, 45f));
                float spacing = 2f;
                for (int j = 0; j < height; j++)
                {
                    float3 point = points[pointIndex];
                    point.x = pos.x + spacing;
                    point.y = j * spacing;
                    point.z = pos.z - spacing;
                    if (j == 0)
                    {
                        hasAnchors[pointIndex] = true;
                    }
                    points[pointIndex] = point;
                    pointIndex++; 
                    point = points[pointIndex];
                    point.x = pos.x - spacing;
                    point.y = j * spacing;
                    point.z = pos.z - spacing;
                    if (j == 0)
                    {
                        hasAnchors[pointIndex] = true;
                    }
                    points[pointIndex] = point;
                    pointIndex++;
                    point = points[pointIndex];
                    point.x = pos.x + 0f;
                    point.y = j * spacing;
                    point.z = pos.z + spacing;
                    if (j == 0)
                    {
                        hasAnchors[pointIndex] = true;
                    }
                    points[pointIndex] = point;
                    pointIndex++;
                }
            }

            // ground details
            for (int i = 0; i < 300; i++)
            {
                Vector3 pos = new Vector3(r.NextFloat(-55f, 55f), 0f, r.NextFloat(-55f, 55f));
                float3 point = points[pointIndex];
                point.x = pos.x + r.NextFloat(-.2f, -.1f);
                point.y = pos.y + r.NextFloat(0f, 3f);
                point.z = pos.z + r.NextFloat(.1f, .2f);
                points[pointIndex] = point;
                pointIndex++;

                point = points[pointIndex];
                point.x = pos.x + r.NextFloat(.2f, .1f);
                point.y = pos.y + r.NextFloat(0f, .2f);
                point.z = pos.z + r.NextFloat(-.1f, -.2f);
                if (r.NextFloat(1) < .1f)
                {
                    hasAnchors[pointIndex] = true;
                }
                points[pointIndex] = point;
                pointIndex++;
            }
            for (int i = 0; i < pointIndex; i++)
            {
                for (int j = i + 1; j < pointIndex; j++)
                {
                    Bar bar;
                    bar.length = math.length(points[i] - points[j]);
                    if (bar.length > 5f || bar.length > .2f) continue;
                    bar.oldPos1 = points[i];
                    bar.pos1 = points[i];
                    bar.oldPos2 = points[j];
                    bar.pos2 = points[j];
                    bar.neighbors1 = 0;
                    bar.neighbors2 = 0;
                    
                    var instance = CommandBuffer.Instantiate(spawner.prefab);
                    CommandBuffer.SetComponent(instance, new Translation { Value = (points[i] + points[j]) / 2 });
                    CommandBuffer.AddComponent(instance, bar);
                }
            }
            //CommandBuffer.DestroyEntity(entity);
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new SpawnJob
        {
            spawner = GetSingleton<BarSpawner>(),
            //entity = ???
            points = new NativeArray<float3>(4000, Allocator.TempJob),
            hasAnchors = new NativeArray<bool>(4000, Allocator.TempJob),
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
        }.Schedule(inputDeps);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        return job;
    }
}
