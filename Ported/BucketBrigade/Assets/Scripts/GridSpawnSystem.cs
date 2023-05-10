using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct GridSpawnSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Grid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Grid>();
        state.EntityManager.Instantiate(config.FirePrefab, config.GridSize * config.GridSize, Allocator.Temp);
        var pos = config.GridOrigin;
        int i = 0;
        int numRandomFires = config.NumStartingFires;

        foreach (var (trans, fire) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Fire>>()) {
            if (numRandomFires > 0) {
                if (UnityEngine.Random.value < 0.01f) {
                    // Create a fire, but not to the point where it's spreading yet
                    fire.ValueRW.t = UnityEngine.Random.Range(math.EPSILON, config.FireSpreadValue - math.EPSILON);
                    numRandomFires--;
                }
            } else {
                fire.ValueRW.t = 0.0f;
			}

            trans.ValueRW.Scale = 1;
            trans.ValueRW.Position.x = pos.x + (i % config.GridSize) * 1.05f;
            trans.ValueRW.Position.y = pos.y - config.MinGridY;
            trans.ValueRW.Position.z = pos.z + (i / config.GridSize) * 1.05f;
            i++;
        }
    }
}