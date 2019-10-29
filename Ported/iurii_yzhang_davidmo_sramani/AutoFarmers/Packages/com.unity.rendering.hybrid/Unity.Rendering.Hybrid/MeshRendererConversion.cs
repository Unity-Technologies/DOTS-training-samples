using System.Collections.Generic;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

namespace Unity.Rendering
{
    [ConverterVersion(1)]
    class MeshRendererConversion : GameObjectConversionSystem
    {
        const bool AttachToPrimaryEntityForSingleMaterial = true;

        protected override void OnUpdate()
        {
            var materials = new List<Material>(10);
            Entities.ForEach((MeshRenderer meshRenderer, MeshFilter meshFilter) =>
            {
                var entity = GetPrimaryEntity(meshRenderer);
                var mesh = meshFilter.sharedMesh;
                meshRenderer.GetSharedMaterials(materials);

                Convert(entity, DstEntityManager, this, meshRenderer, mesh, materials);
            });
        }

        public static void Convert(
                Entity entity,
                EntityManager dstEntityManager,
                GameObjectConversionSystem conversionSystem, 
                Renderer meshRenderer, 
                Mesh mesh, 
                List<Material> materials)
        {
            var materialCount = materials.Count;

            // Don't add RenderMesh (and other required components) unless both mesh and material assigned.
            if ((mesh != null) && (materialCount > 0))
            {
                var renderMesh = new RenderMesh
                {
                    mesh = mesh,
                    castShadows = meshRenderer.shadowCastingMode,
                    receiveShadows = meshRenderer.receiveShadows,
                    layer = meshRenderer.gameObject.layer
                };

                //@TODO: Transform system should handle RenderMeshFlippedWindingTag automatically. This should not be the responsibility of the conversion system.
                float4x4 localToWorld = meshRenderer.transform.localToWorldMatrix;
                var flipWinding = math.determinant(localToWorld) < 0.0;

                if (materialCount == 1 && AttachToPrimaryEntityForSingleMaterial)
                {
                    renderMesh.material = materials[0];
                    renderMesh.subMesh = 0;

                    dstEntityManager.AddSharedComponentData(entity, renderMesh);

                    dstEntityManager.AddComponentData(entity, new PerInstanceCullingTag());
                    dstEntityManager.AddComponentData(entity, new RenderBounds { Value = mesh.bounds.ToAABB() });

                    if (flipWinding)
                        dstEntityManager.AddComponent(entity, ComponentType.ReadWrite<RenderMeshFlippedWindingTag>());

                    conversionSystem.ConfigureEditorRenderData(entity, meshRenderer.gameObject, true);
                }
                else
                {
                    for (var m = 0; m != materialCount; m++)
                    {
                        var meshEntity = conversionSystem.CreateAdditionalEntity(meshRenderer);

                        renderMesh.material = materials[m];
                        renderMesh.subMesh = m;

                        dstEntityManager.AddSharedComponentData(meshEntity, renderMesh);

                        dstEntityManager.AddComponentData(meshEntity, new PerInstanceCullingTag());
                        dstEntityManager.AddComponentData(meshEntity, new RenderBounds { Value = mesh.bounds.ToAABB() });
                        dstEntityManager.AddComponentData(meshEntity, new LocalToWorld { Value = localToWorld });

                        if (!dstEntityManager.HasComponent<Static>(meshEntity))
                        {
                            dstEntityManager.AddComponentData(meshEntity, new Parent { Value = entity });
                            dstEntityManager.AddComponentData(meshEntity, new LocalToParent { Value = float4x4.identity });
                        }

                        if (flipWinding)
                            dstEntityManager.AddComponent(meshEntity, ComponentType.ReadWrite<RenderMeshFlippedWindingTag>());

                        conversionSystem.ConfigureEditorRenderData(meshEntity, meshRenderer.gameObject, true);
                    }
                }
            }
        }
    }
}