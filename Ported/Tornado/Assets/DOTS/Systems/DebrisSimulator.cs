using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Dots
{
    [UpdateAfter(typeof(TornadoMover))]
    public partial class DebrisSimulator : SystemBase
    {
        private EntityQuery m_TornadoQuery;
        
        struct TornadoInfo
        {
            public Entity tornado;
            public float3 position;
            public TornadoConfig config;
        }

        
        protected override void OnCreate()
        {
        }
        
        protected override void OnUpdate()
        {
            var elapsedTime = Time.ElapsedTime;
            var deltaTime = Time.DeltaTime;
            
            if (m_TornadoQuery.IsEmpty)
                return;

            int tornadoCount = m_TornadoQuery.CalculateEntityCount();
            var tornadoInfos = new NativeArray<TornadoInfo>(tornadoCount, Allocator.TempJob);

            Entities
                .WithStoreEntityQueryInField(ref m_TornadoQuery)
                .ForEach((int entityInQueryIndex, in Entity entity, in TornadoConfig config, in Translation translation) =>
                {
                    tornadoInfos[entityInQueryIndex] = new TornadoInfo
                    {
                        tornado = entity,
                        position = translation.Value,
                        config = config
                    };
                }).Run();
            
            foreach (var tornadoData in tornadoInfos)
            {
                TornadoConfig config = tornadoData.config;
                float3 tornadoEntityPosition = tornadoData.position; 

                Entities
                    .WithName("DebrisSimulator")
                    .WithSharedComponentFilter(new DebrisSharedData { tornado = tornadoData.tornado })
                    .ForEach((ref Translation translation, in Debris debris) =>
                    {
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
                            delta.x * config.spinRate + delta.z * inForce) * deltaTime;
                    
                        if (translation.Value.y>config.height) 
                            translation.Value.y = 0f;
                    }).ScheduleParallel();
            }

            tornadoInfos.Dispose();
        }
    }
}