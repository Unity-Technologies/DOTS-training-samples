using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct ZombieMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        state.RequireForUpdate<Trajectory>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<GameConfig>();

        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        var grid = state.EntityManager.GetBuffer<GridCell>(gameConfigEntity);
        
        var gridArray = new NativeArray<byte>(grid.Length, Allocator.TempJob);

        for (int i = 0; i < grid.Length; i++)
        {
            gridArray[i] = grid[i].wallFlags;
        }

        var jobHandle = new MoveZombieJob()
        {
            grid = gridArray,
            gridSize = config.mazeSize
        }.ScheduleParallel(state.Dependency);
        
        jobHandle.Complete();

        gridArray.Dispose();
    }
}

[BurstCompile]
public partial struct MoveZombieJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<byte> grid;
    public int gridSize;
    
    public void Execute(DynamicBuffer<Trajectory> trajectory)
    {
        if(trajectory.Length < 1)
            return;

        NativeArray<float3> positionsPath = new NativeArray<float3>(trajectory.Length, Allocator.Temp); 
        NativeArray<int2> gridPath = new NativeArray<int2>(trajectory.Length, Allocator.Temp); 

        // Temporal debugging
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int index = x + y * gridSize;

                for (int i = 0; i < trajectory.Length; i++)
                {
                    if (index == trajectory[i])
                    {
                        positionsPath[i] = MazeUtils.GridPositionToWorld(x, y);
                        gridPath[i] = new int2(x, y);
                    }
                }
            }
        }

        for (int i = 0; i < positionsPath.Length-1; i++)
        {
            Debug.DrawLine(positionsPath[i], positionsPath[i+1]);
            //MazeUtils.DrawGridCell(gridPath[i], grid[trajectory[i]]);
        }

        positionsPath.Dispose();
        gridPath.Dispose();
        
        //
    }
}
