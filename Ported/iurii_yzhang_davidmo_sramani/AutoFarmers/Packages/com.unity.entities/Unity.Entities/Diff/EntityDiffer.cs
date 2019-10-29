using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;

#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityObject = UnityEngine.Object;
#endif

namespace Unity.Entities
{
    /// <summary>
    /// The exception throw when encountering multiple entities with the same <see cref="EntityGuid"/> value.
    /// </summary>
    [Serializable]
    class DuplicateEntityGuidException : Exception
    {
        /// <summary>
        /// The duplicate guids found in during the diff and the counts of how many times they were duplicated.
        /// </summary>
        public DuplicateEntityGuid[] DuplicateEntityGuids { get; private set; }

        /// <summary>
        /// Initialized a new instance of the <see cref="DuplicateEntityGuidException"/> class.
        /// </summary>
        public DuplicateEntityGuidException(DuplicateEntityGuid[] duplicates)
            : base(CreateMessage(duplicates)) { DuplicateEntityGuids = duplicates; }

        static string CreateMessage(DuplicateEntityGuid[] duplicates)
        {
            var message = $"Found {duplicates.Length} {nameof(EntityGuid)} components that are shared by more than one Entity";

            #if UNITY_EDITOR
            message += "\n" + ToString(duplicates);
            #else
            message += $"; see $exception.{nameof(DuplicateEntityGuids)} for more information.";
            #endif

            return message;
        }

        #if UNITY_EDITOR
        static string ToString(DuplicateEntityGuid[] duplicates)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < duplicates.Length; ++i)
            {
                if (i == 10)
                {
                    sb.AppendLine($"...{duplicates.Length - i} more...");
                    break;
                }

                var dup = duplicates[i];
                var obj = EditorUtility.InstanceIDToObject(dup.EntityGuid.OriginatingId);
                var name = obj != null ? obj.ToString() : "<not found>";

                sb.AppendLine($"guid = {dup.EntityGuid}, count = {dup.DuplicateCount}, obj = {name}");
            }

            return sb.ToString();
        }
        public override string ToString() => ToString(DuplicateEntityGuids);
        #endif
    }

    /// <summary>
    /// Parameters used to configure the the execution of the differ.
    /// </summary>
    [Flags]
    public enum EntityManagerDifferOptions
    {
        None = 0,

        /// <summary>
        /// If set; the resulting <see cref="EntityChanges"/> will include the forward change set.
        /// </summary>
        IncludeForwardChangeSet = 1 << 1,

        /// <summary>
        /// If set; the resulting <see cref="EntityChanges"/> will include the reverse change set.
        ///
        /// This can be applied to the world to reverse the changes (i.e. undo).
        /// </summary>
        IncludeReverseChangeSet = 1 << 2,

        /// <summary>
        /// If set; the shadow world will be updated with the latest changes.
        /// </summary>
        FastForwardShadowWorld = 1 << 3,

        /// <summary>
        /// If set; all references to destroyed or missing entities will be set to Entity.Null before computing changes.
        /// 
        /// When applying a change this is needed to patch references to restored entities (they were destroyed but are being brought back by the change set).
        /// </summary>
        /// <remarks>
        /// Performance scales with the total number of entities with the <see cref="EntityGuid"/> component in the source world.
        /// </remarks>
        ClearMissingReferences = 1 << 4,

        /// <summary>
        /// If this flag is set; the entire world is checks for duplicate <see cref="EntityGuid"/> components.
        /// </summary>
        /// <remarks>
        /// Performance scales with the number of created entities in the source world with the <see cref="EntityGuid"/> component.
        /// </remarks>
        ValidateUniqueEntityGuid = 1 << 5,

        /// <summary>
        /// The default set of options used by the <see cref="EntityDiffer"/>
        /// </summary>
        Default = IncludeForwardChangeSet | 
                  IncludeReverseChangeSet | 
                  FastForwardShadowWorld | 
                  ClearMissingReferences |
                  ValidateUniqueEntityGuid
    }
    
    /// <summary>
    /// The <see cref="EntityDiffer"/> is used to build a set of changes between two worlds.
    /// </summary>
    /// <remarks>
    /// This class can be used to determine both forward and/or reverse changes between the worlds.
    ///
    /// This class relies on the <see cref="EntityGuid"/> to uniquely identify entities, and expects that each entity
    /// will have a unique value for this component. If any duplicate <see cref="EntityGuid"/> values are encountered
    /// a <see cref="DuplicateEntityGuidException"/> will be thrown.
    ///
    /// <seealso cref="EntityManagerDiffer"/> for tracking changes over time.
    /// </remarks>
    unsafe partial class EntityDiffer
    {
#if !NET_DOTS && (ENABLE_PROFILER || UNITY_EDITOR)
        static Profiling.ProfilerMarker s_GetChangesProfilerMarker = new Profiling.ProfilerMarker("GetChanges");
#endif
        
        /// <summary>
        /// Generates a detailed change set between <see cref="srcEntityManager"/> and <see cref="dstEntityManager"/>.
        /// All entities to be considered must have the <see cref="EntityGuid"/> component with a unique value.
        /// The resulting <see cref="Entities.EntityChanges"/> must be disposed when no longer needed.
        /// </summary>
        /// <remarks>
        /// When using the <see cref="EntityManagerDifferOptions.FastForwardShadowWorld"/> the destination world must be a direct ancestor to
        /// the source world, and must only be updated using this call or similar methods. There should be no direct changes to destination world.
        /// </remarks>
        internal static EntityChanges GetChanges(
            EntityManager srcEntityManager,
            EntityManager dstEntityManager,
            EntityManagerDifferOptions options,
            EntityQueryDesc entityQueryDesc,
            BlobAssetCache blobAssetCache,
            Allocator allocator)
        {
#if !NET_DOTS && (ENABLE_PROFILER || UNITY_EDITOR)
            s_GetChangesProfilerMarker.Begin();
#endif
            CheckEntityGuidComponent(entityQueryDesc);
            
            var changes = default(EntityChanges);
            
            if (options == EntityManagerDifferOptions.None)
                return changes;
            
            srcEntityManager.CompleteAllJobs();
            dstEntityManager.CompleteAllJobs();

            var srcEntityQuery = srcEntityManager.CreateEntityQuery(entityQueryDesc);
            var dstEntityQuery = dstEntityManager.CreateEntityQuery(entityQueryDesc);

            // Gather a set of a chunks to consider for diffing in both the src and dst worlds.
            using (var srcChunks = srcEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob, out var srcChunksJob))
            using (var dstChunks = dstEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob, out var dstChunksJob))
            {
                JobHandle clearMissingReferencesJob = default;
                
                if (CheckOption(options, EntityManagerDifferOptions.ClearMissingReferences))
                {
                    // Opt-in feature.
                    // This is a special user case for references to destroyed entities.
                    // If entity is destroyed, any references to that entity will remain set but become invalid (i.e. broken).
                    // This option ensures that references to non-existent entities will be explicitly set to Entity.Null which
                    // will force it to be picked up in the change set.
                    ClearMissingReferences(srcEntityManager, srcChunks, out clearMissingReferencesJob, srcChunksJob);
                }
                
                // @TODO NET_DOTS does not support JobHandle.CombineDependencies with 3 arguments.
#if NET_DOTS
                var archetypeChunkChangesJobDependencies = CombineDependencies(srcChunksJob, dstChunksJob, clearMissingReferencesJob);
#else
                var archetypeChunkChangesJobDependencies = JobHandle.CombineDependencies(srcChunksJob, dstChunksJob, clearMissingReferencesJob);
#endif
                
                // Broad phased chunk comparison.
                using (var archetypeChunkChanges = GetArchetypeChunkChanges(
                    srcChunks: srcChunks,                                  
                    dstChunks: dstChunks, 
                    allocator: Allocator.TempJob, 
                    jobHandle: out var archetypeChunkChangesJob, 
                    dependsOn: archetypeChunkChangesJobDependencies))
                {
                    // Explicitly sync at this point to parallelize subsequent jobs by chunk.
                    archetypeChunkChangesJob.Complete();
                    
                    // Gather a sorted set of entities based on which chunks have changes.
                    using (var srcEntities = GetSortedEntitiesInChunk(
                        archetypeChunkChanges.CreatedSrcChunks, Allocator.TempJob, 
                        jobHandle: out var srcEntitiesJob))
                    using (var dstEntities = GetSortedEntitiesInChunk(
                        archetypeChunkChanges.DestroyedDstChunks, Allocator.TempJob, 
                        jobHandle: out var dstEntitiesJob))
                    using (var srcBlobAssets = GetReferencedBlobAssets(
                        srcChunks, Allocator.TempJob, 
                        jobHandle: out var srcBlobAssetsJob))
                    using (var dstBlobAssets = blobAssetCache.BlobAssetBatch->ToNativeList(Allocator.TempJob))
                    {
                        var duplicateEntityGuids = default(NativeList<DuplicateEntityGuid>);
                        var forwardEntityChanges = default(EntityInChunkChanges);
                        var reverseEntityChanges = default(EntityInChunkChanges);
                        var forwardComponentChanges = default(ComponentChanges);
                        var reverseComponentChanges = default(ComponentChanges);
                        var forwardBlobAssetChanges = default(BlobAssetChanges);
                        var reverseBlobAssetChanges = default(BlobAssetChanges);
                        
                        try
                        {
                            JobHandle getDuplicateEntityGuidsJob = default;
                            JobHandle forwardChangesJob = default;
                            JobHandle reverseChangesJob = default;

                            if (CheckOption(options, EntityManagerDifferOptions.ValidateUniqueEntityGuid))
                            {
                                // Guid validation will happen incrementally and only consider changed entities in the source world.
                                duplicateEntityGuids = GetDuplicateEntityGuids(
                                    srcEntities, Allocator.TempJob, 
                                    jobHandle: out getDuplicateEntityGuidsJob, 
                                    dependsOn: srcEntitiesJob);
                            }

                            if (CheckOption(options, EntityManagerDifferOptions.IncludeForwardChangeSet))
                            {
                                forwardEntityChanges = GetEntityInChunkChanges(
                                    srcEntityManager,
                                    dstEntityManager,
                                    srcEntities,
                                    dstEntities,
                                    Allocator.TempJob,
                                    jobHandle: out var forwardEntityChangesJob,
                                    dependsOn: JobHandle.CombineDependencies(srcEntitiesJob, dstEntitiesJob));
                                
                                forwardComponentChanges = GetComponentChanges(
                                    forwardEntityChanges, 
                                    default,
                                    blobAssetCache.BlobAssetRemap,
                                    Allocator.TempJob,
                                    jobHandle: out var forwardComponentChangesJob,
                                    dependsOn: forwardEntityChangesJob);
                                
                                forwardBlobAssetChanges = GetBlobAssetChanges(
                                    srcBlobAssets,
                                    dstBlobAssets,
                                    Allocator.TempJob, 
                                    jobHandle: out var forwardBlobAssetsChangesJob, 
                                    dependsOn: srcBlobAssetsJob);
                                
                                forwardChangesJob = JobHandle.CombineDependencies(forwardComponentChangesJob, forwardBlobAssetsChangesJob);
                            }
                            
                            if (CheckOption(options, EntityManagerDifferOptions.IncludeReverseChangeSet))
                            {
                                reverseEntityChanges = GetEntityInChunkChanges(
                                    dstEntityManager,
                                    srcEntityManager,
                                    dstEntities,
                                    srcEntities,
                                    Allocator.TempJob,
                                    jobHandle: out var reverseEntityChangesJob,
                                    dependsOn: JobHandle.CombineDependencies(srcEntitiesJob, dstEntitiesJob));
                                
                                reverseComponentChanges = GetComponentChanges(
                                    reverseEntityChanges, 
                                    blobAssetCache.BlobAssetRemap,
                                    default,
                                    Allocator.TempJob,
                                    jobHandle: out var reverseComponentChangesJob,
                                    dependsOn: reverseEntityChangesJob);
                                
                                reverseBlobAssetChanges = GetBlobAssetChanges(
                                    dstBlobAssets,
                                    srcBlobAssets,
                                    Allocator.TempJob, 
                                    jobHandle: out var reverseBlobAssetsChangesJob, 
                                    dependsOn: srcBlobAssetsJob);

                                reverseChangesJob = JobHandle.CombineDependencies(reverseComponentChangesJob, reverseBlobAssetsChangesJob);
                            }
                                
                            JobHandle jobHandle;
                            
                            using (var jobs = new NativeList<JobHandle>(5, Allocator.Temp))
                            {
                                jobs.Add(clearMissingReferencesJob);
                                jobs.Add(getDuplicateEntityGuidsJob);
                                
                                if (CheckOption(options, EntityManagerDifferOptions.IncludeForwardChangeSet) ||
                                    CheckOption(options, EntityManagerDifferOptions.IncludeReverseChangeSet))
                                {
                                    jobs.Add(forwardChangesJob);
                                    jobs.Add(reverseChangesJob);
                                }
                                else
                                {
                                    jobs.Add(srcEntitiesJob);
                                    jobs.Add(dstEntitiesJob);
                                    jobs.Add(srcBlobAssetsJob);
                                }
                                
                                jobHandle = JobHandle.CombineDependencies(jobs);
                            }
                        
                            jobHandle.Complete();

                            if (duplicateEntityGuids.IsCreated && duplicateEntityGuids.Length > 0)
                                throw new DuplicateEntityGuidException(duplicateEntityGuids.ToArray());

                            var forwardChangeSet = CreateEntityChangeSet(forwardEntityChanges, forwardComponentChanges, forwardBlobAssetChanges, allocator);
                            var reverseChangeSet = CreateEntityChangeSet(reverseEntityChanges, reverseComponentChanges, reverseBlobAssetChanges, allocator);
                            
                            changes = new EntityChanges(forwardChangeSet, reverseChangeSet);
                            
                            if (CheckOption(options, EntityManagerDifferOptions.FastForwardShadowWorld))
                            {
                                CopyAndReplaceChunks(srcEntityManager, dstEntityManager, dstEntityQuery, archetypeChunkChanges);

                                var batch = blobAssetCache.BlobAssetBatch;
                                var remap = blobAssetCache.BlobAssetRemap;
                                
                                using (var createdBlobAssets = new NativeList<BlobAssetPtr>(1, Allocator.TempJob))
                                using (var destroyedBlobAssets = new NativeList<BlobAssetPtr>(1, Allocator.TempJob))
                                {
                                    new GatherCreatedAndDestroyedBlobAssets
                                    {
                                        CreatedBlobAssets = createdBlobAssets,
                                        DestroyedBlobAssets = destroyedBlobAssets,
                                        AfterBlobAssets = srcBlobAssets,
                                        BeforeBlobAssets = dstBlobAssets
                                    }.Schedule().Complete();

                                    for (var i = 0; i < destroyedBlobAssets.Length; i++)
                                    {
                                        if (!batch->TryGetBlobAsset(destroyedBlobAssets[i].Hash, out _)) 
                                        {
                                            throw new Exception($"Failed to destroy a BlobAsset to the shadow world. A BlobAsset with the Hash=[{createdBlobAssets[i].Header->Hash}] does not exists.");
                                        }
                                        
                                        batch->ReleaseBlobAsset(destroyedBlobAssets[i].Hash);
                                        
                                        using (var keys = remap.GetKeyArray(Allocator.Temp))
                                        using (var values = remap.GetValueArray(Allocator.Temp))
                                        {
                                            for (var remapIndex = 0; remapIndex < values.Length; remapIndex++)
                                            {
                                                if (destroyedBlobAssets[i].Data != values[remapIndex].Data) 
                                                    continue;
                                                
                                                remap.Remove(keys[remapIndex]);
                                                break;
                                            }
                                        }
                                    }

                                    for (var i = 0; i < createdBlobAssets.Length; i++)
                                    {
                                        if (batch->TryGetBlobAsset(createdBlobAssets[i].Header->Hash, out _)) 
                                        {
                                            throw new Exception($"Failed to copy a BlobAsset to the shadow world. A BlobAsset with the Hash=[{createdBlobAssets[i].Header->Hash}] already exists.");
                                        }
                                        
                                        var blobAssetPtr = batch->AllocateBlobAsset(createdBlobAssets[i].Data, createdBlobAssets[i].Length, createdBlobAssets[i].Header->Hash);
                                        remap.TryAdd(new BlobAssetReferencePtr(createdBlobAssets[i].Data), blobAssetPtr);
                                    }

                                    if (destroyedBlobAssets.Length > 0 || createdBlobAssets.Length > 0)
                                    {
                                        batch->Sort();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            if (duplicateEntityGuids.IsCreated) duplicateEntityGuids.Dispose();
                            if (forwardEntityChanges.IsCreated) forwardEntityChanges.Dispose();
                            if (reverseEntityChanges.IsCreated) reverseEntityChanges.Dispose();
                            if (forwardComponentChanges.IsCreated) forwardComponentChanges.Dispose();
                            if (reverseComponentChanges.IsCreated) reverseComponentChanges.Dispose();
                            if (forwardBlobAssetChanges.IsCreated) forwardBlobAssetChanges.Dispose();
                            if (reverseBlobAssetChanges.IsCreated) reverseBlobAssetChanges.Dispose();
                        }
                    }
                }
            }
            
#if !NET_DOTS && (ENABLE_PROFILER || UNITY_EDITOR)
            s_GetChangesProfilerMarker.End();
#endif
            return changes;
        }
        
        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckEntityGuidComponent(EntityQueryDesc entityQueryDesc)
        {
            foreach (var type in entityQueryDesc.All)
            {
                if (type.GetManagedType() == typeof(EntityGuid))
                {
                    return;
                }
            }
            
            throw new ArgumentException($"{nameof(EntityDiffer)} custom query requires an {nameof(EntityGuid)} component in the All filter.");
        }

        /// <summary>
        /// @TODO NET_DOTS does not support JobHandle.CombineDependencies with 3 arguments.
        /// </summary>
        static JobHandle CombineDependencies(JobHandle job1, JobHandle job2, JobHandle job3)
        {
            var array = new NativeArray<JobHandle>(3, Allocator.Temp)
            {
                [0] = job1, 
                [1] = job2, 
                [2] = job2
            };

            var jobHandle = JobHandle.CombineDependencies(array);
            
            array.Dispose();
            
            return jobHandle;
        }

        static bool CheckOption(EntityManagerDifferOptions options, EntityManagerDifferOptions option)
            => (options & option) == option;
        
        /// <summary>
        /// @TODO NET_DOTS fixed byte Buffer[4] fails to compile when used as a ptr.
        /// </summary>
        static byte* GetChunkBuffer(Chunk* chunk)
        {
#if !NET_DOTS
            return chunk->Buffer;
#else
            return (byte*) chunk + Chunk.kBufferOffset;
#endif
        }
    }
}
