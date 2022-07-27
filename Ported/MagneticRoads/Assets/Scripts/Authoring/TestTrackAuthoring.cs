using Components;
using Unity.Entities;
using Unity.Mathematics;
using Util;

namespace Authoring
{
    public class TestTrackAuthoring : UnityEngine.MonoBehaviour
    {
        public UnityEngine.GameObject Start;
        public UnityEngine.GameObject End;
    }

    public class TestTrackBaker : Baker<TestTrackAuthoring>
    {
        public override void Bake(TestTrackAuthoring authoring)
        {
            var Start = new Spline.RoadTerminator
            {
                Position = new float3(0, 0, 0),
                Normal = new float3(0, 1, 0),
                Tangent = new float3(0, 0, 1)
            };
            var End = new Spline.RoadTerminator
            {
                Position = new float3(0, 0, 10),
                Normal = new float3(0, 1, 0),
                Tangent = new float3(0, 0, 1)
            };

            AddComponent(new RoadSegment
            {
                Start = Start,
                End = End,
                Length = Spline.EvaluateLength(Start, End)
            });
        }
    }
}