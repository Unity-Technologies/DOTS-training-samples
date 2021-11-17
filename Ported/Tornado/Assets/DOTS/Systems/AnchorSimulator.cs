using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class AnchorSimulator : SystemBase
    {
        private EntityQuery m_TornadoQuery;
        private EntityQuery m_AnchorPointQuery;
        private EntityQuery m_BeamQuery;
        private EntityCommandBufferSystem m_CommandBufferSystem;

        private Random m_RandomSeeds;

        struct PointNeighbor
        {
            public Point point;
            public int neighborCount;
        }
        struct TornadoInfo
        {
            public float3 position;
            public float fade;
            public TornadoConfig config;
        }
        
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
            //var parallelEcb = ecb.AsParallelWriter();
            
            Random random = new Random(m_RandomSeeds.NextUInt());
            var elapsedTime = Time.ElapsedTime;
            var deltaTime = Time.DeltaTime;

            if (m_TornadoQuery.IsEmpty)
                return;
            
            int tornadoCount = m_TornadoQuery.CalculateEntityCount();
            var tornadoInfos = new NativeArray<TornadoInfo>(tornadoCount, Allocator.TempJob);

            Entities
                .WithStoreEntityQueryInField(ref m_TornadoQuery)
                .ForEach((int entityInQueryIndex, ref TornadoFader fade, in TornadoConfig config, in Translation translation) =>
                {
                    fade.value = math.saturate(fade.value + deltaTime / 10f);
                    tornadoInfos[entityInQueryIndex] = new TornadoInfo
                        {
                            position = translation.Value,
                            fade = fade.value,
                            config = config
                        };
                }).ScheduleParallel();

            var worldConfigEntity = GetSingletonEntity<WorldConfig>();
            var worldConfig = GetComponent<WorldConfig>(worldConfigEntity);
            var anchorPointMap = new NativeHashMap<Entity, PointNeighbor>(m_AnchorPointQuery.CalculateEntityCount(),Allocator.TempJob);
            Entities
                .WithReadOnly(tornadoInfos)
                .WithDisposeOnCompletion(tornadoInfos)
                .WithStoreEntityQueryInField(ref m_AnchorPointQuery)
                .ForEach((int entityInQueryIndex, in Point inputPoint, in Entity entity, in Anchor anchorPoint) =>
                {
                    Point point = inputPoint;
                    for (int tornadoId = 0; tornadoId < tornadoCount; ++tornadoId)
                    {
                        var tornado = tornadoInfos[tornadoId].config;
                        if (!tornado.simulate)
                            continue;
                        
                        float tornadoFader = tornadoInfos[tornadoId].fade; 
                        var tornadoPos = tornadoInfos[tornadoId].position;

                        float invDamping = 1f - worldConfig.damping;
                        bool isFixedAnchor = HasComponent<FixedAnchorTag>(entity);
                        if (!isFixedAnchor)
                        {
                            float3 start = point.value;
                            point.old.y += .01f;

                            // tornado force
                            float2 tdXZ = new float2(
                                tornadoPos.x + TornadoUtils.TornadoSway(point.value.y, (float)elapsedTime) - point.value.x, 
                                tornadoPos.z - point.value.z);
                            float tornadoDist = math.length(tdXZ);
                            tdXZ /= tornadoDist;
                            if (tornadoDist < tornado.maxForceDist)
                            {
                                float force = (1f - tornadoDist / tornado.maxForceDist);
                                float yFader = math.saturate(1f - point.value.y / tornado.height);
                                force *= tornadoFader * tornado.force * random.NextFloat(-.3f, 1.3f);
                                float3 directionalForce = new float3(
                                    -tdXZ.y + tdXZ.x * tornado.inwardForce * yFader,
                                    tornado.upForce, 
                                    tdXZ.x + tdXZ.y * tornado.inwardForce * yFader);
                                point.old -= directionalForce * force;
                            }

                            point.value += (point.value - point.old) * invDamping;

                            point.old = start;
                            if (point.value.y < 0f)
                            {
                                point.value.y = 0f;
                                point.old.y = -point.old.y;
                                point.old.xz += (point.value.xz - point.old.xz) * worldConfig.friction;
                            }
                        }
                    }

                    anchorPointMap[entity] = new PointNeighbor { point = point, neighborCount = anchorPoint.NeighborCount };
                }).Schedule();
            
            Entities
                .WithStoreEntityQueryInField(ref m_BeamQuery)
                .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref Beam beam, in Entity beamEntity, in Length length) =>
                {
                    bool point1IsFixedAnchor = HasComponent<FixedAnchorTag>(beam.p1);
                    bool point2IsFixedAnchor = HasComponent<FixedAnchorTag>(beam.p2);

                    PointNeighbor p1 = anchorPointMap[beam.p1];
                    PointNeighbor p2 = anchorPointMap[beam.p2];
                    Point oldP1 = p1.point;
                    Point oldP2 = p2.point;

                    float3 delta = p2.point.value - p1.point.value;
                    float  dist = math.length(delta);
                    float  extraDist = dist - length.value;
                    float3 normalizedDelta = delta / dist;
                    float3 push = (normalizedDelta * extraDist) * .5f;

                    if (!point1IsFixedAnchor && !point2IsFixedAnchor)
                    {
                        p1.point.value += push;
                        p2.point.value -= push;
                    }
                    else if (point1IsFixedAnchor)
                    {
                        p2.point.value -= push * 2f;
                    }
                    else if (point2IsFixedAnchor)
                    {
                        p1.point.value += push * 2f;
                    }

                    if (math.dot(normalizedDelta, beam.oldDelta) < .99f)
                    {
                        rotation.Value = Quaternion.LookRotation(delta);
                        beam.oldDelta = normalizedDelta;
                    }
                    translation.Value = p1.point.value + p2.point.value * 0.5f;

                    if (math.abs(extraDist) > worldConfig.breakResistance)
                    {
                        if (p2.neighborCount > 1)
                        {
                            p2.point = oldP2;
                            p2.neighborCount--;
                            
                            // Queue a new point creation that duplicate P2 and update it on the beam
                            var pointEntity = ecb.CreateEntity();
                            ecb.AddComponent(pointEntity, new Anchor { NeighborCount = 1});
                            ecb.AddComponent(pointEntity, p2.point);
                            
                            // We can't update the Beam point right now since the Entity is not fully created yet
                            ecb.SetComponent(beamEntity, new Beam
                            {
                                p1 = beam.p1, 
                                p2 = pointEntity, 
                                oldDelta = beam.oldDelta
                            });
                        }
                        else if (p1.neighborCount > 1)
                        {
                            p1.point = oldP1;
                            p1.neighborCount--;
                            
                            // Queue a new point creation that duplicate P2 and update it on the beam
                            var pointEntity = ecb.CreateEntity();
                            ecb.AddComponent(pointEntity, new Anchor { NeighborCount = 1});
                            ecb.AddComponent(pointEntity, p1.point);
                            
                            // We can't update the Beam point right now since the Entity is not fully created yet
                            ecb.SetComponent(beamEntity, new Beam
                            {
                                p1 = pointEntity, 
                                p2 = beam.p2, 
                                oldDelta = beam.oldDelta
                            });
                        }
                    }
                    
                    anchorPointMap[beam.p1] = p1;
                    anchorPointMap[beam.p2] = p2;
                }).Schedule();
            m_CommandBufferSystem.AddJobHandleForProducer(this.Dependency);
            
            // Now that everything is simulated, update the AnchorPoint's position and neighborCount
            Entities
                .WithReadOnly(anchorPointMap)
                .WithDisposeOnCompletion(anchorPointMap)
                .ForEach((int entityInQueryIndex, ref Anchor anchorPoint, ref Point point, in Entity entity) =>
                {
                    var anchorData = anchorPointMap[entity];
                    anchorPoint.NeighborCount = anchorData.neighborCount;
                    point.value = anchorData.point.value;
                    point.old = anchorData.point.old;
                }).ScheduleParallel();
        }
    }
}