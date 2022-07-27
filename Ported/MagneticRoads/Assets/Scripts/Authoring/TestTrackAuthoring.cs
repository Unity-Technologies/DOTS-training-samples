using Components;
using Unity.Entities;
using Util;
using Unity.Mathematics;

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
            var start = authoring.Start.transform;
            var end = authoring.End.transform;
            AddComponent(new RoadSegment
            {
                Start = new Spline.RoadTerminator()
                {
                    Position = new float3(0,0,0),
                    Normal = new float3(0,1,0),
                    Tangent = new float3(0,0,1)
                },
                End = new Spline.RoadTerminator
                {
                    Position = new float3(0,0,100),
                    Normal = new float3(0,1,0),
                    Tangent = new float3(0,0,1)
                    
                }
            });
        }
    }
}