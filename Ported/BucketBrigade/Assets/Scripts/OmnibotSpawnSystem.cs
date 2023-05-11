using Miscellaneous.Execute;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// [UpdateBefore(typeof(TransformSystemGroup))]
public partial struct OmnibotSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OmnibotSpawner>();
        state.RequireForUpdate<Grid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        
        var config = SystemAPI.GetSingleton<Grid>();
        
        state.EntityManager.Instantiate(config.OmnibotPrefab, config.NumOmnibot, Allocator.Temp);
        foreach (var (trans, omnibot) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Omnibot>>()) {
            omnibot.ValueRW.t = UnityEngine.Random.value < 0.1f ? UnityEngine.Random.value : 0.0f;

            trans.ValueRW.Scale = 1;
            trans.ValueRW.Position.x = Random.Range(-30f,30f);
            trans.ValueRW.Position.y = 0;
            trans.ValueRW.Position.z = Random.Range(0,75f);
        }
    }
}