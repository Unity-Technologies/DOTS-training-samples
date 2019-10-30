using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Unity.Rendering
{
    [ConverterVersion(1)]
    class SkinnedMeshRendererConversion : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            var materials = new List<Material>(10);
            Entities.ForEach((SkinnedMeshRenderer meshRenderer) =>
            {
                var entity = GetPrimaryEntity(meshRenderer);

                meshRenderer.GetSharedMaterials(materials);

                MeshRendererConversion.Convert(entity, DstEntityManager, this, meshRenderer, meshRenderer.sharedMesh, materials);

                // Find the converted entities and add relevant components for skinned meshes.
                foreach (var rendererEntity in GetEntities(meshRenderer))
                {
                    if (DstEntityManager.HasComponent<RenderMesh>(rendererEntity))
                    {
                        DstEntityManager.AddComponentData(rendererEntity, new SkinnedEntityReference { Value = entity });
                        DstEntityManager.AddComponentData(rendererEntity, new BoneIndexOffsetMaterialProperty());
                    }
                }
            });
        }
    }
}