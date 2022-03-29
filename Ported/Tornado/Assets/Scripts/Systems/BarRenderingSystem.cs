using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class BarRenderingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var pointDisplacement = World.GetExistingSystem<PointDisplacementSystem>();
            if (!pointDisplacement.isInitialized) return;

            var points = pointDisplacement.points;
            var links = pointDisplacement.links;

            Entities
                .ForEach((ref Translation translation, ref Rotation rotation/*, ref NonUniformScale scale*/, in Components.Bar bar) =>
                {
                    var link = links[bar.indexLink];
                    var start = points[link.startIndex].currentPosition;
                    var end = points[link.endIndex].currentPosition;
                    var midPoint = (start + end) * 0.5f;
                    var startToEnd = end - start;
                    rotation.Value = quaternion.LookRotation(math.normalize(startToEnd), new float3(0.0f, 1.0f, 0.0f));
                    translation.Value = midPoint;
                    // scale.Value = new float3(bar.thickness, bar.thickness, math.length(startToEnd));
                }).Run();
        }
    }
}