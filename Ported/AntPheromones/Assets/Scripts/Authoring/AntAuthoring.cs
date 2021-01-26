
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AntAuthoring : MonoBehaviour
	, IConvertGameObjectToEntity
{
	public void Convert(Entity entity, EntityManager dstManager
		, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponent<AntPathing>(entity);
		dstManager.AddComponent<AntHeading>(entity);
		dstManager.AddComponent<AntTarget>(entity);
	}
}
