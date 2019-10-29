#if UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP
#define UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_EDITOR_WORLD
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.Entities
{
    
    /// <summary>
    /// When entering playmode or the game starts in the Player.
    /// A default world is created, sometimes you need multiple worlds to be setup when the game starts.
    /// This lets you override the bootstrap of game code world creation.
    /// </summary>
    public interface ICustomBootstrap
    {
        // Returns true if the bootstrap has performed initialization.
        // Returns false if default world initialization should be performed.
        bool Initialize(string defaultWorldName);
    }

    public static class DefaultWorldInitialization
    {
        static bool s_UnloadOrPlayModeChangeShutdownRegistered = false;

        /// <summary>
        /// Invoked after the default World is initialized.
        /// </summary>
        internal static event Action<World> DefaultWorldInitialized;

        /// <summary>
        /// Invoked after the Worlds are destroyed.
        /// </summary>
        internal static event Action DefaultWorldDestroyed;

        /// <summary>
        /// Destroys Editor World when entering Play Mode without Domain Reload. 
        /// RuntimeInitializeOnLoadMethod is called before the new scene is loaded, before Awake and OnEnable of MonoBehaviour.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void CleanupWorldBeforeSceneLoad()
        {
            DomainUnloadOrPlayModeChangeShutdown();
        }

        /// <summary>
        /// Ensures the current World destruction on shutdown or when entering/exiting Play Mode or Domain Reload.
        /// 1) When switching to Play Mode Editor World (if created) has to be destroyed:
        ///     - after the current scene objects are destroyed and OnDisable/Destroy are called,
        ///     - before game scene is loaded and Awake/OnEnable are called.
        /// 2) When switching to Edit Mode Game World has to be destroyed:
        ///     - after the current scene objects are destroyed and OnDisable/Destroy are called,
        ///     - before backup scene is loaded and Awake/OnEnable are called.
        /// 3) When Unloading Domain (as well as Editor/Player exit) Editor or Game World has to be destroyed:
        ///     - after OnDisable/OnBeforeSerialize are called,
        ///     - before AppDomain.DomainUnload.
        /// Point 1) is covered by RuntimeInitializeOnLoadMethod attribute.
        /// For points 2) and 3) there are no entry point in the Unity API and they have to be handled by a proxy MonoBehaviour
        /// which in OnDisable can drive the World cleanup for both Exit Play Mode and Domain Unload.
        /// </summary>
        static void RegisterUnloadOrPlayModeChangeShutdown()
        {
            if (s_UnloadOrPlayModeChangeShutdownRegistered)
                return;

            var go = new GameObject { hideFlags = HideFlags.HideInHierarchy };
            if (Application.isPlaying)
                UnityEngine.Object.DontDestroyOnLoad(go);
            else
                go.hideFlags = HideFlags.HideAndDontSave;

            go.AddComponent<DefaultWorldInitializationProxy>().IsActive = true;

            s_UnloadOrPlayModeChangeShutdownRegistered = true;
        }

        internal static void DomainUnloadOrPlayModeChangeShutdown()
        {
            if (!s_UnloadOrPlayModeChangeShutdownRegistered)
                return;

            World.DisposeAllWorlds();

            WordStorage.Instance.Dispose();
            WordStorage.Instance = null;
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);

            s_UnloadOrPlayModeChangeShutdownRegistered = false;

            DefaultWorldDestroyed?.Invoke();
        }

        static ComponentSystemBase GetOrCreateManagerAndLogException(World world, Type type)
        {
            try
            {
                return world.GetOrCreateSystem(type);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Initializes the default world or runs ICustomBootstrap if one is is available. 
        /// </summary>
        /// <param name="defaultWorldName">The name of the world that will be created. Unless there is a custom bootstrap.</param>
        /// <param name="editorWorld">Editor worlds by default only include systems with [ExecuteAlways]. If editorWorld is true, ICustomBootstrap will not be used.</param>
        public static void Initialize(string defaultWorldName, bool editorWorld)
        {
            RegisterUnloadOrPlayModeChangeShutdown();

            if (!editorWorld)
            {
                var bootStrap = CreateBootStrap();
                if (bootStrap != null && bootStrap.Initialize(defaultWorldName))
                    return;
            }

            var world = new World(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;

            var systems = GetAllSystems(WorldSystemFilterFlags.Default, editorWorld);

            AddSystemsToRootLevelSystemGroups(world, systems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            DefaultWorldInitialized?.Invoke(world);
        }

        /// <summary>
        /// Adds the list of systems to the world by injecting them into the root level system groups
        /// (InitializationSystemGroup, SimulationSystemGroup and PresentationSystemGroup)
        /// </summary>
        public static void AddSystemsToRootLevelSystemGroups(World world, List<Type> systems)
        {
            // create presentation system and simulation system
            var initializationSystemGroup = world.GetOrCreateSystem<InitializationSystemGroup>();
            var simulationSystemGroup = world.GetOrCreateSystem<SimulationSystemGroup>();
            var presentationSystemGroup = world.GetOrCreateSystem<PresentationSystemGroup>();

            // Add systems to their groups, based on the [UpdateInGroup] attribute.
            foreach (var type in systems)
            {
                // Skip the built-in root-level system groups
                if (type == typeof(InitializationSystemGroup) ||
                    type == typeof(SimulationSystemGroup) ||
                    type == typeof(PresentationSystemGroup))
                {
                    continue;
                }

                var groups = type.GetCustomAttributes(typeof(UpdateInGroupAttribute), true);
                if (groups.Length == 0)
                {
                    simulationSystemGroup.AddSystemToUpdateList(GetOrCreateManagerAndLogException(world, type));
                }

                foreach (var g in groups)
                {
                    var group = g as UpdateInGroupAttribute;
                    if (group == null)
                        continue;

                    if (!(typeof(ComponentSystemGroup)).IsAssignableFrom(group.GroupType))
                    {
                        Debug.LogError($"Invalid [UpdateInGroup] attribute for {type}: {group.GroupType} must be derived from ComponentSystemGroup.");
                        continue;
                    }

                    // Warn against unexpected behaviour combining DisableAutoCreation and UpdateInGroup
                    var parentDisableAutoCreation = group.GroupType.GetCustomAttribute<DisableAutoCreationAttribute>() != null;
                    if (parentDisableAutoCreation)
                    {
                        Debug.LogWarning($"A system {type} wants to execute in {group.GroupType} but this group has [DisableAutoCreation] and {type} does not.");
                    }

                    var groupMgr = GetOrCreateManagerAndLogException(world, group.GroupType);
                    if (groupMgr == null)
                    {
                        Debug.LogWarning(
                            $"Skipping creation of {type} due to errors creating the group {group.GroupType}. Fix these errors before continuing.");
                        continue;
                    }

                    var groupSys = groupMgr as ComponentSystemGroup;
                    if (groupSys != null)
                    {
                        groupSys.AddSystemToUpdateList(GetOrCreateManagerAndLogException(world, type));
                    }
                }
            }

            // Update player loop
            initializationSystemGroup.SortSystemUpdateList();
            simulationSystemGroup.SortSystemUpdateList();
            presentationSystemGroup.SortSystemUpdateList();
        }

        /// <summary>
        /// Can be called when in edit mode in the editor to initialize a the default world.
        /// </summary>
        public static void DefaultLazyEditModeInitialize()
        {
#if UNITY_EDITOR
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                // * OnDisable (Serialize monobehaviours in temporary backup)
                // * unload domain
                // * load new domain	
                // * OnEnable (Deserialize monobehaviours in temporary backup)	
                // * mark entered playmode / load scene	
                // * OnDisable / OnDestroy	
                // * OnEnable (Loading object from scene...)	
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    // We are just gonna ignore this enter playmode reload.	
                    // Can't see a situation where it would be useful to create something inbetween.	
                    // But we really need to solve this at the root. The execution order is kind if crazy.	
                    if (UnityEditor.EditorApplication.isPlaying)
                        Debug.LogError("Loading GameObjectEntity in Playmode but there is no active World");
                }
                else
                {
#if !UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_EDITOR_WORLD
                    Initialize("Editor World", true);
#endif
                }
            }
#endif
        }


        /// <summary>
        /// Calculates a list of all systems filtered with WorldSystemFilterFlags, [DisableAutoCreation] etc.
        /// </summary>
        /// <param name="filterFlags"></param>
        /// <param name="requireExecuteAlways">Optionally require that [ExecuteAlways] is present on the system. This is used when creating edit mode worlds.</param>
        /// <returns>The list of filtered systems</returns>
        public static List<Type> GetAllSystems(WorldSystemFilterFlags filterFlags, bool requireExecuteAlways = false)
        {
            var filteredSystemTypes = new List<Type>();

            var allSystemTypes = GetTypesDerivedFrom(typeof(ComponentSystemBase));
            foreach (var systemType in allSystemTypes)
            {
                if (FilterSystemType(systemType, filterFlags, requireExecuteAlways))
                    filteredSystemTypes.Add(systemType);
            }

            return filteredSystemTypes;
        }

        static bool FilterSystemType(Type type, WorldSystemFilterFlags filterFlags, bool requireExecuteAlways)
        {
            // IMPORTANT: keep this logic in sync with SystemTypeGen.cs for DOTS Runtime

            // the entire assembly can be marked for no-auto-creation (test assemblies are good candidates for this)
            var disableAllAutoCreation = type.Assembly.GetCustomAttribute<DisableAutoCreationAttribute>() != null;
            var disableTypeAutoCreation = type.GetCustomAttribute<DisableAutoCreationAttribute>(false) != null;

            // these types obviously cannot be instantiated
            if (type.IsAbstract || type.ContainsGenericParameters)
            {
                if (disableTypeAutoCreation)
                    Debug.LogWarning($"Invalid [DisableAutoCreation] on {type.FullName} (only concrete types can be instantiated)");

                return false;
            }

            // only derivatives of ComponentSystemBase are systems
            if (!type.IsSubclassOf(typeof(ComponentSystemBase)))
                throw new System.ArgumentException($"{type} must already be filtered by ComponentSystemBase");

            if (requireExecuteAlways)
            {
                if (Attribute.IsDefined(type, typeof(ExecuteInEditMode)))
                    Debug.LogError($"{type} is decorated with {typeof(ExecuteInEditMode)}. Support for this attribute will be deprecated. Please use {typeof(ExecuteAlways)} instead.");
                if (!Attribute.IsDefined(type, typeof(ExecuteAlways)))
                    return false;
            }

            // the auto-creation system instantiates using the default ctor, so if we can't find one, exclude from list
            if (type.GetConstructors().All(c => c.GetParameters().Length != 0))
            {
                // we want users to be explicit
                if (!disableTypeAutoCreation && !disableAllAutoCreation)
                    Debug.LogWarning($"Missing default ctor on {type.FullName} (or if you don't want this to be auto-creatable, tag it with [DisableAutoCreation])");

                return false;
            }

            if (disableTypeAutoCreation || disableAllAutoCreation)
            {
                if (disableTypeAutoCreation && disableAllAutoCreation)
                    Debug.LogWarning($"Redundant [DisableAutoCreation] on {type.FullName} (attribute is already present on assembly {type.Assembly.GetName().Name}");

                return false;
            }

            var systemFlags = WorldSystemFilterFlags.Default;
            var attrib = type.GetCustomAttribute<WorldSystemFilterAttribute>(true);
            if (attrib != null)
                systemFlags = attrib.FilterFlags;

            return (filterFlags & systemFlags) != 0;
        }
        
        static IEnumerable<System.Type> GetTypesDerivedFrom(Type type)
        {
            #if UNITY_EDITOR
            return UnityEditor.TypeCache.GetTypesDerivedFrom(type);
            #else

            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                    continue;

                try
                {
                    var assemblyTypes = assembly.GetTypes();
                    foreach (var t in assemblyTypes)
                    {
                        if (type.IsAssignableFrom(t))
                            types.Add(t);
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    foreach (var t in e.Types)
                    {
                        if (t != null && type.IsAssignableFrom(t))
                            types.Add(t);
                    }

                    Debug.LogWarning($"DefaultWorldInitialization failed loading assembly: {(assembly.IsDynamic ? assembly.ToString() : assembly.Location)}");
                }
            }

            return types;
            #endif
        }

        static ICustomBootstrap CreateBootStrap()
        {
            var bootstrapTypes = GetTypesDerivedFrom(typeof(ICustomBootstrap));
            Type selectedType = null;

            foreach (var bootType in bootstrapTypes)
            {
                if (bootType.IsAbstract || bootType.ContainsGenericParameters)
                    continue;

                if (selectedType == null)
                    selectedType = bootType;
                else if (selectedType.IsAssignableFrom(bootType))
                    selectedType = bootType;
                else if (!bootType.IsAssignableFrom(selectedType))
                    Debug.LogError("Multiple custom ICustomBootstrap specified, ignoring " + bootType);
            }
            ICustomBootstrap bootstrap = null;
            if (selectedType != null)
                bootstrap = Activator.CreateInstance(selectedType) as ICustomBootstrap;

            return bootstrap;
        }
    }
}
