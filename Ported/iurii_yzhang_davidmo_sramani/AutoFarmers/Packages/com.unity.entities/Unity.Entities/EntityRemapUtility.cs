using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#if !NET_DOTS
using Unity.Properties;
#endif
using EntityOffsetInfo = Unity.Entities.TypeManager.EntityOffsetInfo;

namespace Unity.Entities
{
    public static unsafe class EntityRemapUtility
    {
        public struct EntityRemapInfo
        {
            public int SourceVersion;
            public Entity Target;
        }

        public struct SparseEntityRemapInfo
        {
            public Entity Src;
            public Entity Target;
        }

        public static void GetTargets(out NativeArray<Entity> output, NativeArray<EntityRemapInfo> remapping)
        {
            NativeArray<Entity> temp = new NativeArray<Entity>(remapping.Length, Allocator.TempJob);
            var outputs = 0;
            for (var i = 0; i < remapping.Length; ++i)
                if (remapping[i].Target != Entity.Null)
                    temp[outputs++] = remapping[i].Target;
            output = new NativeArray<Entity>(outputs, Allocator.Persistent);
            UnsafeUtility.MemCpy(output.GetUnsafePtr(), temp.GetUnsafePtr(), sizeof(Entity) * outputs);
            temp.Dispose();
        }

        public static void AddEntityRemapping(ref NativeArray<EntityRemapInfo> remapping, Entity source, Entity target)
        {
            remapping[source.Index] = new EntityRemapInfo { SourceVersion = source.Version, Target = target };
        }

        public static Entity RemapEntity(ref NativeArray<EntityRemapInfo> remapping, Entity source)
        {
            return RemapEntity((EntityRemapInfo*)remapping.GetUnsafeReadOnlyPtr(), source);
        }

        public static Entity RemapEntity(EntityRemapInfo* remapping, Entity source)
        {
            if (source.Version == remapping[source.Index].SourceVersion)
                return remapping[source.Index].Target;
            else
            {
                // When moving whole worlds, we do not allow any references that aren't in the new world
                // to avoid any kind of accidental references
                return Entity.Null;
            }
        }

        public static Entity RemapEntityForPrefab(SparseEntityRemapInfo* remapping, int remappingCount, Entity source)
        {
            // When instantiating prefabs,
            // internal references are remapped.
            for (int i = 0; i != remappingCount; i++)
            {
                if (source == remapping[i].Src)
                    return remapping[i].Target;
            }
            // And external references are kept.
            return source;
        }

        public struct EntityPatchInfo
        {
            public int Offset;
            public int Stride;
        }

        public struct BufferEntityPatchInfo
        {
            // Offset within chunk where first buffer header can be found
            public int BufferOffset;
            // Stride between adjacent buffers that need patching
            public int BufferStride;
            // Offset (from base pointer of array) where entities live
            public int ElementOffset;
            // Stride between adjacent buffer elements
            public int ElementStride;
        }

        public struct ManagedEntityPatchInfo
        {
            // Type the managed component
            public ComponentType Type;
        }

#if NET_DOTS
        // @TODO TINY -- Need to use UnsafeArray to provide a view of the data in sEntityOffsetArray in the static type manager
        public static EntityOffsetInfo[] CalculateEntityOffsets<T>()
        {
            return null;
        }

#else
        public static EntityOffsetInfo[] CalculateEntityOffsets<T>()
        {
            return CalculateEntityOffsets(typeof(T));
        }

        public static bool HasEntityMembers(Type type)
        {
            if (type == typeof(Entity))
                return true;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType && !field.FieldType.IsPrimitive)
                {
                    if (HasEntityMembers(field.FieldType))
                        return true;
                }
            }

            return false;
        }

        public static EntityOffsetInfo[] CalculateEntityOffsets(Type type)
        {
            var offsets = new List<EntityOffsetInfo>();
            CalculateEntityOffsetsRecurse(ref offsets, type, 0);
            if (offsets.Count > 0)
                return offsets.ToArray();
            else
                return null;
        }

        static void CalculateEntityOffsetsRecurse(ref List<EntityOffsetInfo> offsets, Type type, int baseOffset)
        {
            if (type == typeof(Entity))
            {
                offsets.Add(new EntityOffsetInfo { Offset = baseOffset });
            }
            else
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fields)
                {
                    if (field.FieldType.IsValueType && !field.FieldType.IsPrimitive)
                        CalculateEntityOffsetsRecurse(ref offsets, field.FieldType, baseOffset + UnsafeUtility.GetFieldOffset(field));
                    else if(field.FieldType.IsClass && field.FieldType.IsGenericType)
                    {
                        foreach(var ga in field.FieldType.GetGenericArguments())
                        {
                            if (ga == typeof(Entity))
                            {
                                offsets.Add(new EntityOffsetInfo { Offset = baseOffset }); 
                                break;
                            }
                        }
                    }
                }
            }
        }

#endif

        public static EntityPatchInfo* AppendEntityPatches(EntityPatchInfo* patches, EntityOffsetInfo* offsets, int offsetCount, int baseOffset, int stride)
        {
            if (offsets == null)
                return patches;

            for (int i = 0; i < offsetCount; i++)
                patches[i] = new EntityPatchInfo { Offset = baseOffset + offsets[i].Offset, Stride = stride };
            return patches + offsetCount;
        }

        public static BufferEntityPatchInfo* AppendBufferEntityPatches(BufferEntityPatchInfo* patches, EntityOffsetInfo* offsets, int offsetCount, int bufferBaseOffset, int bufferStride, int elementStride)
        {
            if (offsets == null)
                return patches;

            for (int i = 0; i < offsetCount; i++)
            {
                patches[i] = new BufferEntityPatchInfo
                {
                    BufferOffset = bufferBaseOffset,
                    BufferStride = bufferStride,
                    ElementOffset = offsets[i].Offset,
                    ElementStride = elementStride,
                };
            }

            return patches + offsetCount;
        }

        public static ManagedEntityPatchInfo* AppendManagedEntityPatches(ManagedEntityPatchInfo* patches, ComponentType type)
        {
            patches[0] = new ManagedEntityPatchInfo
            {
                Type = type
            };

            return patches + 1;
        }

        public static void PatchEntities(EntityOffsetInfo[] scalarPatches, byte* chunkBuffer, ref NativeArray<EntityRemapInfo> remapping)
        {
            // Patch scalars (single components) with entity references.
            for (int i = 0; i < scalarPatches.Length; i++)
            {
                byte* entityData = chunkBuffer + scalarPatches[i].Offset;
                Entity* entity = (Entity*)entityData;
                *entity = RemapEntity(ref remapping, *entity);
            }
        }

        public static void PatchEntities(EntityPatchInfo* scalarPatches, int scalarPatchCount,
            BufferEntityPatchInfo* bufferPatches, int bufferPatchCount,
            byte* chunkBuffer, int entityCount, ref NativeArray<EntityRemapInfo> remapping)
        {
            // Patch scalars (single components) with entity references.
            for (int p = 0; p < scalarPatchCount; p++)
            {
                byte* entityData = chunkBuffer + scalarPatches[p].Offset;
                for (int i = 0; i != entityCount; i++)
                {
                    Entity* entity = (Entity*)entityData;
                    *entity = RemapEntity(ref remapping, *entity);
                    entityData += scalarPatches[p].Stride;
                }
            }

            // Patch buffers that contain entity references
            for (int p = 0; p < bufferPatchCount; ++p)
            {
                byte* bufferData = chunkBuffer + bufferPatches[p].BufferOffset;

                for (int i = 0; i != entityCount; ++i)
                {
                    BufferHeader* header = (BufferHeader*)bufferData;

                    byte* elemsBase = BufferHeader.GetElementPointer(header) + bufferPatches[p].ElementOffset;
                    int elemCount = header->Length;

                    for (int k = 0; k != elemCount; ++k)
                    {
                        Entity* entityPtr = (Entity*)elemsBase;
                        *entityPtr = RemapEntity(ref remapping, *entityPtr);
                        elemsBase += bufferPatches[p].ElementStride;
                    }

                    bufferData += bufferPatches[p].BufferStride;
                }
            }
        }

        public static void PatchEntitiesForPrefab(EntityPatchInfo* scalarPatches, int scalarPatchCount,
            BufferEntityPatchInfo* bufferPatches, int bufferPatchCount,
            byte* chunkBuffer, int indexInChunk, int entityCount, SparseEntityRemapInfo* remapping, int remappingCount)
        {
            // Patch scalars (single components) with entity references.
            for (int p = 0; p < scalarPatchCount; p++)
            {
                byte* entityData = chunkBuffer + scalarPatches[p].Offset;
                for (int e = 0; e != entityCount; e++)
                {
                    Entity* entity = (Entity*)(entityData + scalarPatches[p].Stride * (e + indexInChunk));
                    *entity = RemapEntityForPrefab(remapping + e * remappingCount, remappingCount, *entity);
                }
            }

            // Patch buffers that contain entity references
            for (int p = 0; p < bufferPatchCount; ++p)
            {
                byte* bufferData = chunkBuffer + bufferPatches[p].BufferOffset;

                for (int e = 0; e != entityCount; e++)
                {
                    BufferHeader* header = (BufferHeader*)(bufferData + bufferPatches[p].BufferStride * (e + indexInChunk));

                    byte* elemsBase = BufferHeader.GetElementPointer(header) + bufferPatches[p].ElementOffset;
                    int elemCount = header->Length;

                    for (int k = 0; k != elemCount; ++k)
                    {
                        Entity* entityPtr = (Entity*)elemsBase;
                        *entityPtr = RemapEntityForPrefab(remapping + e * remappingCount, remappingCount, *entityPtr);
                        elemsBase += bufferPatches[p].ElementStride;
                    }
                }
            }
        }

#if !NET_DOTS
        internal static void PatchEntityInBoxedType(object container, EntityRemapInfo* remapInfo)
        {
            var visitor = new EntityRemappingVisitor(remapInfo);
            var changeTracker = new ChangeTracker();
            var type = container.GetType();

            var resolved = PropertyBagResolver.Resolve(type);
            if (resolved != null)
            {
                resolved.Accept(ref container, ref visitor, ref changeTracker);
            }
            else
                throw new ArgumentException($"Type '{type.FullName}' not supported for visiting.");
        }

        internal static void PatchEntityForPrefabInBoxedType(object container, SparseEntityRemapInfo* remapInfo, int remapInfoCount)
        {
            var visitor = new EntityRemappingVisitor(remapInfo, remapInfoCount);
            var changeTracker = new ChangeTracker();
            var type = container.GetType();

            var resolved = PropertyBagResolver.Resolve(type);
            if (resolved != null)
            {
                resolved.Accept(ref container, ref visitor, ref changeTracker);
            }
            else
                throw new ArgumentException($"Type '{type.FullName}' not supported for visiting.");
        }

        internal unsafe class EntityRemappingVisitor : PropertyVisitor
        {
            protected EntityRemappingAdapter _EntityRemapAdapter { get; }
            protected EntityRemappingForPrefabAdapter _EntityRemapForPrefabAdapter { get; }

            public EntityRemappingVisitor(EntityRemapUtility.EntityRemapInfo* remapInfo)
            {
                _EntityRemapAdapter = new EntityRemappingAdapter(remapInfo);
                AddAdapter(_EntityRemapAdapter);
            }

            public EntityRemappingVisitor(EntityRemapUtility.SparseEntityRemapInfo* remapInfo, int remapInfoCount)
            {
                _EntityRemapForPrefabAdapter = new EntityRemappingForPrefabAdapter(remapInfo, remapInfoCount);
                AddAdapter(_EntityRemapForPrefabAdapter);
            }
        }

        internal unsafe class EntityRemappingAdapter : IPropertyVisitorAdapter
            , IVisitAdapter<Entity>
            , IVisitAdapter
        {
            protected EntityRemapUtility.EntityRemapInfo* RemapInfo { get; }

            unsafe public EntityRemappingAdapter(EntityRemapUtility.EntityRemapInfo* remapInfo)
            {
                RemapInfo = remapInfo;
            }

            unsafe public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref Entity value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, Entity>
            {
                value = EntityRemapUtility.RemapEntity(RemapInfo, value);
                return VisitStatus.Handled;
            }

            public VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, TValue>
            {
                return VisitStatus.Unhandled;
            }
        }

        internal unsafe class EntityRemappingForPrefabAdapter : IPropertyVisitorAdapter
            , IVisitAdapter<Entity>
            , IVisitAdapter
        {
            protected EntityRemapUtility.SparseEntityRemapInfo* RemapInfo { get; }
            protected int RemapInfoCount { get; }

            unsafe public EntityRemappingForPrefabAdapter(EntityRemapUtility.SparseEntityRemapInfo* remapInfo, int remapInfoCount)
            {
                RemapInfo = remapInfo;
                RemapInfoCount = remapInfoCount;
            }

            unsafe public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref Entity value, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, Entity>
            {
                value = EntityRemapUtility.RemapEntityForPrefab(RemapInfo, RemapInfoCount, value);
                return VisitStatus.Handled;
            }

            public VisitStatus Visit<TProperty, TContainer, TValue>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker) where TProperty : IProperty<TContainer, TValue>
            {
                return VisitStatus.Unhandled;
            }
        }
#endif
    }
}
