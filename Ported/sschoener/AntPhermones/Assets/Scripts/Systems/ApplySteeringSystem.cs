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
        var maxSpeed = antSteering.MaxSpeed;
        var acceleration = antSteering.Acceleration;
        var pheromoneStrength = antSteering.PheromoneSteerStrength;
        var wallStrength = antSteering.WallSteerStrength;
        return Entities.ForEach((ref SpeedComponent speed, ref FacingAngleComponent facingAngle, in PheromoneSteeringComponent pheromoneSteering, in WallSteeringComponent wallSteering) =>
        {
            facingAngle.Value += pheromoneSteering.Value * pheromoneStrength;
            facingAngle.Value += wallSteering.Value * wallStrength;
            float targetSpeed = maxSpeed;
            targetSpeed *= 1 - (math.abs(pheromoneSteering.Value) + math.abs(wallSteering.Value)) / 3;
            speed.Value += (targetSpeed - speed.Value) * acceleration;
        }).Schedule(inputDeps);
    }
}