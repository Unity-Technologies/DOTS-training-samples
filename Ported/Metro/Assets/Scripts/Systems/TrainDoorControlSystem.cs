using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TrainDoorControlSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        NativeList<RefRO<Train>> trains = new NativeList<RefRO<Train>>(Allocator.TempJob);
        foreach (RefRO<Train> train in SystemAPI.Query<RefRO<Train>>())
        {
            trains.Add(train);
        }

        new DoorJob()
        {
            trains = trains,
            ecbp = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct DoorJob : IJobEntity
{
    [ReadOnly] public NativeList<RefRO<Train>> trains;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([EntityIndexInQuery] int sortKey, RefRW<Carriage> carriage)
    {
        //Find the owner train
        RefRO<Train> myTrain = default;
        foreach (RefRO<Train> train in trains)
        {
            if (train.ValueRO.trainID == carriage.ValueRO.ownerTrainID)
            {
                myTrain = train;
            }
        }

        return;
        
        switch (myTrain.ValueRO.State)
        {
            //TODO workout the side of the doors
            case TrainState.DoorOpening:
                OpenDoors(sortKey, myTrain, carriage);
                break;
            case TrainState.DoorClosing:
                CloseDoors();
                break;
            default:
                return;
        }
    }

    private void OpenDoors(int sortKey, RefRO<Train> train, RefRW<Carriage> carriage)
    {
        //All door Entities
        for (int i = 0; i < carriage.ValueRW.LeftDoors.Length; i++)
        {
            //ecbp.AddComponent<LocalTransform>(0, door);
            //ecbp.SetComponentEnabled<>(sortKey, carriage.ValueRW.LeftDoors[i], false);
        }
    }

    private void CloseDoors()
    {
        
    }
}
