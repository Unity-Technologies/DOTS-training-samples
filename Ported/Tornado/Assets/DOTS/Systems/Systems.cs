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


                // Setup beams:
                var white = new float4(1f, 1f, 1f, 1f);
                var up = new float3(0f, 1f, 0f);
                for (int i = 0; i < pointCount; i++)
                {
                    for (int j = i + 1; j < pointCount; j++)
                    {
                        
                        var p1 = pointPosList[i];
                        var p2 = pointPosList[j];
                        var delta = p2 - p1;
                        var length = math.length(delta);

                        if (length < 5f && length > .2f)
                        {
                            var beam = EntityManager.Instantiate(spawner.beamPrefab);

                            EntityManager.AddComponentData(beam, new BeamData()
                            {
                                p1 = pointPosList[i],
                                p2 = pointPosList[j]
                            });

                            var pointDeltaNorm = math.normalize(delta);
                            var upDot = math.acos(math.abs(math.dot(up, pointDeltaNorm))) / Mathf.PI;
                            var color = white * upDot * random.NextFloat(.7f, 1f);
                            EntityManager.AddComponentData(beam, new URPMaterialPropertyBaseColor() { Value = color });


                            var thickness = random.NextFloat(.25f, .35f);
                            var pos = (p1 + p2) * 0.5f;
                            var rot = Quaternion.LookRotation(delta);
                            var scale = new float3(thickness, thickness, length);

                            EntityManager.AddComponentData(beam, new Unity.Transforms.Translation() { Value = pos });
                            EntityManager.AddComponentData(beam, new Unity.Transforms.Rotation() { Value = rot });
                            EntityManager.AddComponentData(beam, new Unity.Transforms.NonUniformScale() { Value = scale });
                        }

                            
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