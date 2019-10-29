using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Unity.Entities
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface EntityManagerBaseInterfaceForObsolete
    {
    }

    public partial class World
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("BehaviourManagers have been renamed to Systems. (RemovedAfter 2019-08-25) (UnityUpgradable) -> Systems", true)]
        public IEnumerable<ComponentSystemBase> BehaviourManagers => null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("CreateManager has been renamed to CreateSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> CreateSystem(*)", true)]
        public ComponentSystemBase CreateManager(Type type, params object[] constructorArgumnents)
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetOrCreateManager has been renamed to GetOrCreateSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> GetOrCreateSystem(*)", true)]
        public ComponentSystemBase GetOrCreateManager(Type type)
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("AddManager has been renamed to AddSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> AddSystem(*)", true)]
        public T AddManager<T>(T manager) where T : ComponentSystemBase
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetExistingManager has been renamed to GetExistingSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> GetExistingSystem(*)", true)]
        public ComponentSystemBase GetExistingManager(Type type)
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("DestroyManager has been renamed to DestroySystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> DestroySystem(*)", true)]
        public void DestroyManager(ComponentSystemBase manager)
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("CreateManager has been renamed to CreateSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> CreateSystem<T>(*)", true)]
        public T CreateManager<T>(params object[] constructorArgumnents) where T : ComponentSystemBase
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetOrCreateManager has been renamed to GetOrCreateSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> GetOrCreateSystem<T>()", true)]
        public T GetOrCreateManager<T>() where T : ComponentSystemBase
        {
            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetExistingManager has been renamed to GetExistingSystem. (RemovedAfter 2019-08-25) (UnityUpgradable) -> GetExistingSystem<T>()", true)]
        public T GetExistingManager<T>() where T : ComponentSystemBase
        {
            throw new NotImplementedException();
        }
    }

    public static class WorldObsoleteExtensions {
        // special handling to handle EntityManager rename.  I can't get the script updater to rewrite this automatically via
        // (UnityUpgradable) -> EntityManager
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetExistingManager<EntityManager>() has been renamed to an EntityManager property. (RemovedAfter 2019-08-25)")]
        public static EntityManager GetExistingManager<T>(this World world) where T : EntityManagerBaseInterfaceForObsolete
        {
            return world.EntityManager;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetOrCreateManager<EntityManager>() has been renamed to an EntityManager property.(RemovedAfter 2019-08-25)")]
        public static EntityManager GetOrCreateManager<T>(this World world) where T : EntityManagerBaseInterfaceForObsolete
        {
            return world.EntityManager;
        }

        // Include System API name variants, even though we never had this API -- the script updater will likely
        // aggressively update *Manager to *System
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetExistingSystem<EntityManager>() has been renamed to an EntityManager property. (RemovedAfter 2019-08-25)")]
        public static EntityManager GetExistingSystem<T>(this World world) where T : EntityManagerBaseInterfaceForObsolete
        {
            return world.EntityManager;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("GetOrCreateSystem<EntityManager>() has been renamed to an EntityManager property. (RemovedAfter 2019-08-25)")]
        public static EntityManager GetOrCreateSystem<T>(this World world) where T : EntityManagerBaseInterfaceForObsolete
        {
            return world.EntityManager;
        }
    }
}
