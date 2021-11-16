using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = Unity.Mathematics.Random;

namespace Dots
{
    public partial class AnchorSimulator : SystemBase
    {
        private EntityQuery TornadoQuery;
        private EntityQuery AnchorPointQuery;
        private EntityQuery BeamQuery;
        private EntityCommandBufferSystem CommandBufferSystem;

        private Random randomSeeds;

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
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            randomSeeds = new Random(1234);
        }
        
        public static float TornadoSway(float y, float time)
        {
            return math.sin(y / 5f + time / 4f) * 3f;
        }
        
        protected override void OnUpdate()
        {
            // An ECB system will play back later in the frame the buffers it creates.
            // This is useful to defer the structural changes that would cause sync points.
            var ecb = CommandBufferSystem.CreateCommandBuffer();
            var parallelEcb = ecb.AsParallelWriter();
            
            Random random = new Random(randomSeeds.NextUInt());
            var time = Time.ElapsedTime;

            if (TornadoQuery.IsEmpty)
                return;
            int tornadoCount = TornadoQuery.CalculateEntityCount();
            var tornadoInfos = new NativeArray<TornadoInfo>(tornadoCount, Allocator.TempJob);

            Entities.WithoutBurst()
                .WithStoreEntityQueryInField(ref TornadoQuery)
                .ForEach((int entityInQueryIndex, ref TornadoFader fade, in TornadoConfig config, in Translation translation) =>
                {

                    fade.value = math.saturate(fade.value + (float)time / 10f);
                    tornadoInfos[entityInQueryIndex] = new TornadoInfo
                        {
                            position = translation.Value,
                            fade = fade.value,
                            config = config
                        };
                }).ScheduleParallel();

            var worldConfigEntity = GetSingletonEntity<WorldConfig>();
            var worldConfig = GetComponent<WorldConfig>(worldConfigEntity);
            var anchorPointMap = new NativeHashMap<Entity, PointNeighbor>(AnchorPointQuery.CalculateEntityCount(),Allocator.TempJob);
            Entities.WithoutBurst()
                .WithReadOnly(tornadoInfos)
                .WithDisposeOnCompletion(tornadoInfos)
                .WithStoreEntityQueryInField(ref AnchorPointQuery)
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
                        bool isFixedAnchor = HasComponent<FixedAnchor>(entity);
                        if (!isFixedAnchor)
                        {
                            float3 start = point.value;
                            point.old.y += .01f;

                            // tornado force
                            float2 tdXZ = new float2(
                                tornadoPos.x + TornadoSway(point.value.y, (float)time) - point.value.x, 
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
            
            Entities.WithoutBurst()
                .WithStoreEntityQueryInField(ref BeamQuery)
                .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref Beam beam, in Entity entity, in Length length) =>
                {
                    bool point1IsFixedAnchor = HasComponent<FixedAnchor>(beam.p1);
                    bool point2IsFixedAnchor = HasComponent<FixedAnchor>(beam.p2);

                    PointNeighbor p1 = anchorPointMap[beam.p1];
                    PointNeighbor p2 = anchorPointMap[beam.p2];
                    Point point1 = p1.point;
                    Point point2 = p2.point;

                    float3 delta = point2.value - point1.value;
                    float dist = math.length(delta);
                    float3 normalizedDelta = delta / dist;
                    float extraDist = dist - length.value;
                    float3 push = (normalizedDelta * extraDist) * .5f;

                    if (!point1IsFixedAnchor && !point2IsFixedAnchor)
                    {
                        point1.value += push;
                        point2.value -= push;
                    }
                    else if (point1IsFixedAnchor)
                    {
                        point2.value -= push * 2f;
                    }
                    else if (point2IsFixedAnchor)
                    {
                        point1.value += push * 2f;
                    }

                    translation.Value = point1.value + point2.value * 0.5f;
                    
                    if (math.dot(normalizedDelta, beam.oldDelta) < .99f)
                    {
                        rotation.Value = Quaternion.LookRotation(delta);
                        beam.oldDelta = normalizedDelta;
                    }

                    if (math.abs(extraDist) > worldConfig.breakResistance)
                    {
                        if (p2.neighborCount > 1)
                        {
                            p2.neighborCount--;
                            anchorPointMap[beam.p2] = p2;
                            
                            // Queue point creation and update in the ECB
                            var pointEntity = ecb.CreateEntity();
                            ecb.AddComponent(pointEntity, new Anchor { NeighborCount = 1});
                            ecb.AddComponent(pointEntity, point2);
                            
                            //we can't update the Beam point right now since the Entity is not fully created yet
                            ecb.SetComponent(entity, new Beam
                            {
                                p1 = beam.p1, 
                                p2 = pointEntity, 
                                oldDelta = beam.oldDelta
                            });
                        }
                        else if (p1.neighborCount > 1)
                        {
                            p1.neighborCount--;
                            anchorPointMap[beam.p1] = p1;
                            
                            // Queue point creation and update in the ECB
                            var pointEntity = ecb.CreateEntity();
                            ecb.AddComponent(pointEntity, new Anchor { NeighborCount = 1});
                            ecb.AddComponent(pointEntity, point1);
                            
                            //we can't update the Beam point right now since the Entity is not fully created yet
                            ecb.SetComponent(entity, new Beam
                            {
                                p1 = pointEntity, 
                                p2 = beam.p2, 
                                oldDelta = beam.oldDelta
                            });
                        }
                    }

                    p1.point = point1;
                    p2.point = point2;
                    
                    anchorPointMap[beam.p1] = p1;
                    anchorPointMap[beam.p2] = p2;
                }).Schedule();
            CommandBufferSystem.AddJobHandleForProducer(this.Dependency);
            
            // Now that everything is simulated, update the points position and neighborCount
            Entities.WithoutBurst()
                .WithReadOnly(anchorPointMap)
                .WithDisposeOnCompletion(anchorPointMap)
                .ForEach((int entityInQueryIndex, ref Anchor anchorPoint, ref Point point, in Entity entity) =>
                {
                    var anchorData = anchorPointMap[entity];
                    parallelEcb.SetComponent(entityInQueryIndex, entity, new Anchor { NeighborCount = anchorData.neighborCount});
                    parallelEcb.SetComponent(entityInQueryIndex, entity, anchorData.point);
                }).ScheduleParallel();
            CommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}