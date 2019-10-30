using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    static unsafe partial class EntityDiffer
    {
        readonly struct BlobAssetChanges : IDisposable
        {
            public readonly NativeList<BlobAssetChange> CreatedBlobAssets;
            public readonly NativeList<ulong> DestroyedBlobAssets;
            public readonly NativeList<byte> BlobAssetData;
            public readonly bool IsCreated;

            public BlobAssetChanges(Allocator allocator)
            {
                CreatedBlobAssets = new NativeList<BlobAssetChange>(1, allocator);
                DestroyedBlobAssets = new NativeList<ulong>(1, allocator);
                BlobAssetData = new NativeList<byte>(1, allocator);
                IsCreated = true;
            }

            public void Dispose()
            {
                CreatedBlobAssets.Dispose();
                DestroyedBlobAssets.Dispose();
                BlobAssetData.Dispose();
            }
        }

        // @TODO: Re-enable burst on this... in some editor cases the memory can get unmapped and throw.
        // [BurstCompile]
        internal struct GatherUniqueBlobAssetReferences : IJob
        {
            [NativeDisableUnsafePtrRestriction] public TypeManager.TypeInfo* TypeInfo;
            [NativeDisableUnsafePtrRestriction] public TypeManager.EntityOffsetInfo* BlobAssetRefOffsets;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            public NativeList<BlobAssetPtr> BlobAssets;
            public NativeHashMap<ulong, int> BlobAssetsMap;

            public void Execute()
            {
                for (var chunkIndex = 0; chunkIndex < Chunks.Length; chunkIndex++)
                {
                    var chunk = Chunks[chunkIndex].m_Chunk;
                    var archetype = chunk->Archetype;

                    if (!archetype->ContainsBlobAssetRefs)
                        continue;

                    var typesCount = archetype->TypesCount;
                    var entityCount = Chunks[chunkIndex].Count;

                    for (var unorderedTypeIndexInArchetype = 0; unorderedTypeIndexInArchetype < typesCount; unorderedTypeIndexInArchetype++)
                    {
                        var typeIndexInArchetype = archetype->TypeMemoryOrder[unorderedTypeIndexInArchetype];
                        var componentTypeInArchetype = archetype->Types[typeIndexInArchetype];

                        if (componentTypeInArchetype.IsZeroSized || componentTypeInArchetype.IsSharedComponent)
                            continue;

                        var typeInfo = TypeInfo[componentTypeInArchetype.TypeIndex & TypeManager.ClearFlagsMask];
                        var blobAssetRefCount = typeInfo.BlobAssetRefOffsetCount;

                        if (blobAssetRefCount == 0)
                            continue;

                        var blobAssetRefOffsets = BlobAssetRefOffsets + typeInfo.BlobAssetRefOffsetStartIndex;
                        var chunkBuffer = chunk->Buffer;
                        var subArrayOffset = archetype->Offsets[typeIndexInArchetype];
                        var componentArrayStart = chunkBuffer + subArrayOffset;

                        if (componentTypeInArchetype.IsBuffer)
                        {
                            var header = (BufferHeader*) componentArrayStart;
                            var strideSize = archetype->SizeOfs[typeIndexInArchetype];
                            var elementSize = typeInfo.ElementSize;

                            for (var entityIndex = 0; entityIndex < entityCount; entityIndex++)
                            {
                                var bufferStart = BufferHeader.GetElementPointer(header);
                                var bufferEnd = bufferStart + header->Length * elementSize;

                                for (var componentData = bufferStart; componentData < bufferEnd; componentData += elementSize)
                                {
                                    AddBlobAssets(componentData, blobAssetRefOffsets, blobAssetRefCount);
                                }

                                header += strideSize;
                            }
                        }
                        else
                        {
                            var componentSize = archetype->SizeOfs[unorderedTypeIndexInArchetype];
                            var end = componentArrayStart + componentSize * entityCount;

                            for (var componentData = componentArrayStart; componentData < end; componentData += componentSize)
                            {
                                AddBlobAssets(componentData, blobAssetRefOffsets, blobAssetRefCount);
                            }
                        }
                    }
                }
            }

            void AddBlobAssets(
                byte* componentData,
                TypeManager.EntityOffsetInfo* blobAssetRefOffsets,
                int blobAssetRefCount)
            {
                for (var i = 0; i < blobAssetRefCount; ++i)
                {
                    var blobAssetRefOffset = blobAssetRefOffsets[i].Offset;
                    var blobAssetRefPtr = (BlobAssetReferenceData*) (componentData + blobAssetRefOffset);

                    if (blobAssetRefPtr->m_Ptr == null)
                        continue;

                    // @TODO: remove validation checks to re-enable burst
                    void* validationPtr = null;
                    try
                    {
                        // Try to read ValidationPtr, this might throw if the memory has been unmapped
                        validationPtr = blobAssetRefPtr->Header->ValidationPtr;
                    }
                    catch(Exception)
                    {
                    }
                    
                    if (validationPtr != blobAssetRefPtr->m_Ptr) 
                        continue;

                    if (BlobAssetsMap.TryGetValue(blobAssetRefPtr->Header->Hash, out _))
                        continue;

                    BlobAssetsMap.TryAdd(blobAssetRefPtr->Header->Hash, BlobAssets.Length);
                    BlobAssets.Add(new BlobAssetPtr(blobAssetRefPtr->Header));
                }
            }
        }

        struct BlobAssetPtrComparer : IComparer<BlobAssetPtr>
        {
            public int Compare(BlobAssetPtr x, BlobAssetPtr y) => x.CompareTo(y);
        }

        [BurstCompile]
        struct GatherCreatedAndDestroyedBlobAssets : IJob
        {
            [ReadOnly] public NativeList<BlobAssetPtr> AfterBlobAssets;
            [ReadOnly] public NativeList<BlobAssetPtr> BeforeBlobAssets;
            [WriteOnly] public NativeList<BlobAssetPtr> CreatedBlobAssets;
            [WriteOnly] public NativeList<BlobAssetPtr> DestroyedBlobAssets;

            public void Execute()
            {
                var afterIndex = 0;
                var beforeIndex = 0;

                while (afterIndex < AfterBlobAssets.Length && beforeIndex < BeforeBlobAssets.Length)
                {
                    var afterBlobAsset = AfterBlobAssets[afterIndex];
                    var beforeBlobAsset = BeforeBlobAssets[beforeIndex];

                    var compare = afterBlobAsset.CompareTo(beforeBlobAsset);

                    if (compare < 0)
                    {
                        CreatedBlobAssets.Add(afterBlobAsset);
                        afterIndex++;
                    }
                    else if (compare == 0)
                    {
                        afterIndex++;
                        beforeIndex++;
                    }
                    else
                    {
                        beforeIndex++;
                        DestroyedBlobAssets.Add(beforeBlobAsset);
                    }
                }

                while (afterIndex < AfterBlobAssets.Length)
                {
                    CreatedBlobAssets.Add(AfterBlobAssets[afterIndex++]);
                }

                while (beforeIndex < BeforeBlobAssets.Length)
                {
                    DestroyedBlobAssets.Add(BeforeBlobAssets[beforeIndex++]);
                }
            }
        }

        [BurstCompile]
        struct GatherBlobAssetChanges : IJob
        {
            [ReadOnly] public NativeList<BlobAssetPtr> CreatedBlobAssets;
            [ReadOnly] public NativeList<BlobAssetPtr> DestroyedBlobAssets;
            [WriteOnly] public NativeList<BlobAssetChange> CreatedBlobAssetChanges;
            [WriteOnly] public NativeList<ulong> DestroyedBlobAssetChanges;
            public NativeList<byte> BlobAssetData;

            public void Execute()
            {
                var totalBlobAssetLength = 0;

                for (var i = 0; i < CreatedBlobAssets.Length; i++)
                {
                    var length = CreatedBlobAssets[i].Header->Length;
                    CreatedBlobAssetChanges.Add(new BlobAssetChange {Length = length, Hash = CreatedBlobAssets[i].Header->Hash});
                    totalBlobAssetLength += length;
                }

                for (var i = 0; i < DestroyedBlobAssets.Length; i++)
                {
                    DestroyedBlobAssetChanges.Add(DestroyedBlobAssets[i].Header->Hash);
                }

                BlobAssetData.Capacity = totalBlobAssetLength;

                for (var i = 0; i < CreatedBlobAssets.Length; i++)
                {
                    BlobAssetData.AddRange(CreatedBlobAssets[i].Data, CreatedBlobAssets[i].Length);
                }
            }
        }

        static NativeList<BlobAssetPtr> GetReferencedBlobAssets(
            NativeArray<ArchetypeChunk> chunks,
            Allocator allocator,
            out JobHandle jobHandle,
            JobHandle dependsOn = default)
        {
            var blobAssets = new NativeList<BlobAssetPtr>(1, allocator);
            var blobAssetsMap = new NativeHashMap<ulong, int>(1, allocator);

            var gatherUniqueBlobAssets = new GatherUniqueBlobAssetReferences
            {
                TypeInfo = TypeManager.GetTypeInfoPointer(),
                BlobAssetRefOffsets = TypeManager.GetBlobAssetRefOffsetsPointer(),
                Chunks = chunks,
                BlobAssets = blobAssets,
                BlobAssetsMap = blobAssetsMap
            }.Schedule(dependsOn);

            var sortBlobAssets = new SortNativeArrayWithComparer<BlobAssetPtr, BlobAssetPtrComparer>
            {
                Array = blobAssets.AsDeferredJobArray()
            }.Schedule(gatherUniqueBlobAssets);

            jobHandle = blobAssetsMap.Dispose(sortBlobAssets);

            return blobAssets;
        }

        static BlobAssetChanges GetBlobAssetChanges(
            NativeList<BlobAssetPtr> afterBlobAssets,
            NativeList<BlobAssetPtr> beforeBlobAssets,
            Allocator allocator,
            out JobHandle jobHandle,
            JobHandle dependsOn = default)
        {
            var changes = new BlobAssetChanges(allocator);

            var createdBlobAssets = new NativeList<BlobAssetPtr>(1, Allocator.TempJob);
            var destroyedBlobAssets = new NativeList<BlobAssetPtr>(1, Allocator.TempJob);

            jobHandle = new GatherCreatedAndDestroyedBlobAssets
            {
                AfterBlobAssets = afterBlobAssets,
                BeforeBlobAssets = beforeBlobAssets,
                CreatedBlobAssets = createdBlobAssets,
                DestroyedBlobAssets = destroyedBlobAssets
            }.Schedule(dependsOn);

            jobHandle = new GatherBlobAssetChanges
            {
                CreatedBlobAssets = createdBlobAssets,
                DestroyedBlobAssets = destroyedBlobAssets,
                CreatedBlobAssetChanges = changes.CreatedBlobAssets,
                DestroyedBlobAssetChanges = changes.DestroyedBlobAssets,
                BlobAssetData = changes.BlobAssetData
            }.Schedule(jobHandle);
            
            jobHandle = JobHandle.CombineDependencies
            (
                createdBlobAssets.Dispose(jobHandle),
                destroyedBlobAssets.Dispose(jobHandle)
            );

            return changes;
        }
    }
}