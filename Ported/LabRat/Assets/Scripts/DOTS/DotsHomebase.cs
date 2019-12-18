
using ECSExamples;
using Unity.Entities;

namespace DotsPort
{
	public class DotsHomebase : Homebase, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			HomebaseComponent homebaseComponent = new HomebaseComponent();

			dstManager.AddComponentData(entity, homebaseComponent);
		}
	}
}
