using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[RequiresEntityConversion]
public class AuthoringCompBase : MonoBehaviour, IConvertGameObjectToEntity
{
	public float Variable;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var data = new CompBase { 
			Value = Variable
		};
	}
}