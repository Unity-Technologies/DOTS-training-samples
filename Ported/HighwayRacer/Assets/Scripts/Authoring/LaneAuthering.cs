using Unity.Entities;
using Unity.Rendering;

namespace Authoring
{

    public class LaneAuthoring : UnityEngine.MonoBehaviour
    {

        public class LaneBaker : Baker<LaneAuthoring>
        {
            public override void Bake(LaneAuthoring authoring)
            {
                AddComponent(new Lane()
                {
                    LaneNumber = 0,
                    SegmentNumber = 0,
                    LaneLength = 0.0f
                });
            }
        }
    }

    public struct Lane : IComponentData
    {
        public int LaneNumber;
        public int SegmentNumber;
        public float LaneLength;
        public float LaneRadius;
    }
}
