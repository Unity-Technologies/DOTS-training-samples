using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

namespace Unity.Rendering
{
    [WorldSystemFilter(WorldSystemFilterFlags.EntitySceneOptimizations)]
    class FrozenStaticRendererSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var group = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[] { typeof(SceneSection), typeof(RenderMesh), typeof(LocalToWorld), typeof(Static) },
                    None = new ComponentType[] { typeof(FrozenRenderSceneTag) }
                });

            var sections = new List<SceneSection>();
            EntityManager.GetAllUniqueSharedComponentData(sections);

            // @TODO: Perform full validation that all Low HLOD levels are in section 0 
            int hasStreamedLOD = 0;
            foreach (var section in sections)
            {
                group.SetFilter(section);
                if (section.Section != 0)
                    hasStreamedLOD = 1;
            }

            foreach (var section in sections)
            {
                group.SetFilter(section);
                EntityManager.AddSharedComponentData(group, new FrozenRenderSceneTag { SceneGUID = section.SceneGUID, SectionIndex = section.Section, HasStreamedLOD = hasStreamedLOD});
            }
            
            group.ResetFilter();
        }
    }
}