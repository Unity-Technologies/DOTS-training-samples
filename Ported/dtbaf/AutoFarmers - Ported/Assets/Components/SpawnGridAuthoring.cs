using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
[AddComponentMenu("DOTS Samples/SpawnConversion/SpawnGrid")]
public class SpawnGridAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int ColumnCount;
    public int RowCount;
    public GameObject Prefab;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var meshRenderer = Prefab.GetComponent<MeshRenderer>();
        var meshFilter = Prefab.GetComponent<MeshFilter>();
        var materials = new List<Material>(10);
        var mesh = meshFilter.sharedMesh;
        meshRenderer.GetSharedMaterials(materials);

        var cx = ((float)(RowCount / 2) * 1.1f) + 0.5f;
        var cz = ((float)(ColumnCount / 2) * 1.1f) + 0.5f;
        for (int x = 0; x < RowCount; x++)
        for (int y = 0; y < ColumnCount; y++)
        {
            var tx = (((float)x) * 1.1f) - cx;
            var tz = (((float)y) * 1.1f) - cz;

            var segmentEntity = conversionSystem.CreateAdditionalEntity(gameObject);
            var pos = new float3(tx, 0.0f, tz);

            var localToWorld = new LocalToWorld
            {
                Value = float4x4.Translate(pos)
            };
            var aabb = new AABB
            {
                Center = pos,
                Extents = new float3(0.5f, 0.5f, 0.5f)
            };
            var worldRenderBounds = new WorldRenderBounds
            {
                Value = aabb
            };

            dstManager.AddComponentData(segmentEntity, localToWorld);
            dstManager.AddComponentData(segmentEntity, worldRenderBounds);
            dstManager.AddComponent(segmentEntity, ComponentType.ChunkComponent<ChunkWorldRenderBounds>());
            dstManager.AddComponent(segmentEntity, typeof(Frozen));

            Convert(segmentEntity, dstManager, conversionSystem, meshRenderer, mesh, materials);
        }
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

            if (materialCount == 1)
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
