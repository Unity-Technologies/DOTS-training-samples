using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    //@TODO: Use field offset / union here... There seems to be an issue in mono preventing it...
    unsafe struct EntityQueryFilter
    {
        public struct SharedComponentData
        {
            public const int Capacity = 2;

            public int Count;
            public fixed int IndexInEntityQuery[Capacity];
            public fixed int SharedComponentIndex[Capacity];
        }

        // Saves the index of ComponentTypes in this group that have changed.
        public struct ChangedFilter
        {
            public const int Capacity = 2;

            public int Count;
            public fixed int IndexInEntityQuery[Capacity];
        }

        public uint RequiredChangeVersion;

        public SharedComponentData Shared;
        public ChangedFilter Changed;

        public bool RequiresMatchesFilter
        {
            get { return Shared.Count != 0 || Changed.Count != 0; }
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public void AssertValid()
        {
            Assert.IsTrue((Shared.Count <= SharedComponentData.Capacity && Shared.Count > 0) || (Changed.Count <= ChangedFilter.Capacity && Changed.Count > 0));
        }
#endif
    }

    /// <summary>
    /// Can be passed into IJobChunk.RunWithoutJobs to iterate over an entity query without running any jobs.
    /// </summary>
    public unsafe struct ArchetypeChunkIterator
    {
        internal readonly UnsafeMatchingArchetypePtrList m_MatchingArchetypeList;
        internal readonly ComponentJobSafetyManager* m_SafetyManager;
        int m_CurrentArchetypeIndex;
        int m_CurrentChunkInArchetypeIndex;
        int m_CurrentChunkFirstEntityIndexInQuery;
        Chunk* m_PreviousMatchingChunk;
        internal EntityQueryFilter m_Filter;
        internal readonly uint m_GlobalSystemVersion;
        
        internal MatchingArchetype* CurrentMatchingArchetype
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if(m_CurrentArchetypeIndex < 0 || m_CurrentArchetypeIndex >= m_MatchingArchetypeList.Length)
                    throw new InvalidOperationException("Tried to get an out of bounds Current matching archetype of an ArchetypeChunkIterator. Try calling Reset() and MoveNext().");
#endif
                return m_MatchingArchetypeList.Ptr[m_CurrentArchetypeIndex];
            }
        }
        
        internal Archetype* CurrentArchetype 
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if(m_CurrentArchetypeIndex < 0 || m_CurrentArchetypeIndex >= m_MatchingArchetypeList.Length)
                    throw new InvalidOperationException("Tried to get an out of bounds Current matching archetype of an ArchetypeChunkIterator. Try calling Reset() and MoveNext().");
#endif
                return CurrentMatchingArchetype->Archetype;
            }
        }

        internal Chunk* CurrentChunk
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if(m_CurrentChunkInArchetypeIndex >= CurrentArchetype->Chunks.Count)
                    throw new InvalidOperationException("Tried to get an out of bounds Current chunk of an ArchetypeChunkIterator. Try calling Reset() and MoveNext().");
#endif
                return CurrentArchetype->Chunks.p[m_CurrentChunkInArchetypeIndex];
            }
        }
        
        internal ArchetypeChunk CurrentArchetypeChunk
        {
            get
            {
                return new ArchetypeChunk
                {
                    m_Chunk = CurrentChunk,
                    entityComponentStore = m_MatchingArchetypeList.entityComponentStore
                };
            }
        }
        
        internal ArchetypeChunkIterator(UnsafeMatchingArchetypePtrList match, ComponentJobSafetyManager* safetyManager, uint globalSystemVersion,
            ref EntityQueryFilter filter)
        {
            m_MatchingArchetypeList = match;
            m_CurrentArchetypeIndex = 0;
            m_CurrentChunkInArchetypeIndex = -1;
            m_CurrentChunkFirstEntityIndexInQuery = 0;
            m_PreviousMatchingChunk = null;
            m_Filter = filter;
            m_GlobalSystemVersion = globalSystemVersion;
            m_SafetyManager = safetyManager;
        }

        /// <summary>
        /// Moves the iterator to the next chunk, returning false if at the end of the ArchetypeChunk list.
        /// </summary>
        /// <remarks>The initial call to MoveNext sets the iterator to the first chunk.</remarks>
        internal bool MoveNext()
        {
            while (true)
            {
                // if we've reached the end of the archetype list we're done
                if (m_CurrentArchetypeIndex >= m_MatchingArchetypeList.Length)
                    return false;

                // increment chunk index
                m_CurrentChunkInArchetypeIndex++;

                // if we've reached the end of the chunk list for this archetype...
                if (m_CurrentChunkInArchetypeIndex >= CurrentArchetype->Chunks.Count)
                {
                    // move to the next archetype
                    m_CurrentArchetypeIndex++;
                    m_CurrentChunkInArchetypeIndex = -1;
                    continue;
                }

                // if the current chunk does not match the filter...
                if (!CurrentMatchingArchetype->ChunkMatchesFilter(m_CurrentChunkInArchetypeIndex, ref m_Filter))
                {
                    // recurse
                    continue;
                }

                // Aggregate the entity index
                if (m_PreviousMatchingChunk != null)
                    m_CurrentChunkFirstEntityIndexInQuery += m_PreviousMatchingChunk->Count;

                // note: Previous matching chunk is set while its actually the current chunk to support entity index aggregation
                m_PreviousMatchingChunk = CurrentChunk;

                return true;
            }
        }

        /// <summary>
        /// Sets the iterator to its initial position, which is before the first element in the collection.
        /// </summary>
        internal void Reset()
        {
            m_CurrentArchetypeIndex = 0;
            m_CurrentChunkInArchetypeIndex = -1;
            m_CurrentChunkFirstEntityIndexInQuery = 0;
            m_PreviousMatchingChunk = null;
        }
        
        /// <summary>
        /// The index of the first entity of the current chunk, as if all entities accessed by this iterator were
        /// in a singular array.
        /// </summary>
        internal int CurrentChunkFirstEntityIndex
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var dummy = CurrentChunk; // this line throws an exception if we're outside the valid range of chunks in the iterator
#endif
                return m_CurrentChunkFirstEntityIndexInQuery;
            }    
        }

        internal void* GetCurrentChunkComponentDataPtr(bool isWriting, int indexInEntityQuery)
        {
            int indexInArchetype = CurrentMatchingArchetype->IndexInArchetype[indexInEntityQuery];
            return ChunkIterationUtility.GetChunkComponentDataPtr(CurrentChunk, isWriting, indexInArchetype, m_GlobalSystemVersion);
        }

        /// <summary>
        /// Calculates the total number of chunks this iterator can access.
        /// </summary>
        /// <returns>Number of chunks that can be accessed.</returns>
        internal int CalculateChunkCount()
        {
            return ChunkIterationUtility.CalculateChunkCount(m_MatchingArchetypeList, ref m_Filter);
        }

        /// <summary>
        /// Confirms if this ArchetypeChunkIterator uses a filter.
        /// </summary>
        /// <returns>True if the entity query this ArchetypeChunkIterator was created from uses filters, otherwise false.</returns>
        internal bool RequiresFilter()
        {
            return m_Filter.RequiresMatchesFilter;
        }
        
        internal Entity* GetEntityPtr()
        {
            var archetype = CurrentArchetype;
            var chunk = CurrentChunk;
            var indexInArchetype = 0;
            
            return (Entity*)chunk->Buffer + archetype->Offsets[indexInArchetype];
        }
        
        internal void* GetComponentDataPtr(bool isWriting, int typeIndexInQuery, uint systemVersion)
        {
            var archetype = CurrentArchetype;
            var chunk = CurrentChunk;

            var indexInArchetype = CurrentMatchingArchetype->IndexInArchetype[typeIndexInQuery];
            
            if (isWriting)
                chunk->SetChangeVersion(indexInArchetype, systemVersion);

            return chunk->Buffer + archetype->Offsets[indexInArchetype];
        }
        
        internal object GetManagedObject(ManagedComponentStore managedComponentStore, int typeIndexInQuery, int entityInChunkIndex)
        {
            return managedComponentStore.GetManagedObject(CurrentChunk, CurrentMatchingArchetype->IndexInArchetype[typeIndexInQuery], entityInChunkIndex);
        }
    }
}
