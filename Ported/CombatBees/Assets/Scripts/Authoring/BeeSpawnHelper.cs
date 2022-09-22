using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

static class BeeSpawnHelper
{
    public static Entity BeePrefab { get; set; }

    public static void SpawnBees(EntityCommandBuffer ecs, ref NativeArray<Entity> bees, BeeTeam team, float2 spawnPos)
    {
        ecs.Instantiate(BeePrefab, bees);
        foreach (var newBee in bees)
        {
            ecs.AddComponent(newBee, new BeePrototype { Hive = team, GroundSpawnPosition = spawnPos});
        }
    }

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