using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Quaternion = UnityEngine.Quaternion;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    [UpdateAfter(typeof(BuildingSpawner))]
    [UpdateAfter(typeof(TornadoMover))]
    public partial class AnchorSimulator : SystemBase
    {
        private EntityCommandBufferSystem m_CommandBufferSystem;

        private Random m_RandomSeeds;
        
        protected override void OnCreate()
        {
            // Looking up another system in the world is an expensive operation.
            // In order to not do it every frame, we store the reference in a field.
            m_CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            m_RandomSeeds = new Random(1234);
        }
        
        protected override void OnUpdate()
        {
            // An ECB system will play back later in the frame the buffers it creates.
            // This is useful to defer the structural changes that would cause sync points.
            var ecb = m_CommandBufferSystem.CreateCommandBuffer();
            var parallelEcb = ecb.AsParallelWriter();
            
            Random random = new Random(m_RandomSeeds.NextUInt());
            var elapsedTime = Time.ElapsedTime;
            
            // Get the world and tornado configs
            var worldConfigEntity = GetSingletonEntity<WorldConfig>();
            var worldConfig = GetComponent<WorldConfig>(worldConfigEntity);
            var tornadoEntity = GetSingletonEntity<TornadoConfig>();
            var tornadoConfig = GetComponent<TornadoConfig>(tornadoEntity);
            var tornadoPosition = GetComponent<Translation>(tornadoEntity).Value;
            var tornadoFader = GetComponent<TornadoFader>(tornadoEntity).value;
            if (!tornadoConfig.simulate)
                return;

            // Get the beams and point lists for the solver
            NativeArray<AnchorPoint> anchorPoints = BuildingManager.AnchorPoints;
            NativeArray<Beam> beams = BuildingManager.Beams;

            Entities
                .WithNativeDisableParallelForRestriction(anchorPoints)
                .WithName("Simulation")
                .ForEach((ref Building buildingComponent) =>
                {
                    int anchorPointStart = buildingComponent.index.x;
                    int anchorPointCount = buildingComponent.index.y;
                    for (int i = 0; i < anchorPointCount; ++i)
                    {
                        float invDamping = 1f - worldConfig.damping;
                        AnchorPoint anchorPoint = anchorPoints[anchorPointStart + i];
                        if (!anchorPoint.fixedPoint)
                        {
                            float3 point = anchorPoint.position;
                            float3 start = point;
                            float3 old = anchorPoint.oldPosition;
                            old.y += .01f;

                            // tornado force
                            float2 tdXZ = new float2(
                                tornadoPosition.x + TornadoUtils.TornadoSway(point.y, (float)elapsedTime) - point.x,
                                tornadoPosition.z - point.z);
                            float tornadoDist = math.length(tdXZ);
                            tdXZ /= tornadoDist;
                            if (tornadoDist < tornadoConfig.maxForceDist)
                            {
                                float force = (1f - tornadoDist / tornadoConfig.maxForceDist);
                                float yFader = math.saturate(1f - point.y / tornadoConfig.height);
                                force *= tornadoFader * tornadoConfig.force * random.NextFloat(-.3f, 1.3f);
                                float3 directionalForce = new float3(
                                    -tdXZ.y + tdXZ.x * tornadoConfig.inwardForce * yFader,
                                    tornadoConfig.upForce,
                                    tdXZ.x + tdXZ.y * tornadoConfig.inwardForce * yFader);
                                old -= directionalForce * force;
                            }

                            point += (point - old) * invDamping;

                            old = start;
                            if (point.y < 0f)
                            {
                                point.y = 0f;
                                old.y = -old.y;
                                old.xz += (point.xz - old.xz) * worldConfig.friction;
                            }

                            anchorPoint.position = point;
                            anchorPoint.oldPosition = old;

                            anchorPoints[anchorPointStart + i] = anchorPoint;
                        }
                    }
                }).ScheduleParallel();

            Entities
                .WithNativeDisableParallelForRestriction(anchorPoints)
                .WithNativeDisableParallelForRestriction(beams)
                .WithName("SimulationBeams")
                .ForEach((ref Building buildingComponent) =>
                {
                    int anchorPointStart = buildingComponent.index.x;
                    int anchorPointCount = buildingComponent.index.y;
                    int beamStart = buildingComponent.index.z;
                    int beamCount = buildingComponent.index.w;
                    
                    for (int i = 0; i < beamCount; ++i)
                    {
                        int beamIndex = beamStart + i;
                        Beam beam = beams[beamIndex];
                        if (beam.fixedBeam)
                            continue;
                        
                        int p1Index = anchorPointStart + beam.points.x;
                        int p2Index = anchorPointStart + beam.points.y;
                        AnchorPoint p1 = anchorPoints[p1Index];
                        AnchorPoint p2 = anchorPoints[p2Index];
                        float3 oldPoint1 = p1.position;
                        float3 oldPoint2 = p2.position;
                        
                        float3 delta = p2.position - p1.position;
                        float dist = math.length(delta);
                        float extraDist = dist - beam.length;
                        float3 normalizedDelta = delta / dist;
                        float3 push = (normalizedDelta * extraDist) * .5f;

                        if (!p1.fixedPoint && !p2.fixedPoint)
                        {
                            p1.position += push;
                            p2.position -= push;
                        }
                        else if (p1.fixedPoint)
                        {
                            p2.position -= push * 2f;
                        }
                        else if (p2.fixedPoint)
                        {
                            p1.position += push * 2f;
                        }

                        beam.position = (p1.position + p2.position) * 0.5f;
                        if (math.dot(normalizedDelta, beam.oldDelta) < .99f)
                        {
                            beam.oldDelta = normalizedDelta;
                            beam.rotation = Quaternion.LookRotation(delta);
                        }

                        if (math.abs(extraDist) > worldConfig.breakResistance)
                        {
                            if (p2.neighbors > 1)
                            {
                                p2.neighbors--;

                                AnchorPoint newPoint = new AnchorPoint
                                {
                                    position = p2.position,
                                    oldPosition = p2.oldPosition,
                                    fixedPoint = false,
                                    neighbors = 1
                                };
                                int newIndex = anchorPointStart + anchorPointCount;
                                anchorPoints[newIndex] = newPoint;
                                beam.points.y = anchorPointCount;
                                anchorPointCount++;
                                buildingComponent.index.y = anchorPointCount;
                                
                                p2.position = oldPoint2;
                            }
                            else if (p1.neighbors > 1)
                            {
                                p1.neighbors--;

                                AnchorPoint newPoint = new AnchorPoint
                                {
                                    position = p1.position,
                                    oldPosition = p1.oldPosition,
                                    fixedPoint = false,
                                    neighbors = 1
                                };
                                int newIndex = anchorPointStart + anchorPointCount;
                                anchorPoints[newIndex] = newPoint;
                                beam.points.x = anchorPointCount;
                                anchorPointCount++;
                                buildingComponent.index.y = anchorPointCount;
                                
                                p1.position = oldPoint1;
                            }
                        }
                        beams[beamIndex] = beam;
                        anchorPoints[p1Index] = p1;
                        anchorPoints[p2Index] = p2;
                    }
                }).ScheduleParallel();

            Entities
                .WithName("BeamRenderUpdate")
                .WithNone<FixedBeamTag>() // Only update beams that can move ! 
                .WithNativeDisableParallelForRestriction(beams)
                .ForEach((ref Translation translation, ref Rotation rotation, in BeamComponent beamComponent) =>
                {
                    Beam beam = beams[beamComponent.beamIndex];
                    translation.Value = beam.position;
                    rotation.Value = beam.rotation;
                }).ScheduleParallel();
        }
    }
}