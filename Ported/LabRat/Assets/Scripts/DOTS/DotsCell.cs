
using ECSExamples;
using Unity.Entities;

namespace DotsPort
{
	public class DotsCell : Cell, IConvertGameObjectToEntity
	{
		private int CalculateWallBits()
		{
			int wallBits = 0;
			
			for(int i = 0; i < 4; i++)
			{
				Direction wallDirection = (Direction)i;

				if(HasWall(wallDirection))
				{
					wallBits |= 1 << i;
				}
			}

			return wallBits;
		}
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			CellComponent cellComponent = new CellComponent()
			{
				WallBits = CalculateWallBits(),
			};
		}
	}
}
