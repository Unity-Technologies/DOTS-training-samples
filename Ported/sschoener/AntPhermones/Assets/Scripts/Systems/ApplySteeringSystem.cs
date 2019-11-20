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

        public void Execute([ReadOnly] ref PheromoneSteeringComponent pheromone, [ReadOnly] ref WallSteeringComponent wall, [WriteOnly] ref SpeedComponent speed, ref FacingAngleComponent facingAngle)
        {
            facingAngle.Value += pheromone.Value * PheromoneStrength;
            facingAngle.Value += wall.Value * WallStrength;
            float targetSpeed = MaxSpeed;
            targetSpeed *= 1 - (math.abs(pheromone.Value) + math.abs(wall.Value)) / 3;
            speed.Value = (targetSpeed - speed.Value) * Acceleration;
        }
    }
}