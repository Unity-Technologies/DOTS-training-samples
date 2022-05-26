using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


[BurstCompile]
public partial struct CarSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarConfigComponent>();
        state.RequireForUpdate<HighwayConfig>();
        state.RequireForUpdate<LaneTag>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var carConfig = SystemAPI.GetSingleton<CarConfigComponent>();
        var highwayConfig = SystemAPI.GetSingleton<HighwayConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var cars = CollectionHelper.CreateNativeArray<Entity>(carConfig.CarCount, allocator);
        ecb.Instantiate(carConfig.CarPrefab, cars);

        int maxNumCarsPerLane = highwayConfig.InsideLaneLength / (int)carConfig.CarLength;
        int[] carPerLane = new int[highwayConfig.LaneCount];

        int carId = 0;
        foreach (var car in cars)
        {
            int laneNum = Random.Range(1, highwayConfig.LaneCount);

            List<int> lanes = new List<int>();
            for (int i = 0; i < highwayConfig.LaneCount; i++)
            {
                lanes.Add(i);
            }

            while (lanes.Count > 0)
            {
                int index = Random.Range(0, lanes.Count);
                int lane = lanes[index];
                lanes.RemoveAt(index);

                if (carPerLane[lane] <= maxNumCarsPerLane)
                {
                    laneNum = lane;
                }
                else
                {
                    laneNum = -1;
                    break;
                }
            }

            if (laneNum != -1)
            {
                carPerLane[laneNum]++;

                float cruisingSpeed = Random.Range(carConfig.CruisingSpeedMin, carConfig.CruisingSpeedMax);
                float overtakeSpeed = Random.Range(carConfig.OvertakeSpeedMin, carConfig.OvertakeSpeedMax);
                float overtakeDistance = Random.Range(carConfig.OvertakeDistanceMin, carConfig.OvertakeDistanceMax);
                float overtakeTime = Random.Range(carConfig.OvertakeDurationMin, carConfig.OvertakeDurationMax);

                ecb.AddComponent<CarId>(car);
                ecb.SetComponent(car, new CarId { Value = carId++ });
                ecb.SetComponent(car, new CruisingSpeed { Value = cruisingSpeed });
                ecb.SetComponent(car, new OvertakeSpeed { Value = overtakeSpeed });
                ecb.SetComponent(car, new OvertakeDistance { Value = overtakeDistance });
                ecb.SetComponent(car, new OvertakeTime { Value = overtakeTime });
                ecb.SetComponent(car, new LaneComponent { LaneNumber = laneNum + 1 });
                ecb.SetComponent(car, new CurrentLaneComponent { CurrentLaneNumber = laneNum + 1});
                ecb.AddComponent(car, new CarTraveledDistance { Value = (carPerLane[laneNum]) * carConfig.CarLength});
            }
            else // there's no space for more cars
            {
                ecb.DestroyEntity(car);
            }
        }

        state.Enabled = false;
    }
}
