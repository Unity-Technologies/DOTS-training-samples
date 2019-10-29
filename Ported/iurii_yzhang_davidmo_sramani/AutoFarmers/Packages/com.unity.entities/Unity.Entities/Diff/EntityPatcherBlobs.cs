using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    [DisableAutoCreation]
    unsafe class EntityPatcherBlobAssetSystem : ComponentSystem
    {
        static EntityQueryDesc s_EntityGuidQueryDesc;

        static EntityQueryDesc EntityGuidQueryDesc
        {
            get
            {
                return s_EntityGuidQueryDesc ?? (s_EntityGuidQueryDesc = new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        typeof(EntityGuid)
                    },
                    Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
                });
            }
        }

        DynamicBlobAssetBatch* m_BlobAssetBatchPtr;

        protected override void OnCreate()
        {
            m_BlobAssetBatchPtr = DynamicBlobAssetBatch.Allocate(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            DynamicBlobAssetBatch.Free(m_BlobAssetBatchPtr);
            m_BlobAssetBatchPtr = null;
        }

        public void AllocateBlobAsset(void* ptr, int len, ulong hash)
        {
            m_BlobAssetBatchPtr->AllocateBlobAsset(ptr, len, hash);
        }

        public void ReleaseBlobAsset(ulong hash)
        {
            m_BlobAssetBatchPtr->ReleaseBlobAsset(hash);
        }

        public bool TryGetBlobAsset(ulong hash, out BlobAssetPtr ptr)
        {
            return m_BlobAssetBatchPtr->TryGetBlobAsset(hash, out ptr);
        }

        public void ReleaseUnusedBlobAssets()
        {
            using (var chunks = EntityManager.CreateEntityQuery(EntityGuidQueryDesc).CreateArchetypeChunkArray(Allocator.TempJob))
            using (var blobAssets = new NativeList<BlobAssetPtr>(1, Allocator.TempJob))
            using (var blobAssetsMap = new NativeHashMap<ulong, int>(1, Allocator.TempJob))
            {
                new EntityDiffer.GatherUniqueBlobAssetReferences
                {
                    TypeInfo = TypeManager.GetTypeInfoPointer(),
                    BlobAssetRefOffsets = TypeManager.GetBlobAssetRefOffsetsPointer(),
                    Chunks = chunks,
                    BlobAssets = blobAssets,
                    BlobAssetsMap = blobAssetsMap
                }.Schedule().Complete();

                m_BlobAssetBatchPtr->RemoveUnusedBlobAssets(blobAssetsMap);
            }
        }

        protected override void OnUpdate() { }
    }
    
    public static unsafe partial class EntityPatcher
    {
        static void ApplyBlobAssetChanges(
            EntityManager entityManager,
            NativeArray<EntityGuid> packedEntityGuids,
            NativeMultiHashMap<int, Entity> packedEntities,
            NativeArray<ComponentType> packedTypes,
            NativeArray<BlobAssetChange> createdBlobAssets,
            NativeArray<byte> createdBlobAssetData,
            NativeArray<ulong> destroyedBlobAssets,
            NativeArray<BlobAssetReferenceChange> blobAssetReferenceChanges)
        {
            if (createdBlobAssets.Length == 0 && blobAssetReferenceChanges.Length == 0)
                return;
            
            var patcherBlobAssetSystem = entityManager.World.GetOrCreateSystem<EntityPatcherBlobAssetSystem>();

            var blobAssetDataPtr = (byte*) createdBlobAssetData.GetUnsafePtr();
            
            for (var i = 0; i < createdBlobAssets.Length; i++)
            {
                if (!patcherBlobAssetSystem.TryGetBlobAsset(createdBlobAssets[i].Hash, out _)) 
                {
                    patcherBlobAssetSystem.AllocateBlobAsset(blobAssetDataPtr, createdBlobAssets[i].Length, createdBlobAssets[i].Hash);
                }
                
                blobAssetDataPtr += createdBlobAssets[i].Length;
            }

            for (var i = 0; i < destroyedBlobAssets.Length; i++)
            {
                patcherBlobAssetSystem.ReleaseBlobAsset(destroyedBlobAssets[i]);
            }
            
            for (var i = 0; i < blobAssetReferenceChanges.Length; i++)
            {
                var packedComponent = blobAssetReferenceChanges[i].Component;
                var component = packedTypes[packedComponent.PackedTypeIndex];
                var targetOffset = blobAssetReferenceChanges[i].Offset;

                BlobAssetReferenceData targetBlobAssetReferenceData;
                if (patcherBlobAssetSystem.TryGetBlobAsset(blobAssetReferenceChanges[i].Value, out var blobAssetPtr))
                {
                    targetBlobAssetReferenceData = new BlobAssetReferenceData {m_Ptr = (byte*) blobAssetPtr.Data};
                }

                if (packedEntities.TryGetFirstValue(packedComponent.PackedEntityIndex, out var entity, out var iterator))
                {
                    do
                    {
                        if (!entityManager.Exists(entity))
                        {
                            Debug.LogWarning($"ApplyBlobAssetReferencePatches<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but entity to patch does not exist.");
                        }
                        else if (!entityManager.HasComponent(entity, component))
                        {
                            Debug.LogWarning($"ApplyBlobAssetReferencePatches<{component}>({packedEntityGuids[packedComponent.PackedEntityIndex]}) but component in entity to patch does not exist.");
                        }
                        else
                        {
                            if (component.IsBuffer)
                            {
                                var pointer = (byte*) entityManager.GetBufferRawRW(entity, component.TypeIndex);
                                UnsafeUtility.MemCpy(pointer + targetOffset, &targetBlobAssetReferenceData, sizeof(BlobAssetReferenceData));
                            }
#if !NET_DOTS
                            else if (component.IsManagedComponent)
                            {
                                var obj = entityManager.GetManagedComponentDataAsObject(entity, component);
                                var pointer = (byte*) UnsafeUtility.PinGCObjectAndGetAddress(obj, out ulong handle);
                                pointer += TypeManager.ObjectOffset;
                                UnsafeUtility.MemCpy(pointer + targetOffset, &targetBlobAssetReferenceData, sizeof(BlobAssetReferenceData));
                                UnsafeUtility.ReleaseGCObject(handle);
                            }
#endif
                            else
                            {
                                var pointer = (byte*) entityManager.GetComponentDataRawRW(entity, component.TypeIndex);
                                UnsafeUtility.MemCpy(pointer + targetOffset, &targetBlobAssetReferenceData, sizeof(BlobAssetReferenceData));
                            }
                        }
                    } 
                    while (packedEntities.TryGetNextValue(out entity, ref iterator));
                }
            }
            
            // Workaround to catch some special cases where the memory is never released. (e.g. reloading a scene, toggling live-link on/off).
            patcherBlobAssetSystem.ReleaseUnusedBlobAssets();
        }
    }
}
