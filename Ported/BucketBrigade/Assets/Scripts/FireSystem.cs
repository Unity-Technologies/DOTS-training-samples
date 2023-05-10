using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FireSystem : ISystem
{
    public const float fireRate = 0.1f; // Needs to move to a config variable

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (burner, meshColor, transform, fire) in SystemAPI.Query<RefRO<Burner>, RefRW<URPMaterialPropertyBaseColor>, RefRW<LocalTransform>, RefRO<Fire>>().WithAll<Fire>()) {
            meshColor.ValueRW.Value = math.lerp(burner.ValueRO.startingColor, burner.ValueRO.fullBurningColor, fire.ValueRO.t);
            transform.ValueRW.Position.y = math.lerp(-2.0f, 2.0f, fire.ValueRO.t);
        }

        new FireJob {
            rate = SystemAPI.Time.DeltaTime * fireRate
        }.ScheduleParallel();
    }
}

[WithAll(typeof(Burner))]
[BurstCompile]
public partial struct FireJob : IJobEntity {
    public float rate;
    void Execute(ref Fire fire) {
        if (fire.t > 0.0f) {
            fire.t = math.clamp(fire.t + rate, 0.0f, 1.0f);
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