
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

        public static void Instantiate(EntityCommandBuffer ecb, Entity prefab, float3 pos, int team, ref Random random)
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
                Size = random.NextFloat(0.25f, 0.5f)
            });
            ecb.AddComponent(bee, new Team { TeamId = (byte)team });
            ecb.AddSharedComponent(bee, new TeamShared { TeamId = (byte)team });
        }

        public static void Instantiate(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity prefab, float3 pos, int team, ref Random random)
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
                Size = random.NextFloat(0.25f, 0.5f)
            });
            ecb.AddComponent(sortKey, bee, new Team { TeamId = (byte)team });
            ecb.AddSharedComponent(sortKey, bee, new TeamShared { TeamId = (byte)team });
        }
    }

}