using Unity.Entities;
using UnityEngine;

namespace Authoring
{
	public class TrackAuthoring : MonoBehaviour
	{
		public class TrackBaker : Baker<TrackAuthoring>
		{
			public override void Bake(TrackAuthoring authoring)
			{
				// TODO: Doesn't really make sense to have length or segment id specified in the authoring script, should
				// be modified by the track spawning system when placing track segments?
				AddComponent(new Track());
			}
		}
	}

	public struct Track : IComponentData
	{
		public float Length;
		public int SegmentId;
		
		// TODO: Enum specifying straight v curved, or should it be a separate component?
	}
}