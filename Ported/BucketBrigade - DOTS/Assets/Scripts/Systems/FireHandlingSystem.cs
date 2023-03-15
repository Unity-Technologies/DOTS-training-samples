using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

//[BurstCompile]
public partial struct FireHandlingSystem : ISystem
{
    public ComponentLookup<OnFire> m_OnFireActive;

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // I should make sure that this runs after every grid has spawned, right ?
        //state.RequireForUpdate<GridTilesSpawningSystem>();
        state.RequireForUpdate<Config>();

        m_OnFireActive = state.GetComponentLookup<OnFire>();
        UnityEngine.Debug.Log("OnCreate");
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnityEngine.Debug.Log("Update");

        var config = SystemAPI.GetSingleton<Config>();

        // Updates the Component Lookup with the fresh World state
        m_OnFireActive.Update(ref state);

        var ignitionTestJob = new IgnitionTestJob
        {
            OnFireActive = m_OnFireActive,
            flashpoint = config.flashpoint
        };
        ignitionTestJob.ScheduleParallel();

    }
}

//[BurstCompile]
public partial struct IgnitionTestJob : IJobEntity
{
    [Unity.Collections.NativeDisableParallelForRestriction] public ComponentLookup<OnFire> OnFireActive;
    public float flashpoint;

    void Execute(Entity entity, Tile tile)
    {
        
        var isOnFire = tile.Temperature >= flashpoint;
        UnityEngine.Debug.LogFormat("Is on fire: {0}", isOnFire);
        OnFireActive.SetComponentEnabled(entity, isOnFire);
    }
}
