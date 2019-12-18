
using Unity.Entities;
using UnityEngine;

namespace DotsPort
{
	public class DotsWall : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			WallComponent wallComponent = new WallComponent();

			dstManager.AddComponentData(entity, wallComponent);
		}
	}
}
