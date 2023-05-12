using System;
using Metro;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(TrainSystemGroup))]

public partial struct TrainMoverSystem : ISystem
{
    BufferLookup<LinkedEntityGroup> m_ChildBufferLookup;
    BufferLookup<TrackPoint> m_TrackBufferLookup;
    ComponentLookup<EnRouteComponent> m_EnrouteLookup;
    ComponentLookup<DepartingComponent> m_DepartingLookup;
    ComponentLookup<ArrivingComponent> m_ArrivingLookup;
    ComponentLookup<UnloadingComponent> m_UnloadingLookup;
    ComponentLookup<LoadingComponent> m_LoadingLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<Train>();
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrackPoint>();

        m_ChildBufferLookup = state.GetBufferLookup<LinkedEntityGroup>();
        m_TrackBufferLookup = state.GetBufferLookup<TrackPoint>();
        m_EnrouteLookup = state.GetComponentLookup<EnRouteComponent>();
        m_DepartingLookup = state.GetComponentLookup<DepartingComponent>();
        m_ArrivingLookup = state.GetComponentLookup<ArrivingComponent>();
        m_UnloadingLookup = state.GetComponentLookup<UnloadingComponent>();
        m_LoadingLookup = state.GetComponentLookup<LoadingComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        float deltaTime = SystemAPI.Time.DeltaTime;

        m_ChildBufferLookup.Update(ref state);
        m_TrackBufferLookup.Update(ref state);
        m_EnrouteLookup.Update(ref state);
        m_DepartingLookup.Update(ref state);
        m_ArrivingLookup.Update(ref state);
        m_UnloadingLookup.Update(ref state);
        m_LoadingLookup.Update(ref state);
        
        // Create the job.
        var trainMoverJob = new TrainMoverJob
        {
            config = config,
            deltaTime = deltaTime,
            childBufferLookup = m_ChildBufferLookup,
            trackBufferLookup = m_TrackBufferLookup,
            enrouteLookup = m_EnrouteLookup,
            departingLookup = m_DepartingLookup,
            arrivingLookup = m_ArrivingLookup,
            unloadingLookup = m_UnloadingLookup,
            loadingLookup = m_LoadingLookup
        };
        
        state.Dependency = trainMoverJob.Schedule(state.Dependency);
    }
}

