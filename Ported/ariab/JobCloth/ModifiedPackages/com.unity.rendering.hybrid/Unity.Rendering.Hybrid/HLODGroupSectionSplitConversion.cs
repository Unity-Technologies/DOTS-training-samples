using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
class HLODGroupSectionSplitConversion : GameObjectConversionSystem
{
    void RecursivelySetSection(Transform transform, SceneSection section)
    {
        foreach (var entity in GetEntities(transform.gameObject))
            DstEntityManager.SetSharedComponentData(entity, section);

        for (int i = 0;i != transform.childCount;i++)
            RecursivelySetSection(transform.GetChild(i), section);
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((HLOD hlod) =>
        {
            if (!hlod.autoLODSections)
                return;

            var section = DstEntityManager.GetSharedComponentData<SceneSection>(GetPrimaryEntity(hlod));
            var lodCount = hlod.LODParentTransforms.Length;

            for (int i = 0; i < lodCount; ++i)
            {
                // Lowest LOD in section 0, everything else in section 1
                section.Section = (i == lodCount - 1) ? 0 : 1;
                RecursivelySetSection(hlod.LODParentTransforms[i], section);
            }
        });
    }
}
