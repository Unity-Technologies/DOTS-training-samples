using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RootTranslation : IComponentData
{
	public float3 Value;
}
