using System;
using System.Collections.Generic;
using Unity.Entities.Conversion;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using static Unity.Debug;

namespace Unity.Entities
{
    [DisallowMultipleComponent]
    [AddComponentMenu("DOTS/Convert To Entity")]
    public class ConvertToEntity : MonoBehaviour
    {
        public enum Mode
        {
            ConvertAndDestroy,
            ConvertAndInjectGameObject
        }

        public Mode ConversionMode;

        void Awake()
        {
            if (World.DefaultGameObjectInjectionWorld != null)
            {
                var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
                system.AddToBeConverted(World.DefaultGameObjectInjectionWorld, this);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"{nameof(ConvertToEntity)} failed because there is no {nameof(World.DefaultGameObjectInjectionWorld)}", this);
            }
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ConvertToEntitySystem : ComponentSystem
    {
        Dictionary<World, List<ConvertToEntity>> m_ToBeConverted = new Dictionary<World, List<ConvertToEntity>>();

        public BlobAssetStore BlobAssetStore { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            BlobAssetStore = new BlobAssetStore();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (BlobAssetStore != null)
            {
                BlobAssetStore.Dispose();
                BlobAssetStore = null;
            }
        }

        // using `this.World` is a sign of a problem - that World is only needed so that this system will update, but
        // adding entities to it directly is wrong (must be directed via m_ToBeConverted).
        // ReSharper disable once UnusedMember.Local
        new World World => throw new InvalidOperationException($"Do not use `this.World` directly (use {nameof(m_ToBeConverted)})");

        protected override void OnUpdate()
        {
            if (m_ToBeConverted.Count != 0)
                Convert();
        }

        public void AddToBeConverted(World world, ConvertToEntity convertToEntity)
        {
            if (!m_ToBeConverted.TryGetValue(world, out var list))
            {
                list = new List<ConvertToEntity>();
                m_ToBeConverted.Add(world, list);
            }
            list.Add(convertToEntity);
        }

        static bool IsConvertAndInject(GameObject go)
        {
            var mode = go.GetComponent<ConvertToEntity>()?.ConversionMode;
            return mode == ConvertToEntity.Mode.ConvertAndInjectGameObject;
        }

        static void AddRecurse(EntityManager manager, Transform transform, HashSet<Transform> toBeDetached, List<Transform> toBeInjected)
        {
            if (transform.GetComponent<StopConvertToEntity>() != null)
            {
                toBeDetached.Add(transform);
                return;
            }

            GameObjectEntity.AddToEntityManager(manager, transform.gameObject);

            if (IsConvertAndInject(transform.gameObject))
            {
                toBeDetached.Add(transform);
                toBeInjected.Add(transform);
            }
            else
            {
                foreach (Transform child in transform)
                    AddRecurse(manager, child, toBeDetached, toBeInjected);
            }
        }

        static void InjectOriginalComponents(GameObjectConversionMappingSystem mappingSystem, Transform transform)
        {
            var entity = mappingSystem.GetPrimaryEntity(transform.gameObject);
            foreach (var com in transform.GetComponents<Component>())
            {
                if (com is GameObjectEntity || com is ConvertToEntity || com is ComponentDataProxyBase || com is StopConvertToEntity)
                    continue;

                mappingSystem.DstEntityManager.AddComponentObject(entity, com);
            }
        }

        void Convert()
        {
            var toBeDetached = new HashSet<Transform>();
            var toBeDestroyed = new HashSet<GameObject>();

            try
            {
                var toBeInjected = new List<Transform>();

                foreach (var convertToWorld in m_ToBeConverted)
                {
                    var toBeConverted = convertToWorld.Value;

                    var settings = new GameObjectConversionSettings(
                        convertToWorld.Key,
                        GameObjectConversionUtility.ConversionFlags.AssignName);
                    
                    settings.blobAssetStore = BlobAssetStore;

                    using (var gameObjectWorld = settings.CreateConversionWorld())
                    {
                        toBeConverted.RemoveAll(convert =>
                        {
                            var parent = convert.transform.parent;
                            var remove = parent != null && parent.GetComponentInParent<ConvertToEntity>() != null;
                            if (remove && parent.GetComponentInParent<StopConvertToEntity>() != null)
                            {
                                LogWarning(
                                    $"{nameof(ConvertToEntity)} will be ignored because of a {nameof(StopConvertToEntity)} higher in the hierarchy",
                                    convert.gameObject);
                            }

                            return remove;
                        });

                        foreach (var convert in toBeConverted)
                            toBeDestroyed.Add(convert.gameObject);

                        foreach (var convert in toBeConverted)
                            AddRecurse(gameObjectWorld.EntityManager, convert.transform, toBeDetached, toBeInjected);

                        GameObjectConversionUtility.Convert(gameObjectWorld);

                        var mappingSystem = gameObjectWorld.GetExistingSystem<GameObjectConversionMappingSystem>();
                        foreach (var convert in toBeInjected)
                            InjectOriginalComponents(mappingSystem, convert);
                    }

                    toBeInjected.Clear();
                }
            }
            finally
            {
                m_ToBeConverted.Clear();

                foreach (var transform in toBeDetached)
                    transform.parent = null;

                foreach (var go in toBeDestroyed)
                    UnityObject.DestroyImmediate(go);
            }
        }
    }
}
