using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct WaterAndFireLocatorSystem : ISystem
{
    bool m_HasDiscardedFirstFrame;
    bool m_HasSetLocations;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Skipping first frame to let the transform system run once.
        if (!m_HasDiscardedFirstFrame)
        {
            m_HasDiscardedFirstFrame = true;
            return;
        }
        
        if (!m_HasSetLocations)
        {
            SetFireAndWaterLocations(ref state);
            m_HasSetLocations = true;
        }
        
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
            SetFireAndWaterLocations(ref state);
    }

    void SetFireAndWaterLocations(ref SystemState state)
    {
        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
        var waterQuery = SystemAPI.QueryBuilder().WithAll<WaterCell, LocalToWorld>().Build();
        var random = Random.CreateFromIndex((uint)(SystemAPI.Time.DeltaTime * 10000));
        foreach (var (teamData, teamState) in SystemAPI.Query<
                     RefRW<TeamData>, 
                     RefRW<TeamState>>())
        {
            var randomPos = random.NextFloat2(float2.zero, settings.RowsAndColumns * settings.DefaultGridSize);
            SystemUtilities.GetNearestWaterPosition(in randomPos, in settings, in waterQuery, out var waterPosition);
            teamData.ValueRW.WaterPosition = waterPosition;
                
            SystemUtilities.GetNearestFirePosition(in waterPosition, in settings, in temperatures, out var firePosition);
            teamData.ValueRW.FirePosition = firePosition;
                
            teamState.ValueRW.Value = TeamStates.Idle;
        }        
    }
}