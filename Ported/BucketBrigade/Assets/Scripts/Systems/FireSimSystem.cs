using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct FireSimSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<GridTemperatures>();
        // m_ShootingLookup = state.GetComponentLookup<GridTemperatures>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    void DebugDisplayGrid(GridTemperatures grid, int sideSize)
    {
        string output = "";
        for (int i = 0; i < sideSize; i++)
        {
            for (int j = 0; j < sideSize; j++)
            {
                output += $"{grid.Get(i,j)},";
            }

            output += "\n";
        }
        Debug.Log(output);
    }

    public const float OnFireTemp = 0.2f;
    public const float k_MaxHeat = 1.0f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridTemperatures = SystemAPI.GetSingleton<GridTemperatures>();
        var config = SystemAPI.GetSingleton<Config>();
        var dt = SystemAPI.Time.DeltaTime;
        var currentTime = SystemAPI.Time.ElapsedTime;
        var heatTransferRate = config.heatTransferRate;

        // writes temperatures for fire spreading, increase temperature over time
        if (currentTime > gridTemperatures.NextGridUpdateTime)
        {
            gridTemperatures.NextGridUpdateTime = currentTime + heatTransferRate;
            var baseHeatIncreaseRate = config.baseHeatIncreaseRate;
            for (int j = 0; j < config.gridSize; j++)
            {
                for (int i = 0; i < config.gridSize; i++)
                {
                    var currentTemp = gridTemperatures.Get(i, j);
                    if (currentTemp > 0)
                    {
                        // each cell manage their own heat increase and don't try to increase heat for other cells (else you'd have redundant heat changes
                        // increase own heat. bonus to own heat if neighbours are on fire
                        // seed heat to neighbours if they are at 0
                        // heat between 0 and 1
                        float UpdateNeighbourAndGetBonus(int i, int j, float currentTemp)
                        {
                            if (i >= config.gridSize || j >= config.gridSize || i < 0 || j < 0) return 0f;
                            var neighbourTemp = gridTemperatures.Get(i, j);
                            if (neighbourTemp == 0 && currentTemp > OnFireTemp)
                            {
                                gridTemperatures.Set(i, j, 0.05f);
                                return 0f;
                            }
                            else
                            {
                                return neighbourTemp;
                            }
                        }

                        float bonus = 1f;
                        bonus += UpdateNeighbourAndGetBonus(i + 1, j, currentTemp);
                        bonus += UpdateNeighbourAndGetBonus(i - 1, j, currentTemp);
                        bonus += UpdateNeighbourAndGetBonus(i, j + 1, currentTemp);
                        bonus += UpdateNeighbourAndGetBonus(i, j - 1, currentTemp);
                        // bonus between 1 and 5

                        var ownHeatChange = dt * baseHeatIncreaseRate * bonus;
                        var newHeat = math.min(currentTemp + ownHeatChange, k_MaxHeat);
                        gridTemperatures.Set(i, j, newHeat);
                    }
                }
            }

            // DebugDisplayGrid(gridTemperatures, config.fireCellCount);
        }

        // writes result in DisplayHeight
        // set OnFireTag
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var fireCell in SystemAPI.Query<FireCellAspect>())
        {
            // Set Display height according to temperature. Flicker will be applied on transform
            var temp = gridTemperatures.Get(fireCell.CellInfo.ValueRO.indexX, fireCell.CellInfo.ValueRO.indexY);
            fireCell.DisplayHeight.ValueRW.height = temp;

            // Update OnFire tag
            if (temp >= OnFireTemp)
            {
                ecb.AddComponent<OnFireTag>(fireCell.Self);
            }
            else
            {
                ecb.RemoveComponent<OnFireTag>(fireCell.Self);
            }
        }
    }
}
