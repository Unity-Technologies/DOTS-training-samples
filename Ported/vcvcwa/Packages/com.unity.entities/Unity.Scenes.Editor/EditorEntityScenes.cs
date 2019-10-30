using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Conversion;
using Unity.Entities.Serialization;
using Unity.Entities.Streaming;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.VersionControl;
using static Unity.Entities.GameObjectConversionUtility;
using Hash128 = Unity.Entities.Hash128;
using UnityObject = UnityEngine.Object;
using AssetImportContext = UnityEditor.Experimental.AssetImporters.AssetImportContext;

namespace Unity.Scenes.Editor
{
    public static class EditorEntityScenes
    {
        static readonly ProfilerMarker k_ProfileEntitiesSceneSave = new ProfilerMarker("EntitiesScene.Save");
        static readonly ProfilerMarker k_ProfileEntitiesSceneWriteObjRefs = new ProfilerMarker("EntitiesScene.WriteObjectReferences");
        static readonly ProfilerMarker k_ProfileEntitiesSceneSaveHeader = new ProfilerMarker("EntitiesScene.WriteHeader");
        static readonly ProfilerMarker k_ProfileEntitiesSceneSaveConversionLog = new ProfilerMarker("EntitiesScene.WriteConversionLog");

        public static bool IsEntitySubScene(Scene scene)
        {
            return scene.isSubScene;
        }

        static AABB GetBoundsAndRemove(EntityManager entityManager, EntityQuery query)
        {
            var bounds = MinMaxAABB.Empty;
            using (var allBounds = query.ToComponentDataArray<SceneBoundingVolume>(Allocator.TempJob))
            {
                foreach(var b in allBounds)
                    bounds.Encapsulate(b.Value);
            }

            using (var entities = query.ToEntityArray(Allocator.TempJob))
            {
                foreach (var e in entities)
                {
                    // Query includes SceneBoundingVolume & SceneSection
                    // If thats the only data, just destroy the entity
                    if (entityManager.GetComponentCount(e) == 2)
                        entityManager.DestroyEntity(e);
                    else
                        entityManager.RemoveComponent<SceneBoundingVolume>(e);
                }
            }

            return bounds;
        }

        internal static string GetSceneWritePath(Hash128 sceneGUID, EntityScenesPaths.PathType type, string subsectionName, AssetImportContext ctx)
        {
            var prefix = string.IsNullOrEmpty(subsectionName) ? "" : subsectionName + ".";
            var path = ctx.GetResultPath(prefix + EntityScenesPaths.GetExtension(type));

            return path;
        }


        public static SceneSectionData[] WriteEntityScene(Scene scene, GameObjectConversionSettings settings)
        {
            var world = new World("ConversionWorld");
            var entityManager = world.EntityManager;
            settings.DestinationWorld = world;

            bool disposeBlobAssetCache = false;
            if (settings.blobAssetStore == null)
            {
                settings.blobAssetStore = new BlobAssetStore();
                disposeBlobAssetCache = true;
            }

            List<(int, LogEventData)> journalData = null;

            settings.ConversionWorldPreDispose += conversionWorld =>
            {
                var mappingSystem = conversionWorld.GetExistingSystem<GameObjectConversionMappingSystem>();
                journalData = mappingSystem.JournalData.SelectLogEventsOrdered().ToList();
            };
            
            ConvertScene(scene, settings);
            EntitySceneOptimization.Optimize(world);

            if (settings.AssetImportContext != null)
            {
                using(var allTypes = new NativeHashMap<ComponentType, int>(100, Allocator.Temp))
                using(var archetypes = new NativeList<EntityArchetype>(Allocator.Temp))
                {
                    entityManager.GetAllArchetypes(archetypes);
                    foreach (var archetype in archetypes)
                    {
                        using (var componentTypes = archetype.GetComponentTypes())
                            foreach (var componentType in componentTypes)
                                if (allTypes.TryAdd(componentType, 0))
                                    TypeDependencyCache.AddDependency(settings.AssetImportContext, componentType);
                    }
                }
                
                TypeDependencyCache.AddAllSystemsDependency(settings.AssetImportContext);
            }


            var sceneSections = new List<SceneSectionData>();

            var subSectionList = new List<SceneSection>();
            entityManager.GetAllUniqueSharedComponentData(subSectionList);
            var extRefInfoEntities = new NativeArray<Entity>(subSectionList.Count, Allocator.Temp);

            NativeArray<Entity> entitiesInMainSection;
            
            var sectionQuery = entityManager.CreateEntityQuery(
                new EntityQueryDesc
                {
                    All = new[] {ComponentType.ReadWrite<SceneSection>()},
                    Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
                }
            );

            var sectionBoundsQuery = entityManager.CreateEntityQuery(
                new EntityQueryDesc
                {
                    All = new[] {ComponentType.ReadWrite<SceneBoundingVolume>(), ComponentType.ReadWrite<SceneSection>()},
                    Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
                }
            );

            var sceneGUID = settings.SceneGUID;
            
            {
                var section = new SceneSection {SceneGUID = sceneGUID, Section = 0};
                sectionQuery.SetSharedComponentFilter(new SceneSection { SceneGUID = sceneGUID, Section = 0 });
                sectionBoundsQuery.SetSharedComponentFilter(new SceneSection { SceneGUID = sceneGUID, Section = 0 });
                entitiesInMainSection = sectionQuery.ToEntityArray(Allocator.TempJob);


                var bounds = GetBoundsAndRemove(entityManager, sectionBoundsQuery);
                
                // Each section will be serialized in its own world, entities that don't have a section are part of the main scene.
                // An entity that holds the array of external references to the main scene is required for each section.
                // We need to create them all before we start moving entities to section scenes,
                // otherwise they would reuse entities that have been moved and mess up the remapping tables.
                for(int sectionIndex = 1; sectionIndex < subSectionList.Count; ++sectionIndex)
                {
                    if (subSectionList[sectionIndex].Section == 0)
                        // Main section, the only one that doesn't need an external ref array
                        continue;

                    var extRefInfoEntity = entityManager.CreateEntity();
                    entityManager.AddSharedComponentData(extRefInfoEntity, subSectionList[sectionIndex]);
                    extRefInfoEntities[sectionIndex] = extRefInfoEntity;
                }

                // Public references array, only on the main section.
                var refInfoEntity = entityManager.CreateEntity();
                entityManager.AddBuffer<PublicEntityRef>(refInfoEntity);
                entityManager.AddSharedComponentData(refInfoEntity, section);
                var publicRefs = entityManager.GetBuffer<PublicEntityRef>(refInfoEntity);

//                entityManager.Debug.CheckInternalConsistency();

                //@TODO do we need to keep this index? doesn't carry any additional info
                for (int i = 0; i < entitiesInMainSection.Length; ++i)
                {
                    PublicEntityRef.Add(ref publicRefs,
                        new PublicEntityRef {entityIndex = i, targetEntity = entitiesInMainSection[i]});
                }

                UnityEngine.Debug.Assert(publicRefs.Length == entitiesInMainSection.Length);

                // Save main section
                var sectionWorld = new World("SectionWorld");
                var sectionManager = sectionWorld.EntityManager;

                var entityRemapping = entityManager.CreateEntityRemapArray(Allocator.TempJob);
                sectionManager.MoveEntitiesFrom(entityManager, sectionQuery, entityRemapping);

                // The section component is only there to break the conversion world into different sections
                // We don't want to store that on the disk
                //@TODO: Component should be removed but currently leads to corrupt data file. Figure out why.
                //sectionManager.RemoveComponent(sectionManager.UniversalQuery, typeof(SceneSection));

				var sectionFileSize = WriteEntityScene(sectionManager, sceneGUID, "0", settings.AssetImportContext, out var objectRefCount);
                sceneSections.Add(new SceneSectionData
                {
                    FileSize = sectionFileSize,
                    SceneGUID = sceneGUID,
                    ObjectReferenceCount = objectRefCount,
                    SubSectionIndex = 0,
                    BoundingVolume = bounds
                });

                entityRemapping.Dispose();
                sectionWorld.Dispose();
            }

            {
                // Index 0 is the default value of the shared component, not an actual section
                for(int subSectionIndex = 0; subSectionIndex < subSectionList.Count; ++subSectionIndex)
                {
                    var subSection = subSectionList[subSectionIndex];
                    if (subSection.Section == 0)
                        continue;

                    sectionQuery.SetSharedComponentFilter(subSection);
                    sectionBoundsQuery.SetSharedComponentFilter(subSection);

                    var bounds = GetBoundsAndRemove(entityManager, sectionBoundsQuery);
                    
                    var entitiesInSection = sectionQuery.ToEntityArray(Allocator.TempJob);

                    if (entitiesInSection.Length > 0)
                    {
                        // Fetch back the external reference entity we created earlier to not disturb the mapping
                        var refInfoEntity = extRefInfoEntities[subSectionIndex];
                        entityManager.AddBuffer<ExternalEntityRef>(refInfoEntity);
                        var externRefs = entityManager.GetBuffer<ExternalEntityRef>(refInfoEntity);

                        // Store the mapping to everything in the main section
                        //@TODO maybe we don't need all that? is this worth worrying about?
                        for (int i = 0; i < entitiesInMainSection.Length; ++i)
                        {
                            ExternalEntityRef.Add(ref externRefs, new ExternalEntityRef{entityIndex = i});
                        }

                        var entityRemapping = entityManager.CreateEntityRemapArray(Allocator.TempJob);

                        // Entities will be remapped to a contiguous range in the section world, but they will
                        // also come with an unpredictable amount of meta entities. We have the guarantee that
                        // the entities in the main section won't be moved over, so there's a free range of that
                        // size at the end of the remapping table. So we use that range for external references.
                        var externEntityIndexStart = entityRemapping.Length - entitiesInMainSection.Length;

                        entityManager.AddComponentData(refInfoEntity,
                            new ExternalEntityRefInfo
                            {
                                SceneGUID = sceneGUID,
                                EntityIndexStart = externEntityIndexStart
                            });

                        var sectionWorld = new World("SectionWorld");
                        var sectionManager = sectionWorld.EntityManager;

                        // Insert mapping for external references, conversion world entity to virtual index in section
                        for (int i = 0; i < entitiesInMainSection.Length; ++i)
                        {
                            EntityRemapUtility.AddEntityRemapping(ref entityRemapping, entitiesInMainSection[i],
                                new Entity {Index = i + externEntityIndexStart, Version = 1});
                        }

                        sectionManager.MoveEntitiesFrom(entityManager, sectionQuery, entityRemapping);

                        // Now that all the required entities have been moved over, we can get rid of the gap between
                        // real entities and external references. This allows remapping during load to deal with a
                        // smaller remap table, containing only useful entries.

                        int highestEntityIndexInUse = 0;
                        for (int i = 0; i < externEntityIndexStart; ++i)
                        {
                            var targetIndex = entityRemapping[i].Target.Index;
                            if (targetIndex < externEntityIndexStart && targetIndex > highestEntityIndexInUse)
                                highestEntityIndexInUse = targetIndex;
                        }

                        var oldExternEntityIndexStart = externEntityIndexStart;
                        externEntityIndexStart = highestEntityIndexInUse + 1;

                        sectionManager.SetComponentData
                        (
                            EntityRemapUtility.RemapEntity(ref entityRemapping, refInfoEntity),
                            new ExternalEntityRefInfo
                            {
                                SceneGUID = sceneGUID,
                                EntityIndexStart = externEntityIndexStart
                            }
                        );

                        // When writing the scene, references to missing entities are set to Entity.Null by default
                        // (but only if they have been used, otherwise they remain untouched)
                        // We obviously don't want that to happen to our external references, so we add explicit mapping
                        // And at the same time, we put them back at the end of the effective range of real entities.
                        for (int i = 0; i < entitiesInMainSection.Length; ++i)
                        {
                            var src = new Entity {Index = i + oldExternEntityIndexStart, Version = 1};
                            var dst = new Entity {Index = i + externEntityIndexStart, Version = 1};
                            EntityRemapUtility.AddEntityRemapping(ref entityRemapping, src, dst);
                        }

                        // The section component is only there to break the conversion world into different sections
                        // We don't want to store that on the disk
                        //@TODO: Component should be removed but currently leads to corrupt data file. Figure out why.
                        //sectionManager.RemoveComponent(sectionManager.UniversalQuery, typeof(SceneSection));
                        var fileSize = WriteEntityScene(sectionManager, sceneGUID, subSection.Section.ToString(), settings.AssetImportContext, out var objectRefCount, entityRemapping);
                        sceneSections.Add(new SceneSectionData
                        {
                            FileSize = fileSize,
                            SceneGUID = sceneGUID,
                            ObjectReferenceCount = objectRefCount,
                            SubSectionIndex = subSection.Section,
                            BoundingVolume = bounds
                        });

                        entityRemapping.Dispose();
                        sectionWorld.Dispose();
                    }

                    entitiesInSection.Dispose();
                }
            }

            {
                var noSectionQuery = entityManager.CreateEntityQuery(
                    new EntityQueryDesc
                    {
                        None = new[] {ComponentType.ReadWrite<SceneSection>()},
                        Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
                    }
                );
                if (noSectionQuery.CalculateEntityCount() != 0)
                    Debug.LogWarning($"{noSectionQuery.CalculateEntityCount()} entities in the scene '{scene.path}' had no SceneSection and as a result were not serialized at all.");
            }
            
            sectionQuery.Dispose();
            sectionBoundsQuery.Dispose();
            entitiesInMainSection.Dispose();
            world.Dispose();
            
            // Save the new header

            var sceneSectionsArray = sceneSections.ToArray();
            WriteHeader(sceneGUID, sceneSectionsArray, scene.name, settings.AssetImportContext);
            
            // If we are writing assets to assets folder directly, then we need to make sure the asset database see them so they can be loaded.
            if (settings.AssetImportContext == null)
                AssetDatabase.Refresh();

            if (disposeBlobAssetCache)
            {
                settings.blobAssetStore.Dispose();
            }
            
            // Save the log of issues that happened during conversion

            WriteConversionLog(sceneGUID, journalData, scene.name, settings.AssetImportContext);

            return sceneSectionsArray;
        }

        static void EnsureFileIsWritableOrThrow(string path, AssetImportContext ctx)
        {
            if (ctx != null)
                return;
            
            // We're going to do file writing manually, so make sure to do version control dance if needed
            if (Provider.isActive && File.Exists(path) && !AssetDatabase.IsOpenForEdit(path, StatusQueryOptions.UseCachedIfPossible))
            {
                var task = Provider.Checkout(path, CheckoutMode.Asset);
                task.Wait();
                if (!task.success)
                    throw new System.Exception($"Failed to checkout entity cache file {path}");
            }
        }

        static int WriteEntityScene(EntityManager scene, Hash128 sceneGUID, string subsection, AssetImportContext ctx, out int objectReferenceCount, NativeArray<EntityRemapUtility.EntityRemapInfo> entityRemapInfos = default)
        {
            k_ProfileEntitiesSceneSave.Begin();
            
            var entitiesBinaryPath = GetSceneWritePath(sceneGUID, EntityScenesPaths.PathType.EntitiesBinary, subsection, ctx);
            var objRefsPath = GetSceneWritePath(sceneGUID, EntityScenesPaths.PathType.EntitiesUnityObjectReferences, subsection, ctx);
            ReferencedUnityObjects objRefs;
            objectReferenceCount = 0;

    	    EnsureFileIsWritableOrThrow(entitiesBinaryPath, ctx);
    
            // Write binary entity file
            int entitySceneFileSize = 0;
            using (var writer = new StreamBinaryWriter(entitiesBinaryPath))
            {
                if (entityRemapInfos.IsCreated)
                    SerializeUtilityHybrid.Serialize(scene, writer, out objRefs, entityRemapInfos);
                else
                    SerializeUtilityHybrid.Serialize(scene, writer, out objRefs);
                entitySceneFileSize = (int)writer.Length;

                // Write object references
                k_ProfileEntitiesSceneWriteObjRefs.Begin();
                if (objRefs != null)
                {
                    var serializedObjectArray = new List<UnityObject>();
                    serializedObjectArray.Add(objRefs);

                    for (int i = 0;i != objRefs.Array.Length;i++)
                    {
                        var obj = objRefs.Array[i];
                        if (obj != null && !EditorUtility.IsPersistent(obj))
                        {
                            if ((obj.hideFlags & HideFlags.DontSaveInBuild) != 0)
                                serializedObjectArray.Add(obj);
                            else
                                objRefs.Array[i] = null;
                        }
                    }
                    
                    UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(serializedObjectArray.ToArray(), objRefsPath, false);
                    objectReferenceCount = objRefs.Array.Length;
                }
                k_ProfileEntitiesSceneWriteObjRefs.End();
            }
            k_ProfileEntitiesSceneSave.End();
            return entitySceneFileSize;
        }
        
        static void WriteHeader(Entities.Hash128 sceneGUID, SceneSectionData[] sections, string sceneName, AssetImportContext ctx)
        {
            k_ProfileEntitiesSceneSaveHeader.Begin();
    
            string headerPath = GetSceneWritePath(sceneGUID, EntityScenesPaths.PathType.EntitiesHeader, "", ctx);
            EnsureFileIsWritableOrThrow(headerPath, ctx);

            var builder = new BlobBuilder(Allocator.TempJob);

            ref var metaData = ref builder.ConstructRoot<SceneMetaData>();
            builder.Construct(ref metaData.Sections, sections);
            builder.AllocateString(ref metaData.SceneName, sceneName);
            BlobAssetReference<SceneMetaData>.Write(builder, headerPath, SceneMetaDataSerializeUtility.CurrentFileFormatVersion);
            builder.Dispose();
            
            k_ProfileEntitiesSceneSaveHeader.End();
        }

        static void WriteConversionLog(Hash128 sceneGUID, List<(int objectInstanceId, LogEventData eventData)> journalData, string sceneName, AssetImportContext ctx)
        {
            if (journalData.Count == 0)
                return;

            using (k_ProfileEntitiesSceneSaveConversionLog.Auto())
            {
                var conversionLogPath = GetSceneWritePath(sceneGUID, EntityScenesPaths.PathType.EntitiesConversionLog, "", ctx);

                using (var writer = File.CreateText(conversionLogPath))
                {
                    foreach (var (objectInstanceId, eventData) in journalData)
                    {
                        var unityObject = EditorUtility.InstanceIDToObject(objectInstanceId);
                        if (eventData.Type != LogType.Exception)
                            writer.WriteLine($"{eventData.Type}: {eventData.Message} from {unityObject.name}");
                        else
                            writer.WriteLine($"{eventData.Message} from {unityObject.name}");
                    }
                }
            }
        }

        
        // OBSOLETE
        
        [Obsolete("WriteEntityScene now receives its configuration parameters through a GameObjectConversionSettings (RemovedAfter 2019-10-17)")]
        [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly]
        public static SceneSectionData[] WriteEntityScene(Scene scene, Hash128 sceneGUID, ConversionFlags conversionFlags)
            => WriteEntityScene(scene, new GameObjectConversionSettings { SceneGUID = sceneGUID, ConversionFlags = conversionFlags });        
    }    
}
