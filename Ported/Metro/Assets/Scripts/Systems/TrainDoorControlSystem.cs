using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
        new DoorJob().ScheduleParallel();
    }
}

[BurstCompile]
public partial struct DoorJob : IJobEntity
{
    public void Execute(RefRW<Carriage> carriage)
    {
        /*//switch (carriage.ValueRO.ownerTrain.State)
        {
            //TODO workout the side of the doors
            case TrainState.DoorOpening:
                Debug.Log("DOOR OPEN STATE");
                break;
            case TrainState.DoorClosing:
                CloseDoors();
                break;
            default:
                return;
        }*/
    }

    private void OpenDoors(RefRW<Carriage> carriage)
    {
    }

    private void CloseDoors()
    {
        
    }
}
