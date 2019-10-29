# Change log

## [0.2.0] - 2019-12-31

**This version requires Unity 2019.3 0b6+**

### New Features

* Live link UI improvements:
  * It is now possible to select a Live Link compatible build setting, build, and run a Live Link player directly from the UI next to the playbar.
  * Live Link connections currently attached to the editor are now viewable in a dropdown next to the playbar.
* Automatically generate authoring components for IComponentData with IL post-processing. Any component data marked with a GenerateAuthoringComponent attribute will generate the corresponding authoring MonoBehaviour with a Convert method.
* EntityQueryMask has been added, which allows for quick confirmation of if an Entity would be returned by an EntityQuery without filters via EntityQueryMask.Matches(Entity entity).  An EntityQueryMask can be obtained by calling EntityManager.GetEntityQueryMask(EntityQuery query).
* The UnityEngine component StopConvertToEntity can be used to interrupt ConvertToEntity recursion, and should be preferred over a ConvertToEntity set to "convert and inject" for that purpose.
* EntityDebugger now shows IDs in a separate column, so you can still see them when entities have custom names
* Entity references in the Entity Inspector have a "Show" button which will select the referenced Entity in the Debugger.
* An ArchetypeChunkIterator can be created by calling GetArchetypeChunkIterator on an EntityQuery. You may run an IJobChunk while bypassing the Jobs API by passing an ArchetypeChunkIterator into IJobChunk.RunWithoutJobs().
* The [AlwaysSynchronizeSystem] attribute has been added, which can be applied to a JobComponentSystem to force it to synchronize on all of its dependencies before every update.
* BoneIndexOffset has been added, which allows the Animation system to communicate a bone index offset to the Hybrid Renderer.
* Initial support for using Hybrid Components during conversion, see the HybridComponent sample in the StressTests folder.
* Unity Entities now supports the Fast Enter playmode which can be enabled in the project settings. It is recommended to be turned on for all dots projects
* New `GameObjectConversionSystem.ForkSettings()` that provides a very specialized method for creating a fork of the current conversion settings with a different "EntityGuid namespace", which can be used for nested conversions. This is useful for example in net code where multiple root-level variants of the same authoring object need to be created in the destination world.
* EntityManager LockChunkOrder and UnlockChunkOrder are deprecated.

### Fixes

* Setting `ComponentSystemGroup.Enabled` to `false` now calls `OnStopRunning()` recursively on the group's member systems, not just on the group itself.
* Updated Properties pacakge to fix an exception when showing Physics ComponentData in the inspector
* The LocalToParentSystem will no longer write to the LocalToWorld component of entities that have a component with the WriteGroup(typeof(LocalToWorld)).
* Entity Debugger styling work better with Pro theme
* Entity Inspector no longer has runaway indentation
* AddSharedComponentData, SetSharedComponentData did not always update SharedComponentOrderVersion
* Fixes serialization issue when reading in managed IComponentData containing array types and UnityEngine.Object references.
* No longer an exception to re-add a tag component with EntityQuery.
* Significantly improved Entity instantiation performance when running in-Editor.
* `AddComponent<T>(NativeArray<Entity>)` now reliably throws an `ArgumentException` if any of the target entities are invalid.

### Changes

* Updated dependencies for this package.
* 'SubSceneStreamingSystem' has been renamed to `SceneSectionStreamingSystem` and is now internal
* Deprecated `_SceneEntities` in `SubScene.cs`. Please use `SceneSystem.LoadAsync` / `Unload` with the respective SceneGUID instead. This API will be removed after 2019-11-22.
* Updated `com.unity.properties` to `0.10.0-preview`.
* Updated `com.unity.serialization` to `0.6.1-preview`.
* Added support for managed `IComponentData` types such as `class MyComponent : IComponentData {}` which allows managed types such as GameObjects or List<>s to be stored in components. Users should use managed components sparingly in production code when possible as these components cannot be used by the Job System or archetype chunk storage and thus will be significantly slower to work with. Refer to the documentation for [component data](Documentation~/component_data.md) for more details on managed component use, implications and prevention.
* The deprecated `GetComponentGroup()` APIs are now `protected` and can only be called from inside a System like their `GetEntityQuery()` successors.
* All GameObjects with a ConvertToEntity set to "Convert and Destroy" will all be processed within the same conversion pass, this allows cross-referencing.
* Duplicate component adds always ignored
* When adding component to single entity via EntityQuery, entity is moved to matching chunk instead of chunk achetype changing.
* "Used by Systems" list skips queries with filters
* Managed IComponentData no longer require all fields to be non-null after default construction.
* SharedComponentData is serialized inline with entity and managed IComponentData. If a SharedComponent references a UnityEngine.Object type, that type is serialized separately in an "objrefs" resource asset.
* EntityManager calls EntityComponentStore via burst delegates for Add/Remove components.
* EntityComponentStore cannot throw exceptions (since called as burst delegate from main thread.)
* bool ICustomBootstrap.Initialize(string defaultWorldName) has changed API with no deprecated fallback. It now simply gives you a chance to completely replace the default world initialization by returning true.
* ICustomBootstrap & DefaultWorldInitialization is now composable like this:
```
class MyCustomBootStrap : ICustomBootstrap
{
    public bool Initialize(string defaultWorldName)
    {
        Debug.Log("Executing bootstrap");
        var world = new World("Custom world");
        World.DefaultGameObjectInjectionWorld = world;
        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        return true;
    }
}
```
* ICustomBootstrap can now be inherited and only the most deepest subclass bootstrap will be executed.
* DefaultWorldInitialization.GetAllSystems is not affected by bootstrap, it simply returns a list of systems based on the present dlls & attributes.
* `Time` is now available per-World, and is a property in a `ComponentSystem`.  It is updated from the `UnityEngine.Time` during the `InitializationSystemGroup` of each world.  If you need access to time in a sytem that runs in the `InitializationSystemGroup`, make sure you schedule your system after `UpdateWorldTimeSystem`.  `Time` is also a limited `TimeData` struct; if you need access to any of the extended fields available in `UnityEngine.Time`, access `UnityEngine.Time` explicitly`
* Systems are no longer removed from a `ComponentSystemGroup` if they throw an exception from their `OnUpdate`. This behavior was more confusing than helpful.
* Managed IComponentData no longer require implementing the `IEquatable<>` interface and overriding `GetHashCode()`. If either function is provided it will be preferred, otherwise the component will be inspected generically for equality.
* `EntityGuid` is now constructed from an originating ID, a namespace ID, and a serial, which can be safely extracted from their packed form using new getters. Use `a` and `b` fields when wanting to treat this as an opaque struct (the packing may change again in the future, as there are still unused bits remaining). The a/b constructor has been removed, to avoid any ambiguity.
* Updated `com.unity.platforms` to `0.1.6-preview`.

## [0.1.1] - 2019-08-06

### New Features
* EntityManager.SetSharedComponentData(EntityQuery query, T componentData) has been added which lets you efficiently swap a shared component data for a whole query. (Without moving any component data)

### Upgrade guide

* The deprecated `OnCreateManager` and `OnDestroyManager` are now compilation errors in the `NET_DOTS` profile as overrides can not be detected reliably (without reflection).
To avoid the confusion of "why is that not being called", especially when there is no warning issued, this will now be a compilation error. Use `OnCreate` and `OnDestroy` instead.

### Changes

* Updated default version of burst to `1.1.2`

### Fixes

* Fixed potential memory corruption when calling RemoveComponent on a batch of entities that didn't have the component.
* Fixed an issue where an assert about chunk layout compatibility could be triggered when adding a shared component via EntityManager.AddSharedComponentData<T>(EntityQuery entityQuery, T componentData).
* Fixed an issue where Entities without any Components would cause UI errors in the Chunk Info view
* Fixed EntityManager.AddComponent(NativeArray<Entity> entities, ComponentType componentType) so that it handles duplicate entities in the input NativeArray. Duplicate entities are discarded and the component is added only once. Prior to this fix, an assert would be triggered when checking for chunk layout compatibility.
* Fixed invalid update path for `ComponentType.Create`. Auto-update is available in Unity `2019.3` and was removed for previous versions where it would fail (the fallback implementation will work as before).


## [0.1.0] - 2019-07-30

### New Features

* Added the `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_RUNTIME_WORLD` and `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP_EDITOR_WORLD` defines which respectively can be used to disable runtime and editor default world generation.  Defining `#UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOTSTRAP` will still disable all default world generation.
* Allow structural changes to entities (add/remove components, add/destroy entities, etc.) while inside of `ForEach` lambda functions.  This negates the need for using `PostUpdateCommands` inside of ForEach.
* `EntityCommandBuffer` has some additional methods for adding components based on `ComponentType`, or for adding empty components of a certain type (`<T>`)
* EntityManagerDiffer & EntityManagerPatcher provides highly optimized diffing & patching functionality. It is used in the editor for providing scene conversion live link.
* Added support for `EntityManager.MoveEntitiesFrom` with managed arrays (Object Components).
* EntityManager.SetArchetype lets you change an entity to a specific archetype. Removing & adding the necessary components with default values. System state components are not allowed to be removed with this method, it throws an exception to avoid accidental system state removal. (Used in incremental live link conversion it made conversion from 100ms -> 40ms for 1000 changed game objects)
* Entity Debugger's system list now has a string filter field. This makes it easier to find a system by name when you have a lot of systems.
* Added IComponentData type `Asset` that will be used by Tiny to convert Editor assets to runtime assets
* Filled in some `<T>` holes in the overloads we provide in `EntityManager`
* New `Entities.WithIncludeAll()` that will include in matching all components that are normally ignored by default (currently `Prefab` and `Disabled`)
* EntityManager.CopyAndReplaceEntitiesFrom has been added it can be used to store & restore a backup of the world for the purposes of general purpose simulation rollback.

### Upgrade guide

* WorldDiff has been removed. It has been replaced by EntityManagerDiff & EntityManagerPatch.
* Renamed `EntityGroupManager` to `EntityQueryManager`.

### Changes

* EntityArchetype.GetComponentTypes no longer includes Entity in the list of components (it is implied). Behaviour now matches the EntityMangager.GetComponentTypes method. This matches the behavior of the corresponding `EntityManager` function.
* `EntityCommandBuffer.AddComponent(Entity, ComponentType)` no longer fails if the target entity already has the specified component.
*  DestroyEntity(EntityQuery entityQuery) now uses burst internally.

### Fixes

* Entity Inspector now shows DynamicBuffer elements in pages of five at a time
* Resources folder renamed to Styles so as not to add editor assets to built player
* `EntityQueryBuilder.ShallowEquals` (used from `Entities.ForEach`) no longer boxes and allocs GC
* Improved error message for unnecessary/invalid `UpdateBefore` and `UpdateAfter`
* Fixed leak in BlobBuilder.CreateBlobAssetReference
* ComponentSystems are now properly preserved when running the UnityLinker. Note this requires 19.3a10 to work correctly. If your project is not yet using 19.3 you can workaround the issue using the link.xml file. https://docs.unity3d.com/Manual//IL2CPP-BytecodeStripping.html
* Types that trigger an exception in the TypeManager won't prevent other types from initializing properly.

## [0.0.12-preview.33] - 2019-05-24

### New Features

* `[DisableAutoCreation]` can now apply to entire assemblies, which will cause all systems contained within to be excluded from automatic system creation. Useful for test assemblies.
* Added `ComponentSystemGroup.RemoveSystemFromUpdateList()`
* `EntityCommandBuffer` has commands for adding/removing components, deleting entities and adding shared components based on an EntityQuery and its filter. Not available in the `Concurrent` version

### Changes

* Generic component data types must now be registered in advance. Use [RegisterGenericComponentType] attribute to register each concrete use. e.g. `[assembly: RegisterGenericComponentType(typeof(TypeManagerTests.GenericComponent<int>))]`
* Attempting to call `Playback()` more than once on the same EntityCommandBuffer will now throw an error.
* Improved error checking for `[UpdateInGroup]`, `[UpdateBefore]`, and `[UpdateAfter]` attributes
* TypeManager no longer imposes alignment requirements on components containing pointers. Instead, it now throws an exception if you try to serialize a blittable component containing an unmanaged pointer, which suggests different alternatives.

### Fixes

* Fixed regression where accessing and destroying a blob asset in a burst job caused an exception
* Fixed bug where entities with manually specified `CompositeScale` were not updated by `TRSLocalToWorldSystem`.
* Error message when passing in invalid parameters to CreateSystem() is improved.
* Fixed bug where an exception due to aggressive pointer restrictions could leave the `TypeManager` in an invalid state
* SceneBoundingVolume is now generated seperately for each subsection
* SceneBoundingVolume no longer throws exceptions in conversion flow
* Fixed regression where calling AddComponent(NativeArray<Entity> entities, ComponentType componentType) could cause a crash.
* Fixed bug causing error message to appear in Inspector header when `ConvertToEntity` component was added to a disabled GameObject.

## [0.0.12-preview.32] - 2019-05-16

### New Features

* Added BlobBuilder which is a new API to build Blob Assets that does not require preallocating one contiguous block of memory. The BlobAllocator is now marked obsolete.
* Added versions of `IJobForEach` that support `DynamicBuffer`s
  * Due to C# language constraints, these overloads needed different names. The format for these overloads follows the following structure:
    * All job names begin with either `IJobForEach` or `IJobForEachEntity`
    * All jobs names are then followed by an underscore `_` and a combination of letter corresponding to the parameter types of the job
      * `B` - `IBufferElementData`
      * `C` - `IComponentData`
      * `E` - `Entity` (`IJobForEachWithEntity` only)
    * All suffixes for `WithEntity` jobs begin with `E`
    * All data types in a suffix are in alphabetical order
  * Here is the complete list of overloads:
    * `IJobForEach_C`, `IJobForEach_CC`, `IJobForEach_CCC`, `IJobForEach_CCCC`, `IJobForEach_CCCCC`, `IJobForEach_CCCCCC`
    * `IJobForEach_B`, `IJobForEach_BB`, `IJobForEach_BBB`, `IJobForEach_BBBB`, `IJobForEach_BBBBB`, `IJobForEach_BBBBBB`
    * `IJobForEach_BC`, `IJobForEach_BCC`, `IJobForEach_BCCC`, `IJobForEach_BCCCC`, `IJobForEach_BCCCCC`, `IJobForEach_BBC`, `IJobForEach_BBCC`, `IJobForEach_BBCCC`, `IJobForEach_BBCCCC`, `IJobForEach_BBBC`, `IJobForEach_BBBCC`, `IJobForEach_BBBCCC`, `IJobForEach_BBBCCC`, `IJobForEach_BBBBC`, `IJobForEach_BBBBCC`, `IJobForEach_BBBBBC`
    * `IJobForEachWithEntity_EB`, `IJobForEachWithEntity_EBB`, `IJobForEachWithEntity_EBBB`, `IJobForEachWithEntity_EBBBB`, `IJobForEachWithEntity_EBBBBB`, `IJobForEachWithEntity_EBBBBBB`
    * `IJobForEachWithEntity_EC`, `IJobForEachWithEntity_ECC`, `IJobForEachWithEntity_ECCC`, `IJobForEachWithEntity_ECCCC`, `IJobForEachWithEntity_ECCCCC`, `IJobForEachWithEntity_ECCCCCC`
    * `IJobForEachWithEntity_BC`, `IJobForEachWithEntity_BCC`, `IJobForEachWithEntity_BCCC`, `IJobForEachWithEntity_BCCCC`, `IJobForEachWithEntity_BCCCCC`, `IJobForEachWithEntity_BBC`, `IJobForEachWithEntity_BBCC`, `IJobForEachWithEntity_BBCCC`, `IJobForEachWithEntity_BBCCCC`, `IJobForEachWithEntity_BBBC`, `IJobForEachWithEntity_BBBCC`, `IJobForEachWithEntity_BBBCCC`, `IJobForEachWithEntity_BBBCCC`, `IJobForEachWithEntity_BBBBC`, `IJobForEachWithEntity_BBBBCC`, `IJobForEachWithEntity_BBBBBC`
    * Note that you can still use `IJobForEach` and `IJobForEachWithEntity` as before if you're using only `IComponentData`.
* EntityManager.SetEnabled API automatically enables & disables an entity or set of entities. If LinkedEntityGroup is present the whole group is enabled / disabled. Inactive game objects automatically get a LinkedEntityGroup added so that EntityManager.SetEnabled works as expected out of the box.
* Add `WithAnyReadOnly` and `WithAllReadyOnly` methods to EntityQueryBuilder to specify queries that filter on components with access type ReadOnly.
* No longer throw when the same type is in a WithAll and ForEach delegate param for ForEach queries.
* `DynamicBuffer` CopyFrom method now supports another DynamicBuffer as a parameter.
* Fixed cases that would not be handled correctly by the api updater.

### Upgrade guide

* Usages of BlobAllocator will need to be changed to use BlobBuilder instead. The API is similar but Allocate now returns the data that can be populated:

  ```csharp
  ref var root = ref builder.ConstructRoot<MyData>();
  var floatArray = builder.Allocate(3, ref root.floatArray);
  floatArray[0] = 0; // root.floatArray[0] can not be used and will throw on access
  ```

* ISharedComponentData with managed fields must implement IEquatable and GetHashCode
* IComponentData and ISharedComponentData implementing IEquatable must also override GetHashCode

### Fixes

* Comparisons of managed objects (e.g. in shared components) now work as expected
* Prefabs referencing other prefabs are now supported in game object entity conversion process
* Fixed a regression where ComponentDataProxy was not working correctly on Prefabs due to a ordering issue.
* Exposed GameObjectConversionDeclarePrefabsGroup for declaring prefab references. (Must happen before any conversion systems run)
* Inactive game objects are automatically converted to be Disabled entities
* Disabled components are ignored during conversion process. Behaviour.Enabled has no direct mapping in ECS. It is recommended to Disable whole entities instead
* Warnings are now issues when asking for a GetPrimaryEntity that is not a game object that is part of the converted group. HasPrimaryEntity can be used to check if the game object is part of the converted group in case that is necessary.
* Fixed a race condition in `EntityCommandBuffer.AddBuffer()` and `EntityCommandBuffer.SetBuffer()`

## [0.0.12-preview.31] - 2019-05-01

### New Features

### Upgrade guide

* Serialized entities file format version has changed, Sub Scenes entity caches will require rebuilding.

### Changes

* Adding components to entities that already have them is now properly ignored in the cases where no data would be overwritten. That means the inspectable state does not change and thus determinism can still be guaranteed.
* Restored backwards compatibility for `ForEach` API directly on `ComponentSystem` to ease people upgrading to the latest Unity.Entities package on top of Megacity.
* Rebuilding the entity cache files for sub scenes will now properly request checkout from source control if required.

### Fixes

* `IJobForEach` will only create new entity queries when scheduled, and won't rely on injection anymore. This avoids the creation of useless queries when explicit ones are used to schedule those jobs. Those useless queries could cause systems to keep updating even though the actual queries were empty.
* APIs changed in the previous version now have better obsolete stubs and upgrade paths.  All obsolete APIs requiring manual code changes will now soft warn and continue to work, instead of erroring at compile time.  These respective APIs will be removed in a future release after that date.
* LODGroup conversion now handles renderers being present in a LOD Group in multipe LOD levels correctly
* Fixed potential memory leak when disposing an EntityCommandBuffer after certain types of playback errors
* Fixed an issue where chunk utilization histograms weren't properly clipped in EntityDebugger
* Fixed an issue where tag components were incorrectly shown as subtractive in EntityDebugger
* ComponentSystem.ShouldRunSystem() exception message now more accurately reports the most likely reason for the error when the system does not exist.

### Known Issues

* It might happen that shared component data with managed references is not compared for equality correctly with certain profiles.


## [0.0.12-preview.30] - 2019-04-05

### New Features
Script templates have been added to help you create new component types and systems, similar to Unity's built-in template for new MonoBehaviours. Use them via the Assets/Create/ECS menu.

### Upgrade guide

Some APIs have been deprecated in this release:

[API Deprecation FAQ](https://forum.unity.com/threads/api-deprecation-faq-0-0-23.636994/)

** Removed obsolete ComponentSystem.ForEach
** Removed obsolete [Inject]
** Removed obsolete ComponentDataArray
** Removed obsolete SharedComponentDataArray
** Removed obsolete BufferArray
** Removed obsolete EntityArray
** Removed obsolete ComponentGroupArray

####ScriptBehaviourManager removal
* The ScriptBehaviourManager class has been removed.
* ComponentSystem and JobComponentSystem remain as system base classes (with a common ComponentSystemBase class)
  * ComponentSystems have overridable methods OnCreateManager and OnDestroyManager. These have been renamed to OnCreate and OnDestroy.
    * This is NOT handled by the obsolete API updater and will need to be done manually.
    * The old OnCreateManager/OnDestroyManager will continue to work temporarily, but will print a warning if a system contains them.
* World APIs have been updated as follows:
  * CreateManager, GetOrCreateManager, GetExistingManager, DestroyManager, BehaviourManagers have been renamed to CreateSystem, GetOrCreateSystem, GetExistingSystem, DestroySystem, Systems.
    * These should be handled by the obsolete API updater.
  * EntityManager is no longer accessed via GetExistingManager. There is now a property directly on World: World.EntityManager.
    * This is NOT handled by the obsolete API updater and will need to be done manually.
    * Searching and replacing Manager<EntityManager> should locate the right spots. For example, world.GetExistingManager<EntityManager>() should become just world.EntityManager.

#### IJobProcessComponentData renamed to IJobForeach
This rename unfortunately cannot be handled by the obsolete API updater.
A global search and replace of IJobProcessComponentData to IJobForEach should be sufficient.

#### ComponentGroup renamed to EntityQuery
ComponentGroup has been renamed to EntityQuery to better represent what it does.
All APIs that refer to ComponentGroup have been changed to refer to EntityQuery in their name, e.g. CreateEntityQuery, GetEntityQuery, etc.

#### EntityArchetypeQuery renamed to EntityQueryDesc
EntityArchetypeQuery has been renamed to EntityQueryDesc

### Changes
* Minimum required Unity version is now 2019.1.0b9
* Adding components to entities that already have them is now properly ignored in the cases where no data would be overwritten.
* UNITY_CSHARP_TINY is now NET_DOTS to match our other NET_* defines

### Fixes
* Fixed exception in inspector when Script is missing
* The presence of chunk components could lead to corruption of the entity remapping during deserialization of SubScene sections.
* Fix for an issue causing filtering with IJobForEachWithEntity to try to access entities outside of the range of the group it was scheduled with.

<!-- Template for version sections
## [0.0.0-preview.0]

### New Features


### Upgrade guide


### Changes


### Fixes
-->
