using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Entities
{
    /// <summary>
    /// Purpose of this class is to provide a consistent cache of BlobAsset object in order to avoid rebuilding them when it is not necessary
    /// </summary>
    /// <remarks>
    /// Right now the lifetime scope of this cache is bound to the LiveLinkDiffGenerator's one and it is scoped by SubScene.
    /// In other words the cache is created when we enter edit mode for a given SubScene and it is released when we close edit mode.
    /// And instance of this cache is exposed in <see cref="Unity.Entities.GameObjectConversionSettings"/> to allow users to query and avoid rebuilding assets.
    /// During conversion process the user must rely on the <see cref="BlobAssetComputationContext{TS,TB}"/> to associate the BlobAsset with their corresponding Authoring GameObject and to determine which ones are to compute.
    /// Thread-safety: nothing is thread-safe, we assume this class is consumed through the main-thread only.
    /// Calling Dispose on an instance will reset the content and dispose all BlobAssetReference object stored.
    /// </remarks>
    public class BlobAssetStore : IDisposable
    {
        public BlobAssetStore()
        {
            m_BlobAssets = new NativeHashMap<Hash128, BlobAssetReferenceData>(128, Allocator.Persistent);
            m_HashByOwner = new NativeMultiHashMap<int, Hash128>(128, Allocator.Persistent);
            m_RefCounterPerBlobHash = new NativeHashMap<Hash128, int>(128, Allocator.Persistent);
        }

        /// <summary>
        /// Call this method to clear the whole content of the Cache
        /// </summary>
        /// <param name="disposeAllBlobAssetReference">If true all BlobAssetReference present in the cache will be dispose. If false they will remain present in memory</param>
        public void ResetCache(bool disposeAllBlobAssetReference)
        {
            if (disposeAllBlobAssetReference)
            {
                using (var blobDataArray = m_BlobAssets.GetValueArray(Allocator.Temp))
                {
                    for (int i = 0; i < blobDataArray.Length; i++)
                    {
                        blobDataArray[i].Dispose();
                    }
                }
            }

            m_BlobAssets.Clear();
            m_HashByOwner.Clear();
            m_RefCounterPerBlobHash.Clear();
        }

        /// <summary>
        /// Try to access to a BlobAssetReference from its key
        /// </summary>
        /// <param name="key">The key associated with the BlobAssetReference when it was added to the cache</param>
        /// <param name="blobAssetReference">The corresponding BlobAssetReference or default if none was found</param>
        /// <typeparam name="T">The type of BlobAsset</typeparam>
        /// <returns></returns>
        public bool TryGet<T>(Hash128 key, out BlobAssetReference<T> blobAssetReference) where T : struct
        {
            var typedHash = ComputeTypedHash(key, typeof(T));
            
            var res = m_BlobAssets.TryGetValue(typedHash, out var blobData);
            if (res)
            {
                m_CacheHit++;
                
                blobAssetReference = BlobAssetReference<T>.Create(blobData);
                return true;
            }

            m_CacheMiss++;
            blobAssetReference = default;
            return false;
        }

        /// <summary>
        /// Check if the Store contains a BlobAsset of a given type and hash
        /// </summary>
        /// <param name="key">The hash associated with the BlobAsset</param>
        /// <typeparam name="T">The type of the BlobAsset</typeparam>
        /// <returns>true if the Store contains the BlobAsset or false if it doesn't</returns>
        public bool Contains<T>(Hash128 key)
        {
            var typedHash = ComputeTypedHash(key, typeof(T));
            return m_BlobAssets.ContainsKey(typedHash);
        }

        /// <summary>
        /// Get a BlobAssetReference from its key
        /// </summary>
        /// <param name="key">The key associated with the BlobAssetReference</param>
        /// <param name="result">The BlobAssetReference if found or default</param>
        /// <typeparam name="T">The type of BlobAsset</typeparam>
        /// <returns>true if the BlobAssetReference was found, false if not found</returns>
        public bool TryAdd<T>(Hash128 key, BlobAssetReference<T> result) where T : struct
        {
            var typedHash = ComputeTypedHash(key, typeof(T));
            if (m_BlobAssets.ContainsKey(typedHash))
            {
                return false;
            }

            m_BlobAssets.Add(typedHash, result.m_data);
            return true;
        }

        /// <summary>
        /// Remove a BlobAssetReference from the store
        /// </summary>
        /// <param name="key">The key associated with the BlobAssetReference</param>
        /// <param name="releaseBlobAsset">If true the BlobAsset data will be released</param>
        /// <typeparam name="T">The type of the BlobAsset</typeparam>
        /// <returns>True if the BLobAsset was removed from the store, false if it wasn't found</returns>
        public bool Remove<T>(Hash128 key, bool releaseBlobAsset)
        {
            var typedHash = ComputeTypedHash(key, typeof(T));
            
            if (releaseBlobAsset && m_BlobAssets.TryGetValue(typedHash, out var blobData))
            {
                blobData.Dispose();
            }
            var res = m_BlobAssets.Remove(typedHash);
            return res;
        }

        /// <summary>
        /// Get the Reference Counter value of a given BlogAsset
        /// </summary>
        /// <param name="hash">The hash associated with the BLobAsset</param>
        /// <returns>The value of the reference counter, 0 if there is no BlobAsset for the given hash</returns>
        internal int GetBlobAssetRefCounter(Hash128 hash)
        {
            return m_RefCounterPerBlobHash.TryGetValue(hash, out var counter) ? counter : 0;
        }
        
        /// <summary>
        /// Calling dispose will reset the cache content and release all the BlobAssetReference that were stored
        /// </summary>
        public void Dispose()
        {
            ResetCache(true);
            m_BlobAssets.Dispose();
            m_HashByOwner.Dispose();
            m_RefCounterPerBlobHash.Dispose();
        }

        static Hash128 ComputeTypedHash(Hash128 key, Type type)
        {
            return new Hash128(math.hash(new uint4x2 { c0 = key.Value, c1 = new uint4((uint)type.GetHashCode())}));
        }

        /// <summary>
        /// Number of times the cache was successfully accessed
        /// </summary>
        /// <remarks>
        /// Each TryGet returning a valid content will increment this counter
        /// </remarks>
        internal int CacheHit => m_CacheHit;

        /// <summary>
        /// Number of times the cache failed to return a BlobAssetReference for the given key
        /// </summary>
        /// <remarks>
        /// Each TryGet returning false will increment this counter
        /// </remarks>
        internal int CacheMiss => m_CacheMiss;

        NativeHashMap<Hash128, BlobAssetReferenceData> m_BlobAssets;
        NativeHashMap<Hash128, int> m_RefCounterPerBlobHash;
        NativeMultiHashMap<int, Hash128> m_HashByOwner;
        
        int m_CacheHit;
        int m_CacheMiss;

        internal void UpdateBlobAssetForGameObject<TB>(int ownerId, NativeArray<Hash128> newBlobHashes) where TB : struct
        {
            var leftLength = newBlobHashes.Length;
            var toInc = new NativeArray<Hash128>(leftLength, Allocator.Temp);
            var toDec = new NativeArray<Hash128>(m_HashByOwner.CountValuesForKey(ownerId), Allocator.Temp);
            
            var curLeftIndex = 0;
            var curIncIndex = 0;
            var curDecIndex = 0;
            
            var leftRes = curLeftIndex < leftLength;
            var rightRes = m_HashByOwner.TryGetFirstValue(ownerId, out var rightHash, out var it);

            var maxHash = new Hash128(UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue, UInt32.MaxValue);
            
            // We will parse newBlobHashes, considered the left part and the store hashes for this ownerId, considered the right part
            //  in order to build a list of BlobAssets to increment (the ones only present in left part) and the ones to decrement
            //  (only present in the right part). If a hash is present on both side, we do not change its RefCounter
            do 
            {
                var leftHash = leftRes ? newBlobHashes[curLeftIndex] : maxHash;
                rightHash = rightRes ? rightHash : maxHash;

                // Both side equal? We are synchronized, step next for both sides
                if (rightHash == leftHash)
                {
                    leftRes = ++curLeftIndex < leftLength;
                    rightRes = m_HashByOwner.TryGetNextValue(out rightHash, ref it);
                    continue;
                }

                // More items on the left, add them to the "toAdd" list
                if (leftHash < rightHash)
                {
                    do
                    {
                        // Get left hash
                        leftHash = newBlobHashes[curLeftIndex++];

                        // Add to "toInc"
                        toInc[curIncIndex++] = leftHash;

                        // Check if there's more left item
                        leftRes = curLeftIndex < leftLength;
                    } while (leftRes && (leftHash < rightHash));
                }

                // More items on the right, add them to the "toRemove" list
                else
                {
                    do
                    {
                        // Add to "toDec"
                        toDec[curDecIndex++] = rightHash;

                        // Get next right item
                        rightRes = m_HashByOwner.TryGetNextValue(out rightHash, ref it);

                    } while (rightRes && leftHash > rightHash);
                }
            } while (leftRes || rightRes);

            // Increment each hash in "toInc" if they exist, add them to the RefCounter hash if they are new
            for (int i = 0; i < curIncIndex; i++)
            {
                var hash = toInc[i];
                if (m_RefCounterPerBlobHash.TryGetValue(hash, out var counter))
                {
                    m_RefCounterPerBlobHash[hash] = counter + 1;
                }
                else
                {
                    m_RefCounterPerBlobHash.Add(hash, 1);
                }
            }

            // Decrement each hash in "toDec", remove the BlobAsset if it reaches 0
            for (int i = 0; i < curDecIndex; i++)
            {
                // Decrement the hash of the previously assigned Blob Asset
                var hash = toDec[i];
                var oldHashCount = --m_RefCounterPerBlobHash[hash];
                    
                // If it reaches 0, we dispose the Blob Asset and remove the counter
                if (oldHashCount == 0)
                {
                    Remove<TB>(hash, true);
                    m_RefCounterPerBlobHash.Remove(hash);
                }                
            }

            // Clear the former list of BlobAsset hashes and replace by the new one
            m_HashByOwner.Remove(ownerId);

            for (int i = 0; i < leftLength; i++)
            {
                m_HashByOwner.Add(ownerId, newBlobHashes[i]);
            }
        }

        internal bool GetBlobAssetsOfGameObject(GameObject gameObject, Allocator allocator, out NativeArray<Hash128> result)
        {
            var key = gameObject.GetInstanceID();
            if (!m_HashByOwner.ContainsKey(key))
            {
                result = default;
                return false;
            }
            
            var count = m_HashByOwner.CountValuesForKey(key);
            result = new NativeArray<Hash128>(count, allocator);

            var index = 0;
            if (m_HashByOwner.TryGetFirstValue(key, out var hash, out var it))
            {
                result[index++] = hash;

                while (m_HashByOwner.TryGetNextValue(out hash, ref it))
                {
                    result[index++] = hash;
                }
            }

            return true;
        }
    }
}
