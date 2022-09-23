using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
static class BeeSpawnHelper
{
    public static Entity BeePrefab { get; set; }

    [BurstCompile]
    public static void SpawnBees(EntityCommandBuffer ecs, ref NativeArray<Entity> bees, BeeTeam team, float3 spawnPos)
    {
        ecs.Instantiate(BeePrefab, bees);
        foreach (var newBee in bees)
        {
            ecs.AddComponent(newBee, new BeePrototype { Hive = team, SpawnPosition = spawnPos});
        }
    }
    
    [BurstCompile]
    public static void SpawnBees(EntityCommandBuffer.ParallelWriter ecs, int sortKey, ref NativeArray<Entity> bees, BeeTeam team, float3 spawnPos)
    {
        ecs.Instantiate(sortKey, BeePrefab, bees);
        foreach (var newBee in bees)
        {
            ecs.AddComponent(sortKey, newBee, new BeePrototype { Hive = team, SpawnPosition = spawnPos});
        }
    }

    [BurstCompile]
    public static float4 GetTeamColor(BeeTeam hive)
    {
        switch (hive)
        {
            case BeeTeam.Blue:
                return new float4(0, 0, 1, 1);
            case BeeTeam.Yellow:
                return new float4(1, 1, 0, 1);
        }
        
        return new float4(1, 1, 1, 1); // white
    }
}