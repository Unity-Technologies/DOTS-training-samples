using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawningSystem : ISystem
{
    private uint m_UpdateCounter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireSpawner>();
        state.RequireForUpdate<TeamSpawner>();
        state.RequireForUpdate<GameSettings>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnFireCells(ref state);
        SpawnTeams(ref state);
    }
    
    void SpawnFireCells(ref SystemState state)
    {
        var fireCellsQuery = SystemAPI.QueryBuilder().WithAll<FireCell>().Build();
        if (fireCellsQuery.IsEmpty)
        {
            var gameSetting = SystemAPI.GetSingleton<GameSettings>();
            var fireSpawner = SystemAPI.GetSingleton<FireSpawner>();
            var prefab = fireSpawner.Prefab;
            var size = gameSetting.Rows * gameSetting.Columns; 
            
            var instances = state.EntityManager.Instantiate(prefab, size, Allocator.Temp);

            var index = 0;
            for (var x = 0; x < gameSetting.Rows; x++)
            {
                for (var y = 0; y < gameSetting.Columns; y++)
                {
                    var entity = instances[index++];
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    transform.ValueRW.Position = new float3(x * .3f, 0f, y * .3f);

                }
            }
        }
    }

    void SpawnTeams(ref SystemState state)
    {
        var teamsQuery = SystemAPI.QueryBuilder().WithAll<Team>().Build();
        if (teamsQuery.IsEmpty)
        {
            var teamSpawner = SystemAPI.GetSingleton<TeamSpawner>();
            for (var i = 0; i < teamSpawner.NumberOfTeams; ++i)
            {
                var teamEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(teamEntity, new Team());
                
                var workerBuffer = state.EntityManager.AddBuffer<TeamMembers>(teamEntity);

                var prefab = teamSpawner.WorkerPrefab;
                var instances = state.EntityManager.Instantiate(prefab, teamSpawner.WorkersPerTeam, Allocator.Temp);
                
                for (var j = 0; j < teamSpawner.WorkersPerTeam; ++j)
                {
                    var workerEntity = instances[j];
                    workerBuffer.Add(new TeamMembers { Worker = workerEntity });
                }
            }
        }
    }
}