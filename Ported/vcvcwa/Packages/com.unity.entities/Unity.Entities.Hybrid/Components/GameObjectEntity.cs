using System;
using System.Collections.Generic;
using Unity.Entities.Conversion;
using UnityEngine;
using UnityEngine.Assertions;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using GameObject = UnityEngine.GameObject;
using Component = UnityEngine.Component;

namespace Unity.Entities
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [AddComponentMenu("")]
    public class GameObjectEntity : MonoBehaviour
    {
        public EntityManager EntityManager
        {
            get
            {
                if (enabled && gameObject.activeInHierarchy)
                    ReInitializeEntityManagerAndEntityIfNecessary();
                return m_EntityManager;
            }
        }
        EntityManager m_EntityManager;

        public Entity Entity
        {
            get
            {
                if (enabled && gameObject.activeInHierarchy)
                    ReInitializeEntityManagerAndEntityIfNecessary();
                return m_Entity;
            }
        }
        Entity m_Entity;

        void ReInitializeEntityManagerAndEntityIfNecessary()
        {
            // in case e.g., on a prefab that was open for edit when domain was unloaded
            // existing m_EntityManager lost all its data, so simply create a new one
            if (m_EntityManager != null && !m_EntityManager.IsCreated && !m_Entity.Equals(default))
                Initialize();
        }

        // TODO: Very wrong error messages when creating entity with empty ComponentType array?
        public static Entity AddToEntityManager(EntityManager entityManager, GameObject gameObject)
        {
            GetComponents(gameObject, true, out var types, out var components);

            EntityArchetype archetype;
            try
            {
                archetype = entityManager.CreateArchetype(types);
            }
            catch (Exception)
            {
                for (int i = 0; i < types.Length; ++i)
                {
                    if (Array.IndexOf(types, types[i]) != i)
                    {
                        Debug.LogWarning($"GameObject '{gameObject}' has multiple {types[i]} components and cannot be converted, skipping.");
                        return Entity.Null;
                    }
                }

                throw;
            }

            var entity = CreateEntity(entityManager, archetype, components, types);

            return entity;
        }

        //@TODO: is this used? deprecate?
        public static void AddToEntity(EntityManager entityManager, GameObject gameObject, Entity entity)
        {
            var components = gameObject.GetComponents<Component>();

            for (var i = 0; i != components.Length; i++)
            {
                var component = components[i];
                if (component == null || component is ComponentDataProxyBase || component is GameObjectEntity || component.IsComponentDisabled())
                    continue;

                entityManager.AddComponentObject(entity, component);
            }
        }

        static void GetComponents(GameObject gameObject, bool includeGameObjectComponents, out ComponentType[] types, out Component[] components)
        {            
            components = gameObject.GetComponents<Component>();

            var componentCount = 0;
            for (var i = 0; i != components.Length; i++)
            {
                var component = components[i];
                if (component == null)
                {
                    UnityEngine.Debug.LogWarning($"The referenced script is missing on {gameObject.name}", gameObject);
                    continue;
                }

                if (component is ComponentDataProxyBase)
                    componentCount++;
                else if (includeGameObjectComponents && !(component is GameObjectEntity) && !component.IsComponentDisabled())
                    componentCount++;
                else
                    components[i] = null;
            }

            types = new ComponentType[componentCount];

            var t = 0;
            for (var i = 0; i != components.Length; i++)
            {
                var component = components[i];
                if (component == null)
                    continue;

                if (component is ComponentDataProxyBase proxy)
                    types[t++] = proxy.GetComponentType();
                else
                    types[t++] = component.GetType();
            }

            Assert.AreEqual(t, types.Length);
        }

        static Entity CreateEntity(EntityManager entityManager, EntityArchetype archetype, IReadOnlyList<Component> components, IReadOnlyList<ComponentType> types)
        {
            var entity = entityManager.CreateEntity(archetype);
            var t = 0;
            for (var i = 0; i != components.Count; i++)
            {
                var component = components[i];
                if (component == null)
                    continue;

                if (component is ComponentDataProxyBase proxy)
                {
                    proxy.UpdateComponentData(entityManager, entity);
                    t++;
                }
                else
                {
                    entityManager.SetComponentObject(entity, types[t], component);
                    t++;
                }
            }
            return entity;
        }

        void Initialize()
        {
            DefaultWorldInitialization.DefaultLazyEditModeInitialize();
            if (World.DefaultGameObjectInjectionWorld != null)
            {
                m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                m_Entity = AddToEntityManager(m_EntityManager, gameObject);
            }
        }

        protected virtual void OnEnable()
        {
            Initialize();
        }

        protected virtual void OnDisable()
        {
            if (EntityManager != null && EntityManager.IsCreated && EntityManager.Exists(Entity))
                EntityManager.DestroyEntity(Entity);

            m_EntityManager = null;
            m_Entity = Entity.Null;
        }

        public static void CopyAllComponentsToEntity(GameObject gameObject, EntityManager entityManager, Entity entity)
        {
            foreach (var proxy in gameObject.GetComponents<ComponentDataProxyBase>())
            {
                // TODO: handle shared components and tag components
                var type = proxy.GetComponentType();
                entityManager.AddComponent(entity, type);
                proxy.UpdateComponentData(entityManager, entity);
            }
        }
    }
}
