using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateDistanceFieldSystem))]
    public class UpdateParticleDistanceSystem : JobComponentSystem
    {
        EntityQuery m_DistanceFieldQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_DistanceFieldQuery = GetEntityQuery(
                ComponentType.ReadOnly<DistanceFieldComponent>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var distanceFieldModel = m_DistanceFieldQuery.GetSingleton<DistanceFieldComponent>();

            float time = .1f * Time.time;
            JobHandle handle;
            switch (distanceFieldModel.ModelType)
            {
                case DistanceFieldModel.SpherePlane:
                    handle = new UpdateParticlesSpherePlane
                    {
                        Time = time
                    }.Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.Metaballs:
                    handle = new UpdateParticlesMetaBalls
                    {
                        Time = time
                    }.Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.SpinMixer:
                    handle = new UpdateParticlesSpinMixer
                    {
                        Time = time
                    }.Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.SphereField:
                    handle = new UpdateParticlesSphereField
                    {
                        Time = time
                    }.Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.FigureEight:
                    handle = new UpdateParticlesFigureEight
                    {
                        Time = time
                    }.Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.PerlinNoise:
                    handle = new UpdateParticlesPerlinNoise().Schedule(this, inputDeps);
                    break;
                case DistanceFieldModel.Sphere:
                    handle = new UpdateParticlesSphere().Schedule(this, inputDeps);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return handle;
        }

        [BurstCompile]
        struct UpdateParticlesSpherePlane : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public float Time;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                const float sphereRadius = 5;
                float distanceToOrigin = math.length(position.Value);
                float sphereDist = distanceToOrigin - sphereRadius;
                var sphereNormal = position.Value / distanceToOrigin;

                float planeDist = position.Value.y;
                var planeNormal = new float3(0f, 1f, 0f);

                float t = math.sin(Time * 8) * .4f + .4f;
                distanceField.Distance = math.lerp(sphereDist, planeDist, t);
                distanceField.Normal = math.normalize(math.lerp(sphereNormal, planeNormal, t));
            }
        }

        [BurstCompile]
        struct UpdateParticlesSpinMixer : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public float Time;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                float distance = float.MaxValue;
                float3 normal = new float3(0, 0, 0);
                for (int i = 0; i < 6; i++)
                {
                    float orbitRadius = (i / 2 + 2) * 2;
                    float angle = Time * 20f * (1f + i * .1f);

                    float3 c = new float3(
                        math.cos(angle) * orbitRadius,
                        math.sin(angle),
                        math.sin(angle) * orbitRadius
                    );

                    const float sphereRadius = 2;
                    float3 delta = (position.Value - c);
                    float deltaLength = math.length(delta);
                    float newDist = deltaLength - sphereRadius;
                    if (newDist < distance)
                    {
                        normal = delta / deltaLength;
                        distance = newDist;
                    }
                }

                distanceField.Distance = distance;
                distanceField.Normal = normal;
            }
        }

        [BurstCompile]
        struct UpdateParticlesSphereField : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public float Time;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                float spacing = 5f + Mathf.Sin(Time * 5f) * 2f;
                float3 pos = position.Value;
                pos += spacing * .5f;
                pos -= spacing * (.5f + math.floor(pos / spacing));
                
                float distanceToOrigin = math.length(pos);
                const float sphereRadius = 5f;
                distanceField.Distance = distanceToOrigin - sphereRadius;
                distanceField.Normal = pos / distanceToOrigin;
            }
        }

        [BurstCompile]
        struct UpdateParticlesFigureEight : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public float Time;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                const float ringRadius = 4f;
                var pos = position.Value;
                float flipper = pos.z < 0f ? -1f : 1f;
                pos.z = math.abs(pos.z);

                float3 point = math.normalize(new float3(pos.x, 0f, pos.z - ringRadius)) * ringRadius;
                float angle = math.atan2(point.z, point.x) + Time * 8f;
                point += new float3(0, 0, ringRadius);

                float3 normal = pos - point;
                normal.z *= flipper;
                float normLength = math.length(normal); 
                
                distanceField.Normal = normal / normLength;
                
                float wave = math.cos(angle * flipper * 3f) * .5f + .5f;
                wave *= wave * .5f;
                distanceField.Distance = normLength - (.5f + wave);
            }
        }

        [BurstCompile]
        struct UpdateParticlesPerlinNoise : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                float perlin = noise.cnoise(position.Value.xz * .2f);
                distanceField.Distance = position.Value.y - perlin * 6f;
                distanceField.Normal = new float3(0, 1, 0);
            }
        }
        
        [BurstCompile]
        struct UpdateParticlesSphere : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                const float sphereRadius = 2;
                float distanceToOrigin = math.length(position.Value); 
                distanceField.Distance = distanceToOrigin - sphereRadius;
                distanceField.Normal = position.Value / distanceToOrigin;
            }
        }

        [BurstCompile]
        struct UpdateParticlesMetaBalls : IJobForEach<PositionComponent, PositionInDistanceFieldComponent>
        {
            public float Time;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                ref PositionInDistanceFieldComponent distanceField
            )
            {
                float distance = float.MaxValue;
                float3 normal = new float3(0,0,0);
                for (int i = 0; i < 5; i++)
                {
                    float orbitRadius = i * .5f + 2f;
                    float angle1 = Time * 4f * (1f + i * .1f);
                    float angle2 = Time * 4f * (1.2f + i * .117f);
                    float angle3 = Time * 4f * (1.3f + i * .1618f);
                    float3 c = orbitRadius * new float3(
                        math.cos(angle1),
                        math.sin(angle2),
                        math.sin(angle3)
                    );

                    float3 delta = position.Value - c;
                    float deltaLength = math.length(delta);

                    const float sphereRadius = 2;
                    float newDist = SmoothMin(distance, deltaLength - sphereRadius, 2f);
                    if (newDist < distance)
                    {
                        normal = delta / deltaLength;
                        distance = newDist;
                    }
                }

                distanceField.Distance = distance;
                distanceField.Normal = normal;
                
                float SmoothMin(float a, float b, float radius)
                {
                    float e = math.max(radius - math.abs(a - b), 0);
                    return math.min(a, b) - e * e * 0.25f / radius;
                }
                
            }
        }
    }
}
