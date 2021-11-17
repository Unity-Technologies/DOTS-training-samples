using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class BeamRendererSystem : SystemBase
    {
        NativeArray<Matrix4x4> transforms;

        protected override void OnCreate()
        {
            transforms = new NativeArray<Matrix4x4>(2000, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            Debug.Log("BeamRendererSystem.OnUpdate");
            var i = 0;
            Entities
            .WithoutBurst()
            .ForEach((in TransformMatrix t) =>
            {
                transforms[i++] = t.matrix;
            }).Run();
        }
    }

    public partial class BeamSpawner : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("BeamSpawner.OnCreate");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, in BeamSpawnerData spawner) =>
            {
                Debug.Log("BeamSpawner.OnUpdate");
                
                var random = new Random(1234);
                var pointPosList = new NativeArray<float3>(2000, Allocator.Temp);
                var pointCount = 0;
                for (int i = 0; i < spawner.buildingCount; i++)
                {
                    var height = random.NextInt(4, 12);
                    var pos = new float3(random.NextFloat(-45f, 45f), 0f, random.NextFloat(-45f, 45f));
                    float spacing = 2f;
                    for (int j = 0; j < height; j++)
                    {
                        var point = new float3();
                        point.x = pos.x + spacing;
                        point.y = j * spacing;
                        point.z = pos.z - spacing;
                        pointPosList[pointCount++] = point;

                        point = new float3();
                        point.x = pos.x - spacing;
                        point.y = j * spacing;
                        point.z = pos.z - spacing;
                        pointPosList[pointCount++] = point;

                        point = new float3();
                        point.x = pos.x + 0f;
                        point.y = j * spacing;
                        point.z = pos.z + spacing;
                        pointPosList[pointCount++] = point;
                    }
                }

                Debug.Log($"BeamSpawner has created {pointCount} points");
                for (var i = 0; i < pointCount; ++i)
                {
                    Debug.Log(pointPosList[i]);
                }

                // Setup beams:
                
                for (int i = 0; i < pointCount; i++)
                {
                    for (int j = i + 1; j < pointCount; j++)
                    {
                        // var beam = ecb.CreateEntity();
                        var beam = EntityManager.CreateEntity();
                        var p1 = pointPosList[i];
                        var p2 = pointPosList[j];

                        EntityManager.AddComponentData(beam, new BeamData()
                        {
                            p1 = pointPosList[i],
                            p2 = pointPosList[j]
                        });
                        
                        
                        var delta = p2 - p1;                        
                        var length = math.length(delta);                        
                        var thickness = random.NextFloat(.25f, .35f);
                        var pos = (p1 + p2) * 0.5f;                        
                        var rot = Quaternion.LookRotation(delta);                        
                        var scale = new float3(thickness, thickness, length);                        
                        var matrix = float4x4.TRS(pos, rot, scale);                        
                        EntityManager.AddComponentData(beam, new TransformMatrix()
                        {
                            matrix = matrix
                        });
                    }
                }
                // We only want to call this once. 
                Enabled = false;
                // Alternative to being called once...
                // EntityManager.DestroyEntity(entity);
                
            }).Run();
        }
    }
}