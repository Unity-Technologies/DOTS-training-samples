using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public GameObject CurvedTrackPrefab;

		public GameObject StraightTrackPrefab;

        public GameObject CarPrefab;

        public GameObject LanePrefab;
		
		public int NumCars;

		public int NumLanes;

        [Range(0f, 10f)]
        public float TrackSize = 1;

		public class ConfigBaker : Baker<ConfigAuthoring>
		{
			public override void Bake(ConfigAuthoring authoring)
			{
				AddComponent(new Config
				{
					CurvedTrackPrefab = GetEntity(authoring.CurvedTrackPrefab),
					StraightTrackPrefab = GetEntity(authoring.StraightTrackPrefab),
                    CarPrefab = GetEntity(authoring.CarPrefab),
                    LanePrefab = GetEntity(authoring.LanePrefab),
					NumCars = authoring.NumCars,
					NumLanes = authoring.NumLanes,
                    TrackSize = authoring.TrackSize
                });
			}
		}
	}

	public struct Config : IComponentData
	{
		public Entity CurvedTrackPrefab;
		public Entity StraightTrackPrefab;
        public Entity CarPrefab;
        public Entity LanePrefab;
		public int NumCars;
		public int NumLanes;
        public float TrackSize;

        // Some constants
        public const float CurveRadius = 31.46f;
        public const float LaneOffset = 1.9f;
        public const float SegmentLength = 60.0f;
    }
}
