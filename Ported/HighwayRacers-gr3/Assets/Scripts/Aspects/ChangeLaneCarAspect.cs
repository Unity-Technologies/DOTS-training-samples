using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public readonly partial struct ChangeLaneCarAspect : IAspect<ChangeLaneCarAspect>
{
    public readonly Entity self;
    private readonly RefRO<LaneComponent> m_lane;
    private readonly RefRO<OvertakeSpeed> m_overtakeSpeed;
    private readonly RefRO<OvertakeTime> m_overtakeTime;
    private readonly RefRO<CarStateComponent> m_currentState;
    private readonly RefRO<CarTraveledDistance> m_travelDistance;

    private readonly RefRW<OvertakeLaneComponent> m_overtakeLane;
    private readonly RefRW<CurrentLaneComponent> m_currentLane;
    private readonly RefRW<CurrentOvertakeTime> m_currentOvertakeTime;
    public int StartingLane
    {
        get => m_lane.ValueRO.LaneNumber;
    }

    public float OvertakeTime
    {
        get => m_overtakeTime.ValueRO.Value;
    }
    
    public float CurrentDistance
    {
        get => m_travelDistance.ValueRO.Value;
    }

    public float OvertakeSpeed
    {
        get => m_overtakeSpeed.ValueRO.Value;
    }
    
    public int CurrentCarState
    {
        get => m_currentState.ValueRO.StateAsInt;
    }
    
    public float CurrentOvertakeTime
    {
        get => m_currentOvertakeTime.ValueRO.Value;
        set => m_currentOvertakeTime.ValueRW.Value = value;
    }
    
    public int OvertakeLane
    {
        get => m_overtakeLane.ValueRO.OvertakeLaneNumber;
        set => m_overtakeLane.ValueRW.OvertakeLaneNumber = value;
    }
    
    public int CurrentLane
    {
        get => m_currentLane.ValueRO.CurrentLaneNumber;
        set => m_currentLane.ValueRW.CurrentLaneNumber = value;
    }


}
