using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Random = UnityEngine.Random;

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
        NativeArray<int> randomFires = new NativeArray<int>(config.NumStartingFires, Allocator.Temp);
        for (int j = 0; j < config.NumStartingFires; j++) {
            // Could have collisions with other random rolls, but not worried about that here
            randomFires[j] = Random.Range(0, config.GridSize * config.GridSize);
        }

        foreach (var (trans, fire) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Fire>>()) {
            if (config.NumStartingFires > 0) {
                for (int j = 0; j < config.NumStartingFires; j++) {
                    // Create a fire, but not to the point where it's spreading yet
                    if (randomFires[j] == i) {
                        fire.ValueRW.t = Random.Range(math.EPSILON, config.FireSpreadValue - math.EPSILON);
                        break;
                    }
                }
            } else {
                fire.ValueRW.t = 0.0f;
            }

            trans.ValueRW.Scale = 1;
            trans.ValueRW.Position.x = pos.x + (i % config.GridSize) * 1.05f;
            trans.ValueRW.Position.y = pos.y - config.MinGridY;
            trans.ValueRW.Position.z = pos.z + (i / config.GridSize) * 1.05f;
            // TODO: enable entity visuals here after initial position is properly set

            i++;
        }
    }   
}