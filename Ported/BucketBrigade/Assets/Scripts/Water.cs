using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Water : IComponentData
{
	public float MaxVolume;
}
