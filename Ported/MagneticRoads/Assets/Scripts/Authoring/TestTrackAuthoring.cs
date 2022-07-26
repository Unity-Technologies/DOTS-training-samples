using Components;
using Unity.Entities;

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
            AddComponent(new Track()
            {
                StartPos = start.position,
                StartNorm = start.up,
                StartTang = start.forward,
                EndPos = end.position,
                EndNorm = end.up,
                EndTang = end.forward,
            });
        }
    }
}