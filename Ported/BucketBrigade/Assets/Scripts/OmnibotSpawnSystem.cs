using Miscellaneous.Execute;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

// [UpdateBefore(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct OmnibotSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<OmnibotSpawner>();
        state.RequireForUpdate<Grid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        
        var config = SystemAPI.GetSingleton<Grid>();
        
        var startPoint = config.GridOrigin;
        var endPoint = new float3(startPoint.x + config.GridSize * 1.05f, 0, startPoint.z + config.GridSize * 1.05f);
        
        state.EntityManager.Instantiate(config.OmnibotPrefab, config.NumOmnibot, Allocator.Temp);
        foreach (var (trans, omnibot) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Omnibot>>()) {
            omnibot.ValueRW.t = UnityEngine.Random.value < 0.1f ? UnityEngine.Random.value : 0.0f;

            trans.ValueRW.Scale = config.BotScale;
            trans.ValueRW.Position.x = Random.Range(startPoint.x,endPoint.x);
            trans.ValueRW.Position.y = 0;
            trans.ValueRW.Position.z = Random.Range(startPoint.z,endPoint.z);
        }
    }
}