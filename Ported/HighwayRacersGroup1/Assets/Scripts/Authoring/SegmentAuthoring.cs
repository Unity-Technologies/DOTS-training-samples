using Unity.Entities;
using Unity.Mathematics;

public class SegmentAuthoring : UnityEngine.MonoBehaviour
{
    class SegmentBaker : Baker<SegmentAuthoring>
    {
        public override void Bake(SegmentAuthoring authoring)
        {
            AddComponent<Segment>();
        }
    }
}

public struct Segment : IComponentData
{
    public float3 Right;
}
