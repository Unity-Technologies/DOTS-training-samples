using System;
using Unity.Entities;
using Unity.Jobs;

[Serializable]
public struct ClothMeshToken : IComponentData
{
	public JobHandle jobHandle;
}
