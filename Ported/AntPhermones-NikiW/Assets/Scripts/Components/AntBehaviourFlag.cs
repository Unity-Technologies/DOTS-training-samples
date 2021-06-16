using System;
using Unity.Entities;

/// <summary>
///     Denotes that this entity exhibits ant behaviour.
///     Denotes that this entities <see cref="AntSimulationTransform2D"/> will get updated towards the <see cref="AntSimulationSystem"/>'s pheromones.
/// </summary>
[GenerateAuthoringComponent]
public struct AntBehaviourFlag : IComponentData
{
}