using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Unity.Rendering
{
    [UpdateAfter(typeof(LODGroupConversion))]
    class HLODGroupConversion : GameObjectConversionSystem
    {    
        protected override void OnUpdate()
        {
            Entities.ForEach((HLOD hlod) =>
            {
                var lodGroup = hlod.GetComponent<LODGroup>();
                var hlodEntity = GetPrimaryEntity(hlod);
    
                // If conversion of LODGroup failed, skip HLOD conversion too
                if (!DstEntityManager.HasComponent<MeshLODGroupComponent>(hlodEntity))
                    return;
    
                DstEntityManager.AddComponent(hlodEntity, ComponentType.ReadWrite<HLODComponent>());
                            
                var LODCount = lodGroup.lodCount;
                if (LODCount != hlod.LODParentTransforms.Length)
                {
                    Debug.LogWarning("HLOD out of sync with LODGroup", hlod);
                    return;
                }
    
                for (int i = 0; i != LODCount; i++)
                {
                    var childGroups = hlod.CalculateLODGroups(i);
                    var HLODMask = 1 << i;
                    
                    foreach (var childGroup in childGroups)
                    {
                        var childLodGroupEntities = GetEntities(childGroup);
                        if (childLodGroupEntities.Count() == 0)
                        {
                            Debug.LogWarning($"Missing child group '{childGroup.gameObject.name}' in LOD group '{hlod.gameObject.name}', this can happen because the child group is disabled.");
                            return;
                        }
    
                        foreach (var childLodGroupEntity in childLodGroupEntities)
                        {
                            if (DstEntityManager.HasComponent<MeshLODGroupComponent>(childLodGroupEntity))
                            {
                                var group = DstEntityManager.GetComponentData<MeshLODGroupComponent>(childLodGroupEntity);
                                group.ParentGroup = hlodEntity;
                                group.ParentMask = HLODMask;
                                DstEntityManager.SetComponentData(childLodGroupEntity, group);
                            }
                        }
                    }
                    
                    var outsideRenderers = CalculateRenderersOutsideLODGroups(hlod, i);
                    foreach (var r in outsideRenderers)
                    {
                        foreach (var rendererEntity in GetEntities(r))
                        {
                            if (DstEntityManager.HasComponent<RenderMesh>(rendererEntity))
                            {
                                //@TODO: Not quite sure if this makes sense. Test that this behaviour is reasonable.
                                if (DstEntityManager.HasComponent<MeshLODComponent>(rendererEntity))
                                    continue;
    
                                var lodComponent = new MeshLODComponent { Group = hlodEntity, LODMask = HLODMask  };
                                DstEntityManager.AddComponentData(rendererEntity, lodComponent);
                            }
                        }
                    }
                }
            });
        }
        
        public static Renderer[] CalculateRenderersOutsideLODGroups(HLOD hlod, int index)
        {
            var allLODRenderers = new HashSet<Renderer>();
    
            var transform = hlod.LODParentTransforms[index];
            if (transform == null)
                return new Renderer[0];
                
            var lodGroups = hlod.CalculateLODGroups(index);
            foreach (var lodGroup in lodGroups)
            {
                foreach (var lod in lodGroup.GetLODs())
                {
                    foreach (var r in lod.renderers)
                        allLODRenderers.Add(r);
                }
            }
    
            var outsideOfLODGroups = new List<Renderer>();
    
            var renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                if (!allLODRenderers.Contains(r))
                    outsideOfLODGroups.Add(r);
            }
    
            return outsideOfLODGroups.ToArray();
        }
    }
}
