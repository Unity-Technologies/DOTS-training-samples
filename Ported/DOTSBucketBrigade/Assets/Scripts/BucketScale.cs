using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class BucketScale : SystemBase
    {
        private float3 m_DefaultScale;
        
        protected override void OnStartRunning()
        {
            var spawner = GetSingleton<SpawnerConfig>();
            m_DefaultScale = GetComponent<NonUniformScale>(spawner.BucketPrefab).Value;
        }

        protected override void OnUpdate()
        {
            var defaultScale = m_DefaultScale;
            var config = GetSingleton<BucketBrigadeConfig>();
            Entities.WithAll<BucketTag>().ForEach((ref NonUniformScale scale, in WaterLevel waterLevel)
                =>
            {
                scale.Value = math.lerp(defaultScale, defaultScale*config.FullBucketScaleFactor, waterLevel.Level / waterLevel.Capacity);
            }).ScheduleParallel();
        }
    }
}