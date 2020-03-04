using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(ThrowerTargetSystem))]
public class RenderUpdateSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.ForEach((ref Translation translation,ref Rotation rotation,in ArmFingerComponentData renderData) =>
        {
            translation = new Translation()
            {
                Value = renderData.position + 0.5f * renderData.forward
            };
            rotation = new Rotation()
            {
                Value = quaternion.LookRotation(renderData.forward,renderData.anchorUp)
            };
        }).Run();

        return inputDeps;
    }
}
