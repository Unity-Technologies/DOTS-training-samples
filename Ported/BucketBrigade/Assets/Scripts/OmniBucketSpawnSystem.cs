using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(OmnibotSpawnerSystem))]
[BurstCompile]
public partial struct OmniBucketSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Grid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var config = SystemAPI.GetSingleton<Grid>();

        var omnibotsQuery = SystemAPI.QueryBuilder().WithAll<Omnibot>().Build();
        var omnibotEntityArray = omnibotsQuery.ToEntityArray(state.WorldUpdateAllocator);
        

        state.EntityManager.Instantiate(config.OmniBucketPrefab, omnibotEntityArray.Length, Allocator.Temp);
        var i = 0;
        foreach (var bucket in SystemAPI.Query<RefRW<OmniBucket>>())
        {
            // Assign OMniBot to Bucket
            bucket.ValueRW.TargetOmniBotEntity = omnibotEntityArray[i];

            i++;
        }
    }
}