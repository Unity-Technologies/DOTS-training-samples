using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    static unsafe partial class EntityDiffer
    {
        [BurstCompile]
        struct PatchAndAddClonedChunks : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> SrcChunks;
            [ReadOnly] public NativeArray<ArchetypeChunk> DstChunks;

            [NativeDisableUnsafePtrRestriction] public EntityComponentStore* DstEntityComponentStore;

            public void Execute(int index)
            {
                var srcChunk = SrcChunks[index].m_Chunk;
                var dstChunk = DstChunks[index].m_Chunk;

                var archetype = srcChunk->Archetype;
                var typeCount = archetype->TypesCount;

                for (var typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    //@TODO: Overridable dst system version here?
                    dstChunk->SetChangeVersion(typeIndex, srcChunk->GetChangeVersion(typeIndex));
                }

                DstEntityComponentStore->AddExistingEntitiesInChunk(dstChunk);
            }
        }
        
        internal static void CopyAndReplaceChunks(
            EntityManager srcEntityManager, 
            EntityManager dstEntityManager, 
            EntityQuery dstEntityQuery, 
            ArchetypeChunkChanges archetypeChunkChanges)
        {
            var archetypeChanges = dstEntityManager.EntityComponentStore->BeginArchetypeChangeTracking();

            DestroyChunks(dstEntityManager, archetypeChunkChanges.DestroyedDstChunks.Chunks);
            CloneAndAddChunks(srcEntityManager, dstEntityManager, archetypeChunkChanges.CreatedSrcChunks.Chunks);
            
            var changedArchetypes = dstEntityManager.EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            dstEntityManager.EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);

            //@TODO-opt: use a query that searches for all chunks that have chunk components on it
            //@TODO-opt: Move this into a job
            // Any chunk might have been recreated, so the ChunkHeader might be invalid 
            using (var allDstChunks = dstEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                foreach (var chunk in allDstChunks)
                {
                    var metaEntity = chunk.m_Chunk->metaChunkEntity;
                    if (metaEntity != Entity.Null)
                    {
                        if (dstEntityManager.Exists(metaEntity))
                            dstEntityManager.SetComponentData(metaEntity, new ChunkHeader {ArchetypeChunk = chunk});
                    }
                }
            }
            
            srcEntityManager.EntityComponentStore->IncrementGlobalSystemVersion();
            dstEntityManager.EntityComponentStore->IncrementGlobalSystemVersion();
        }

        static void DestroyChunks(EntityManager entityManager, NativeList<ArchetypeChunk> chunks)
        {
            for (var i = 0; i < chunks.Length; i++)
            {
                Assert.IsTrue(chunks[i].entityComponentStore == entityManager.EntityComponentStore);
                entityManager.DestroyChunkForDiffing(chunks[i].m_Chunk);
            }
        }
        
        static void CloneAndAddChunks(EntityManager srcEntityManager, EntityManager dstEntityManager, NativeList<ArchetypeChunk> chunks)
        {
            var cloned = new NativeArray<ArchetypeChunk>(chunks.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            
            for (var i = 0; i < chunks.Length; i++)
            {
                var srcChunk = chunks[i].m_Chunk;
                
                var dstChunk = CloneChunkWithoutAllocatingEntities(
                    dstEntityManager,
                    srcChunk,
                    srcEntityManager.ManagedComponentStore);

                cloned[i] = new ArchetypeChunk {m_Chunk = dstChunk};
            }
            
            // Ensure capacity in the dst world before we start linking entities.
            dstEntityManager.EntityComponentStore->EnsureCapacity(srcEntityManager.EntityCapacity);
            dstEntityManager.EntityComponentStore->CopyNextFreeEntityIndex(srcEntityManager.EntityComponentStore);
                
            new PatchAndAddClonedChunks
            {
                SrcChunks = chunks,
                DstChunks = cloned,
                DstEntityComponentStore = dstEntityManager.EntityComponentStore
            }.Schedule(chunks.Length, 64).Complete();
            
            cloned.Dispose();
        }
        
        static Chunk* CloneChunkWithoutAllocatingEntities(EntityManager dstEntityManager, Chunk* srcChunk, ManagedComponentStore srcManagedComponentStore)
        {
            var dstEntityComponentStore = dstEntityManager.EntityComponentStore;
            var dstManagedComponentStore = dstEntityManager.ManagedComponentStore;

            // Copy shared component data
            var dstSharedIndices = stackalloc int[srcChunk->Archetype->NumSharedComponents];
            srcChunk->SharedComponentValues.CopyTo(dstSharedIndices, 0, srcChunk->Archetype->NumSharedComponents);
            dstManagedComponentStore.CopySharedComponents(srcManagedComponentStore, dstSharedIndices, srcChunk->Archetype->NumSharedComponents);

            // @TODO: Why don't we memcpy the whole chunk. So we include all extra fields???
            
            // Allocate a new chunk
            var srcArchetype = srcChunk->Archetype;
            var dstArchetype = dstEntityComponentStore->GetOrCreateArchetype(srcArchetype->Types, srcArchetype->TypesCount);

            var dstChunk = dstEntityComponentStore->GetCleanChunkNoMetaChunk(dstArchetype, dstSharedIndices);
            dstManagedComponentStore.Playback(ref dstEntityComponentStore->ManagedChangesTracker);

            dstChunk->metaChunkEntity = srcChunk->metaChunkEntity;
            
            // Release any references obtained by GetCleanChunk & CopySharedComponents
            for (var i = 0; i < srcChunk->Archetype->NumSharedComponents; i++)
                dstManagedComponentStore.RemoveReference(dstSharedIndices[i]);

            dstEntityComponentStore->SetChunkCountKeepMetaChunk(dstChunk, srcChunk->Count);
            dstManagedComponentStore.Playback(ref dstEntityComponentStore->ManagedChangesTracker);

            dstChunk->Archetype->EntityCount += srcChunk->Count;

            var copySize = Chunk.GetChunkBufferSize();
            UnsafeUtility.MemCpy((byte*) dstChunk + Chunk.kBufferOffset, (byte*) srcChunk + Chunk.kBufferOffset, copySize);

            // @TODO: Class components should be duplicated instead of copied by ref?
            if (dstChunk->ManagedArrayIndex != -1)
                ManagedComponentStore.CopyManagedObjects(srcManagedComponentStore, srcChunk->Archetype, srcChunk->ManagedArrayIndex, srcChunk->Capacity, 0, dstManagedComponentStore, dstChunk->Archetype, dstChunk->ManagedArrayIndex, dstChunk->Capacity, 0, srcChunk->Count);
            
            BufferHeader.PatchAfterCloningChunk(dstChunk);
            dstChunk->SequenceNumber = srcChunk->SequenceNumber;

            return dstChunk;
        }
    }
}