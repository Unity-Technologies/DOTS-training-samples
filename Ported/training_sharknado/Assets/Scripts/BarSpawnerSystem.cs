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

    [ BurstCompile ]
    public struct SpawnJob : IJobForEachWithEntity< BarSpawner >
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        
        public void Execute( Entity entity, int jobIndex, [ ReadOnly ] ref BarSpawner spawner )
        {
            var points = new NativeArray< float3 >( 4000, Allocator.Temp );
            var hasAnchors = new NativeArray< bool >( 4000, Allocator.Temp );

            int pointIndex = 0;
            Random r = new Random(4);
            // HACK(wyatt): this is used to get bar rotation looking "correct"
            var left = math.normalize( new float3( 1, 0, 1 ) );
            float spacing = 2f;
            float3 point;

            for (int i = 0; i < 35; i++)
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
            for (int i = 0; i < 300; i++)
            {
                point = new float3( r.NextFloat( -55f, 55f ), 0f, r.NextFloat( -55f, 55f ) );
                point.x = point.x + r.NextFloat(-.2f, -.1f);
                point.y = point.y + r.NextFloat(0f, 3f);
                point.z = point.z + r.NextFloat(.1f, .2f);
                points[pointIndex] = point;
                pointIndex++;

                point.x = point.x + r.NextFloat(.2f, .1f);
                point.y = point.y + r.NextFloat(0f, .2f);
                point.z = point.z + r.NextFloat(-.1f, -.2f);
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
                    bar.length = math.length( points[ i ] - points[ j ] );

                    if ( bar.length < 5f && bar.length > .2f )
                    {
                        bar.oldPos1 = points[i];
                        bar.pos1 = points[i];
                        bar.oldPos2 = points[j];
                        bar.pos2 = points[j];
                        bar.neighbors1 = 0;
                        bar.neighbors2 = 0;

                        float3 diff = bar.pos2 - bar.pos1;
                        var forward = math.normalizesafe( diff );
                        var rot = quaternion.LookRotationSafe( forward, math.cross( forward, left ) );

                        var instance = CommandBuffer.Instantiate( jobIndex, spawner.prefab);
                        CommandBuffer.RemoveComponent( jobIndex, instance, typeof( Scale ) );
                        CommandBuffer.AddComponent( jobIndex, instance, bar );
                        CommandBuffer.AddComponent( jobIndex, instance, typeof( NonUniformScale ) );
                        CommandBuffer.SetComponent( jobIndex, instance, new NonUniformScale { Value = new float3( .1f, .1f, bar.length ) } );
                        CommandBuffer.SetComponent( jobIndex, instance, new Translation { Value = ( points[i] + points[j] ) / 2 });
                        CommandBuffer.SetComponent( jobIndex, instance, new Rotation { Value = rot } );
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
