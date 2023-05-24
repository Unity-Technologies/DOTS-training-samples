using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
                teamData.ValueRW.FirePosition = random.NextFloat2(float2.zero, gameSetting.RowsAndColumns * k_DefaultGridSize);
                teamData.ValueRW.WaterPosition = GetRandomWaterPosition(ref state, random);
                teamState.ValueRW.Value = TeamStates.Idle;
            }
        }
    }

    float2 GetRandomWaterPosition(ref SystemState state, Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }
}