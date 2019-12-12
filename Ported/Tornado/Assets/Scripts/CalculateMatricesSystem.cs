using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(PushToRenderSystem))]
public class CalculateMatricesSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.ForEach( (Entity entity, ref DynamicBuffer<ConstrainedPointEntry> points, ref DynamicBuffer<RenderMatrixEntry> matricies) =>
        {
            // TODO: Cache the length of the points and only if different change the render matrix buffer to match

            if (matricies.Length != points.Length)
            {
                matricies.ResizeUninitialized(points.Length);
            }

            for (var i = 0; i < points.Length; i++)
            {
                matricies[i] = new RenderMatrixEntry {Value = float4x4.Translate(points[i].Value.position)};
            }

        }).Run();
        
        return inputDeps;
    }
}