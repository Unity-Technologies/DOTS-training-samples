
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Instantiation
{
    public static class Bee
    {
        static readonly float4 blueColor = new float4(0, 0, 1, 1);
        static readonly float4 yellowColor = new float4(1, 1, 0, 1);

        public static void Instantiate(EntityCommandBuffer ecb, Entity prefab, float3 pos, float size, int team)
        {
            var bee = ecb.Instantiate(prefab);
            ecb.SetComponent(bee, new Translation
            {
                Value = pos
            });
            ecb.SetComponent(bee, new URPMaterialPropertyBaseColor
            {
                Value = team == 0 ? blueColor : yellowColor
            });
            ecb.SetComponent(bee, new BeeMovement
            {
                Velocity = float3.zero,
                Size = size
            });
            ecb.AddSharedComponent(bee, new Team { TeamId = team });
        }

        public static void Instantiate(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity prefab, float3 pos, float size, int team)
        {
            var bee = ecb.Instantiate(sortKey, prefab);
            ecb.SetComponent(sortKey, bee, new Translation
            {
                Value = pos
            });
            ecb.SetComponent(sortKey, bee, new URPMaterialPropertyBaseColor
            {
                Value = team == 0 ? blueColor : yellowColor
            });
            ecb.SetComponent(sortKey, bee, new BeeMovement
            {
                Velocity = float3.zero,
                Size = size
            });
            ecb.AddSharedComponent(sortKey, bee, new Team { TeamId = team });
        }
    }

}