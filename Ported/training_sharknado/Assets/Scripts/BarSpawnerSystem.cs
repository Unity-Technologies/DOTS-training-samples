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

    // [ BurstCompile ]
    public struct SpawnJob : IJobForEachWithEntity< BarSpawner >
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        
        public void Execute( Entity entity, int jobIndex, [ ReadOnly ] ref BarSpawner spawner )
        {
            int numPoints = spawner.pointCountBuildings * 39 + spawner.pointCountDetails * 2;
            var points = new NativeArray< float3 >(numPoints, Allocator.Temp );
            var hasAnchors = new NativeArray< bool >(numPoints, Allocator.Temp );

            int pointIndex = 0;
            Random r = new Random(4);
            // HACK(wyatt): this is used to get bar rotation looking "correct"
            var left = math.normalize( new float3( 1, 0, 1 ) );
            float spacing = 2f;
            float3 point;

            for (int i = 0; i < spawner.pointCountBuildings; i++)
            {
                int height = r.NextInt(4, 12);
                // height = 1;

                float3 pos = new float3(r.NextFloat(-45f, 45f), 0f, r.NextFloat(-45f, 45f));

                point.x = pos.x + spacing;
                point.y = 0;
                point.z = pos.z - spacing;
                hasAnchors[pointIndex] = true;
                points[pointIndex] = point;
                pointIndex++;

                point.x = pos.x - spacing;
                point.y = 0;
                point.z = pos.z - spacing;
                hasAnchors[pointIndex] = true;
                points[pointIndex] = point;
                pointIndex++;

                point.x = pos.x + 0f;
                point.y = 0;
                point.z = pos.z + spacing;
                hasAnchors[pointIndex] = true;
                points[pointIndex] = point;
                pointIndex++;

                for (int j = 1; j <= height; j++)
                {
                    float offset = ( j % 2 ) * .1f;
                    
                    point.x = pos.x + spacing;
                    point.y = j * spacing;
                    point.z = pos.z - spacing;
                    points[pointIndex] = point;
                    pointIndex++;

                    point.x = pos.x - spacing;
                    point.y = j * spacing;
                    point.z = pos.z - spacing;
                    points[ pointIndex ] = point;
                    pointIndex++;

                    point.x = pos.x + 0f;
                    point.y = j * spacing;
                    point.z = pos.z + spacing;
                    points[ pointIndex ] = point;
                    pointIndex++;
                }
            }

            // ground details
            for (int i = 0; i < spawner.pointCountDetails; i++)
            {
                float3 pos = new float3( r.NextFloat( -55f, 55f ), 0f, r.NextFloat( -55f, 55f ) );
                point.x = pos.x + r.NextFloat(-.2f, -.1f);
                point.y = pos.y + r.NextFloat(0f, 3f);
                point.z = pos.z + r.NextFloat(.1f, .2f);
                points[pointIndex] = point;
                pointIndex++;

                point.x = pos.x + r.NextFloat(.2f, .1f);
                point.y = pos.y + r.NextFloat(0f, .2f);
                point.z = pos.z + r.NextFloat(-.1f, -.2f);
                points[pointIndex] = point;
                pointIndex++;

                if (r.NextFloat(1) < .1f)
                {
                    hasAnchors[ pointIndex - 1 ] = true;
                }
            }

            for (int i = 0; i < pointIndex; i++)
            {
                for (int j = i + 1; j < pointIndex; j++)
                {
                    BarPoint1 barpoint1;
                    BarPoint2 barpoint2;
                    BarLength barlength;
                    barlength.value = math.length( points[ i ] - points[ j ] );

                    if (barlength.value < 5f && barlength.value > .2f )
                    {
                        barpoint1.oldPos = points[i];
                        barpoint1.pos = points[i];
                        barpoint2.oldPos = points[j];
                        barpoint2.pos = points[j];
                        barpoint1.neighbors = 0;
                        barpoint2.neighbors = 0;
                        if (hasAnchors[i]) barpoint1.neighbors = -1;
                        if (hasAnchors[j]) barpoint2.neighbors = -1;

                        float3 diff = barpoint2.pos - barpoint1.pos;
                        var forward = math.normalizesafe( diff );
                        var rot = quaternion.LookRotationSafe( forward, math.cross( forward, left ) );

                        var instance = CommandBuffer.Instantiate( jobIndex, spawner.prefab);
                        CommandBuffer.RemoveComponent( jobIndex, instance, typeof( Scale ) );
                        CommandBuffer.AddComponent( jobIndex, instance, typeof( NonUniformScale ) );
                        CommandBuffer.SetComponent( jobIndex, instance, new NonUniformScale { Value = new float3( .1f, .1f, barlength.value ) } );
                        // CommandBuffer.SetComponent( jobIndex, instance, new Translation { Value = ( points[i] + points[j] ) / 2 });
                        // CommandBuffer.SetComponent( jobIndex, instance, new Rotation { Value = rot } );
                        
                        CommandBuffer.AddComponent( jobIndex, instance, barpoint1);
                        CommandBuffer.AddComponent(jobIndex, instance, barpoint2);
                        CommandBuffer.AddComponent(jobIndex, instance, barlength);
                    }
                }
            }

            CommandBuffer.RemoveComponent( jobIndex, entity, typeof( BarSpawner ) );
        }
    }

    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        var job = new SpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        }.Schedule( this, inputDeps );

        m_EntityCommandBufferSystem.AddJobHandleForProducer( job );
        return job;
    }
}
