using Unity.Entities;
using UnityEngine;

namespace Authoring
{
	public class ConfigAuthoring : MonoBehaviour
	{
		public GameObject CurvedTrackPrefab;

		public GameObject StraightTrackPrefab;

		public int NumCars;

		public int NumLanes;

        public float TrackLength;
        public float SegmentLength;

		public class ConfigBaker : Baker<ConfigAuthoring>
		{
			public override void Bake(ConfigAuthoring authoring)
			{
				AddComponent(new Config
				{
					CurvedTrackPrefab = GetEntity(authoring.CurvedTrackPrefab),
					StraightTrackPrefab = GetEntity(authoring.StraightTrackPrefab),
					NumCars = authoring.NumCars,
					NumLanes = authoring.NumLanes,
                    TrackLength = authoring.TrackLength,
                    SegmentLength = authoring.SegmentLength
				});
			}
		}
	}

	public struct Config : IComponentData
	{
		public Entity CurvedTrackPrefab;
		public Entity StraightTrackPrefab;
		public int NumCars;
		public int NumLanes;
        public float TrackLength;
        public float SegmentLength;
	}
}
