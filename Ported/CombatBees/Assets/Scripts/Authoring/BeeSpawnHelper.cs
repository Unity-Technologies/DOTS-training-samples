using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
static class BeeSpawnHelper {

    [BurstCompile]
    public static void SpawnBees(in Entity BeePrefab, ref EntityCommandBuffer ecs, ref NativeArray<Entity> bees, BeeTeam team, in float3 spawnPos)
    {
        ecs.Instantiate(BeePrefab, bees);
        foreach (var newBee in bees)
        {
            ecs.AddComponent(newBee, new BeePrototype { Hive = team, SpawnPosition = spawnPos});
        }
    }
    
    [BurstCompile]
    public static void SpawnBees(in Entity BeePrefab, ref EntityCommandBuffer.ParallelWriter ecs, int sortKey, ref NativeArray<Entity> bees, BeeTeam team, in float3 spawnPos)
    {
        ecs.Instantiate(sortKey, BeePrefab, bees);
        foreach (var newBee in bees)
        {
            ecs.AddComponent(sortKey, newBee, new BeePrototype { Hive = team, SpawnPosition = spawnPos});
        }
    }

    [BurstCompile]
    public static void GetTeamColor(BeeTeam hive, out float4 retVal)
    {
        switch (hive)
        {
            case BeeTeam.Blue:
                retVal = new float4(.3088235f, .4279919f, 1, 1);
                break;
            case BeeTeam.Yellow:
                retVal = new float4(.9436232f, .9485294f, .5928309f, 1);
                break;
            default:
                retVal = new float4(1, 1, 1, 1); // white
                break;
        }
    }
}