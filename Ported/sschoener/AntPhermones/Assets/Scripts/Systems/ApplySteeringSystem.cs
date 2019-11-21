using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ComputePheromoneSteeringSystem))]
[UpdateAfter(typeof(ComputeWallSteeringSystem))]
public class ApplySteeringSystem : JobComponentSystem
{
    EntityQuery m_AntSteeringQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntSteeringQuery = GetEntityQuery(ComponentType.ReadOnly<AntSteeringSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var antSteering = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>();
        return new ApplySteeringJob {
            MaxSpeed = antSteering.MaxSpeed,
            Acceleration = antSteering.Acceleration,
            PheromoneStrength = antSteering.PheromoneSteerStrength,
            WallStrength = antSteering.WallSteerStrength
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct ApplySteeringJob : IJobForEach<PheromoneSteeringComponent, WallSteeringComponent, SpeedComponent, FacingAngleComponent>
    {
        public float MaxSpeed;
        public float Acceleration;
        public float PheromoneStrength;
        public float WallStrength;

        public void Execute([ReadOnly] ref PheromoneSteeringComponent pheromoneSteering, [ReadOnly] ref WallSteeringComponent wallSteering, [WriteOnly] ref SpeedComponent speed, ref FacingAngleComponent facingAngle)
        {
            facingAngle.Value += pheromoneSteering.Value * PheromoneStrength;
            facingAngle.Value += wallSteering.Value * WallStrength;
            float targetSpeed = MaxSpeed;
            targetSpeed *= 1 - (math.abs(pheromoneSteering.Value) + math.abs(wallSteering.Value)) / 3;
            speed.Value += (targetSpeed - speed.Value) * Acceleration;
        }
    }
}