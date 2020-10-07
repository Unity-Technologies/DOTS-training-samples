using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct HeatMap : IComponentData
{
	[HideInInspector]
	public int SizeX;
	
	[HideInInspector]
	public int SizeZ;
}
