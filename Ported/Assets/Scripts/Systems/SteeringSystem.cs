using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public struct ComputeSteering : IJobChunk
{
    public ComponentTypeHandle<Velocity> VelocityHandle;
    [ReadOnly] public ComponentTypeHandle<Translation> TranslationHandle;
    [ReadOnly] public ComponentTypeHandle<WanderingSteering> WanderingSteeringHandle;
    [ReadOnly] public ComponentTypeHandle<PheromoneSteering> PheromoneSteeringHandle;
    [ReadOnly] public ComponentTypeHandle<MapContainmentSteering> ContainmentSteeringHandle;
    [ReadOnly] public ComponentTypeHandle<GeneralDirection> GeneralDirectionHandle;
    [ReadOnly] public ComponentTypeHandle<ProximitySteering> ProximitySteeringHandle;
    [ReadOnly] public ComponentTypeHandle<ObstacleAvoidanceSteering> AvoidanceSteeringHandle;
    public float DeltaTime;
    public float AntMaxSpeed;
    public float AntMaxTurn;
    public float AntAcceleration;
    public float WanderingStrength;
    public float PheromoneStrength;
    public float ContainmentStrength;
    public float GeneralDirectionStrength;
    public float ProximityStrength;
    public float AvoidanceStrength;

    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        NativeArray<Velocity> velocity = chunk.GetNativeArray(VelocityHandle);
        NativeArray<Translation> translation = chunk.GetNativeArray(TranslationHandle);
        NativeArray<WanderingSteering> wanderingSteering = chunk.GetNativeArray(WanderingSteeringHandle);
        NativeArray<PheromoneSteering> pheromoneSteering = chunk.GetNativeArray(PheromoneSteeringHandle);
        NativeArray<MapContainmentSteering> containmentSteering = chunk.GetNativeArray(ContainmentSteeringHandle);
        NativeArray<GeneralDirection> generalDirection = chunk.GetNativeArray(GeneralDirectionHandle);
        NativeArray<ProximitySteering> proximitySteering = chunk.GetNativeArray(ProximitySteeringHandle);
        NativeArray<ObstacleAvoidanceSteering> avoidanceSteering = chunk.GetNativeArray(AvoidanceSteeringHandle);

        for (int i = 0; i < chunk.Count; ++i)
        {
            var velocityComp = velocity[i];
            float2 finalSteering = wanderingSteering[i].Value * WanderingStrength
                                   + pheromoneSteering[i].Value * PheromoneStrength
                                   + containmentSteering[i].Value * ContainmentStrength
                                   + generalDirection[i].Value * GeneralDirectionStrength
                                   + proximitySteering[i].Value * ProximityStrength
                                   + avoidanceSteering[i].Value * AvoidanceStrength;
            finalSteering = math.normalizesafe(finalSteering);
            float alignment = (math.dot(finalSteering, velocityComp.Direction) + 1.0f) / 2.0f;
            float targetSpeed = alignment * AntMaxSpeed;
            float accelerationThisFrame = AntAcceleration * DeltaTime;
            float acceleration =  math.clamp(targetSpeed - velocityComp.Speed, -accelerationThisFrame, accelerationThisFrame);
            float newSpeed = targetSpeed + acceleration;

            float targetFacingAngle = math.atan2(finalSteering.y, finalSteering.x);
            float currentFacingAngle = math.atan2(velocityComp.Direction.y, velocityComp.Direction.x);

            float turnRateThisFrame = AntMaxTurn / DeltaTime;
            float turn = math.clamp(targetFacingAngle - currentFacingAngle, -turnRateThisFrame, turnRateThisFrame);
            float newFacingAngle = currentFacingAngle + turn;
                
            float newDirectionX = math.cos(newFacingAngle);
            float newDirectionY = math.sin(newFacingAngle);

            velocityComp.Direction = new float2(newDirectionX, newDirectionY);
            velocityComp.Speed = newSpeed;
            velocity[i] = velocityComp;
        }

        velocity.Dispose();
        translation.Dispose();
        wanderingSteering.Dispose();
        pheromoneSteering.Dispose();
        containmentSteering.Dispose();
        generalDirection.Dispose();
        proximitySteering.Dispose();
        avoidanceSteering.Dispose();
    }
}

/**
 * This system will take care of Mixing up all active steering on ants and proceed to update their rotation / speed
 */
[UpdateBefore(typeof(AntMoveSystem))]
public partial class SteeringSystem : SystemBase
{
    private float m_AntMaxSpeed = 0.2f;
    private float m_AntMaxTurn = math.PI / 2.0f;
    private float m_AntAcceleration = 0.07f;
    
    private float m_WanderingStrength = 1.0f;
    private float m_PheromoneStrength = 1.0f;
    private float m_ContainmentStrength = 1.0f;
    private float m_GeneralDirectionStrength = 1.0f;
    private float m_ProximityStrength = 1.0f;
    private float m_AvoidanceStrength = 1.0f;
    private EntityQuery m_AntQuery;

    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_AntMaxSpeed = config.AntMaxSpeed;
        m_AntMaxTurn = config.AntMaxTurn;
        m_AntAcceleration = config.AntAcceleration;
        m_WanderingStrength = config.WanderingStrength;
        m_PheromoneStrength = config.PheromoneStrength;
        m_ContainmentStrength = config.ContainmentStrength;
        m_GeneralDirectionStrength =config.GeneralDirectionStrength;
        m_ProximityStrength = config.ProximityStrength;
        m_AvoidanceStrength = config.AvoidanceStrength;
        m_AntQuery = GetEntityQuery(ComponentType.ReadWrite<Velocity>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<WanderingSteering>(),
            ComponentType.ReadOnly<PheromoneSteering>(),
            ComponentType.ReadOnly<MapContainmentSteering>(),
            ComponentType.ReadOnly<GeneralDirection>(),
            ComponentType.ReadOnly<ProximitySteering>(),
            ComponentType.ReadOnly<ObstacleAvoidanceSteering>());

        var random = new Random(1234);
        Entities.ForEach((ref Velocity antVelocity) =>
            {
                antVelocity.Direction = random.NextFloat2Direction();
                antVelocity.Speed = 0.0f;
            }).Run();
    }

    protected override void OnUpdate()
    {
        ComputeSteering job = new ComputeSteering()
        {
            VelocityHandle = GetComponentTypeHandle<Velocity>(false),
            TranslationHandle = GetComponentTypeHandle<Translation>(true),
            WanderingSteeringHandle = GetComponentTypeHandle<WanderingSteering>(true),
            PheromoneSteeringHandle = GetComponentTypeHandle<PheromoneSteering>(true),
            ContainmentSteeringHandle = GetComponentTypeHandle<MapContainmentSteering>(true),
            GeneralDirectionHandle = GetComponentTypeHandle<GeneralDirection>(true),
            ProximitySteeringHandle = GetComponentTypeHandle<ProximitySteering>(true),
            AvoidanceSteeringHandle = GetComponentTypeHandle<ObstacleAvoidanceSteering>(true),
            DeltaTime = Time.DeltaTime,
            AntMaxSpeed = m_AntMaxSpeed,
            AntMaxTurn = m_AntMaxTurn,
            AntAcceleration = m_AntAcceleration,
            WanderingStrength = m_WanderingStrength,
            PheromoneStrength = m_PheromoneStrength,
            ContainmentStrength = m_ContainmentStrength,
            GeneralDirectionStrength = m_GeneralDirectionStrength,
            ProximityStrength = m_ProximityStrength,
            AvoidanceStrength = m_AvoidanceStrength
        };
        Dependency = job.Schedule(m_AntQuery, Dependency);
    }
}