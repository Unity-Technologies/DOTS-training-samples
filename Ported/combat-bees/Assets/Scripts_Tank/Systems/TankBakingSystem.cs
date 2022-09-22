using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

#region step1
// Baking systems are similar to regular systems, so we need to explicitly declare them so.
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)][DisableAutoCreation]
partial struct TankBakingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        // Adding the color component cannot be done synchronously from inside the for loop.
        // So we'll use a command buffer. But this time we don't need to defer the execution
        // to later in the frame, we'll playback the buffer immediately after the loop.
        // For this reason we can create the buffer directly, without using a command
        // buffer system singleton.
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // At the moment, there's only one tank Prefab. However, it's best practice to write
        // code that can handle as many entities as possible. This ForEach loop does this.
        foreach(var group in SystemAPI.Query<DynamicBuffer<ChildrenWithRenderer>>()
                    .WithEntityQueryOptions(EntityQueryOptions.IncludePrefab))
        {
            // Notice the EntityCommandBuffer parameter and the WithImmediatePlayback.
            // WithImmediatePlayback only works with Run.
            var entities = group.AsNativeArray().Reinterpret<Entity>();

            // Unlike Bakers, baking systems can modify all the entities they want.
            ecb.AddComponent(entities, new URPMaterialPropertyBaseColor());
        }

        // Immediate playback. The only purpose for the command buffer was to avoid
        // a structural change happening inside the for loop.
        ecb.Playback(state.EntityManager);
    }
}
#endregion