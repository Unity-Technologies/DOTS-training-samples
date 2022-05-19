using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
partial struct CarriageDoorOpenJob : IJobEntity
{
    private const float DOOR_ACCELERATION = 0.0015f;
    private const float DOOR_FRICTION = 0.9f;
    private const float DOOR_ARRIVAL_THRESHOLD = 0.001f;

    public ComponentDataFromEntity<Translation> translationDataFromEntity;

    void Execute(CarriageDoor carriageDoor)
    {
        var translationDoorLeft = translationDataFromEntity[carriageDoor.door_LEFT];
        var translationDoorRight = translationDataFromEntity[carriageDoor.door_RIGHT];
        float3 _DOOR_POS = translationDoorLeft.Value;
        bool arrived = Approach.Apply(ref _DOOR_POS.x, ref carriageDoor.door_SPEED, carriageDoor.left_OPEN_X, DOOR_ACCELERATION,
            DOOR_ARRIVAL_THRESHOLD, DOOR_FRICTION);
        translationDoorLeft.Value = _DOOR_POS;
        translationDoorRight.Value = new float3(-_DOOR_POS.x, _DOOR_POS.y, _DOOR_POS.z);

        //Carriage door needs to tell the carriage it is part of that the doors are now open. 
        //return arrived;

    }
}


[BurstCompile]
partial struct CarriageDoorCloseJob : IJobEntity
{
    private const float DOOR_ACCELERATION = 0.0015f;
    private const float DOOR_FRICTION = 0.9f;
    private const float DOOR_ARRIVAL_THRESHOLD = 0.001f;

    public ComponentDataFromEntity<Translation> translationDataFromEntity;

    void Execute(CarriageDoor carriageDoor)
    {
        var translationDoorLeft = translationDataFromEntity[carriageDoor.door_LEFT];
        var translationDoorRight = translationDataFromEntity[carriageDoor.door_RIGHT];
        float3 _DOOR_POS = translationDoorLeft.Value;
        bool arrived = Approach.Apply(ref _DOOR_POS.x, ref carriageDoor.door_SPEED, carriageDoor.left_CLOSED_X, DOOR_ACCELERATION,
            DOOR_ARRIVAL_THRESHOLD, DOOR_FRICTION);
        translationDoorLeft.Value = _DOOR_POS;
        translationDoorRight.Value = new float3(-_DOOR_POS.x, _DOOR_POS.y, _DOOR_POS.z);

        //Carriage door needs to tell the carriage it is part of that the doors are now open. 
        //return arrived;

    }
}

partial struct CarriageDoorSystem : ISystem
{
    private ComponentDataFromEntity<Translation> translationDataFromEntity;

    public void OnCreate(ref SystemState state)
    {
        translationDataFromEntity = state.GetComponentDataFromEntity<Translation>(false);
    }
    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        translationDataFromEntity.Update(ref state);

        //If door is at station do:

        var openJob = new CarriageDoorOpenJob { translationDataFromEntity = translationDataFromEntity };

        openJob.Schedule();

    }
}