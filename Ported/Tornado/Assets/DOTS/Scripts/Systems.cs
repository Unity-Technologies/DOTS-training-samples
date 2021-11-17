using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BeamSpawner : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("BeamSpawner.OnCreate");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, in BeamSpawnerData spawner) =>
            {
                var random = new Random(1234);
                var pointPosList = new NativeList<float3>(2000, Allocator.Temp);
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
                        pointPosList.Add(point);

                        point = new float3();
                        point.x = pos.x - spacing;
                        point.y = j * spacing;
                        point.z = pos.z - spacing;
                        pointPosList.Add(point);

                        point = new float3();
                        point.x = pos.x + 0f;
                        point.y = j * spacing;
                        point.z = pos.z + spacing;
                        pointPosList.Add(point);
                    }
                }

                var pointCount = pointPosList.Length;
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

                            EntityManager.AddComponentData(beam, new Translation() { Value = pos });
                            EntityManager.AddComponentData(beam, new Rotation() { Value = rot });
                            EntityManager.AddComponentData(beam, new NonUniformScale() { Value = scale });
                        }
                    }
                }

            }).Run();
            Enabled = false;
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InitializationSystemGroup))]
    public partial class TornadoSpawner : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("TornadoSpawner.OnCreate");
        }

        protected override void OnUpdate()
        {
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity tornado, in TornadoConfigData tornadoConfig) =>
                {
                    // Debug.Log($"TornadoSpawner.OnUpdate");
                    EntityManager.AddComponentData(tornado, new Translation() { Value = tornadoConfig.position });

                    var random = new Random(5678);
                    // var white = new float4(1, 1, 1, 1);
                    var baseColor = new float4(1, 0, 0, 1);

                    for (var i = 0; i < tornadoConfig.particleCount; ++i)
                    {
                        var particle = EntityManager.Instantiate(tornadoConfig.particlePrefab);
                        EntityManager.AddComponent<ParticleTag>(particle);
                        var color = baseColor * random.NextFloat(.7f, 1f);
                        EntityManager.AddComponentData(particle, new URPMaterialPropertyBaseColor() { Value = color });

                        var pos = new float3(
                            random.NextFloat(-tornadoConfig.initRange, tornadoConfig.initRange),
                            random.NextFloat(0, tornadoConfig.height),
                            random.NextFloat(-tornadoConfig.initRange, tornadoConfig.initRange)
                            );
                        EntityManager.AddComponentData(particle, new Translation() { Value = pos });
                        EntityManager.AddComponentData(particle, new Rotation() { Value = new quaternion() });

                        var scale = new float3(1, 1, 1);
                        scale *= random.NextFloat(.3f,.7f);
                        EntityManager.AddComponentData(particle, new NonUniformScale() { Value = scale });
                    }

                    Enabled = false;
                }).Run();
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class MoveTornado : SystemBase
    {
        protected override void OnCreate()
        {

        }

        protected override void OnUpdate()
        {
            // Debug.Log("AnimateTornado.OnUpdate");
            Translation tornadoPosition = new Translation();
            Entities
                .ForEach((ref Translation pos, in Entity tornado, in TornadoConfigData tornadoConfig) =>
                {
                    pos.Value = new float3(pos.Value.x + 1, pos.Value.y + 1, pos.Value.z);
                    tornadoPosition = pos;
                }).Run();

            Entities
                .ForEach((ref Translation pos, in Entity particle, in ParticleTag tag) =>
            {
                
            }).ScheduleParallel();
        }
    }
}