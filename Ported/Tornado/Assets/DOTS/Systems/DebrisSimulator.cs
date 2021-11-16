using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Dots
{
    public partial class DebrisSimulator : SystemBase
    {
        private EntityQuery TornadoQuery;
        private Random randomSeeds;
        
        struct TornadoInfo
        {
            public float3 position;
            public TornadoConfig config;
        }
        
        protected override void OnCreate()
        {
            randomSeeds = new Random(1234);
        }
        
        protected override void OnUpdate()
        {
            Random random = new Random(randomSeeds.NextUInt());
            var elapsedTime = Time.ElapsedTime;
            var deltaTime = Time.DeltaTime;
            
            if (TornadoQuery.IsEmpty)
                return;
            int tornadoCount = TornadoQuery.CalculateEntityCount();
            var tornadoInfos = new NativeHashMap<Entity, TornadoInfo>(tornadoCount, Allocator.TempJob);

            Entities
                .WithStoreEntityQueryInField(ref TornadoQuery)
                .ForEach((in Entity entity, in TornadoConfig config, in Translation translation) =>
                {
                    tornadoInfos[entity] = new TornadoInfo
                    {
                        position = translation.Value,
                        config = config
                    };
                }).Schedule();
            
            Entities
                .WithReadOnly(tornadoInfos)
                .WithDisposeOnCompletion(tornadoInfos)
                .ForEach((ref Translation translation, in DebrisTag debris) =>
                {
                    if (!tornadoInfos.ContainsKey(debris.tornado))
                    {
                        return;
                    }

                    TornadoConfig config = tornadoInfos[debris.tornado].config;
                    float3 tornadoEntityPosition = tornadoInfos[debris.tornado].position; 
                    
                    var tornadoPos = new float3(
                        tornadoEntityPosition.x + TornadoUtils.TornadoSway(translation.Value.y, (float)elapsedTime), 
                        translation.Value.y, 
                        tornadoEntityPosition.z);
                    
                    var delta = tornadoPos - translation.Value;
                    var dist = math.length(delta);
                    float inForce = dist - math.saturate(tornadoPos.y / config.height) * config.maxForceDist * debris.radiusMult + 2f;

                    delta /= dist;
                    translation.Value += new float3(-delta.z * config.spinRate + delta.x * inForce, 
                        config.upwardSpeed, 
                        delta.x * config.spinRate + delta.z * inForce) * (float)deltaTime;
                    
                    if (translation.Value.y>config.height) 
                        translation.Value.y = 0f;
                    
                }).ScheduleParallel();
        }
    }
}