using System.Collections.Generic;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

namespace Unity.Rendering
{
    class MeshRendererConversion : GameObjectConversionSystem
    {
        const bool AttachToPrimaryEntityForSingleMaterial = true;
        
        protected override void OnUpdate()
        {
            var sceneBounds = MinMaxAABB.Empty;

            var materials = new List<Material>(10);
            Entities.ForEach((MeshRenderer meshRenderer, MeshFilter meshFilter) =>
            {
                var entity = GetPrimaryEntity(meshRenderer);
    
                var dst = new RenderMesh();
                dst.mesh = meshFilter.sharedMesh;
                dst.castShadows = meshRenderer.shadowCastingMode;
                dst.receiveShadows = meshRenderer.receiveShadows;
                dst.layer = meshRenderer.gameObject.layer;

    
                //@TODO: Transform system should handle RenderMeshFlippedWindingTag automatically. This should not be the responsibility of the conversion system.
                float4x4 localToWorld = meshRenderer.transform.localToWorldMatrix;
                var flipWinding = math.determinant(localToWorld) < 0.0;

                meshRenderer.GetSharedMaterials(materials);
                var materialCount = materials.Count; 

                if (materialCount == 1 && AttachToPrimaryEntityForSingleMaterial)
                {
                    dst.material = materials[0];
                    dst.subMesh = 0;
                
                    DstEntityManager.AddSharedComponentData(entity, dst);
                    DstEntityManager.AddComponentData(entity, new PerInstanceCullingTag());
                    
                    if (flipWinding)
                        DstEntityManager.AddComponent(entity, ComponentType.ReadWrite<RenderMeshFlippedWindingTag>());
                }
                else
                {
                    for (var m = 0; m != materialCount; m++)
                    {
                        var meshEntity = CreateAdditionalEntity(meshRenderer);
                    
                        dst.material = materials[m];
                        dst.subMesh = m;
                    
                        DstEntityManager.AddSharedComponentData(meshEntity, dst);

                        DstEntityManager.AddComponentData(meshEntity, new PerInstanceCullingTag());

                        DstEntityManager.AddComponentData(meshEntity, new LocalToWorld { Value = localToWorld });
                        if (!DstEntityManager.HasComponent<Static>(meshEntity))
                        {
                            DstEntityManager.AddComponentData(meshEntity, new Parent { Value = entity });
                            DstEntityManager.AddComponentData(meshEntity, new LocalToParent {Value = float4x4.identity});
                        }
                        
                        if (flipWinding)
                            DstEntityManager.AddComponent(meshEntity, ComponentType.ReadWrite<RenderMeshFlippedWindingTag>());
                    }
                }
                
                sceneBounds.Encapsulate(meshRenderer.bounds.ToAABB());
            });
        }
    }
}
