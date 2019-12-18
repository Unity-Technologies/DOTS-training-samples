
using Unity.Entities;

namespace DotsPort
{
	public struct BoardDesignComponent : IComponentData
	{
		public Entity MouseSpawnerPrefab;
		public Entity CatSpawnerPrefab;
		public Entity HomebasePrefab;
	}
}
