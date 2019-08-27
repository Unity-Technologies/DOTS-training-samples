#define DETAIL_MARKERS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using ConversionFlags = Unity.Entities.GameObjectConversionUtility.ConversionFlags;
using UnityObject = UnityEngine.Object;

public struct EntitiesEnumerator : IEnumerable<Entity>, IEnumerator<Entity>
{
    Entity[] m_Entities;
    int[]    m_Next;
    int      m_FirstIndex;
    int      m_CurIndex;
    bool     m_IsFirst;

    internal EntitiesEnumerator(Entity[] entities, int[] next, int index)
    {
        m_Entities = entities;
        m_Next = next;
        m_FirstIndex = index;
        m_CurIndex = -1;
        m_IsFirst = true;
    }

    public EntitiesEnumerator GetEnumerator() => this;
    IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;

    public bool MoveNext()
    {
        if (m_IsFirst)
        {
            m_IsFirst = false;
            m_CurIndex = m_FirstIndex;
            return m_CurIndex != -1;
        }

        var next = m_Next[m_CurIndex];
        if (next == -1)
            return false;

        m_CurIndex = next;
        return true;
    }

    public void Reset()
    {
        m_IsFirst = true;
        m_CurIndex = m_FirstIndex;
    }

    public Entity Current => m_Entities[m_CurIndex];
    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }
}

struct FreeList<T>
{
    public T[]    Data;
    public int[]  Next;

    int  m_NextFree;

    public void Init ()
    {
        m_NextFree = -1;
        Data = new T[0];
        Next = new int[0];
    }

    public int Alloc()
    {
        if (m_NextFree == -1)
            Grow(Data.Length != 0 ? 2 * Data.Length : 128);
        int id = m_NextFree;
        m_NextFree = Next[id];

        Next[id] = -1;

        return id;
    }

    public void Release(int id)
    {
        Next[id] = m_NextFree;
        m_NextFree = id;
    }

    void Grow(int capacity)
    {
        int oldLength = Data.Length;
        Array.Resize(ref Data, capacity);
        Array.Resize(ref Next, capacity);

        for (int i = oldLength; i < capacity - 1; ++i)
            Next[i] = i + 1;
        Next[capacity - 1] = -1;
        m_NextFree = oldLength;
    }

    // The following methods reuse the Next index array while the array is allocated
    // (And thus not in use for the free list)
    // These two utility methods turn it into a single linked list.
    public int AllocLinkedList(int listIndex)
    {
        int lastIndex = listIndex;
        while ((listIndex = Next[listIndex]) != -1)
            lastIndex = listIndex;

        Assert.AreEqual(-1, Next[lastIndex]);
        
        int newIdx = Alloc();
        Next[lastIndex] = newIdx;
        return newIdx;
    }
    
    public int CountLinkedList(int index)
    {
        int count = 1;
        while ((index = Next[index]) != -1)
            count++;
        return count;
    }
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
    EntityManager                        m_DstManager;

    NativeHashMap<int, int>              m_GameObjectToEntity = new NativeHashMap<int, int>(100 * 1000, Allocator.Persistent);
    NativeMultiHashMap<int, int>         m_GameObjectDependents = new NativeMultiHashMap<int, int>(100 * 1000, Allocator.Persistent);
    List<GameObject>                     m_GameObjectDependentsList = new List<GameObject>();

    FreeList<Entity>                     m_Entities = new FreeList<Entity>();
    EntityArchetype[]                    m_Archetypes;

                                         // prefabs and everything they contain will be stored in this set, to be tagged with the Prefab component in dst world  
    HashSet<GameObject>                  m_DstPrefabs = new HashSet<GameObject>();
                                         // each will be marked as a linked entity group containing all of its converted descendants in the dst world
    HashSet<GameObject>                  m_DstLinkedEntityGroups = new HashSet<GameObject>();
                                         // assets that were declared via DeclareReferencedAssets
    HashSet<UnityObject>                 m_DstAssets = new HashSet<UnityObject>();

    internal bool                        AllowAddingMoreConversionObjects = true;

    public World DstWorld                => m_Settings.DestinationWorld;
    public bool  AddEntityGUID           => (m_Settings.ConversionFlags & ConversionFlags.AddEntityGUID) != 0;
    public bool  ForceStaticOptimization => (m_Settings.ConversionFlags & ConversionFlags.ForceStaticOptimization) != 0;
    public bool  AssignName              => (m_Settings.ConversionFlags & ConversionFlags.AssignName) != 0;
    public bool  IsLiveLink              => (m_Settings.ConversionFlags & (ConversionFlags.SceneViewLiveLink | ConversionFlags.GameViewLiveLink)) != 0;

    public GameObjectConversionMappingSystem(GameObjectConversionSettings settings)
    {
        m_Settings = settings;
        m_DstManager = m_Settings.DestinationWorld.EntityManager;

        m_Entities.Init();

        InitArchetypes();
    }

    protected override void OnUpdate() { }

    protected override void OnDestroy()
    {
        m_GameObjectToEntity.Dispose();
        m_GameObjectDependents.Dispose();
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

    static bool IsActive(GameObject go)
    {
        if (!IsPrefab(go))
            return go.activeInHierarchy;

        var parent = go.transform;
        while (parent != null)
        {
            if (!parent.gameObject.activeSelf)
                return false;

            parent = parent.parent;
        }

        return true;
    }

    public static EntityGuid GetEntityGuid(UnityObject uobject, int index)
    {
#if false
        var id = GlobalObjectId.GetGlobalObjectIdSlow(go);
        // For the time being use InstanceID until we support GlobalObjectID API
        //Debug.Log(id);
        var idStr = $"{id}:{index}";
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(idStr);
        UnityEngine.Hash128 hash = UnityEngine.Hash128.Compute(idStr);

        EntityGuid entityGuid;
        Assert.AreEqual(sizeof(EntityGuid), sizeof(UnityEngine.Hash128));
        UnsafeUtility.MemCpy(&entityGuid, &hash, sizeof(UnityEngine.Hash128));
        
        return entityGuid;
#else
        EntityGuid entityGuid;
        entityGuid.a = (ulong)uobject.GetInstanceID();
        entityGuid.b = (ulong)index;

        return entityGuid;
#endif
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

    Entity CreateEntity(UnityObject uobject, int index)
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
                if (!IsActive(go))
                    flags |= k_DisabledMask;
            }
            else if (uobject is Component)
                throw new ArgumentException("Object must be a GameObject, Prefab, or Asset", nameof(uobject));

            var entity = m_DstManager.CreateEntity(m_Archetypes[flags]);

            if ((flags & k_EntityGUIDMask) != 0)
                m_DstManager.SetComponentData(entity, GetEntityGuid(uobject, index));

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
                m_DstManager.SetName(entity, uobject.ToString());
            #endif

            return entity;
        }
    }

    internal void CreatePrimaryEntity(UnityObject uobject)
    {
        var entity = CreateEntity(uobject, 0);

        int index = m_Entities.Alloc();
        m_Entities.Data[index] = entity;

        if (!m_GameObjectToEntity.TryAdd(uobject.GetInstanceID(), index))
            throw new InvalidOperationException();
    }

    internal void CreatePrimaryEntities()
    {
        AllowAddingMoreConversionObjects = false;

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

    public void LogWarning(string warning, UnityObject context)
    {
        Debug.LogWarning(warning, context);
    }

    static bool IsPrefab(GameObject go)
    {
        return !go.scene.IsValid();
    }

    static bool IsAsset(UnityObject uobject)
    {
        return !(uobject is GameObject) && !(uobject is Component);
    }

    public Entity TryGetPrimaryEntity(UnityObject uobject)
    {
        if (uobject == null)
            return Entity.Null;

        var instanceID = uobject.GetInstanceID();
        if (m_GameObjectToEntity.TryGetValue(instanceID, out var index))
            return m_Entities.Data[index];

        return Entity.Null;
    }

    public Entity GetPrimaryEntity(UnityObject uobject)
    {
        var entity = TryGetPrimaryEntity(uobject);
        if (entity == Entity.Null && uobject != null)
        {
            switch (uobject)
            {
                case Component _:
                    throw new InvalidOperationException(); // should not be possible to get here (user code auto requests the Component's GameObject)
                
                case GameObject go:
                    LogWarning(IsPrefab(go)
                        ? $"GetPrimaryEntity(GameObject '{uobject.name}') is a Prefab that was not declared for conversion and will be ignored. (Did you forget to declare it using {nameof(IDeclareReferencedPrefabs)} or a [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)})] system?)"
                        : $"GetPrimaryEntity(GameObject '{uobject.name}') was not included in the conversion and will be ignored.", uobject);
                    break;

                default:
                    LogWarning($"GetPrimaryEntity(Object '{uobject.name}') is an Asset that was not declared for conversion and will be ignored. (Did you forget to declare it using a [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)})] system?)", uobject);
                    break;
            }
        }

        return entity;
    }

    void AddAdditional(int index, Entity entity)
    {
        int newIdx = m_Entities.AllocLinkedList(index);
        m_Entities.Data[newIdx] = entity;
    }

    public Entity CreateAdditionalEntity(GameObject go)
    {
        #if DETAIL_MARKERS
        using (m_CreateAdditional.Auto())
        #endif
        {
            if (go == null)
                throw new ArgumentException("CreateAdditionalEntity must be called with a valid GameObject");

            var instanceID = go.GetInstanceID();
            int index;
            if (m_GameObjectToEntity.TryGetValue(instanceID, out index))
            {
                int count = m_Entities.CountLinkedList(index);
                var entity = CreateEntity(go, count);
                AddAdditional(index, entity);

                return entity;
            }
            else
            {
                if (IsPrefab(go))
                    throw new ArgumentException($"CreateAdditionalEntity(GameObject '{go.name}') is a prefab that was not declared for conversion. Did you forget to declare it using IDeclarePrefabReferences or a [UpdateInGroup(typeof(GameObjectConversionDeclarePrefabsGroup))] system. It will be ignored.");
                else
                    throw new ArgumentException($"CreateAdditionalEntity(GameObject '{go.name}') is a game object that was not included in the conversion. It will be ignored.");
            }
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
        if (!IsLiveLink)
            throw new ArgumentException("Incremental conversion can only be used when the conversion world was specifically created for it");

        if (m_Settings.ConversionFlags != flags)
            throw new ArgumentException("Conversion flags don't match");

        if (!EntityManager.UniversalQuery.IsEmptyIgnoreFilter)
            throw new ArgumentException("Conversion world is expected to be empty");

        m_DstLinkedEntityGroups.Clear();

        HashSet<GameObject> dependencies;
        using (new ProfilerMarker("CalculateDependencies").Auto())
        {
            dependencies = CalculateDependencies(gameObjects);   
            if (dependencies == null)
                throw new ArgumentException("Missing dependencies");
        }

        using (new ProfilerMarker($"ClearIncrementalConversion ({dependencies.Count} GameObjects)").Auto())
        {
            foreach (var go in dependencies)
                ClearIncrementalConversion(go);
        }

        using (new ProfilerMarker($"CreateGameObjectEntities ({dependencies.Count} GameObjects)").Auto())
        {
            foreach (var go in dependencies)
                CreateGameObjectEntity(EntityManager, go, this);
        }

        //Debug.Log($"Incremental processing {EntityManager.UniversalQuery.CalculateEntityCount()}");
    }

    void ClearIncrementalConversion(GameObject gameObject)
    {
        int index;
        if (!m_GameObjectToEntity.TryGetValue(gameObject.GetInstanceID(), out index))
            throw new ArgumentException($"GameObject {gameObject} has changed but is not known to the incremental conversion.");

        var e = m_Entities.Data[index];

        if (m_DstManager.HasComponent<Prefab>(e))
            throw new ArgumentException("An Entity with a Prefab tag cannot be updated during incremental conversion");

        m_DstManager.SetArchetype(e, GetCleanIncrementalArchetype(e));

        var additionalIndex = m_Entities.Next[index];
        while (additionalIndex != -1)
        {
            e = m_Entities.Data[additionalIndex];
            if (m_DstManager.HasComponent<LinkedEntityGroup>(e))
                throw new ArgumentException("An Entity with a LinkedEntityGroup cannot be destroyed during incremental conversion");
            if (m_DstManager.HasComponent<Prefab>(e))
                throw new ArgumentException("An Entity with a Prefab tag cannot be updated during incremental conversion");

            m_DstManager.DestroyEntity(e);

            int releaseIndex = additionalIndex;
            additionalIndex = m_Entities.Next[additionalIndex];
            m_Entities.Release(releaseIndex);
        }

        m_Entities.Next[index] = -1;
    }

    /// <summary>
    /// Is the game object included in the set of converted objects.
    /// </summary>
    public bool HasPrimaryEntity(UnityObject uobject)
    {
        if (uobject == null)
            return false;

        var instanceID = uobject.GetInstanceID();
        return m_GameObjectToEntity.ContainsKey(instanceID);
    }

    public EntitiesEnumerator GetEntities(UnityObject uobject)
    {
        var instanceID = uobject != null ? uobject.GetInstanceID() : 0;
        int index;
        if (uobject != null)
        {
            if (m_GameObjectToEntity.TryGetValue(instanceID, out index))
                return new EntitiesEnumerator(m_Entities.Data, m_Entities.Next, index);
        }

        return new EntitiesEnumerator(m_Entities.Data, m_Entities.Next, -1);
    }

    internal void AddGameObjectOrPrefab(GameObject gameObjectOrPrefab)
    {
        if (!AllowAddingMoreConversionObjects)
            throw new ArgumentException("AddGameObjectOrPrefab can only be called from a System using [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))].");

        if (gameObjectOrPrefab == null)
            return;
        if (m_DstPrefabs.Contains(gameObjectOrPrefab))
            return;

        m_DstLinkedEntityGroups.Add(gameObjectOrPrefab);

        var isPrefab = !gameObjectOrPrefab.scene.IsValid();
        if (isPrefab)
            CreateEntitiesForGameObjectsRecurse(gameObjectOrPrefab.transform, EntityManager, m_DstPrefabs);
        else
            CreateEntitiesForGameObjectsRecurse(gameObjectOrPrefab.transform, EntityManager, null);
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
        //@TODO: Dont apply to prefabs (runtime instances should just be pickable by the scene ... Custom one should probably have a special culling mask though?)

        bool liveLinkScene = (m_Settings.ConversionFlags & ConversionFlags.SceneViewLiveLink) != 0;
        bool liveLinkGameView = (m_Settings.ConversionFlags & ConversionFlags.GameViewLiveLink) != 0;

        // NOTE: When no live link is present all entities will simply be batched together for the whole scene

        // In SceneView Live Link mode we want to show the original MeshRenderer
        if (hasGameObjectBasedRenderingRepresentation && liveLinkScene)
        {
            m_DstManager.AddSharedComponentData(entity, new EditorRenderData
            {
                PickableObject = pickableObject,
                SceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask
            });
        }
        // When live linking game view, we still want custom renderers to be pickable.
        // Otherwise they will not even be visible in scene view at all.
        else if (!hasGameObjectBasedRenderingRepresentation && liveLinkGameView)
        {
            m_DstManager.AddSharedComponentData(entity, new EditorRenderData
            {
                PickableObject = pickableObject,
                SceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask | EditorRenderData.LiveLinkEditSceneViewMask
            });
        }
    }

    /// <summary>
    /// DeclareReferencedPrefab includes the referenced Prefab in the conversion process.
    /// Once it has been declared you can use GetPrimaryEntity to find the Entity for the GameObject.
    /// All entities in the Prefab will be made part of the LinkedEntityGroup, thus Instantiate will clone the whole group.
    /// All entities in the Prefab will be tagged with the Prefab component thus will not be picked up by an EntityQuery by default.
    /// </summary>
    public void DeclareReferencedPrefab(GameObject prefab)
    {
        if (!AllowAddingMoreConversionObjects)
            throw new ArgumentException($"{nameof(DeclareReferencedPrefab)} can only be called from a System using [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)}))].");

        if (prefab == null)
            return;

        if (m_DstPrefabs.Contains(prefab))
            return;

        if (!IsPrefab(prefab)) //@TODO: register warning
            return;

        m_DstLinkedEntityGroups.Add(prefab);
        CreateEntitiesForGameObjectsRecurse(prefab.transform, EntityManager, m_DstPrefabs);
    }

    public void DeclareReferencedAsset(UnityObject asset)
    {
        if (!AllowAddingMoreConversionObjects)
            throw new ArgumentException($"{nameof(DeclareReferencedAsset)} can only be called from a System using [UpdateInGroup(typeof({nameof(GameObjectDeclareReferencedObjectsGroup)}))].");

        if (asset == null)
            return;
        
        if (m_DstAssets.Contains(asset))
            return;

        if (!IsAsset(asset)) //@TODO: register warning
            return;
        
        m_DstAssets.Add(asset);
        var entity = EntityManager.CreateEntity(typeof(Asset), asset.GetType());
        EntityManager.SetComponentObject(entity, asset.GetType(), asset);
    }

    public Guid GetGuidForAssetExport(UnityObject asset)
    {
        if (!IsAsset(asset))
            throw new ArgumentException("Object is not an asset", nameof(asset));
        
        return m_Settings.GetGuidForAssetExport(asset);
    }

    public Stream TryCreateAssetExportWriter(UnityObject asset)
        => m_Settings.TryCreateAssetExportWriter(asset);

    internal void GenerateLinkedEntityGroups()
    {
        // Create LinkedEntityGroup for each root GameObject entity
        // Instantiate & Destroy will destroy the entity as a group.
        foreach (var dstLinkedEntityGroup in m_DstLinkedEntityGroups)
        {
            var selfAndChildren = dstLinkedEntityGroup.GetComponentsInChildren<Transform>(true);

            var entityGroupRoot = GetPrimaryEntity(dstLinkedEntityGroup);

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

            // This optimization caused breakage on terrains.
            // No need for linked root if it ends up being just one entity...
            //if (buffer.Length == 1)
            //    m_DstManager.RemoveComponent<LinkedEntityGroup>(linkedRoot);
        }
    }

    // Add prefab tag to all entities that were converted from a prefab game object source
    internal void AddPrefabComponentDataTag()
    {
        foreach (var dstPrefab in m_DstPrefabs)
        {
            foreach(var entity in GetEntities(dstPrefab))
                m_DstManager.AddComponent<Prefab>(entity);
        }
    }

    static bool IsComponentDisabled(Component component)
    {
        switch (component)
        {
            case Renderer r:
                return !r.enabled;
            case Collider c:
                return !c.enabled;
            case LODGroup l:
                return !l.enabled;
            case Behaviour b:
                return !b.enabled;
        }

        return false;
    }

    static unsafe bool GetComponents(GameObject gameObject, ComponentType* componentTypes, int componentTypesLength, GameObjectConversionMappingSystem system)
    {
        int outputIndex = 0;
        gameObject.GetComponents(s_ComponentsCache);

        if (componentTypesLength < s_ComponentsCache.Count)
        {
            system.LogWarning($"Too many components on {gameObject.name}", gameObject);
            return false;
        }
                
        for (var i = 0; i != s_ComponentsCache.Count; i++)
        {
            var com = s_ComponentsCache[i];

            if (com == null)
                system.LogWarning($"The referenced script is missing on {gameObject.name}", gameObject);
            else if (IsComponentDisabled(com))
            {
            }
            else if (com is GameObjectEntity)
            {
            }
            else
            {
                var componentData = com as ComponentDataProxyBase;
                
                s_ComponentsCache[outputIndex] = com;
                if (componentData != null)
                    componentTypes[outputIndex] = componentData.GetComponentType();
                else
                    componentTypes[outputIndex] = com.GetType();

                outputIndex++;
            }
        }

        s_ComponentsCache.RemoveRange(outputIndex, s_ComponentsCache.Count - outputIndex);
        return true;
    }

    internal static void CopyComponentDataProxyToEntity(EntityManager entityManager, GameObject gameObject, Entity entity)
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

    static unsafe void CreateGameObjectEntity(EntityManager entityManager, GameObject gameObject, GameObjectConversionMappingSystem system)
    {
        var componentTypes = stackalloc ComponentType[128];
        if (!GetComponents(gameObject, componentTypes, 128, system))
            return;

        EntityArchetype archetype;
        try
        {
            archetype = entityManager.CreateArchetype(componentTypes, s_ComponentsCache.Count);
        }
        catch (Exception)
        {
            for (int i = 0; i < s_ComponentsCache.Count; ++i)
            {
                if (NativeArrayExtensions.IndexOf<ComponentType, ComponentType>(componentTypes, s_ComponentsCache.Count, componentTypes[i]) != i)
                {
                    system.LogWarning($"GameObject '{gameObject}' has multiple {componentTypes[i]} components and cannot be converted, skipping.", null);
                    return;
                }
            }

            throw;
        }
        var entity = entityManager.CreateEntity(archetype);
        
        for (var i = 0; i != s_ComponentsCache.Count; i++)
        {
            var com = s_ComponentsCache[i];
            
            //@TODO: avoid cast
            var componentDataProxy = com as ComponentDataProxyBase;

            if (componentDataProxy != null)
            {
                componentDataProxy.UpdateComponentData(entityManager, entity);
            }
            else if (com != null)
            {
                entityManager.SetComponentObject(entity, componentTypes[i], com);
            }
        }
    }

    void CreateEntitiesForGameObjectsRecurse(Transform transform, EntityManager gameObjectEntityManager, HashSet<GameObject> outDiscoveredGameObjects)
    {
        // If a game object is disabled, we add a linked entity group so that EntityManager.SetEnabled() on the primary entity will result in the whole hierarchy becoming enabled.
        if (!transform.gameObject.activeSelf)
            DeclareLinkedEntityGroup(transform.gameObject);

        CreateGameObjectEntity(gameObjectEntityManager, transform.gameObject, this);
        outDiscoveredGameObjects?.Add(transform.gameObject);

        int childCount = transform.childCount;
        for (int i = 0; i != childCount;i++)
            CreateEntitiesForGameObjectsRecurse(transform.GetChild(i), gameObjectEntityManager, outDiscoveredGameObjects);
    }

    public void CreateEntitiesForGameObjects(Scene scene, World gameObjectWorld)
    {
        var gameObjectEntityManager = gameObjectWorld.EntityManager;
        var gameObjects = scene.GetRootGameObjects();

        foreach (var go in gameObjects)
            CreateEntitiesForGameObjectsRecurse(go.transform, gameObjectEntityManager, null);
    }

    public void CleanupTemporaryCaches()
    {
        s_ComponentsCache.Clear();
    }
}


//@TODO: Change of active state is not live linked. Should trigger rebuild?
