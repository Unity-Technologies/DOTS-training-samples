using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FireSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Fire>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var config = SystemAPI.GetSingleton<Grid>();
        var targetQuery = SystemAPI.QueryBuilder().WithAll<Fire>().Build();
        float4 startingColor = config.StartingGridColor;
        float4 fullBurnColor = config.FullBurningGridColor;

        foreach (var (meshColor, transform, fire) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<LocalTransform>, RefRO<Fire>>()) {
            meshColor.ValueRW.Value = math.lerp(startingColor, fullBurnColor, fire.ValueRO.t);
            transform.ValueRW.Position.y = math.lerp(-2.0f, 2.0f, fire.ValueRO.t);
        }

        var job = new FireJob {
            neighoringFires = targetQuery.ToComponentDataArray<Fire>(state.WorldUpdateAllocator),
            rate = SystemAPI.Time.DeltaTime * config.FireGrowthRate,
            spreadVal = config.FireSpreadValue
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[WithAll(typeof(Fire))]
[BurstCompile]
public partial struct FireJob : IJobEntity {
    [ReadOnly] public NativeArray<Fire> neighoringFires;
    public float rate;
    public float spreadVal;

    void Execute([EntityIndexInQuery] int index, ref Fire fire) {
        //var allFires = chunk.GetNativeArray(ref FireHandle);
        
        if (fire.t > 0.0f) {
            fire.t = math.clamp(fire.t + rate, 0.0f, 1.0f);
		} else {
            // Look at all neighbors for flashpoint.  Watch for boundaries on grid
            bool flashpoint = false;
            if (index > 0 && neighoringFires[index - 1].t > spreadVal)
                flashpoint = true;
            else if (index < neighoringFires.Length - 1 && neighoringFires[index + 1].t > spreadVal)
                flashpoint = true;

            if (flashpoint)
                fire.t = math.EPSILON;
        }
    }
}