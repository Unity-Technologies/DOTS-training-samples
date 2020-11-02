using Unity.Properties.UI;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Context to be used when inspecting an <see cref="Entity"/>.
    /// </summary>
    class EntityInspectorContext : InspectionContext
    {
        const string k_InvalidEntityName = "{ Invalid Entity }";

        const WorldFlags ReadonlyFlags = WorldFlags.Conversion
                                         | WorldFlags.Shadow
                                         | WorldFlags.Staging
                                         | WorldFlags.Streaming;

        internal World World { get; private set; }
        internal EntityContainer EntityContainer { get; private set; }

        internal EntityManager EntityManager => EntityContainer.EntityManager;
        internal Entity Entity => EntityContainer.Entity;
        internal bool IsReadOnly => EntityContainer.IsReadOnly;

        internal void SetContext(EntitySelectionProxy proxy)
        {
            World = proxy.World;
            var isReadonly = !EditorApplication.isPlaying || IsWorldReadOnly(World);
            // TODO: Remove once we allow to write back the data.
            isReadonly = true;
            EntityContainer = new EntityContainer(World.EntityManager, proxy.Entity, isReadonly);
        }

        internal bool TargetExists()
        {
            return World.IsCreated && EntityManager.Exists(Entity);
        }

        internal string GetTargetName()
        {
            return !TargetExists() ? k_InvalidEntityName : EntityManager.GetName(Entity);
        }

        internal void SetTargetName(string name)
        {
            if (!TargetExists())
                return;

            EntityManager.SetName(Entity, name);
        }

        internal GameObject GetOriginatingGameObject()
        {
            if (!this.TryGetComponentData(out EntityGuid guid))
                return null;

            return (GameObject)EditorUtility.InstanceIDToObject(guid.OriginatingId);
        }

        static bool IsWorldReadOnly(World world)
        {
            return (world.Flags & ReadonlyFlags) != WorldFlags.None;
        }
    }

    static class EntityInspectorContextClassExtensions
    {
        public static bool TryGetComponentData<T>(this EntityInspectorContext context, out T component)
            where T : class, IComponentData
        {
            if (!context.TargetExists() || !context.EntityManager.HasComponent<T>(context.Entity))
            {
                component = default;
                return false;
            }

            component = context.EntityManager.GetComponentData<T>(context.Entity);
            return true;
        }

        public static bool TryGetChunkComponentData<T>(this EntityInspectorContext context, out T component)
            where T : class, IComponentData
        {
            if (!context.TargetExists() || !context.EntityManager.HasComponent<T>(context.Entity))
            {
                component = default;
                return false;
            }

            component = context.EntityManager.GetChunkComponentData<T>(context.Entity);
            return true;
        }
    }

    static class EntityInspectorContextStructExtensions
    {
        public static bool TryGetComponentData<T>(this EntityInspectorContext context, out T component)
            where T : struct, IComponentData
        {
            if (!context.TargetExists() || !context.EntityManager.HasComponent<T>(context.Entity))
            {
                component = default;
                return false;
            }

            component = context.EntityManager.GetComponentData<T>(context.Entity);
            return true;
        }

        public static bool TryGetChunkComponentData<T>(this EntityInspectorContext context, out T component)
            where T : struct, IComponentData
        {
            if (!context.TargetExists() || !context.EntityManager.HasComponent<T>(context.Entity))
            {
                component = default;
                return false;
            }

            component = context.EntityManager.GetChunkComponentData<T>(context.Entity);
            return true;
        }
    }
}
