
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class AntAuthoring : MonoBehaviour
	, IConvertGameObjectToEntity
{
	public void Convert(Entity entity, EntityManager dstManager
		, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponent<AntPathing>(entity);
		dstManager.AddComponent<AntHeading>(entity);
		dstManager.AddComponent<AntTarget>(entity);
		
		dstManager.AddComponentData(entity, new WeightLeft()
		{
			Degrees = 0,
			Rads = 0,
			Weight = 0
		});
		dstManager.AddComponentData(entity, new WeightRight()
		{
			Degrees = 0,
			Rads = 0,
			Weight = 0
		});
		dstManager.AddComponentData(entity, new WeightForward()
		{
			Degrees = 0,
			Rads = 0,
			Weight = 0
		});
	}
}
