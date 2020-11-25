using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;

public class ResourceManagerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var resParams = GetSingleton<ResourceParams>();
        var resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        var stackHeights = bufferFromEntity[bufferEntity];

    }

}