using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace Unity.Entities
{
    sealed class ArchetypeChunkDataDebugView
    {
        private ArchetypeChunkData m_ChunkData;

        public ArchetypeChunkDataDebugView(ArchetypeChunkData chunkData)
        {
            m_ChunkData = chunkData;
        }

        public unsafe ArchetypeChunk[] Items
        {
            get
            {
                var result = new ArchetypeChunk[m_ChunkData.Count];
                for (var i = 0; i < result.Length; ++i)
                    result[i] = *(ArchetypeChunk*)&m_ChunkData.p[i];
                return result;
            }
        }
    }

    sealed class UnsafeMatchingArchetypePtrListDebugView
        {
        private UnsafeMatchingArchetypePtrList m_MatchingArchetypeList;

        public UnsafeMatchingArchetypePtrListDebugView(UnsafeMatchingArchetypePtrList MatchingArchetypeList)
        {
            m_MatchingArchetypeList = MatchingArchetypeList;
        }

        public unsafe MatchingArchetype*[] Items
        {
            get
            {
                var result = new MatchingArchetype*[m_MatchingArchetypeList.Length];
                for (var i = 0; i < result.Length; ++i)
                    result[i] = m_MatchingArchetypeList.Ptr[i];
                return result;
            }
        }
    }

#if !NET_DOTS
    sealed unsafe class DebugViewUtility
    {
        [DebuggerDisplay("{name} {entity} Components: {components.Count}")]
        public struct Components
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string name;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Entity entity;
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public List<object> components;
        }
        
        public static object GetComponent(void* pointer, Type type)
        {
            if (typeof(IBufferElementData).IsAssignableFrom(type))
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(type);
                var instance = (IList)Activator.CreateInstance(constructedListType);
                var size = Marshal.SizeOf(type);
                BufferHeader* header = (BufferHeader*) pointer;
                var begin = BufferHeader.GetElementPointer(header);
                for (var i = 0; i < header->Length; ++i)
                {
                    var item = begin + (size * i);
                    instance.Add(Marshal.PtrToStructure((IntPtr) item, type));
                }
                return instance;
            }
            if(typeof(IComponentData).IsAssignableFrom(type) || typeof(Entity).IsAssignableFrom(type))
            {
                return Marshal.PtrToStructure((IntPtr) pointer, type);
            }
            return null;
        }        
        
        public static Components GetComponents(EntityManager m, Entity e)
        {
            Components components = new Components();
            components.entity = e;
            components.components = new List<object>();
            if (!m.Exists(e))
                return components;
#if UNITY_EDITOR                
            components.name = m.GetName(e);            
            components.components.Add(components.name);  
#endif
            m.EntityComponentStore->GetChunk(e, out var chunk, out var chunkIndex);
            if (chunk == null)
                return components;
            var archetype = chunk->Archetype;
            var types = chunk->Archetype->TypesCount;
            for (var i = 0; i < types; ++i)
            {
                var componentType = chunk->Archetype->Types[i];
                if (componentType.IsSharedComponent)
                    continue;
                var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);
                var type = TypeManager.GetType(typeInfo.TypeIndex);
                var offset = archetype->Offsets[i];
                var size = archetype->SizeOfs[i];
                var pointer = chunk->Buffer + (offset + size * chunkIndex);
                components.components.Add(GetComponent(pointer, type));
            }
            return components;
        }
        
    }
#endif
    
#if !NET_DOTS
    sealed class EntityManagerDebugView
    {
        private EntityManager m_target;
        public EntityManagerDebugView(EntityManager target)
        {
            m_target = target;
        }

        struct Comparer : IComparer<Entity>
        {
            public int Compare(Entity x, Entity y)
            {
                if (x.Index < y.Index)
                    return -1;
                if (x.Index > y.Index)
                    return 1;
                if (x.Version < y.Version)
                    return -1;
                if (x.Version > y.Version)
                    return 1;
                return 0;
            }
        }
        
        unsafe public List<DebugViewUtility.Components> Entities
        {
            get
            {
                var entities = m_target.GetAllEntities();
                entities.Sort(new Comparer());
                using(entities)
                {
                    var result = new List<DebugViewUtility.Components>();
                    for (var i = 0; i < entities.Length; ++i)
                        result.Add(DebugViewUtility.GetComponents(m_target, entities[i]));
                    return result;
                }
            }
        }
    }

    sealed class ArchetypeChunkDebugView
    {
        private ArchetypeChunk m_ArchetypeChunk;
        public ArchetypeChunkDebugView(ArchetypeChunk ArchetypeChunk)
        {
            m_ArchetypeChunk = ArchetypeChunk;
        }

        public unsafe IList[] Components
        {
            get
            {
                var chunk = m_ArchetypeChunk.m_Chunk;
                if (chunk == null)
                    return new IList[0];
                var archetype = chunk->Archetype;
                var types = chunk->Archetype->TypesCount;
                var entities = chunk->Count;
                var result = new IList[types];
                for (var i = 0; i < types; ++i)
                {
                    var componentType = chunk->Archetype->Types[i];
                    if (componentType.IsSharedComponent)
                        continue;
                    var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);
                    var type = TypeManager.GetType(typeInfo.TypeIndex);
                    var offset = archetype->Offsets[i];
                    var size = archetype->SizeOfs[i];
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(type);
                    if (typeof(IBufferElementData).IsAssignableFrom(type))
                        constructedListType = listType.MakeGenericType(constructedListType);
                    var instance = (IList)Activator.CreateInstance(constructedListType);
                    for (var j = 0; j < entities; ++j)
                    {
                       var pointer = chunk->Buffer + (offset + size * j);
                       instance.Add(DebugViewUtility.GetComponent(pointer, type));
                    }
                    result[i] = instance;
                }

                return result;
            }
        }

        public unsafe IList[] Entities
        {
            get
            {
                var chunk = m_ArchetypeChunk.m_Chunk;
                if (chunk == null)
                    return new IList[0];
                var archetype = chunk->Archetype;
                var types = chunk->Archetype->TypesCount;
                var entities = chunk->Count;
                var result = new IList[entities];
                for (var j = 0; j < entities; ++j)
                {
                    var instance = new List<object>();
                    for (var i = 0; i < types; ++i)
                    {
                        var componentType = chunk->Archetype->Types[i];
                        if (componentType.IsSharedComponent)
                            continue;
                        var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);
                        var type = TypeManager.GetType(typeInfo.TypeIndex);
                        var offset = archetype->Offsets[i];
                        var size = archetype->SizeOfs[i];
                        var pointer = chunk->Buffer + (offset + size * j);
                        instance.Add(DebugViewUtility.GetComponent(pointer,type));
                    }
                    result[j] = instance;
                }

                return result;
            }
        }

        public unsafe int[] SharedComponentValueArray
        {
            get
            {
                var chunk = m_ArchetypeChunk.m_Chunk;
                if (chunk == null)
                    return new int[0];
                var archetype = chunk->Archetype;
                int[] result = new int[archetype->NumSharedComponents];
                for (var i = 0; i < archetype->NumSharedComponents; ++i)
                    result[i] = chunk->SharedComponentValues[i];
                return result;
            }
        }

        public unsafe uint[] ChangeVersion
        {
            get
            {
                var chunk = m_ArchetypeChunk.m_Chunk;
                if (chunk == null)
                    return new uint[0];
                var archetype = chunk->Archetype;
                uint[] result = new uint[archetype->TypesCount];
                for (var i = 0; i < archetype->TypesCount; ++i)
                    result[i] = chunk->GetChangeVersion(i);
                return result;
            }
        }

    }
#else
    sealed class EntityManagerDebugView
    {
    }
    sealed class ArchetypeChunkDebugView
    {
    }
#endif

#if !NET_DOTS
    sealed class EntityArchetypeDebugView
    {
        private EntityArchetype m_EntityArchetype;
        public EntityArchetypeDebugView(EntityArchetype entityArchetype)
        {
            m_EntityArchetype = entityArchetype;
        }

        public unsafe struct ChunkPtr
        {
            public Chunk* m_Chunk;
        }

        unsafe public List<ChunkPtr> Chunks
        {
            get
            {
                List<ChunkPtr> result = new List<ChunkPtr>();
                var archetype = m_EntityArchetype.Archetype;
                for (var i = 0; i < archetype->Chunks.Count; ++i)
                    result.Add(new ChunkPtr{m_Chunk = archetype->Chunks.p[i]});
                return result;
            }
        }
        
        public unsafe Type[] Types
        {
            get
            {
                var archetype = m_EntityArchetype.Archetype;
                if (archetype == null)
                    return new Type[0];
                var types = archetype->TypesCount;
                var result = new Type[types];
                for (var i = 0; i < types; ++i)
                {
                    var type = TypeManager.GetType(TypeManager.GetTypeInfo(archetype->Types[i].TypeIndex).TypeIndex);
                    result[i] = type;
                }
                return result;
            }
        }

        public unsafe int[] Offsets
        {
            get
            {
                var archetype = m_EntityArchetype.Archetype;
                if (archetype == null)
                    return new int[0];
                int[] result = new int[archetype->TypesCount];
                Marshal.Copy((IntPtr)archetype->Offsets, result, 0, archetype->TypesCount);
                return result;
            }
        }

        public unsafe int[] SizeOfs
        {
            get
            {
                var archetype = m_EntityArchetype.Archetype;
                if (archetype == null)
                    return new int[0];
                int[] result = new int[archetype->TypesCount];
                Marshal.Copy((IntPtr)archetype->SizeOfs, result, 0, archetype->TypesCount);
                return result;
            }
        }

        public unsafe int[] TypeMemoryOrder
        {
            get
            {
                var archetype = m_EntityArchetype.Archetype;
                if (archetype == null)
                    return new int[0];
                int[] result = new int[archetype->TypesCount];
                Marshal.Copy((IntPtr)archetype->TypeMemoryOrder, result, 0, archetype->TypesCount);
                return result;
            }
        }

        public unsafe int[] ManagedArrayOffset
        {
            get
            {
                var archetype = m_EntityArchetype.Archetype;
                if (archetype == null || archetype->ManagedArrayOffset == null)
                    return new int[0];
                int[] result = new int[archetype->NumManagedArrays];
                Marshal.Copy((IntPtr)archetype->ManagedArrayOffset, result, 0, archetype->NumManagedArrays);
                return result;
            }
        }
    }
#else
    sealed class EntityArchetypeDebugView
    {
    }
#endif
    
}
