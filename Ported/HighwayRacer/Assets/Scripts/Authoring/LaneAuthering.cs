using Unity.Entities;
using Unity.Rendering;

namespace Authoring
{

    public class LaneAuthoring : UnityEngine.MonoBehaviour
    {
        public float TrackLength = 1;
        public int LaneNumber;
        public int SegmentNumber;

        public class LaneBaker : Baker<LaneAuthoring>
        {
            public override void Bake(LaneAuthoring authoring)
            {
                AddComponent(new Lane()
                {
                    TrackLength = authoring.TrackLength,
                    LaneNumber = authoring.LaneNumber,
                    SegmentNumber = authoring.SegmentNumber,
            });
        }
    }
}

public struct Lane : IComponentData
    {
        public float TrackLength;
        public int LaneNumber;
        public int SegmentNumber;
    }
}