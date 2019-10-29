using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Entities
{
    unsafe struct BlobAssetCache : IDisposable
    {
        public NativeHashMap<BlobAssetReferencePtr, BlobAssetPtr> BlobAssetRemap;
        public DynamicBlobAssetBatch* BlobAssetBatch;

        public BlobAssetCache(Allocator allocator)
        {
            BlobAssetBatch = DynamicBlobAssetBatch.Allocate(allocator);
            BlobAssetRemap = new NativeHashMap<BlobAssetReferencePtr, BlobAssetPtr>(1, allocator);
        }
            
        public void Dispose()
        {
            DynamicBlobAssetBatch.Free(BlobAssetBatch);
            BlobAssetRemap.Dispose();
            BlobAssetBatch = null;
        }
    }
    
    readonly unsafe struct BlobAssetReferencePtr : IEquatable<BlobAssetReferencePtr>
    {
        public readonly void* Data;
        public BlobAssetReferencePtr(void* data) => Data = data;
        public bool Equals(BlobAssetReferencePtr other) => Data == other.Data;
        public override bool Equals(object obj) => obj is BlobAssetPtr other && Equals(other);
        public static bool operator ==(BlobAssetReferencePtr left, BlobAssetReferencePtr right) => left.Equals(right);
        public static bool operator !=(BlobAssetReferencePtr left, BlobAssetReferencePtr right) => !left.Equals(right);
        
        public override int GetHashCode()
        {
            var onStack = Data;
            return (int)math.hash(&onStack, sizeof(BlobAssetReferencePtr*));
        }
    }
    
    readonly unsafe struct BlobAssetPtr : IEquatable<BlobAssetPtr>, IComparable<BlobAssetPtr>
    {
        public readonly BlobAssetHeader* Header;
        public void* Data => Header + 1;
        public int Length => Header->Length;
        public ulong Hash => Header->Hash;

        public BlobAssetPtr(BlobAssetHeader* header) => Header = header;
        public bool Equals(BlobAssetPtr other) => Hash == other.Hash;
        public override bool Equals(object obj) => obj is BlobAssetPtr other && Equals(other);
        public static bool operator ==(BlobAssetPtr left, BlobAssetPtr right) => left.Equals(right);
        public static bool operator !=(BlobAssetPtr left, BlobAssetPtr right) => !left.Equals(right);
        public int CompareTo(BlobAssetPtr other) => Header->Hash.CompareTo(other.Header->Hash);
        
        public override int GetHashCode()
        {
            var onStack = Header;
            return (int)math.hash(&onStack, sizeof(BlobAssetHeader*));
        }
    }

    unsafe struct DynamicBlobAssetBatch
    {
        Allocator m_Allocator;
        UnsafeList* m_BlobAssets;
        
        public static DynamicBlobAssetBatch* Allocate(Allocator allocator)
        {
            var batch = (DynamicBlobAssetBatch*) UnsafeUtility.Malloc(sizeof(DynamicBlobAssetBatch), UnsafeUtility.AlignOf<DynamicBlobAssetBatch>(), allocator);
            batch->m_Allocator = allocator;
            batch->m_BlobAssets = UnsafeList.Create(sizeof(BlobAssetPtr), UnsafeUtility.AlignOf<BlobAssetPtr>(), 1, allocator);
            return batch;
        }

        public static void Free(DynamicBlobAssetBatch* batch)
        {
            var blobAssets = (BlobAssetPtr*) batch->m_BlobAssets->Ptr;
            
            for (var i = 0; i < batch->m_BlobAssets->Length; i++)
                UnsafeUtility.Free(blobAssets[i].Header, batch->m_Allocator);

            UnsafeList.Destroy(batch->m_BlobAssets);
            UnsafeUtility.Free(batch, batch->m_Allocator);
        }

        public NativeList<BlobAssetPtr> ToNativeList(Allocator allocator)
        {
            var list = new NativeList<BlobAssetPtr>(m_BlobAssets->Length, allocator);
            list.ResizeUninitialized(m_BlobAssets->Length);
            UnsafeUtility.MemCpy(list.GetUnsafePtr(), m_BlobAssets->Ptr, sizeof(BlobAssetPtr) * m_BlobAssets->Length);
            return list;
        }

        public BlobAssetPtr AllocateBlobAsset(void* data, int length, ulong hash)
        {
            var blobAssetHeader = (BlobAssetHeader*) UnsafeUtility.Malloc(length + sizeof(BlobAssetHeader), 16, m_Allocator);
            
            blobAssetHeader->Length = length;
            blobAssetHeader->ValidationPtr = blobAssetHeader + 1;
            blobAssetHeader->Allocator = Allocator.None;
            blobAssetHeader->Hash = hash;
            
            UnsafeUtility.MemCpy(blobAssetHeader + 1, data, length);
            
            m_BlobAssets->Add(new BlobAssetPtr(blobAssetHeader));

            return new BlobAssetPtr(blobAssetHeader);
        }

        public void Sort() => NativeSortExtension.Sort((BlobAssetPtr*) m_BlobAssets->Ptr, m_BlobAssets->Length);

        public bool TryGetBlobAsset(ulong hash, out BlobAssetPtr blobAssetPtr)
        {
            var blobAssets = (BlobAssetPtr*) m_BlobAssets->Ptr;
            
            for (var i = 0; i < m_BlobAssets->Length; i++)
            {
                if (blobAssets[i].Header->Hash != hash) 
                    continue;
                
                blobAssetPtr = new BlobAssetPtr(blobAssets[i].Header);
                return true;
            }

            blobAssetPtr = default;
            return false;
        }

        public void ReleaseBlobAsset(ulong hash)
        {
            var blobAssets = (BlobAssetPtr*) m_BlobAssets->Ptr;

            for (var i = 0; i < m_BlobAssets->Length; i++)
            {
                if (blobAssets[i].Hash != hash) 
                    continue;
                
                UnsafeUtility.Free(blobAssets[i].Header, m_Allocator);
                m_BlobAssets->RemoveAtSwapBack<BlobAssetPtr>(i);
                return;
            }
        }

        public void RemoveUnusedBlobAssets(NativeHashMap<ulong, int> usedBlobAssets)
        {
            var blobAssets = (BlobAssetPtr*) m_BlobAssets->Ptr;
            
            for (var i = 0; i < m_BlobAssets->Length; i++)
            {
                if (!usedBlobAssets.ContainsKey(blobAssets[i].Hash))
                {
                    UnsafeUtility.Free(blobAssets[i].Header, m_Allocator);
                    m_BlobAssets->RemoveAtSwapBack<BlobAssetPtr>(i--);
                }
            }
        }
    }
}