using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateThumbIKChainSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.WithName("UpdateThumbIK")
            .ForEach((in ArmComponent arm, in Translation translation) =>
            {
                float3 thumbPosition = translation.Value + arm.HandRight * ThumbConstants.XOffset;
                throw new NotImplementedException();
            }).Schedule(inputDeps);
    }
}
