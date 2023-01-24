using Unity.Entities;
using UnityEngine;

namespace Authoring
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public GameObject CurvedTrackPrefab;

		public GameObject StraightTrackPrefab;

        public GameObject CarPrefab;
		
		public int NumCars;

		public int NumLanes;

		public class ConfigBaker : Baker<ConfigAuthoring>
		{
			public override void Bake(ConfigAuthoring authoring)
			{
				AddComponent(new Config
				{
					CurvedTrackPrefab = GetEntity(authoring.CurvedTrackPrefab),
					StraightTrackPrefab = GetEntity(authoring.StraightTrackPrefab),
                    CarPrefab = GetEntity(authoring.CarPrefab),
					NumCars = authoring.NumCars,
					NumLanes = authoring.NumLanes
				});
			}
		}
	}

	public struct Config : IComponentData
	{
		public Entity CurvedTrackPrefab;
		public Entity StraightTrackPrefab;
        public Entity CarPrefab;
		public int NumCars;
		public int NumLanes;
	}
}
