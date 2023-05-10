using Metro;
using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct DoorSystem : ISystem
{
    private ComponentTypeHandle<LocalTransform> transformTypeHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        transformTypeHandle = state.GetComponentTypeHandle<LocalTransform>();
        // This makes the system not update unless at least one entity exists that has the Door component.
        state.RequireForUpdate<Door>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var doorsQuery = SystemAPI.QueryBuilder().WithAll<Door, LocalTransform>().Build();

        // Because we cached the TypeHandle in OnCreate, we have to Update it each frame before use.
        transformTypeHandle.Update(ref state);

        // The more convenient way to get a type handle is to use SystemAPI,
        // which handles the caching and Update() for you.
        var doorTypeHandle = SystemAPI.GetComponentTypeHandle<Door>(true);

        var job = new DoorOpenJob
        {
            TransformTypeHandle = transformTypeHandle,
            DoorTypeHandle = doorTypeHandle,
            DeltaTime = SystemAPI.Time.DeltaTime
        };
    }

    [BurstCompile]
    struct DoorOpenJob : IJobChunk
    {
        public float DeltaTime;
        public ComponentTypeHandle<LocalTransform> TransformTypeHandle;
        [ReadOnly] public ComponentTypeHandle<Door> DoorTypeHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            // The useEnableMask parameter is true when one or more entities in
            // the chunk have components of the query that are disabled.
            // If none of the query component types implement IEnableableComponent,
            // we can assume that useEnabledMask will always be false.
            // However, it's good practice to add this guard check just in case
            // someone later changes the query or component types.
            Assert.IsFalse(useEnabledMask);

            var chunkTransforms = chunk.GetNativeArray(ref TransformTypeHandle);
            var chunkRotationSpeeds = chunk.GetNativeArray(ref DoorTypeHandle);
            for (int i = 0, chunkEntityCount = chunk.Count; i < chunkEntityCount; i++)
            {
                var lerp = DeltaTime / Door.OpeningTime;
                var offset = new float3(lerp * Door.DoorWidth, 0, 0);
                chunkTransforms[i] = chunkTransforms[i].Translate(offset);
            }
        }
    }
}
