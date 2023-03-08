using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BucketSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            var rand = new Random(123);
            for (int i = 0; i < config.totalBuckets; i++)
            {
                var bucket = state.EntityManager.Instantiate(config.bucketPrefab);
                var x = rand.NextFloat(0f, config.simulationWidth);
                var z = rand.NextFloat(0f, config.simulationDepth);
                
                state.EntityManager.SetComponentData(bucket, 
                    LocalTransform.FromPosition(x, 0.5f, z));

                state.EntityManager.SetComponentData(bucket,
                   new URPMaterialPropertyBaseColor() { Value = config.bucketEmptyColor });

            }
        }
    }
}
