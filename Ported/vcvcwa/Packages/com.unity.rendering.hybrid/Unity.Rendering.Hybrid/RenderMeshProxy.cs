using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.Rendering
{
    /// <summary>
    /// Defines the mesh and rendering properties of an entity.
    /// </summary>
    /// <remarks>
    /// Add a RenderMesh component to an entity to define its graphical attributes. The entity must also have a
    /// LocalToWorld component from the Unity.Transforms namespace.
    ///
    /// The standard ECS conversion systems add RenderMesh components to entities created from GameObjects that contain
    /// [UnityEngine.MeshRenderer](https://docs.unity3d.com/ScriptReference/MeshRenderer.html) and
    /// [UnityEngine.MeshFilter](https://docs.unity3d.com/ScriptReference/MeshFilter.html) components.
    ///
    /// RenderMesh is a shared component, which means all entities of the same Archetype and same RenderMesh settings
    /// are stored together in the same chunks of memory. The rendering system batches the entities together to reduce
    /// the number of draw calls.
    /// </remarks>
    [Serializable]
    // Culling system requires a maximum of 128 entities per chunk (See ChunkInstanceLodEnabled)
    [MaximumChunkCapacity(128)]
    public struct RenderMesh : ISharedComponentData, IEquatable<RenderMesh>
    {
        /// <summary>
        /// A reference to a [UnityEngine.Mesh](https://docs.unity3d.com/ScriptReference/Mesh.html) object.
        /// </summary>
        public Mesh                 mesh;
        /// <summary>
        /// A reference to a [UnityEngine.Material](https://docs.unity3d.com/ScriptReference/Material.html) object.
        /// </summary>
        /// <remarks>For efficient rendering, the material should enable GPU instancing.
        /// For entities converted from GameObjects, this value is derived from the Materials array of the source
        /// Mesh Renderer Component.
        /// </remarks>
        public Material             material;
        /// <summary>
        /// The submesh index.
        /// </summary>
        public int                  subMesh;

        /// <summary>
        /// The [LayerMask](https://docs.unity3d.com/ScriptReference/LayerMask.html) index.
        /// </summary>
        /// <remarks>
        /// For entities converted from GameObjects, this value is copied from the Layer setting of the source
        /// GameObject.
        /// </remarks>
        [LayerField]
        public int                  layer;

        /// <summary>
        /// How shadows are cast.
        /// </summary>
        /// <remarks>See [ShadowCastingMode](https://docs.unity3d.com/ScriptReference/Rendering.ShadowCastingMode.html).
        /// For entities converted from GameObjects, this value is copied from the Cast Shadows property of the source
        /// Mesh Renderer Component.
        /// </remarks>
        public ShadowCastingMode    castShadows;
        /// <summary>
        /// Whether shadows should be cast onto the object.
        /// </summary>
        /// <remarks>[Progressive Lightmappers](https://docs.unity3d.com/Manual/ProgressiveLightmapper.html) only.
        /// For entities converted from GameObjects, this value is copied from the Receive Shadows property of the source
        /// Mesh Renderer Component.
        /// </remarks>
        public bool                 receiveShadows;

        /// <summary>
        /// Two RenderMesh objects are equal if their respective property values are equal.
        /// </summary>
        /// <param name="other">Another RenderMesh.</param>
        /// <returns>True, if the properties of both RenderMeshes are equal.</returns>
        public bool Equals(RenderMesh other)
        {
            return
                mesh == other.mesh &&
                material == other.material &&
                subMesh == other.subMesh &&
                layer == other.layer &&
                castShadows == other.castShadows &&
                receiveShadows == other.receiveShadows;
        }

        /// <summary>
        /// A representative hash code.
        /// </summary>
        /// <returns>A number that is guaranteed to be the same when generated from two objects that are the same.</returns>
        public override int GetHashCode()
        {
            int hash = 0;
            if (!ReferenceEquals(mesh, null)) hash ^= mesh.GetHashCode();
            if (!ReferenceEquals(material, null)) hash ^= material.GetHashCode();
            hash ^= subMesh.GetHashCode();
            hash ^= layer.GetHashCode();
            hash ^= castShadows.GetHashCode();
            hash ^= receiveShadows.GetHashCode();
            return hash;
        }
    }

    [AddComponentMenu("DOTS/Deprecated/RenderMeshProxy-Deprecated")]
    public class RenderMeshProxy : SharedComponentDataProxy<RenderMesh>
    {
        internal override void UpdateComponentData(EntityManager manager, Entity entity)
        {
            // Hack to make rendering not break if there is no local to world
            if (!manager.HasComponent<LocalToWorld>(entity))
                manager.AddComponentData(entity, new LocalToWorld {Value = float4x4.identity});

            base.UpdateComponentData(manager, entity);
        }
    }
}
