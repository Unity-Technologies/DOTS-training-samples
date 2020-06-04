using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class WaterPresentation : SystemBase
    {
        private float3 m_DefaultBucketScale;
        
        protected override void OnStartRunning()
        {
            var spawner = GetSingleton<SpawnerConfig>();
            m_DefaultBucketScale = GetComponent<NonUniformScale>(spawner.BucketPrefab).Value;
        }

        protected override void OnUpdate()
        {
            var defaultBucketScale = m_DefaultBucketScale;
            var config = GetSingleton<BucketBrigadeConfig>();
            Entities.WithAll<BucketTag>().ForEach((ref NonUniformScale scale, in WaterLevel waterLevel)
                =>
            {
                scale.Value = math.lerp(defaultBucketScale, defaultBucketScale*config.FullBucketScaleFactor, waterLevel.Level / waterLevel.Capacity);
            }).ScheduleParallel();
            
            Entities.WithAll<WaterSource>().ForEach((ref NonUniformScale scale, in InitialScale initialScale, in WaterLevel waterLevel)
                =>
            {
                scale.Value = math.lerp(float3.zero, initialScale.Value, waterLevel.Level / waterLevel.Capacity);
            }).ScheduleParallel();
        }
    }
}