using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;

namespace Unity.Entities
{
    /// <summary>
    /// A block of unmanaged memory containing the components for entities sharing the same
    /// <see cref="Unity.Entities.Archetype"/>.
    /// </summary>
    [DebuggerTypeProxy(typeof(ArchetypeChunkDebugView))]
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public unsafe struct ArchetypeChunk : IEquatable<ArchetypeChunk>
    {
        [FieldOffset(0)]
        [NativeDisableUnsafePtrRestriction] internal Chunk* m_Chunk;
        [FieldOffset(8)]
        [NativeDisableUnsafePtrRestriction] internal EntityComponentStore* entityComponentStore;

        /// <summary>
        /// The number of entities currently stored in the chunk.
        /// </summary>
        public int Count => m_Chunk->Count;
        /// <summary>
        /// The number of entities that can fit in this chunk.
        /// </summary>
        /// <remarks>The capacity of a chunk depends on the size of the components making up the
        /// <see cref="Unity.Entities.Archetype"/> of the entities stored in the chunk.</remarks>
        public int Capacity => m_Chunk->Capacity;
        /// <summary>
        /// Whether this chunk is exactly full.
        /// </summary>
        public bool Full => Count == Capacity;

        internal ArchetypeChunk(Chunk* chunk, EntityComponentStore* entityComponentStore)
        {
            m_Chunk = chunk;
            this.entityComponentStore = entityComponentStore;
        }

        /// <summary>
        /// Two ArchetypeChunk instances are equal if they reference the same block of chunk memory.
        /// </summary>
        /// <param name="lhs">An ArchetypeChunk</param>
        /// <param name="rhs">Another ArchetypeChunk</param>
        /// <returns>True, if both ArchetypeChunk instances reference the same memory, or both contain null memory
        /// references.</returns>
        public static bool operator ==(ArchetypeChunk lhs, ArchetypeChunk rhs)
        {
            return lhs.m_Chunk == rhs.m_Chunk;
        }

        /// <summary>
        /// Two ArchetypeChunk instances are only equal if they reference the same block of chunk memory.
        /// </summary>
        /// <param name="lhs">An ArchetypeChunk</param>
        /// <param name="rhs">Another ArchetypeChunk</param>
        /// <returns>True, if the ArchetypeChunk instances reference different blocks of memory.</returns>
        public static bool operator !=(ArchetypeChunk lhs, ArchetypeChunk rhs)
        {
            return lhs.m_Chunk != rhs.m_Chunk;
        }

        /// <summary>
        /// Two ArchetypeChunk instances are equal if they reference the same block of chunk memory.
        /// </summary>
        /// <param name="compare">An object</param>
        /// <returns>True if <paramref name="compare"/> is an `ArchetypeChunk` instance that references the same memory,
        /// or both contain null memory references; otherwise false.</returns>
        public override bool Equals(object compare)
        {
            return this == (ArchetypeChunk) compare;
        }

        /// <summary>
        /// Computes a hashcode to support hash-based collections.
        /// </summary>
        /// <returns>The computed hash.</returns>
        public override int GetHashCode()
        {
            UIntPtr chunkAddr   = (UIntPtr) m_Chunk;
            long    chunkHiHash = ((long) chunkAddr) >> 15;
            int     chunkHash   = (int)chunkHiHash;
            return chunkHash;
        }

        /// <summary>
        /// The archetype of the entities stored in this chunk.
        /// </summary>
        /// <remarks>All entities in a chunk must have the same <see cref="Unity.Entities.Archetype"/>.</remarks>
        public EntityArchetype Archetype
        {
            get
            {
                return new EntityArchetype()
                {
                    Archetype = m_Chunk->Archetype
                };
            }
        }

        /// <summary>
        /// A special "null" ArchetypeChunk that you can use to test whether ArchetypeChunk instances are valid.
        /// </summary>
        /// <remarks>An ArchetypeChunk struct that refers to a chunk of memory that has been freed will be equal to
        /// this "null" ArchetypeChunk instance.</remarks>
        public static ArchetypeChunk Null => new ArchetypeChunk();

        /// <summary>
        /// Two ArchetypeChunk instances are equal if they reference the same block of chunk memory.
        /// </summary>
        /// <param name="archetypeChunk">Another ArchetypeChunk instance</param>
        /// <returns>True, if both ArchetypeChunk instances reference the same memory or both contain null memory
        /// references.</returns>
        public bool Equals(ArchetypeChunk archetypeChunk)
        {
            return this.m_Chunk == archetypeChunk.m_Chunk;
        }

        /// <summary>
        /// The number of shared components in the archetype associated with this chunk.
        /// </summary>
        /// <returns>The shared component count.</returns>
        public int NumSharedComponents()
        {
            return m_Chunk->Archetype->NumSharedComponents;
        }

        /// <summary>
        /// Reports whether this ArchetypeChunk instance is invalid.
        /// </summary>
        /// <returns>True, if no <see cref="Unity.Entities.Archetype"/> is associated with the this ArchetypeChunk
        /// instance.</returns>
        public bool Invalid()
        {
            return m_Chunk->Archetype == null;
        }

        /// <summary>
        /// Reports whether this ArchetypeChunk is locked.
        /// </summary>
        /// <seealso cref="EntityManager.LockChunk(ArchetypeChunk"/>
        /// <seealso cref="EntityManager.UnlockChunk(ArchetypeChunk"/>
        /// <returns>True, if locked.</returns>
        public bool Locked()
        {
            return m_Chunk->Locked;
        }

        /// <summary>
        /// Provides a native array interface to entity instances stored in this chunk.
        /// </summary>
        /// <remarks>The native array returned by this method references existing data, not a copy.</remarks>
        /// <param name="archetypeChunkEntityType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkEntityType()"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.</param>
        /// <returns>A native array containing the entities in the chunk.</returns>
        public NativeArray<Entity> GetNativeArray(ArchetypeChunkEntityType archetypeChunkEntityType)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(archetypeChunkEntityType.m_Safety);
#endif
            var archetype = m_Chunk->Archetype;
            var buffer = m_Chunk->Buffer;
            var length = m_Chunk->Count;
            var startOffset = archetype->Offsets[0];
            var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Entity>(buffer + startOffset, length, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, archetypeChunkEntityType.m_Safety);
#endif
            return result;
        }

        /// <summary>
        /// Reports whether any of IComponentData components in the chunk, of the type identified by
        /// <paramref name="chunkComponentType"/>, could have changed.
        /// </summary>
        /// <remarks>
        /// Note that for efficiency, the change version applies to whole chunks not individual entities. The change
        /// version is incremented even when another job or system that has declared write access to a component does
        /// not actually change the component value.</remarks>
        /// <param name="chunkComponentType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkComponentType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.
        /// </param>
        /// <param name="version">The version to compare. In a system, this parameter should be set to the
        /// current <see cref="Unity.Entities.ComponentSystemBase.LastSystemVersion"/> at the time the job is run or
        /// scheduled.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>True, if the version number stored in the chunk for this component is more recent than the version
        /// passed to the <paramref name="version"/> parameter.</returns>
        public bool DidChange<T>(ArchetypeChunkComponentType<T> chunkComponentType, uint version) where T :
#if UNITY_DISABLE_MANAGED_COMPONENTS
            struct,
#endif
            IComponentData
        {
            return ChangeVersionUtility.DidChange(GetComponentVersion(chunkComponentType), version);
        }

        /// <summary>
        /// Reports whether any of dynamic buffer components in the chunk, of the type identified by
        /// <paramref name="chunkBufferType"/>, could have changed.
        /// </summary>
        /// <remarks>
        /// Note that for efficiency, the change version applies to whole chunks not individual entities. The change
        /// version is incremented even when another job or system that has declared write access to a component does
        /// not actually change the component value.</remarks>
        /// <param name="chunkBufferType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkBufferType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.</param>
        /// <param name="version">The version to compare. In a system, this parameter should be set to the
        /// current <see cref="Unity.Entities.ComponentSystemBase.LastSystemVersion"/> at the time the job is run or
        /// scheduled.</param>
        /// <typeparam name="T">The data type of the elements in the dynamic buffer.</typeparam>
        /// <returns>True, if the version number stored in the chunk for this component is more recent than the version
        /// passed to the <paramref name="version"/> parameter.</returns>
        public bool DidChange<T>(ArchetypeChunkBufferType<T> chunkBufferType, uint version) where T : struct, IBufferElementData
        {
            return ChangeVersionUtility.DidChange(GetComponentVersion(chunkBufferType), version);
        }

        public bool DidChange<T>(ArchetypeChunkSharedComponentType<T> chunkSharedComponentData, uint version) where T : struct, ISharedComponentData
        {
            return ChangeVersionUtility.DidChange(GetComponentVersion(chunkSharedComponentData), version);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkComponentType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public uint GetComponentVersion<T>(ArchetypeChunkComponentType<T> chunkComponentType)
            where T : IComponentData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex);
            if (typeIndexInArchetype == -1) return 0;
            return m_Chunk->GetChangeVersion(typeIndexInArchetype);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkBufferType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public uint GetComponentVersion<T>(ArchetypeChunkBufferType<T> chunkBufferType)
            where T : struct, IBufferElementData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkBufferType.m_TypeIndex);
            if (typeIndexInArchetype == -1) return 0;
            return m_Chunk->GetChangeVersion(typeIndexInArchetype);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkSharedComponentData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public uint GetComponentVersion<T>(ArchetypeChunkSharedComponentType<T> chunkSharedComponentData)
            where T : struct, ISharedComponentData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkSharedComponentData.m_TypeIndex);
            if (typeIndexInArchetype == -1) return 0;
            return m_Chunk->GetChangeVersion(typeIndexInArchetype);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkComponentType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetChunkComponentData<T>(ArchetypeChunkComponentType<T> chunkComponentType)
            where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(chunkComponentType.m_Safety);
#endif
            var ptr = entityComponentStore->GetComponentDataWithTypeRO(m_Chunk->metaChunkEntity, chunkComponentType.m_TypeIndex);
            T value;
            UnsafeUtility.CopyPtrToStructure(ptr, out value);
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkComponentType"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void SetChunkComponentData<T>(ArchetypeChunkComponentType<T> chunkComponentType, T value)
            where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(chunkComponentType.m_Safety);
#endif
            var ptr = entityComponentStore->GetComponentDataWithTypeRW(m_Chunk->metaChunkEntity, chunkComponentType.m_TypeIndex, entityComponentStore->GlobalSystemVersion);
            UnsafeUtility.CopyStructureToPtr(ref value, ptr);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkSharedComponentData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetSharedComponentIndex<T>(ArchetypeChunkSharedComponentType<T> chunkSharedComponentData)
            where T : struct, ISharedComponentData
        {
            var archetype = m_Chunk->Archetype;
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, chunkSharedComponentData.m_TypeIndex);
            if (typeIndexInArchetype == -1) return -1;

            var chunkSharedComponentIndex = typeIndexInArchetype - archetype->FirstSharedComponent;
            var sharedComponentIndex = m_Chunk->GetSharedComponentValue(chunkSharedComponentIndex);
            return sharedComponentIndex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkSharedComponentData"></param>
        /// <param name="entityManager"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSharedComponentData<T>(ArchetypeChunkSharedComponentType<T> chunkSharedComponentData, EntityManager entityManager)
            where T : struct, ISharedComponentData
        {
            return entityManager.GetSharedComponentData<T>(GetSharedComponentIndex(chunkSharedComponentData));
        }

        /// <summary>
        /// Reports whether this chunk contains the specified component type.
        /// </summary>
        /// <remarks>When an <see cref="Unity.Entities.EntityQuery"/> includes optional components (using
        /// <see cref="EntityQueryDesc.Any"/>), some chunks returned by the query may contain such components and some
        /// may not. Use this function to determine whether or not the current chunk contains one of these optional
        /// component types.</remarks>
        /// <param name="chunkComponentType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkComponentType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.
        /// </param>
        /// <typeparam name="T">The data type of the component.</typeparam>
        /// <returns>True, if this chunk contains an array of the specified component type.</returns>
        public bool Has<T>(ArchetypeChunkComponentType<T> chunkComponentType)
            where T :
#if UNITY_DISABLE_MANAGED_COMPONENTS
            struct,
#endif
            IComponentData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex);
            return (typeIndexInArchetype != -1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkComponentType"></param>
        /// <returns></returns>
        public bool Has(ArchetypeChunkComponentTypeDynamic chunkComponentType)
        {
            ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex, ref chunkComponentType.m_TypeLookupCache);
            return (chunkComponentType.m_TypeLookupCache != -1);
        }

        /// <summary>
        /// Reports whether this chunk contains a chunk component of the specified component type.
        /// </summary>
        /// <remarks>When an <see cref="Unity.Entities.EntityQuery"/> includes optional components used as chunk
        /// components (with <see cref="EntityQueryDesc.Any"/>), some chunks returned by the query may have these chunk
        /// components and some may not. Use this function to determine whether or not the current chunk contains one of
        /// these optional component types as a chunk component.</remarks>
        /// <param name="chunkComponentType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkComponentType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.
        /// </param>
        /// <typeparam name="T">The data type of the chunk component.</typeparam>
        /// <returns>True, if this chunk contains a chunk component of the specified type.</returns>
        public bool HasChunkComponent<T>(ArchetypeChunkComponentType<T> chunkComponentType)
            where T : struct, IComponentData
        {
            var metaChunkArchetype = m_Chunk->Archetype->MetaChunkArchetype;
            if (metaChunkArchetype == null)
                return false;
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype->MetaChunkArchetype, chunkComponentType.m_TypeIndex);
            return (typeIndexInArchetype != -1);
        }

        /// <summary>
        /// Reports whether this chunk contains a shared component of the specified component type.
        /// </summary>
        /// <remarks>When an <see cref="Unity.Entities.EntityQuery"/> includes optional components used as shared
        /// components (with <see cref="EntityQueryDesc.Any"/>), some chunks returned by the query may have these shared
        /// components and some may not. Use this function to determine whether or not the current chunk contains one of
        /// these optional component types as a shared component.</remarks>
        /// <param name="chunkComponentType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkSharedComponentType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.
        /// </param>
        /// <typeparam name="T">The data type of the shared component.</typeparam>
        /// <returns>True, if this chunk contains a shared component of the specified type.</returns>
        public bool Has<T>(ArchetypeChunkSharedComponentType<T> chunkComponentType)
            where T : struct, ISharedComponentData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex);
            return (typeIndexInArchetype != -1);
        }

        /// <summary>
        /// Reports whether this chunk contains a dynamic buffer containing the specified component type.
        /// </summary>
        /// <remarks>When an <see cref="Unity.Entities.EntityQuery"/> includes optional dynamic buffer types
        /// (with <see cref="EntityQueryDesc.Any"/>), some chunks returned by the query may have these dynamic buffers
        /// components and some may not. Use this function to determine whether or not the current chunk contains one of
        /// these optional dynamic buffers.</remarks>
        /// <param name="chunkBufferType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkBufferType{T}"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.</param>
        /// <typeparam name="T">The data type of the component stored in the dynamic buffer.</typeparam>
        /// <returns>True, if this chunk contains an array of the dynamic buffers containing the specified component type.</returns>
        public bool Has<T>(ArchetypeChunkBufferType<T> chunkBufferType)
            where T : struct, IBufferElementData
        {
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkBufferType.m_TypeIndex);
            return (typeIndexInArchetype != -1);
        }

        /// <summary>
        /// Provides a native array interface to components stored in this chunk.
        /// </summary>
        /// <remarks>The native array returned by this method references existing data, not a copy.</remarks>
        /// <param name="chunkComponentType">An object containing type and job safety information. Create this
        /// object by calling <see cref="Unity.Entities.JobComponentSystem.GetArchetypeChunkComponentTypeType()"/> immediately
        /// before scheduling a job. Pass the object to a job using a public field you define as part of the job struct.</param>
        /// <typeparam name="T">The data type of the component.</typeparam>
        /// <exception cref="ArgumentException">If you call this function on a "tag" component type (which is an empty
        /// component with no fields).</exception>
        /// <returns>A native array containing the components in the chunk.</returns>
        public NativeArray<T> GetNativeArray<T>(ArchetypeChunkComponentType<T> chunkComponentType)
            where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunkComponentType.m_IsZeroSized)
                throw new ArgumentException($"ArchetypeChunk.GetNativeArray<{typeof(T)}> cannot be called on zero-sized IComponentData");

            AtomicSafetyHandle.CheckReadAndThrow(chunkComponentType.m_Safety);
#endif
            var archetype = m_Chunk->Archetype;
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex);
            if (typeIndexInArchetype == -1)
            {
                var emptyResult =
                    NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(null, 0, 0);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref emptyResult, chunkComponentType.m_Safety);
#endif
                return emptyResult;
            }

            var buffer = m_Chunk->Buffer;
            var length = m_Chunk->Count;
            var startOffset = archetype->Offsets[typeIndexInArchetype];
            var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(buffer + startOffset, length, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, chunkComponentType.m_Safety);
#endif
            if (!chunkComponentType.IsReadOnly)
                m_Chunk->SetChangeVersion(typeIndexInArchetype, chunkComponentType.GlobalSystemVersion);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunkComponentType"></param>
        /// <param name="expectedTypeSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public NativeArray<T> GetDynamicComponentDataArrayReinterpret<T>(ArchetypeChunkComponentTypeDynamic chunkComponentType, int expectedTypeSize)
            where T : struct
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunkComponentType.m_IsZeroSized)
                throw new ArgumentException($"ArchetypeChunk.GetDynamicComponentDataArrayReinterpret<{typeof(T)}> cannot be called on zero-sized IComponentData");

            AtomicSafetyHandle.CheckReadAndThrow(chunkComponentType.m_Safety);
#endif
            var archetype = m_Chunk->Archetype;
            ChunkDataUtility.GetIndexInTypeArray(m_Chunk->Archetype, chunkComponentType.m_TypeIndex, ref chunkComponentType.m_TypeLookupCache);
            var typeIndexInArchetype = chunkComponentType.m_TypeLookupCache;
            if (typeIndexInArchetype == -1)
            {
                var emptyResult =
                    NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(null, 0, 0);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref emptyResult, chunkComponentType.m_Safety);
#endif
                return emptyResult;
            }

            var typeSize = archetype->SizeOfs[typeIndexInArchetype];
            var length = m_Chunk->Count;
            var byteLen = length * typeSize;
            var outTypeSize = UnsafeUtility.SizeOf<T>();
            var outLength = byteLen / outTypeSize;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (typeSize != expectedTypeSize)
            {
                throw new InvalidOperationException($"Dynamic chunk component type {TypeManager.GetType(chunkComponentType.m_TypeIndex)} (size = {typeSize}) size does not equal {expectedTypeSize}. Component size must match with expectedTypeSize.");
            }

            if (outTypeSize * outLength != byteLen)
            {
                throw new InvalidOperationException($"Dynamic chunk component type {TypeManager.GetType(chunkComponentType.m_TypeIndex)} (array length {length}) and {typeof(T)} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.");
            }
#endif

            var buffer = m_Chunk->Buffer;
            var startOffset = archetype->Offsets[typeIndexInArchetype];
            var result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(buffer + startOffset, outLength, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref result, chunkComponentType.m_Safety);
#endif
            if (!chunkComponentType.IsReadOnly)
                m_Chunk->SetChangeVersion(typeIndexInArchetype, chunkComponentType.GlobalSystemVersion);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="manager"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ArchetypeChunkComponentObjects<T> GetComponentObjects<T>(ArchetypeChunkComponentType<T> componentType, EntityManager manager)
            where T : class
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(componentType.m_Safety);
#endif
            var archetype = m_Chunk->Archetype;

            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, componentType.m_TypeIndex);

            int offset, length;
            var array = manager.ManagedComponentStore.GetManagedObjectRange(m_Chunk, typeIndexInArchetype, out offset, out length);

            var componentArray = new ArchetypeChunkComponentObjects<T>(offset, length, array);
            return componentArray;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bufferComponentType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BufferAccessor<T> GetBufferAccessor<T>(ArchetypeChunkBufferType<T> bufferComponentType)
            where T : struct, IBufferElementData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(bufferComponentType.m_Safety0);
#endif
            var archetype = m_Chunk->Archetype;
            var typeIndex = bufferComponentType.m_TypeIndex;
            var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, typeIndex);
            if (typeIndexInArchetype == -1)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return new BufferAccessor<T>(null, 0, 0, true, bufferComponentType.m_Safety0, bufferComponentType.m_Safety1, 0);
#else
                return new BufferAccessor<T>(null, 0, 0, 0);
#endif
            }

            int internalCapacity = archetype->BufferCapacities[typeIndexInArchetype];

            if (!bufferComponentType.IsReadOnly)
                m_Chunk->SetChangeVersion(typeIndexInArchetype, bufferComponentType.GlobalSystemVersion);

            var buffer = m_Chunk->Buffer;
            var length = m_Chunk->Count;
            var startOffset = archetype->Offsets[typeIndexInArchetype];
            int stride = archetype->SizeOfs[typeIndexInArchetype];
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferAccessor<T>(buffer + startOffset, length, stride, bufferComponentType.IsReadOnly, bufferComponentType.m_Safety0, bufferComponentType.m_Safety1, internalCapacity);
#else
            return new BufferAccessor<T>(buffer + startOffset, length, stride, internalCapacity);
#endif
        }
#if ENABLE_DOTS_COMPILER_CHUNKS
        public ChunkEntitiesDescription Entities => throw new ArgumentException("Using chunk.Entities is only possible inside a entityQuery.Chunks.ForEach() lambda job.");
#endif
    }

    /// <summary>
    ///
    /// </summary>
    public struct ChunkEntitiesDescription : ISupportForEachWithUniversalDelegate
    {
    }

    /// <summary>
    ///
    /// </summary>
    [ChunkSerializable]
    public struct ChunkHeader : ISystemStateComponentData
    {
        public ArchetypeChunk ArchetypeChunk;

        public static unsafe ChunkHeader Null
        {
            get
            {
                return new ChunkHeader {ArchetypeChunk = new ArchetypeChunk(null, null)}; 
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NativeContainer]
    public unsafe struct BufferAccessor<T>
        where T: struct, IBufferElementData
    {
        [NativeDisableUnsafePtrRestriction]
        private byte* m_BasePointer;
        private int m_Length;
        private int m_Stride;
        private int m_InternalCapacity;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private bool m_IsReadOnly;
#endif

        public int Length => m_Length;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety0;
        private AtomicSafetyHandle m_ArrayInvalidationSafety;

#pragma warning disable 0414 // assigned but its value is never used
        private int m_SafetyReadOnlyCount;
        private int m_SafetyReadWriteCount;
#pragma warning restore 0414

#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        /// <summary>
        ///
        /// </summary>
        /// <param name="basePointer"></param>
        /// <param name="length"></param>
        /// <param name="stride"></param>
        /// <param name="readOnly"></param>
        /// <param name="safety"></param>
        /// <param name="arrayInvalidationSafety"></param>
        /// <param name="internalCapacity"></param>
        public BufferAccessor(byte* basePointer, int length, int stride, bool readOnly, AtomicSafetyHandle safety, AtomicSafetyHandle arrayInvalidationSafety, int internalCapacity)
        {
            m_BasePointer = basePointer;
            m_Length = length;
            m_Stride = stride;
            m_Safety0 = safety;
            m_ArrayInvalidationSafety = arrayInvalidationSafety;
            m_IsReadOnly = readOnly;
            m_SafetyReadOnlyCount = readOnly ? 2 : 0;
            m_SafetyReadWriteCount = readOnly ? 0 : 2;
            m_InternalCapacity = internalCapacity;
        }
#else
        public BufferAccessor(byte* basePointer, int length, int stride, int internalCapacity)
        {
            m_BasePointer = basePointer;
            m_Length = length;
            m_Stride = stride;
            m_InternalCapacity = internalCapacity;
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public DynamicBuffer<T> this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety0);

                if (index < 0 || index >= Length)
                    throw new InvalidOperationException($"index {index} out of range in LowLevelBufferAccessor of length {Length}");
#endif
                BufferHeader* hdr = (BufferHeader*) (m_BasePointer + index * m_Stride);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                return new DynamicBuffer<T>(hdr, m_Safety0, m_ArrayInvalidationSafety, m_IsReadOnly, false, 0, m_InternalCapacity);
#else
                return new DynamicBuffer<T>(hdr, m_InternalCapacity);
#endif
            }
        }
    }

    [BurstCompile]
    unsafe struct GatherArchetypeChunks : IJobParallelFor
    {
        [ReadOnly] public NativeList<EntityArchetype> Archetypes;
        [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;
        [ReadOnly] public NativeArray<int> Offsets;
        [NativeDisableParallelForRestriction]
        public NativeArray<ArchetypeChunk> Chunks;

        public void Execute(int index)
        {
            var archetype = Archetypes[index];
            var offset = Offsets[index];
            for (var i = 0; i < archetype.Archetype->Chunks.Count; ++i)
                Chunks[offset + i] = new ArchetypeChunk(archetype.Archetype->Chunks.p[i], entityComponentStore);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public unsafe struct ArchetypeChunkArray
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        static internal NativeArray<ArchetypeChunk> Create(NativeList<EntityArchetype> archetypes, EntityComponentStore* entityComponentStore, Allocator allocator, AtomicSafetyHandle safetyHandle)
#else
        static internal NativeArray<ArchetypeChunk> Create(NativeList<EntityArchetype> archetypes, EntityComponentStore* entityComponentStore, Allocator allocator)
#endif
        {
            int length = 0;
            var archetypeCount = archetypes.Length;
            var offsets = new NativeArray<int>(archetypeCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < archetypeCount; i++)
            {
                offsets[i] = length;
                length += archetypes[i].Archetype->Chunks.Count;
            }

            var chunks = new NativeArray<ArchetypeChunk>(length, allocator, NativeArrayOptions.UninitializedMemory);
            var gatherChunksJob = new GatherArchetypeChunks
            {
                Archetypes = archetypes,
                entityComponentStore = entityComponentStore,
                Offsets = offsets,
                Chunks = chunks
            };
            var gatherChunksJobHandle = gatherChunksJob.Schedule(archetypeCount,1);
            gatherChunksJobHandle.Complete();

            offsets.Dispose();
            return chunks;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="chunks"></param>
        /// <returns></returns>
        static public int CalculateEntityCount(NativeArray<ArchetypeChunk> chunks)
        {
            int entityCount = 0;
            for (var i = 0; i < chunks.Length; i++)
            {
                entityCount += chunks[i].Count;
            }

            return entityCount;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NativeContainer]
    [NativeContainerSupportsMinMaxWriteRestriction]
    public struct ArchetypeChunkComponentType<T>
    {
        internal readonly int m_TypeIndex;
        internal readonly uint m_GlobalSystemVersion;
        internal readonly bool m_IsReadOnly;
        internal readonly bool m_IsZeroSized;

        public uint GlobalSystemVersion => m_GlobalSystemVersion;
        public bool IsReadOnly => m_IsReadOnly;

#pragma warning disable 0414
        private readonly int m_Length;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private readonly int m_MinIndex;
        private readonly int m_MaxIndex;
        internal readonly AtomicSafetyHandle m_Safety;
#endif
#pragma warning restore 0414

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal ArchetypeChunkComponentType(AtomicSafetyHandle safety, bool isReadOnly, uint globalSystemVersion)
#else
        internal ArchetypeChunkComponentType(bool isReadOnly, uint globalSystemVersion)
#endif
        {
            m_Length = 1;
            m_TypeIndex = TypeManager.GetTypeIndex<T>();
            m_IsZeroSized = TypeManager.GetTypeInfo(m_TypeIndex).IsZeroSized;
            m_GlobalSystemVersion = globalSystemVersion;
            m_IsReadOnly = isReadOnly;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = 0;
            m_Safety = safety;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    [NativeContainer]
    [NativeContainerSupportsMinMaxWriteRestriction]
    public struct ArchetypeChunkComponentTypeDynamic
    {
        internal readonly int m_TypeIndex;
        internal readonly uint m_GlobalSystemVersion;
        internal readonly bool m_IsReadOnly;
        internal readonly bool m_IsZeroSized;

        public uint GlobalSystemVersion => m_GlobalSystemVersion;
        public bool IsReadOnly => m_IsReadOnly;

#pragma warning disable 0414
        private readonly int m_Length;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private readonly int m_MinIndex;
        private readonly int m_MaxIndex;
        internal readonly AtomicSafetyHandle m_Safety;
#endif
#pragma warning restore 0414

        /// <summary>
        ///
        /// </summary>
        public int m_TypeLookupCache;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal ArchetypeChunkComponentTypeDynamic(ComponentType componentType, AtomicSafetyHandle safety, uint globalSystemVersion)
#else
        internal ArchetypeChunkComponentTypeDynamic(ComponentType componentType, uint globalSystemVersion)
#endif
        {
            m_Length = 1;
            m_TypeIndex = componentType.TypeIndex;
            m_IsZeroSized = TypeManager.GetTypeInfo(m_TypeIndex).IsZeroSized;
            m_GlobalSystemVersion = globalSystemVersion;
            m_IsReadOnly = componentType.AccessModeType == ComponentType.AccessMode.ReadOnly;
            m_TypeLookupCache = 0;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = 0;
            m_Safety = safety;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NativeContainer]
    [NativeContainerSupportsMinMaxWriteRestriction]
    public struct ArchetypeChunkBufferType<T>
        where T : struct, IBufferElementData
    {
        internal readonly int m_TypeIndex;
        internal readonly uint m_GlobalSystemVersion;
        internal readonly bool m_IsReadOnly;

        public uint GlobalSystemVersion => m_GlobalSystemVersion;
        public bool IsReadOnly => m_IsReadOnly;

#pragma warning disable 0414
        private readonly int m_Length;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private readonly int m_MinIndex;
        private readonly int m_MaxIndex;

        internal AtomicSafetyHandle m_Safety0;
        internal AtomicSafetyHandle m_Safety1;
        internal int m_SafetyReadOnlyCount;
        internal int m_SafetyReadWriteCount;
#endif
#pragma warning restore 0414

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal ArchetypeChunkBufferType(AtomicSafetyHandle safety, AtomicSafetyHandle arrayInvalidationSafety, bool isReadOnly, uint globalSystemVersion)
#else
        internal ArchetypeChunkBufferType (bool isReadOnly, uint globalSystemVersion)
#endif
        {
            m_Length = 1;
            m_TypeIndex = TypeManager.GetTypeIndex<T>();
            m_GlobalSystemVersion = globalSystemVersion;
            m_IsReadOnly = isReadOnly;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = 0;
            m_Safety0 = safety;
            m_Safety1 = arrayInvalidationSafety;
            m_SafetyReadOnlyCount = isReadOnly ? 2 : 0;
            m_SafetyReadWriteCount = isReadOnly ? 0 : 2;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [NativeContainer]
    [NativeContainerSupportsMinMaxWriteRestriction]
    public struct ArchetypeChunkSharedComponentType<T>
        where T : struct, ISharedComponentData
    {
        internal readonly int m_TypeIndex;

#pragma warning disable 0414
        private readonly int m_Length;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private readonly int m_MinIndex;
        private readonly int m_MaxIndex;
        internal readonly AtomicSafetyHandle m_Safety;
#endif
#pragma warning restore 0414

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal ArchetypeChunkSharedComponentType(AtomicSafetyHandle safety)
#else
        internal unsafe ArchetypeChunkSharedComponentType(bool unused)
#endif
        {
            m_Length = 1;
            m_TypeIndex = TypeManager.GetTypeIndex<T>();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = 0;
            m_Safety = safety;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    [NativeContainer]
    [NativeContainerSupportsMinMaxWriteRestriction]
    public struct ArchetypeChunkEntityType
    {
#pragma warning disable 0414
        private readonly int m_Length;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private readonly int m_MinIndex;
        private readonly int m_MaxIndex;
        internal readonly AtomicSafetyHandle m_Safety;
#endif
#pragma warning restore 0414

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal ArchetypeChunkEntityType(AtomicSafetyHandle safety)
#else
        internal unsafe ArchetypeChunkEntityType(bool unused)
#endif
        {
            m_Length = 1;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_MinIndex = 0;
            m_MaxIndex = 0;
            m_Safety = safety;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [StructLayout(LayoutKind.Sequential)]
    public struct ArchetypeChunkComponentObjects<T>
        where T : class
    {
        /// <summary>
        ///
        /// </summary>
        public readonly int  Length;
        internal int         Offset;
        internal T[]         Array;

        internal const int ArrayByteOffset = 8;

        unsafe internal ArchetypeChunkComponentObjects(int offset, int length, object[] objectArray)
        {
            Length = length;
            Offset = offset;
            Array = null;

            var arrayPtr = (byte*) UnsafeUtility.AddressOf(ref this);
            UnsafeUtility.CopyObjectAddressToPtr(objectArray, arrayPtr + ArrayByteOffset);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= (uint)Length)
                    throw new IndexOutOfRangeException($"index: {index} must be smaller than Length: {Length}");
#endif

                return Array[Offset + index];
            }

            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= (uint)Length)
                    throw new IndexOutOfRangeException($"index: {index} must be smaller than Length: {Length}");
#endif

                Array[Offset + index] = value;
            }
        }
    }
}
