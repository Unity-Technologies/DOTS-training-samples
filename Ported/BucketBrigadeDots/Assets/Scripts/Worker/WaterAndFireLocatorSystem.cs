using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct WaterAndFireLocatorSystem : ISystem
{
    const float k_DefaultGridSize = 0.3f;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var gameSetting = SystemAPI.GetSingleton<GameSettings>();

            var random = Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime);
            foreach (var (teamData, teamState) in SystemAPI.Query<
                         RefRW<TeamData>, 
                         RefRW<TeamState>>())
            {
                teamData.ValueRW.WaterPosition = GetRandomWaterPosition(ref state, ref random);
                teamData.ValueRW.FirePosition = GetNearestFirePosition(ref teamData.ValueRW.WaterPosition);
                teamState.ValueRW.Value = TeamStates.Idle;
            }
        }
    }

    private float2 GetNearestFirePosition(ref float2 waterPos)
    {
        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        var cols = settings.RowsAndColumns;
        var size = settings.Size;
        
        var closestPos = new float2();
        var closestDist = float.MaxValue;
        
        for (var i = 0; i < size; i++)
        {
            if (temperatures[i] <= 0f) continue;
            
            var currentPos = new float2((i % cols) * k_DefaultGridSize, (i / cols) * k_DefaultGridSize);
            var dist = math.distancesq(waterPos, currentPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = currentPos;
            }
        }

        return closestPos;
    }

    float2 GetRandomWaterPosition(ref SystemState state, ref Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }
}