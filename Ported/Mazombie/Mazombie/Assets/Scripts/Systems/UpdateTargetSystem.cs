using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ZombieMovementSystem))]
[BurstCompile]
public partial struct UpdateTargetSystem : ISystem
{
    private EntityQuery _pillQuery;
    private EntityQuery _hunterTargetQuery;
    private Random _random;
    private bool _initialized;
    private NativeArray<ComponentType> _componentTypes;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
        _componentTypes = new NativeArray<ComponentType>(3, Allocator.Persistent);
        _componentTypes[0] = ComponentType.ReadWrite<HunterTarget>();
        _componentTypes[1] = ComponentType.ReadOnly<PillHunter>(); 
        _componentTypes[2] = ComponentType.ReadOnly<NeedUpdateTarget>(); 
        _pillQuery = state.GetEntityQuery(ComponentType.ReadOnly<Pill>());
        _hunterTargetQuery = state.GetEntityQuery(_componentTypes);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _componentTypes.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_initialized)
        {
            _initialized = true;
            var config = SystemAPI.GetSingleton<GameConfig>();
            _random = new Random(config.seed);
        }

        if (_hunterTargetQuery.CalculateEntityCount() <= 0 || _pillQuery.CalculateEntityCount() <= 0) 
            return;
        
        var pillArray = _pillQuery.ToEntityArray(Allocator.Temp);

        var entityArray = _hunterTargetQuery.ToEntityArray(Allocator.Temp);

        var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        for (int i = 0; i < entityArray.Length; i++)
        {
            int randomIndex = _random.NextInt(0, pillArray.Length);
            var position = SystemAPI.GetComponent<LocalToWorldTransform>(pillArray[randomIndex]).Value.Position;
            state.EntityManager.SetComponentData(entityArray[i], new HunterTarget{position = position});
            ecb.SetComponentEnabled<NeedUpdateTarget>(entityArray[i], false);
            ecb.SetComponentEnabled<NeedUpdateTrajectory>(entityArray[i], true);
        }
        
        entityArray.Dispose();
        pillArray.Dispose();
    }
}