using System;
using Unity.Entities;

/// <summary>
///     Denotes that this entity is a static obstacle that <see cref="AntBehaviourFlag" />'s need to avoid.
///     Obstacles can be indirectly queried by ants, who will path around them.
///     Most obstacles are code-generated, but this allows manual spawning.
///     <see cref="AntSimulationSystem" />, <see cref="AntSimulationRenderSystem" /> and <see cref="ObstacleManagementSystem" />.
///     Obstacles have a fixed radius, <see cref="AntSimulationParams.obstacleRadius"/>!
/// </summary>
[GenerateAuthoringComponent]
public struct StaticObstacleFlag : IComponentData
{
}
