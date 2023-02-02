using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct CarSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spawner = SystemAPI.GetSingleton<GlobalSettings>();

        if (!spawner.spawned)
        {
            InitialSpawn(ref spawner, ref state);
        }
        
        
    }
    
    private void InitialSpawn(ref GlobalSettings globalSettings, ref SystemState state)
    {
        var carEntities = new NativeArray<Entity>(globalSettings.amount, Allocator.Temp);
        state.EntityManager.Instantiate(globalSettings.carPrefab, carEntities);
        
        

        for (var i = 0; i < carEntities.Length; i++)
        {
            int lane = Random.Range(0, globalSettings.NumLanes);
            var carEntity = carEntities[i];

            //TODO: position calculation on track
            var position = new float3 {x = (float)lane, y = 0.0f, z = Random.Range(0.0f, globalSettings.LengthLanes)};
            float defaultVelocity = Random.Range(globalSettings.MinVelocity, globalSettings.MaxVelocity);
            float overtakeVelocity = Random.Range(globalSettings.MinOvertakeVelocity, globalSettings.MaxOvertakeVelocity);
            state.EntityManager.SetComponentData(carEntity, new LocalTransform {Position = position, Scale = 1});
            state.EntityManager.SetComponentData(carEntity, new CarPositionInLane{Position = position.z, LanePosition = position.x, LaneIndex = lane});
            state.EntityManager.SetComponentData(carEntity, new CarVelocity {VelX = 0.0f, VelY = defaultVelocity});
            state.EntityManager.SetComponentData(carEntity, new CarDefaultValues() {DefaultVelY = defaultVelocity});
            state.EntityManager.SetComponentData(carEntity, new CarOvertakeState {OvertakeStartTime = 0.0f, OriginalLane = lane, ChangingLane = false, OvertakeVelocity = overtakeVelocity});
            state.EntityManager.SetComponentData(carEntity, new CarIsOvertaking {IsOvertaking = false});
            state.EntityManager.SetComponentData(carEntity, new CarCollision{CollisionFlags = CollisionType.None, FrontVelocity = 0});
            state.EntityManager.SetComponentData(carEntity, new CarIndex{Index = -1});
        }
        
        var bufferEntity = state.EntityManager.CreateEntity();
        var buffer = state.EntityManager.AddBuffer<CarEntity>(bufferEntity);
        buffer.Length = carEntities.Length;
        for (int i = 0; i < carEntities.Length; i++)
        {
            buffer[i] = new CarEntity { Value = carEntities[i] };
        }

        globalSettings.spawned = true;
        SystemAPI.SetSingleton(globalSettings);
    }

    private void InitEntity(Entity carEntity, float3 position, ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}