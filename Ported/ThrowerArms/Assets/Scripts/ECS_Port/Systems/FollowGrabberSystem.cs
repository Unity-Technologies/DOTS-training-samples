using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class FollowGrabberSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var handMatrixAccessor = GetComponentDataFromEntity<HandMatrix>(true);
        JobHandle jobHandle = Entities.WithReadOnly(handMatrixAccessor)
        .ForEach((Entity e, ref Translation translation, in GrabbedState grabbed) =>
        {
            HandMatrix handMatrix = handMatrixAccessor[grabbed.GrabbingEntity];
            translation.Value = math.transform(handMatrix.Value, grabbed.localPosition);
        }).Schedule(inputDependencies);
        return jobHandle;
    }
}