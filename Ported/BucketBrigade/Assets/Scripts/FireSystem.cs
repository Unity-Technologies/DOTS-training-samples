using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FireSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (burner, meshColor, transform) in SystemAPI.Query<RefRO<Burner>, RefRW<URPMaterialPropertyBaseColor>, RefRW<LocalTransform>>().WithAll<Fire>()) {
            float4 color = meshColor.ValueRW.Value;
            float heatScale = UnityEngine.Random.value;
            color = math.lerp(burner.ValueRO.startingColor, burner.ValueRO.fullBurningColor, heatScale);
            meshColor.ValueRW.Value = color;
            transform.ValueRW.Position.y = math.lerp(-2.0f, 2.0f, heatScale);
        }
    }
}

/*
[WithAll(typeof(Burner))]
[BurstCompile]
partial struct FireJob : IJobParallel {
    [ReadOnly] public EntityTypeHandle EntityTypeHandle;
    [ReadOnly] public ComponentTypeHandle<Burner> BurnerHandle;
    public ComponentTypeHandle<URPMaterialPropertyBaseColor> BaseColorTypeHandle;

    void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask) {
        var burners = chunk.GetNativeArray(ref BurnerHandle);
        var entities = chunk.GetNativeArray(EntityTypeHandle);
        var baseColors = chunk.GetNativeArray(ref BaseColorTypeHandle);

        for (int i = 0; i < burners.Length; i++) {
            float4 color = baseColors[i].Value;
            color = math.lerp(burners[i].startingColor, burners[i].fullBurningColor, UnityEngine.Random.value);
            baseColors[i] = new URPMaterialPropertyBaseColor { Value = color };
        }
    }
}
*/