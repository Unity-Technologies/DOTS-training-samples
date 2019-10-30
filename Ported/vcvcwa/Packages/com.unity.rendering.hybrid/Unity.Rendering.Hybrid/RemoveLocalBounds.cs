using Unity.Entities;
using Unity.Transforms;

namespace Unity.Rendering
{
    /* Disabled for now. Makes chunk bounds go out of sync.
    [WorldSystemFilter(WorldSystemFilterFlags.EntitySceneOptimizations)]
    [UpdateAfter(typeof(RenderBoundsUpdateSystem))]
    class RemoveLocalBounds : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var group = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[] { typeof(RenderBounds), typeof(Static) }
                });
            
            EntityManager.RemoveComponent(group, new ComponentTypes (typeof(RenderBounds)));
        }
    }
    */
}