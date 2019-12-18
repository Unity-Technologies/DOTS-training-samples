
using Unity.Entities;

namespace DotsPort
{
	public struct BoardComponent : IComponentData
	{
		public Entity LightCellPrefab;
		public Entity DarkCellPrefab;
	}
}
