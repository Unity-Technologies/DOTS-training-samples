using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct FireSimulationSettings : IComponentData
{
	[Min(1)]
	public int PropagationRange;
	public float PropagationTransfer;
	public float PropagationThreshold;
	public float HeatIncrease;
	public float HeatIncreaseThreshold;

}
