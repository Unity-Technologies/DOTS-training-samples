using Unity.Entities;
using UnityEngine;

namespace Authoring
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public int NumCars;

		public int NumLanes;

		public class ConfigBaker : Baker<ConfigAuthoring>
		{
			public override void Bake(ConfigAuthoring authoring)
			{
				AddComponent(new Config
				{
					NumCars = authoring.NumCars,
					NumLanes = authoring.NumLanes
				});
			}
		}
	}

	public struct Config : IComponentData
	{
		public int NumCars;
		public int NumLanes;
	}
}