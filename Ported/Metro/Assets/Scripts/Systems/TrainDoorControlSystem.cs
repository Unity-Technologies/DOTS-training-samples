using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TrainDoorControlSystem : ISystem
{
    private ComponentLookup<LocalTransform> worldTransforms;
    private ComponentLookup<Train> trains;
    private BufferLookup<DoorEntity> doorsInCarridges;
    private ComponentLookup<Door> doors;
    private ComponentLookup<Platform> platforms;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        doorsInCarridges = SystemAPI.GetBufferLookup<DoorEntity>();
        trains = SystemAPI.GetComponentLookup<Train>();
        doors = SystemAPI.GetComponentLookup<Door>();
        worldTransforms = SystemAPI.GetComponentLookup<LocalTransform>();
        platforms = SystemAPI.GetComponentLookup<Platform>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        worldTransforms.Update(ref state);
        doorsInCarridges.Update(ref state);
        doors.Update(ref state);
        trains.Update(ref state);
        platforms.Update(ref state);
        
        new DoorJob()
        {
            trains = trains,
            localTransforms = worldTransforms,
            doorEntities = doorsInCarridges,
            doors = doors,
            platforms = platforms,
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct DoorJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<Train> trains;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<LocalTransform> localTransforms;
    [ReadOnly] public BufferLookup<DoorEntity> doorEntities;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<Door> doors;
    [ReadOnly] public ComponentLookup<Platform> platforms;
    [ReadOnly] public float deltaTime;
    
    public void Execute(RefRW<Carriage> carriage)
    {
        Entity ownerTrainEntity = carriage.ValueRO.ownerTrainID;
        if (ownerTrainEntity == Entity.Null)
        {
            return;
        }

        RefRO<Train> train = trains.GetRefRO(ownerTrainEntity);
        if (!train.IsValid)
        {
            return;
        }

        if (train.ValueRO.State != TrainState.DoorOpening && train.ValueRO.State != TrainState.DoorClosing)
        {
            return;
        }

        if (train.ValueRO.currentPlatform == Entity.Null)
        {
            return;
        }

        DoorSide doorMoveSide = platforms[train.ValueRO.currentPlatform].TrainDoorOpenSide;
        
        var doors = doorEntities[carriage.ValueRO.Entity];
        
        switch (train.ValueRO.State)
        {
            case TrainState.DoorOpening:
                OpenDoors(doors, doorMoveSide);
                break;
            case TrainState.DoorClosing:
                CloseDoors(doors, doorMoveSide);
                break;
            default:
                return;
        }
    }

    private void OpenDoors(DynamicBuffer<DoorEntity> doors, DoorSide side)
    {
        foreach (DoorEntity door in doors)
        {
            RefRW<Door> doorE = this.doors.GetRefRW(door.doorEntity, false);
            if (doorE.ValueRO.DoorSide == side || side == DoorSide.Both)
            {
                doorE.ValueRW.elapsedMoveTime += deltaTime;
                float3 newPos = math.lerp(DoorInfo.closePos, DoorInfo.openPos,
                    this.doors.GetRefRW(door.doorEntity, false).ValueRO.elapsedMoveTime / 0.5f);
                localTransforms.GetRefRW(door.doorEntity, false).ValueRW.Position = newPos;
            }
        }
    }
    
    private void CloseDoors(DynamicBuffer<DoorEntity> doors, DoorSide side)
    {
        foreach (DoorEntity door in doors)
        {
            RefRW<Door> doorE = this.doors.GetRefRW(door.doorEntity, false);
            if (doorE.ValueRO.DoorSide == side || side == DoorSide.Both)
            {
                //So we don't have to reset the elpased time we can just reverse the time to close the doors
                doorE.ValueRW.elapsedMoveTime -= deltaTime;
                float3 newPos = math.lerp(DoorInfo.closePos, DoorInfo.openPos,
                    this.doors.GetRefRW(door.doorEntity, false).ValueRO.elapsedMoveTime / 0.5f);
                localTransforms.GetRefRW(door.doorEntity, false).ValueRW.Position = newPos;
            }
        }
    }
}
