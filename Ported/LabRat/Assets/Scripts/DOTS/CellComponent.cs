
using Unity.Entities;

namespace DotsPort
{
	public struct CellComponent : IComponentData
	{
		// One bit per direction for the walls...
		public int WallBits;
	}
}
