#define DETAIL_MARKERS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Profiling;
using Unity.Transforms;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using ConversionFlags = Unity.Entities.GameObjectConversionUtility.ConversionFlags;
using UnityLogType = UnityEngine.LogType;
using UnityObject = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;
using static Unity.Debug;

namespace Unity.Entities.Conversion
{
    enum ConversionState
    {
        NotConverting,
        Discovering,
        Converting,
    }

    [DisableAutoCreation]
    class GameObjectConversionMappingSystem : ComponentSystem
    {
        const int k_StaticMask = 1;
        const int k_EntityGUIDMask = 2;
        const int k_DisabledMask = 4;
        const int k_SceneSectionMask = 8;
        const int k_AllMask = 15;

        static List<Component>               s_ComponentsCache = new List<Component>();

        GameObjectConversionSettings         m_Settings;
        readonly EntityManager               m_DstManager;
        readonly JournalingUnityLogger       m_JournalingUnityLogger;

        ConversionState                      m_ConversionState;
        int                                  m_BeginConvertingRefCount;

        ConversionJournalData                m_JournalData;

        NativeMultiHashMap<int, int>         m_GameObjectDependents = new NativeMultiHashMap<int, int>(10000, Allocator.Persistent);
        List<GameObject>                     m_GameObjectDependentsList = new List<GameObject>();

        EntityArchetype[]                    m_Archetypes;

                                             // prefabs and everything they contain will be stored in this set, to be tagged with the Prefab component in dst world
        HashSet<GameObject>                  m_DstPrefabs = new HashSet<GameObject>();
                                             // each will be marked as a linked entity group containing all of its converted descendants in the dst world
        HashSet<GameObject>                  m_DstLinkedEntityGroups = new HashSet<GameObject>();
                                             // assets that were declared via DeclareReferencedAssets
        HashSet<UnityObject>                 m_DstAssets = new HashSet<UnityObject>();

        internal ref ConversionJournalData   JournalData => ref m_JournalData;

        #if UNITY_EDITOR
        // Used for both systems and component types
        Dictionary<Type, bool>               m_ConversionTypeLookupCache = new Dictionary<Type, bool>();
        #endif

        public bool  AddEntityGUID           => (m_Settings.ConversionFlags & ConversionFlags.AddEntityGUID) != 0;
        public bool  ForceStaticOptimization => (m_Settings.ConversionFlags & ConversionFlags.ForceStaticOptimization) != 0;
        public bool  AssignName              => (m_Settings.ConversionFlags & ConversionFlags.AssignName) != 0;
        public bool  IsLiveLink              => (m_Settings.ConversionFlags & (ConversionFlags.SceneViewLiveLink | ConversionFlags.GameViewLiveLink)) != 0;

        public EntityManager   DstEntityManager => m_Settings.DestinationWorld.EntityManager;
        public ConversionState ConversionState  => m_ConversionState;

        public GameObjectConversionMappingSystem(GameObjectConversionSettings settings)
        {
            m_Settings = settings;
            m_DstManager = m_Settings.DestinationWorld.EntityManager;
            m_JournalingUnityLogger = new JournalingUnityLogger(this);

            m_JournalData.Init();

            InitArchetypes();
        }

        public GameObjectConversionSettings ForkSettings(byte entityGuidNamespaceID)
            => m_Settings.Fork(entityGuidNamespaceID);

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            if (m_BeginConvertingRefCount > 0)
                CleanupConversion();

            m_JournalData.Dispose();
            m_GameObjectDependents.Dispose();
        }

        void CleanupConversion()
        {
            m_JournalingUnityLogger.Unhook();
            s_ComponentsCache.Clear();

            m_ConversionState = ConversionState.NotConverting;
            m_BeginConvertingRefCount = 0;
        }

        void InitArchetypes()
        {
            m_Archetypes = new EntityArchetype[k_AllMask + 1];
            var types = new List<ComponentType>();

            for (int i = 0; i <= k_AllMask; i++)
            {
                types.Clear();
                if ((i & k_StaticMask) != 0)
                    types.Add(typeof(Static));
                if ((i & k_EntityGUIDMask) != 0)
                    types.Add(typeof(EntityGuid));
                if ((i & k_DisabledMask) != 0)
                    types.Add(typeof(Disabled));
                if ((i & k_SceneSectionMask) != 0)
                    types.Add(typeof(SceneSection));

                m_Archetypes[i] = m_DstManager.CreateArchetype(types.ToArray());
            }
        }

        public void BeginConversion()
        {
            if (ConversionState == ConversionState.Converting)
                throw new InvalidOperationException("Cannot BeginConversion after conversion has started (call EndConversion first)");

            ++m_BeginConvertingRefCount;

            if (ConversionState == ConversionState.NotConverting)
            {
                m_ConversionState = ConversionState.Discovering;

                m_JournalingUnityLogger.Hook();
            }
        }

        public void EndConversion()
        {
            if (m_BeginConvertingRefCount == 0)
                throw new InvalidOperationException("Conversion has not started");

            if (--m_BeginConvertingRefCount == 0)
                CleanupConversion();
        }

        #if DETAIL_MARKERS
        ProfilerMarker m_CreateEntity = new ProfilerMarker("GameObjectConversion.CreateEntity");
        ProfilerMarker m_CreatePrimaryEntities = new ProfilerMarker("GameObjectConversion.CreatePrimaryEntities");
        ProfilerMarker m_CreateAdditional = new ProfilerMarker("GameObjectConversionCreateAdditionalEntity");
        #endif

        EntityArchetype GetCleanIncrementalArchetype(Entity entity)
        {
            int flags = 0;
            if (m_DstManager.HasComponent<EntityGuid>(entity))
                flags |= k_EntityGUIDMask;
            if (m_DstManager.HasComponent<Static>(entity))
                flags |= k_StaticMask;
            if (m_DstManager.HasComponent<Disabled>(entity))
                flags |= k_DisabledMask;
            if (m_DstManager.HasComponent<SceneSection>(entity))
                flags |= k_SceneSectionMask;

            return m_Archetypes[flags];
        }

        Entity CreateDstEntity(UnityObject uobject, int serial)
        {
            #if DETAIL_MARKERS
            using (m_CreateEntity.Auto())
            #endif
            {
                int flags = 0;
                if (AddEntityGUID)
                    flags |= k_EntityGUIDMask;

                var go = uobject as GameObject;
                if (go != null)
                {
                    if (ForceStaticOptimization || go.GetComponentInParent<StaticOptimizeEntity>() != null)
                        flags |= k_StaticMask;
                    if (!go.IsActiveIgnorePrefab())
                        flags |= k_DisabledMask;
                }
                else if (uobject is Component)
                    throw new ArgumentException("Object must be a GameObject, Prefab, or Asset", nameof(uobject));

                var entity = m_DstManager.CreateEntity(m_Archetypes[flags]);

                if ((flags & k_EntityGUIDMask) != 0)
                    m_DstManager.SetComponentData(entity, uobject.ComputeEntityGuid(m_Settings.NamespaceID, serial));

                if (m_Settings.SceneGUID != default)
                {
                    int sectionIndex = 0;
                    if (go != null)
                    {
                        var section = go.GetComponentInParent<SceneSectionComponent>();
                        if (section != null)
                            sectionIndex = section.SectionIndex;
                    }

                    //@TODO: add an `else` that figures out what referenced this thing, because this is a dependency.
                    // probably easiest to determine after everything has been converted by simply analyzing entity references. -joe

                    m_DstManager.AddSharedComponentData(entity, new SceneSection { SceneGUID = m_Settings.SceneGUID, Section = sectionIndex });
                }

                #if UNITY_EDITOR
                if (AssignName)
                    m_DstManager.SetName(entity, uobject.name);
                #endif

                return entity;
            }
        }

        public Entity CreatePrimaryEntity(UnityObject uobject)
        {
            if (uobject == null)
                throw new ArgumentNullException(nameof(uobject), $"{nameof(CreatePrimaryEntity)} must be called with a valid UnityEngine.Object");

            var entity = CreateDstEntity(uobject, 0);
            m_JournalData.RecordPrimaryEntity(uobject.GetInstanceID(), entity);
            return entity;
        }

        public void CreatePrimaryEntities()
        {
            if (ConversionState != ConversionState.Discovering)
                throw new InvalidOperationException("Unexpected conversion state transition");

            m_ConversionState = ConversionState.Converting;

            #if DETAIL_MARKERS
            using (m_CreatePrimaryEntities.Auto())
            #endif
            {
                Entities.WithIncludeAll().ForEach((Transform transform) =>
                {
                    CreatePrimaryEntity(transform.gameObject);
                });

                //@TODO: inherited classes should probably be supported by queries, so we can delete this loop
                Entities.WithIncludeAll().ForEach((RectTransform transform) =>
                {
                    CreatePrimaryEntity(transform.gameObject);
                });

                //@TODO: [slow] implement this using new inherited query feature so we can do
                //       `Entities.WithAll<Asset>().ForEach((UnityObject asset) => ...)`
                Entities.WithAll<Asset>().ForEach(entity =>
                {
                    using (var types = EntityManager.GetComponentTypes(entity))
                    {
                        var derivedType = types.FirstOrDefault(t => typeof(UnityObject).IsAssignableFrom(t.GetManagedType()));
                        if (derivedType.TypeIndex == 0)
                            throw new Exception("Expected to find a UnityEngine.Object-derived component type in this entity");

                        var asset = EntityManager.GetComponentObject<UnityObject>(entity, derivedType);
                        CreatePrimaryEntity(asset);
                    }
                });
            }
        }

        #if UNITY_EDITOR

        public T GetBuildSettingsComponent<T>() where T : Build.IBuildSettingsComponent
        {
            if (m_Settings.BuildSettings != null)
            {
                return m_Settings.BuildSettings.GetComponent<T>();
            }

            return default;
        }

        public bool TryGetBuildSettingsComponent<T>(out T component) where T : Build.IBuildSettingsComponent
        {

            if (m_Settings.BuildSettings != null)
            {
                return m_Settings.BuildSettings.TryGetComponent(out component);
            }

            component = default;
            return false;
        }

        /// <summary>
        /// Returns whether a GameObjectConversionSystem of the given type, or a IConvertGameObjectToEntity
        /// MonoBehaviour, should execute its conversion methods.  Typically used in an implementation
        /// of GameObjectConversionSystem.ShouldRunConversionSystem
        /// </summary>
        public bool ShouldRunConversionSystem(Type conversionSystemType)
        {
            if (!m_ConversionTypeLookupCache.TryGetValue(conversionSystemType, out var shouldRun))
            {
                var hasFilter = TryGetBuildSettingsComponent<ConversionSystemFilterSettings>(out var filter);
                shouldRun = !hasFilter || filter.ShouldRunConversionSystem(conversionSystemType);

                m_ConversionTypeLookupCache[conversionSystemType] = shouldRun;
            }

            return shouldRun;
        }

        /// <summary>
        /// Returns whether the current build settings configuration includes the given types at runtime.
        /// Typically used in an implementation of GameObjectConversionSystem.ShouldRunConversionSystem,
        /// but can also be used to make more detailed decisions.
        /// </summary>
        public bool BuildHasType(Type componentType)
        {
            if (!m_ConversionTypeLookupCache.TryGetValue(componentType, out var hasType))
            {
                // TODO -- check using a TypeCache obtained from the build settings
                hasType = true;

                m_ConversionTypeLookupCache[componentType] = hasType;
            }

            return hasType;
        }

        /// <summary>
        /// Returns whether the current build settings configuration includes the given types at runtime.
        /// Typically used in an implementation of GameObjectConversionSystem.ShouldRunConversionSystem,
        /// but can also be used to make more detailed decisions.
        /// </summary>
        public bool BuildHasType(params Type[] componentTypes)
        {
            foreach (var type in componentTypes)
            {
                if (!BuildHasType(type))
                    return false;
            }

            return true;
        }

        #endif // UNITY_EDITOR

        public Entity TryGetPrimaryEntity(UnityObject uobject)
        {
            if (uobject == null)
                return Entity.Null;

            if (!m_JournalData.TryGetPrimaryEntity(uobject.GetInstanceID(), out var entity))
                uobject.CheckObjectIsNotComponent();

            return entity;
        }

        static string MakeUnknownObjectMessage<T>(T uobject, [CallerMemberName] string methodName = "")
            where T : UnityObject
        {
            uobject.CheckObjectIsNotComponent(); // cannot get here by user code - all front end API's should auto-fetch owning GameObject

            var sb = new StringBuilder();

            sb.Append(methodName);
            sb.Append($"({typeof(T).Name} '{uobject.name}')");

            if (uobject.IsAsset())
            {
                sb.Append(" is an Asset that was not declared for conversion and will be ignored. ");
                sb.Append($"(Did you forget to declare it using a [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)})] system?)");
            }
            else if (uobject.IsPrefab())
            {
                sb.Append(" is a Prefab that was not declared for conversion and will be ignored. ");
                sb.Append($"(Did you forget to declare it using {nameof(IDeclareReferencedPrefabs)} or via a [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)})] system?)");
            }
            else
                sb.Append(" is a GameObject that was not included in the conversion and will be ignored.");

            return sb.ToString();
        }

        public Entity GetPrimaryEntity(UnityObject uobject)
        {
            var entity = TryGetPrimaryEntity(uobject);
            if (entity == Entity.Null && uobject != null)
                LogWarning(MakeUnknownObjectMessage(uobject), uobject);

            return entity;
        }

        public Entity CreateAdditionalEntity(UnityObject uobject)
        {
            #if DETAIL_MARKERS
            using (m_CreateAdditional.Auto())
            #endif
            {
                if (uobject == null)
                    throw new ArgumentNullException(nameof(uobject), $"{nameof(CreateAdditionalEntity)} must be called with a valid UnityEngine.Object");

                var (id, serial) = m_JournalData.ReserveAdditionalEntity(uobject.GetInstanceID());
                if (serial == 0)
                    throw new ArgumentException(MakeUnknownObjectMessage(uobject), nameof(uobject));

                var entity = CreateDstEntity(uobject, serial);
                m_JournalData.RecordAdditionalEntityAt(id, entity);
                return entity;
            }
        }

        HashSet<GameObject> CalculateDependencies(IEnumerable<GameObject> gameObjects)
        {
            var didProcess = new HashSet<GameObject>();
            var toBeProcessed = new List<GameObject>();

            toBeProcessed.AddRange(gameObjects);

            while (toBeProcessed.Count != 0)
            {
                var go = toBeProcessed[toBeProcessed.Count - 1];
                toBeProcessed.RemoveAt(toBeProcessed.Count - 1);

                if (didProcess.Add(go))
                {
                    if (!HasPrimaryEntity(go))
                        throw new ArgumentException($"Missing dependencies for GameObject '{go.name}'");

                    var indices = m_GameObjectDependents.GetValuesForKey(go.GetInstanceID());
                    foreach (var index in indices)
                    {
                        var dependentGO = m_GameObjectDependentsList[index];
                        if (!didProcess.Contains(dependentGO))
                            toBeProcessed.Add(dependentGO);
                    }
                }
            }

            return didProcess;
        }

        public void PrepareIncrementalConversion(IEnumerable<GameObject> gameObjects, ConversionFlags flags)
        {
            if (m_Settings.ConversionFlags != flags)
                throw new ArgumentException("Conversion flags don't match");

            if (!IsLiveLink)
                throw new InvalidOperationException("Incremental conversion can only be used when the conversion world was specifically created for it");

            if (!EntityManager.UniversalQuery.IsEmptyIgnoreFilter)
                throw new InvalidOperationException("Conversion world is expected to be empty");

            m_DstLinkedEntityGroups.Clear();

            HashSet<GameObject> dependencies;
            using (new ProfilerMarker("CalculateDependencies").Auto())
            {
                dependencies = CalculateDependencies(gameObjects);
                if (dependencies == null)
                    throw new InvalidOperationException("Missing dependencies");
            }

            using (new ProfilerMarker($"ClearIncrementalConversion ({dependencies.Count} GameObjects)").Auto())
            {
                foreach (var go in dependencies)
                    ClearIncrementalConversion(go);
            }

            using (new ProfilerMarker($"CreateGameObjectEntities ({dependencies.Count} GameObjects)").Auto())
            {
                foreach (var go in dependencies)
                    CreateGameObjectEntity(go);
            }

            //Debug.Log($"Incremental processing {EntityManager.UniversalQuery.CalculateEntityCount()}");
        }

        void ClearIncrementalConversion(GameObject gameObject)
        {
            var instanceId = gameObject.GetInstanceID();

            if (!m_JournalData.GetEntities(instanceId, out var entities))
                throw new ArgumentException($"GameObject {gameObject} has changed but is not known to the incremental conversion.");

            entities.MoveNext();
            var primaryEntity = entities.Current;
            if (m_DstManager.HasComponent<Prefab>(primaryEntity))
                throw new ArgumentException("An Entity with a Prefab tag cannot be updated during incremental conversion");

            //@TODO(scobi): implement removal of primary
            m_DstManager.SetArchetype(primaryEntity, GetCleanIncrementalArchetype(primaryEntity));

            while (entities.MoveNext())
            {
                var entity = entities.Current;
                if (m_DstManager.HasComponent<LinkedEntityGroup>(entity))
                    throw new ArgumentException("An Entity with a LinkedEntityGroup cannot be destroyed during incremental conversion");
                if (m_DstManager.HasComponent<Prefab>(entity))
                    throw new ArgumentException("An Entity with a Prefab tag cannot be updated during incremental conversion");

                m_DstManager.DestroyEntity(entity);
            }

            m_JournalData.RemoveForIncremental(gameObject);
        }

        /// <summary>
        /// Is the game object included in the set of converted objects.
        /// </summary>
        public bool HasPrimaryEntity(UnityObject uobject)
        {
            if (uobject == null)
                return false;

            var found = m_JournalData.HasPrimaryEntity(uobject.GetInstanceID());
            if (!found)
                uobject.CheckObjectIsNotComponent();

            return found;
        }

        public MultiListEnumerator<Entity> GetEntities(UnityObject uobject)
        {
            if (uobject == null)
                return MultiListEnumerator<Entity>.Empty;

            if (!m_JournalData.GetEntities(uobject.GetInstanceID(), out var iter))
                uobject.CheckObjectIsNotComponent();

            return iter;
        }

        public void AddGameObjectOrPrefab(GameObject gameObjectOrPrefab)
        {
            if (ConversionState != ConversionState.Discovering)
                throw new InvalidOperationException("AddGameObjectOrPrefab can only be called from a System using [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))].");

            if (gameObjectOrPrefab == null)
                return;
            if (m_DstPrefabs.Contains(gameObjectOrPrefab))
                return;

            m_DstLinkedEntityGroups.Add(gameObjectOrPrefab);

            var outDiscoveredGameObjects = gameObjectOrPrefab.IsPrefab() ? m_DstPrefabs : null;
            CreateEntitiesForGameObjectsRecurse(gameObjectOrPrefab.transform, outDiscoveredGameObjects);
        }

        public void DeclareDependency(GameObject target, GameObject dependsOn)
        {
            if (IsLiveLink)
            {
                int index = m_GameObjectDependentsList.Count;
                m_GameObjectDependentsList.Add(target);
                m_GameObjectDependents.Add(dependsOn.GetInstanceID(), index);

                //@TODO: Remove duplicate dependency values... (Can happen massively with incremental conversion)
            }
        }

        /// <summary>
        /// Adds a LinkedEntityGroup to the primary entity of this GameObject, for all entities that are created from this and all child game objects.
        /// As a result EntityManager.Instantiate and EntityManager.SetEnabled will work on those entities as a group.
        /// </summary>
        public void DeclareLinkedEntityGroup(GameObject gameObject)
        {
            m_DstLinkedEntityGroups.Add(gameObject);
        }

        public void ConfigureEditorRenderData(Entity entity, GameObject pickableObject, bool hasGameObjectBasedRenderingRepresentation)
        {
            #if UNITY_EDITOR
            //@TODO: Dont apply to prefabs (runtime instances should just be pickable by the scene ... Custom one should probably have a special culling mask though?)

            bool liveLinkScene = (m_Settings.ConversionFlags & ConversionFlags.SceneViewLiveLink) != 0;
            //bool liveLinkGameView = (m_Settings.ConversionFlags & ConversionFlags.GameViewLiveLink) != 0;

            // NOTE: When no live link is present all entities will simply be batched together for the whole scene
            
            // In SceneView Live Link mode we want to show the original MeshRenderer
            if (hasGameObjectBasedRenderingRepresentation && liveLinkScene)
            {
                #if UNITY_2020_1_OR_NEWER
                var sceneCullingMask = UnityEditor.GameObjectUtility.ModifyMaskIfGameObjectIsHiddenForPrefabModeInContext(
                    EditorSceneManager.DefaultSceneCullingMask,
                    pickableObject);
                #else
                var sceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask;
                #endif
                m_DstManager.AddSharedComponentData(entity, new EditorRenderData
                {
                    PickableObject = pickableObject,
                    SceneCullingMask = sceneCullingMask
                });
            }
            // Code never hit currently so outcommented:
            // When live linking game view, we still want custom renderers to be pickable.
            // Otherwise they will not even be visible in scene view at all.
            //else if (!hasGameObjectBasedRenderingRepresentation && liveLinkGameView)
            //{
            //    m_DstManager.AddSharedComponentData(entity, new EditorRenderData
            //    {
            //        PickableObject = pickableObject,
            //        SceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask | EditorRenderData.LiveLinkEditSceneViewMask
            //    });
            //}
            #endif
        }

        /// <summary>
        /// DeclareReferencedPrefab includes the referenced Prefab in the conversion process.
        /// Once it has been declared you can use GetPrimaryEntity to find the Entity for the GameObject.
        /// All entities in the Prefab will be made part of the LinkedEntityGroup, thus Instantiate will clone the whole group.
        /// All entities in the Prefab will be tagged with the Prefab component thus will not be picked up by an EntityQuery by default.
        /// </summary>
        public void DeclareReferencedPrefab(GameObject prefab)
        {
            if (ConversionState != ConversionState.Discovering)
                throw new InvalidOperationException($"{nameof(DeclareReferencedPrefab)} can only be called from a System using [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)}))].");

            if (prefab == null)
                return;

            if (m_DstPrefabs.Contains(prefab))
                return;

            if (!prefab.IsPrefab())
            {
                LogWarning("Object is not a Prefab", prefab);
                return;
            }

            m_DstLinkedEntityGroups.Add(prefab);
            CreateEntitiesForGameObjectsRecurse(prefab.transform, m_DstPrefabs);
        }

        public void DeclareReferencedAsset(UnityObject asset)
        {
            if (ConversionState != ConversionState.Discovering)
                throw new InvalidOperationException($"{nameof(DeclareReferencedAsset)} can only be called from a System using [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)}))].");

            if (asset == null)
                return;

            if (m_DstAssets.Contains(asset))
                return;

            if (!asset.IsAsset())
            {
                LogWarning("Object is not an Asset", asset);
                return;
            }

            m_DstAssets.Add(asset);

            var entity = EntityManager.CreateEntity(typeof(Asset), asset.GetType());

            EntityManager.SetComponentObject(entity, asset.GetType(), asset);
        }

        public Guid GetGuidForAssetExport(UnityObject asset)
        {
            if (!asset.IsAsset())
                throw new ArgumentException("Object is not an Asset", nameof(asset));

            return m_Settings.GetGuidForAssetExport(asset);
        }

        public Stream TryCreateAssetExportWriter(UnityObject asset)
            => m_Settings.TryCreateAssetExportWriter(asset);

        public void GenerateLinkedEntityGroups()
        {
            // Create LinkedEntityGroup for each root GameObject entity
            // Instantiate & Destroy will destroy the entity as a group.
            foreach (var dstLinkedEntityGroup in m_DstLinkedEntityGroups)
            {
                var selfAndChildren = dstLinkedEntityGroup.GetComponentsInChildren<Transform>(true);

                var entityGroupRoot = GetPrimaryEntity(dstLinkedEntityGroup);

                if (entityGroupRoot == Entity.Null)
                {
                    LogWarning($"Missing entity for root GameObject '{dstLinkedEntityGroup.name}', check for warnings/errors reported during conversion.", dstLinkedEntityGroup);
                    continue;
                }

                if (m_DstManager.HasComponent<LinkedEntityGroup>(entityGroupRoot))
                    continue;

                var buffer = m_DstManager.AddBuffer<LinkedEntityGroup>(entityGroupRoot);
                foreach (var transform in selfAndChildren)
                {
                    DeclareDependency(dstLinkedEntityGroup, transform.gameObject);

                    foreach (var entity in GetEntities(transform.gameObject))
                        buffer.Add(entity);
                }

                Assert.AreEqual(buffer[0], entityGroupRoot);

                //@TODO: This optimization caused breakage on terrains.
                // No need for linked root if it ends up being just one entity...
                //if (buffer.Length == 1)
                //    m_DstManager.RemoveComponent<LinkedEntityGroup>(linkedRoot);
            }
        }

        // Add prefab tag to all entities that were converted from a prefab game object source
        public void AddPrefabComponentDataTag()
        {
            foreach (var dstPrefab in m_DstPrefabs)
            {
                foreach(var entity in GetEntities(dstPrefab))
                    m_DstManager.AddComponent<Prefab>(entity);
            }
        }

        public static void CopyComponentDataProxyToEntity(EntityManager entityManager, GameObject gameObject, Entity entity)
        {
            foreach (var proxy in gameObject.GetComponents<ComponentDataProxyBase>())
            {
                if (!proxy.enabled)
                    continue;

                var type = proxy.GetComponentType();
                entityManager.AddComponent(entity, type);
                proxy.UpdateComponentData(entityManager, entity);
            }
        }

        unsafe void CreateGameObjectEntity(GameObject gameObject)
        {
            var componentTypes = stackalloc ComponentType[128];
            if (!gameObject.GetComponents(componentTypes, 128, s_ComponentsCache))
                return;

            EntityArchetype archetype;
            try
            {
                archetype = EntityManager.CreateArchetype(componentTypes, s_ComponentsCache.Count);
            }
            catch (Exception)
            {
                for (int i = 0; i < s_ComponentsCache.Count; ++i)
                {
                    if (NativeArrayExtensions.IndexOf<ComponentType, ComponentType>(componentTypes, s_ComponentsCache.Count, componentTypes[i]) != i)
                    {
                        LogWarning($"GameObject '{gameObject}' has multiple {componentTypes[i]} components and cannot be converted, skipping.", gameObject);
                        return;
                    }
                }

                throw;
            }

            var entity = EntityManager.CreateEntity(archetype);

            for (var i = 0; i != s_ComponentsCache.Count; i++)
            {
                var com = s_ComponentsCache[i];

                //@TODO: avoid cast
                var componentDataProxy = com as ComponentDataProxyBase;

                if (componentDataProxy != null)
                {
                    componentDataProxy.UpdateComponentData(EntityManager, entity);
                }
                else if (com != null)
                {
                    EntityManager.SetComponentObject(entity, componentTypes[i], com);
                }
            }
        }

        void CreateEntitiesForGameObjectsRecurse(Transform transform, HashSet<GameObject> outDiscoveredGameObjects)
        {
            // If a game object is disabled, we add a linked entity group so that EntityManager.SetEnabled() on the primary entity will result in the whole hierarchy becoming enabled.
            if (!transform.gameObject.activeSelf)
                DeclareLinkedEntityGroup(transform.gameObject);

            CreateGameObjectEntity(transform.gameObject);
            outDiscoveredGameObjects?.Add(transform.gameObject);

            int childCount = transform.childCount;
            for (int i = 0; i != childCount;i++)
                CreateEntitiesForGameObjectsRecurse(transform.GetChild(i), outDiscoveredGameObjects);
        }

        public void CreateEntitiesForGameObjects(Scene scene)
        {
            var gameObjects = scene.GetRootGameObjects();

            foreach (var go in gameObjects)
                CreateEntitiesForGameObjectsRecurse(go.transform, null);
        }

        public void AddHybridComponent(UnityComponent component)
        {
            //@TODO exception if converting SubScene or doing incremental conversion (requires a conversion flag for SubScene conversion on m_Settings.ConversionFlags)
            var type = component.GetType();

            if (component.GetComponents(type).Length > 1)
            {
                LogWarning($"AddHybridComponent({type}) requires the GameObject to only contain a single component of this type.", component.gameObject);
                return;
            }

            m_JournalData.AddHybridComponent(component.gameObject, type);
        }

        #if !UNITY_DISABLE_MANAGED_COMPONENTS
        internal void CreateCompanionGameObjects()
        {
            foreach (var kvp in m_JournalData.HybridHeadIdIndices)
            {
                var gameObject = kvp.Key;
                var components = m_JournalData.HybridTypes(kvp.Value);
                var entity = GetPrimaryEntity(gameObject);

                if (gameObject.activeSelf == false)
                {
                    LogWarning("GameObject is inactive and wasn't expected to be converted.", gameObject);
                    continue;
                }

                try
                {
                    gameObject.SetActive(false);

                    var companion = UnityObject.Instantiate(gameObject);
                    companion.name = CompanionLink.GenerateCompanionName(entity);

                    foreach (var component in gameObject.GetComponents<Component>())
                    {
                        var type = component.GetType();
                        if (!components.Contains(type))
                        {
                            foreach (var useless in companion.GetComponents(type))
                            {
                                //@TODO some components have [RequireComponent] dependencies to each other, and will require special handling for deletion
                                if (type != typeof(Transform))
                                    UnityObject.DestroyImmediate(useless);
                            }
                        }
                        else
                        {
                            m_DstManager.AddComponentObject(entity, companion.GetComponent(type));
                        }
                    }

                    m_DstManager.AddComponentObject(entity, companion.AddComponent<CompanionLink>());

                    // Can't detach children before instantiate because that won't work with a prefab

                    for (int i = companion.transform.childCount - 1; i >= 0; i -= 1)
                        UnityObject.DestroyImmediate(companion.transform.GetChild(i).gameObject);

                    companion.hideFlags |= HideFlags.HideInHierarchy;
                }
                catch (Exception exception)
                {
                    LogException(exception, gameObject);
                }
                finally
                {
                    gameObject.SetActive(true);
                }
            }
        }
        #endif // !UNITY_DISABLE_MANAGED_COMPONENTS

        public BlobAssetStore GetBlobAssetStore() => m_Settings.blobAssetStore;
    }
}

//@TODO: Change of active state is not live linked. Should trigger rebuild?
