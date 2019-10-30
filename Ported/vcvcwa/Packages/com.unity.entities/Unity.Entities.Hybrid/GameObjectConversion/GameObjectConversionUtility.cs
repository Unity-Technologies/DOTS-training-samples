using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities.Conversion;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

#pragma warning disable 162

namespace Unity.Entities
{
    public static class GameObjectConversionUtility
    {
        static ProfilerMarker s_ConvertScene = new ProfilerMarker("GameObjectConversionUtility.ConvertScene");
        static ProfilerMarker s_CreateConversionWorld = new ProfilerMarker("Create World & Systems");
        static ProfilerMarker s_DestroyConversionWorld = new ProfilerMarker("DestroyWorld");
        static ProfilerMarker s_CreateEntitiesForGameObjects = new ProfilerMarker("CreateEntitiesForGameObjects");
        static ProfilerMarker s_UpdateConversionSystems = new ProfilerMarker("UpdateConversionSystems");
        static ProfilerMarker s_UpdateExportSystems = new ProfilerMarker("UpdateExportSystems");
        static ProfilerMarker s_CreateCompanionGameObjects = new ProfilerMarker("CreateCompanionGameObjects");
        static ProfilerMarker s_AddPrefabComponentDataTag = new ProfilerMarker("AddPrefabComponentDataTag");
        static ProfilerMarker s_GenerateLinkedEntityGroups = new ProfilerMarker("GenerateLinkedEntityGroups");

        [Flags]
        public enum ConversionFlags : uint
        {
            AddEntityGUID = 1 << 0,
            ForceStaticOptimization = 1 << 1,
            AssignName = 1 << 2,
            SceneViewLiveLink = 1 << 3,
            GameViewLiveLink = 1 << 4,
        }

        internal static World CreateConversionWorld(GameObjectConversionSettings settings)
        {
            using (s_CreateConversionWorld.Auto())
            {
                var gameObjectWorld = new World($"GameObject -> Entity Conversion '{settings.DebugConversionName}'");
                gameObjectWorld.CreateSystem<GameObjectConversionMappingSystem>(settings);

                var systemTypes = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.GameObjectConversion);
                systemTypes.AddRange(settings.ExtraSystems);

                var includeExport = settings.GetType() != typeof(GameObjectConversionSettings);
                AddConversionSystems(gameObjectWorld, systemTypes, includeExport);

                settings.ConversionWorldCreated?.Invoke(gameObjectWorld);

                return gameObjectWorld;
            }
        }

        struct DeclaredReferenceObjectsTag : IComponentData { }

        static void DeclareReferencedObjects(World gameObjectWorld, GameObjectConversionMappingSystem mappingSystem)
        {
            var newAllEntitiesQuery = mappingSystem.Entities
                .WithNone<DeclaredReferenceObjectsTag>()
                .ToEntityQuery();

            //@TODO: Revert this again once KevinM adds support for inheritance in queries
            //var newGoEntitiesQuery = mappingSystem.Entities
            //    .WithNone<DeclaredReferenceObjectsTag>()
            //    .WithAll<Transform>()
            //    .ToEntityQuery();
            var newGoEntitiesQuery = mappingSystem.GetEntityQuery(
                new EntityQueryDesc
                {
                    None = new ComponentType[] { typeof(DeclaredReferenceObjectsTag) },
                    All = new ComponentType[] { typeof(Transform) }
                },
                new EntityQueryDesc
                {
                    None = new ComponentType[] { typeof(DeclaredReferenceObjectsTag) },
                    All = new ComponentType[] { typeof(RectTransform) }
                });

            var prefabDeclarers = new List<IDeclareReferencedPrefabs>();
            var declaredPrefabs = new List<GameObject>();

            // loop until no new entities discovered that might need following
            while (!newAllEntitiesQuery.IsEmptyIgnoreFilter)
            {
                using (var newGoEntities = newGoEntitiesQuery.ToEntityArray(Allocator.TempJob))
                {
                    // fetch components that implement IDeclareReferencedPrefabs
                    foreach (var newGoEntity in newGoEntities)
                    {
                        //@TODO: Revert this again once we add support for inheritance in queries
                        //gameObjectWorld.EntityManager.GetComponentObject<Transform>(newGoEntity).GetComponents(prefabDeclarers);
                        ((Transform)gameObjectWorld.EntityManager.Debug.GetComponentBoxed(newGoEntity, typeof(Transform))).GetComponents(prefabDeclarers);

                        // let each component declare any prefab refs it knows about
                        foreach (var prefabDeclarer in prefabDeclarers)
                            prefabDeclarer.DeclareReferencedPrefabs(declaredPrefabs);

                        prefabDeclarers.Clear();
                    }
                }

                // mark as seen for next loop
                gameObjectWorld.EntityManager.AddComponent<DeclaredReferenceObjectsTag>(newAllEntitiesQuery);

                foreach (var declaredPrefab in declaredPrefabs)
                    mappingSystem.DeclareReferencedPrefab(declaredPrefab);
                declaredPrefabs.Clear();

                // give systems a chance to declare prefabs and assets
                gameObjectWorld.GetExistingSystem<GameObjectDeclareReferencedObjectsGroup>().Update();
            }

            // clean up the markers
            gameObjectWorld.EntityManager.RemoveComponent<DeclaredReferenceObjectsTag>(gameObjectWorld.EntityManager.UniversalQuery);
        }

        struct Conversion : IDisposable
        {
            public GameObjectConversionMappingSystem MappingSystem { get; }

            public Conversion(World conversionWorld)
            {
                MappingSystem = conversionWorld.GetExistingSystem<GameObjectConversionMappingSystem>();
                MappingSystem.BeginConversion();
            }

            public void Dispose()
            {
                MappingSystem.EndConversion();
            }
        }

        internal static void Convert(World conversionWorld)
        {
            using (var conversion = new Conversion(conversionWorld))
            {
                using (s_UpdateConversionSystems.Auto())
                {
                    DeclareReferencedObjects(conversionWorld, conversion.MappingSystem);

                    conversion.MappingSystem.CreatePrimaryEntities();

                    conversionWorld.GetExistingSystem<GameObjectBeforeConversionGroup>().Update();
                    conversionWorld.GetExistingSystem<GameObjectConversionGroup>().Update();
                    conversionWorld.GetExistingSystem<GameObjectAfterConversionGroup>().Update();
                }

                using (s_AddPrefabComponentDataTag.Auto())
                    conversion.MappingSystem.AddPrefabComponentDataTag();

#if !UNITY_DISABLE_MANAGED_COMPONENTS
                using (s_CreateCompanionGameObjects.Auto())
                    conversion.MappingSystem.CreateCompanionGameObjects();
#endif

                using (s_GenerateLinkedEntityGroups.Auto())
                    conversion.MappingSystem.GenerateLinkedEntityGroups();

                using (s_UpdateExportSystems.Auto())
                    conversionWorld.GetExistingSystem<GameObjectExportGroup>()?.Update();
            }
        }

        public static void ConvertIncremental(World conversionWorld, IEnumerable<GameObject> gameObjects, ConversionFlags flags)
        {
            using (var conversion = new Conversion(conversionWorld))
            {
                conversion.MappingSystem.PrepareIncrementalConversion(gameObjects, flags);

                using (s_UpdateConversionSystems.Auto())
                {
                    conversionWorld.GetExistingSystem<GameObjectBeforeConversionGroup>().Update();
                    conversionWorld.GetExistingSystem<GameObjectConversionGroup>().Update();
                    conversionWorld.GetExistingSystem<GameObjectAfterConversionGroup>().Update();
                }

                using (s_GenerateLinkedEntityGroups.Auto())
                    conversion.MappingSystem.GenerateLinkedEntityGroups();

                conversionWorld.EntityManager.DestroyEntity(conversionWorld.EntityManager.UniversalQuery);

                // @TODO: Eventually we need to do incremental conversion of prefabs
                // mappingSystem.AddPrefabComponentDataTag();

                // @TODO: Eventually we need to figure out how to handle hybrid components incrementally
                // mappingSystem.AddHybridComponents();
            }
        }

        public static World ConvertIncrementalInitialize(Scene scene, GameObjectConversionSettings settings)
        {
            using (s_ConvertScene.Auto())
            {
                var conversionWorld = CreateConversionWorld(settings);
                using (var conversion = new Conversion(conversionWorld))
                {
                    using (s_CreateEntitiesForGameObjects.Auto())
                        conversion.MappingSystem.CreateEntitiesForGameObjects(scene);

                    Convert(conversionWorld);

                    conversionWorld.EntityManager.DestroyEntity(conversionWorld.EntityManager.UniversalQuery);
                }

                return conversionWorld;
            }
        }

        internal static Entity GameObjectToConvertedEntity(World gameObjectWorld, GameObject gameObject)
        {
            var mappingSystem = gameObjectWorld.GetExistingSystem<GameObjectConversionMappingSystem>();
            return mappingSystem.GetPrimaryEntity(gameObject);
        }

        public static Entity ConvertGameObjectHierarchy(GameObject root, GameObjectConversionSettings settings)
        {
            using (s_ConvertScene.Auto())
            {
                Entity convertedEntity;
                using (var conversionWorld = CreateConversionWorld(settings))
                using (var conversion = new Conversion(conversionWorld))
                {
                    using (s_CreateEntitiesForGameObjects.Auto())
                        conversion.MappingSystem.AddGameObjectOrPrefab(root);

                    Convert(conversionWorld);

                    convertedEntity = conversion.MappingSystem.GetPrimaryEntity(root);

                    settings.ConversionWorldPreDispose?.Invoke(conversionWorld);

                    s_DestroyConversionWorld.Begin();
                }

                s_DestroyConversionWorld.End();
                return convertedEntity;
            }
        }

        public static void ConvertScene(Scene scene, GameObjectConversionSettings settings)
        {
            using (s_ConvertScene.Auto())
            {
                using (var conversionWorld = CreateConversionWorld(settings))
                using (var conversion = new Conversion(conversionWorld))
                {
                    using (s_CreateEntitiesForGameObjects.Auto())
                        conversion.MappingSystem.CreateEntitiesForGameObjects(scene);

                    Convert(conversionWorld);

                    settings.ConversionWorldPreDispose?.Invoke(conversionWorld);

                    s_DestroyConversionWorld.Begin();
                }
                s_DestroyConversionWorld.End();
            }
        }

        static void AddConversionSystems(World gameObjectWorld, IEnumerable<Type> systemTypes, bool includeExport)
        {
            var declareConvert = gameObjectWorld.GetOrCreateSystem<GameObjectDeclareReferencedObjectsGroup>();
            var earlyConvert = gameObjectWorld.GetOrCreateSystem<GameObjectBeforeConversionGroup>();
            var convert = gameObjectWorld.GetOrCreateSystem<GameObjectConversionGroup>();
            var lateConvert = gameObjectWorld.GetOrCreateSystem<GameObjectAfterConversionGroup>();

            var export = includeExport ? gameObjectWorld.GetOrCreateSystem<GameObjectExportGroup>() : null;

            foreach (var systemType in systemTypes)
            {
                var updateInGroupAttrs = systemType.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);
                if (updateInGroupAttrs.Length == 0)
                {
                    AddSystemAndLogException(gameObjectWorld, convert, systemType);
                }
                else
                {
                    foreach (var attribute in updateInGroupAttrs)
                    {
                        var groupType = (attribute as UpdateInGroupAttribute)?.GroupType;

                        if (groupType == declareConvert.GetType())
                        {
                            AddSystemAndLogException(gameObjectWorld, declareConvert, systemType);
                        }
                        else if (groupType == earlyConvert.GetType())
                        {
                            AddSystemAndLogException(gameObjectWorld, earlyConvert, systemType);
                        }
                        else if (groupType == convert.GetType())
                        {
                            AddSystemAndLogException(gameObjectWorld, convert, systemType);
                        }
                        else if (groupType == lateConvert.GetType())
                        {
                            AddSystemAndLogException(gameObjectWorld, lateConvert, systemType);
                        }
                        else if (groupType == typeof(GameObjectExportGroup))
                        {
                            if (export != null)
                                AddSystemAndLogException(gameObjectWorld, export, systemType);
                        }
                        else
                        {
                            Debug.LogWarning($"{systemType} has invalid UpdateInGroup[typeof({groupType}]");
                        }
                    }
                }
            }

            declareConvert.SortSystemUpdateList();
            earlyConvert.SortSystemUpdateList();
            convert.SortSystemUpdateList();
            lateConvert.SortSystemUpdateList();
            export?.SortSystemUpdateList();
        }

        static void AddSystemAndLogException(World world, ComponentSystemGroup group, Type type)
        {
            try
            {
                var system = world.GetOrCreateSystem(type);
#if UNITY_EDITOR
                if (system is GameObjectConversionSystem conversionSystem)
                {
                    // TODO we should log all conversion systems and their enabled/disabled state
                    system.Enabled = conversionSystem.ShouldRunConversionSystem();
                }
#endif
                group.AddSystemToUpdateList(system);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // MIGRATE

        //@TODO(scobi): publish this method from UnityEngineExtensions
        public static EntityGuid GetEntityGuid(GameObject gameObject, int index) =>
            gameObject.ComputeEntityGuid(0, index);

        // OBSOLETE

        [Obsolete("ConvertIncrementalInitialize now receives its configuration parameters through a GameObjectConversionSettings (RemovedAfter 2019-10-17)")]
        [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
        public static World ConvertIncrementalInitialize(World dstEntityWorld, Scene scene, Hash128 sceneHash, ConversionFlags conversionFlags)
            => ConvertIncrementalInitialize(scene, new GameObjectConversionSettings { DestinationWorld = dstEntityWorld, SceneGUID = sceneHash, ConversionFlags = conversionFlags });

        [Obsolete("ConvertScene now receives its configuration parameters through a GameObjectConversionSettings (RemovedAfter 2019-10-17)")]
        [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
        public static void ConvertScene(Scene scene, Hash128 sceneHash, World dstEntityWorld, ConversionFlags conversionFlags = 0)
            => ConvertScene(scene, new GameObjectConversionSettings { SceneGUID = sceneHash, DestinationWorld = dstEntityWorld, ConversionFlags = conversionFlags });
    }
}
