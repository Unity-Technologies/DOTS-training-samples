using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct CellDisplay : IComponentData
{
	public float CoolValue;
	public float4 CoolColor;
	public float CoolHeight;
	public float FireValue;
	public float4 FireColor;
	public float FireHeight;
}

public class CellDisplayAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public float CoolValue = 15.0f;
	public UnityEngine.Color CoolColor;
	[Range(0.0f, 10.0f)]
	public float CoolHeight = 0.0f;
	public float FireValue = 100.0f;
	public UnityEngine.Color FireColor;
	[Range(0.0f, 10.0f)]
	public float FireHeight = 2.0f;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new CellDisplay
		{
			CoolValue = CoolValue,
			CoolColor = new float4(CoolColor.r, CoolColor.g, CoolColor.b, 1),
			CoolHeight = CoolHeight,
			FireValue = FireValue,
			FireColor = new float4(FireColor.r, FireColor.g, FireColor.b, 1),
			FireHeight = FireHeight,
		});
	}
}
