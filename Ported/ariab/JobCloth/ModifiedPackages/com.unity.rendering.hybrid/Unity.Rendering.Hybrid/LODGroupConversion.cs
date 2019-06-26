using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[UpdateAfter(typeof(MeshRendererConversion))]
class LODGroupConversion : GameObjectConversionSystem
{    
    protected override void OnUpdate()
    {
        Entities.ForEach((LODGroup lodGroup) =>
        {
            if (lodGroup.lodCount > 8)
            {
                Debug.LogWarning("LODGroup has more than 8 LOD - Not supported", lodGroup);
                return;
            }

            var lodGroupEntity = GetPrimaryEntity(lodGroup);

            var lodGroupData = new MeshLODGroupComponent(); 
            //@TODO: LOD calculation should respect scale...
            var worldSpaceSize = LODGroupExtensions.GetWorldSpaceSize(lodGroup);
            lodGroupData.LocalReferencePoint = lodGroup.localReferencePoint;
            
            var lodDistances0 = new float4(float.PositiveInfinity);
            var lodDistances1 = new float4(float.PositiveInfinity);
            var lodGroupLODs = lodGroup.GetLODs();
            for (int i = 0; i < lodGroup.lodCount; ++i)
            {
                float d = worldSpaceSize / lodGroupLODs[i].screenRelativeTransitionHeight;
                if (i < 4)
                    lodDistances0[i] = d;
                else
                    lodDistances1[i - 4] = d;
            }

            lodGroupData.LODDistances0 = lodDistances0;
            lodGroupData.LODDistances1 = lodDistances1;

            DstEntityManager.AddComponentData(lodGroupEntity, lodGroupData);
            
            for (int i = 0; i < lodGroupLODs.Length; ++i)
            {
                foreach (var renderer in lodGroupLODs[i].renderers)
                {
                    if (renderer == null)
                    {
                        Debug.LogWarning("Missing renderer in LOD Group", lodGroup);
                        continue;
                    }
                
                    foreach (var rendererEntity in GetEntities(renderer))
                    {
                        if (DstEntityManager.HasComponent<RenderMesh>(rendererEntity))
                        {
                            var lodComponent = new MeshLODComponent { Group = lodGroupEntity, LODMask = 1 << i };
                            if (!DstEntityManager.HasComponent<MeshLODComponent>(rendererEntity))
                                DstEntityManager.AddComponentData(rendererEntity, lodComponent);
                            else
                            {
                                var previousLODComponent = DstEntityManager.GetComponentData<MeshLODComponent>(rendererEntity);
                                if (previousLODComponent.Group != lodComponent.Group)
                                {
                                    Debug.LogWarning("A renderer can not be in multiple different LODGroup.", renderer);
                                    continue;
                                }

                                if ((previousLODComponent.LODMask & (1 << (i-1))) == 0)
                                    Debug.LogWarning("A renderer that is present in the same LODGroup multiple times must be in consecutive LOD levels.", renderer);

                                lodComponent.LODMask |= previousLODComponent.LODMask;
                                DstEntityManager.SetComponentData(rendererEntity, lodComponent);                                   
                            }
                        }
                    }
                }
            }
        });
    }
}
