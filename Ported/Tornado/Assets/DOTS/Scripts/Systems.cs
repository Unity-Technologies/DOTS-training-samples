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
                            EntityManager.AddComponents(beam, new ComponentTypes(
                                new ComponentType(typeof(BeamData)),
                                new ComponentType(typeof(URPMaterialPropertyBaseColor)),
                                new ComponentType(typeof(Translation)),
                                new ComponentType(typeof(Rotation)),
                                new ComponentType(typeof(NonUniformScale))
                            ));

                            EntityManager.SetComponentData(beam, new BeamData()
                            {
                                p1 = pointPosList[i],
                                p2 = pointPosList[j]
                            });

                            var pointDeltaNorm = math.normalize(delta);
                            var upDot = math.acos(math.abs(math.dot(up, pointDeltaNorm))) / Mathf.PI;
                            var color = white * upDot * random.NextFloat(.7f, 1f);
                            EntityManager.SetComponentData(beam, new URPMaterialPropertyBaseColor() { Value = color });

                            var thickness = random.NextFloat(.25f, .35f);
                            var pos = (p1 + p2) * 0.5f;
                            var rot = Quaternion.LookRotation(delta);
                            var scale = new float3(thickness, thickness, length);

                            EntityManager.SetComponentData(beam, new Translation() { Value = pos });
                            EntityManager.SetComponentData(beam, new Rotation() { Value = rot });
                            EntityManager.SetComponentData(beam, new NonUniformScale() { Value = scale });
                        }
                    }
                }

            }).Run();
            Enabled = false;
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
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
                .ForEach((Entity tornado, ref TornadoConfigData tornadoConfig) =>
                {
                    // Debug.Log($"TornadoSpawner.OnUpdate");
                    EntityManager.AddComponentData(tornado, new Translation() { Value = tornadoConfig.position });

                    var random = new Random(5678);
                    // var white = new float4(1, 1, 1, 1);
                    var baseColor = new float4(1, 0, 0, 1);

                    for (var i = 0; i < tornadoConfig.particleCount; ++i)
                    {
                        var particle = EntityManager.Instantiate(tornadoConfig.particlePrefab);
                        EntityManager.AddComponents( particle, new ComponentTypes(
                            new ComponentType(typeof(Particle)), 
                            new ComponentType(typeof(URPMaterialPropertyBaseColor)),
                            new ComponentType(typeof(Translation)),
                            new ComponentType(typeof(Rotation)),
                            new ComponentType(typeof(NonUniformScale))
                            ));

                        EntityManager.SetComponentData(particle, new Particle() { radiusMult = random.NextFloat(0, 1) });
                        var color = baseColor * random.NextFloat(.7f, 1f);
                        EntityManager.SetComponentData(particle, new URPMaterialPropertyBaseColor() { Value = color });

                        var pos = new float3(
                            random.NextFloat(-tornadoConfig.initRange, tornadoConfig.initRange),
                            random.NextFloat(0, tornadoConfig.height),
                            random.NextFloat(-tornadoConfig.initRange, tornadoConfig.initRange)
                            );
                        EntityManager.SetComponentData(particle, new Translation() { Value = pos });
                        EntityManager.SetComponentData(particle, new Rotation() { Value = new quaternion() });

                        var scale = new float3(1, 1, 1);
                        scale *= random.NextFloat(.3f,.7f);
                        EntityManager.SetComponentData(particle, new NonUniformScale() { Value = scale });

                        if (tornadoConfig.rotationModulation == 0f)
                            tornadoConfig.rotationModulation = random.NextFloat(-1, 1) * tornadoConfig.maxForceDist;
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

        public static float TornadoSway(float y, float elapsedTime)
        {
            return Mathf.Sin(y / 5f + elapsedTime / 4f) * 3f;
        }

        protected override void OnUpdate()
        {
            Translation tp = new Translation();
            TornadoConfigData tc = new TornadoConfigData();
            var elapsedTime = (float)Time.ElapsedTime;
            Entities
                .ForEach((ref Translation pos, in Entity tornado, in TornadoConfigData tornadoConfig) =>
                {
                    var tmod = elapsedTime / 6f;
                    pos.Value = new float3(
                        tornadoConfig.position.x + math.cos(tmod) * tornadoConfig.rotationModulation,
                        tornadoConfig.position.y,
                        tornadoConfig.position.z + math.sin(tmod * 1.618f) * tornadoConfig.rotationModulation);

                    tp.Value = pos.Value;
                    tc = tornadoConfig;
                }).Run();

            var deltaTime = Time.DeltaTime;
            Entities
                .ForEach((ref Translation pos, in Particle particle) =>
            {
                var point = pos.Value;
                var tornadoPos = new float3(
                    tp.Value.x + TornadoSway(point.y, elapsedTime), 
                    point.y, 
                    tp.Value.z);
                var delta = tornadoPos - point;
                var dist = math.length(delta);
                float inForce = dist - math.clamp(tornadoPos.y / tc.height, 0f, 1f) * tc.maxForceDist * particle.radiusMult + 2f;

                delta /= dist;
                point += new float3(
                    -delta.z * tc.spinRate + delta.x * inForce, 
                    tc.upwardSpeed, 
                    delta.x * tc.spinRate + delta.z * inForce) * deltaTime;
                if (point.y > tc.height)
                {
                    point = new float3(point.x, 0f, point.z);
                }
                pos.Value = point;
            }).ScheduleParallel();
        }
    }
}