using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Entities
{
    internal unsafe partial struct EntityComponentStore
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        public Archetype* GetOrCreateArchetype(ComponentTypeInArchetype* inTypesSorted, int count)
        {
            var srcArchetype = GetExistingArchetype(inTypesSorted, count);
            if (srcArchetype != null)
                return srcArchetype;

            srcArchetype = CreateArchetype(inTypesSorted, count);

            var types = stackalloc ComponentTypeInArchetype[count + 1];

            // Setup Instantiable archetype
            {
                UnsafeUtility.MemCpy(types, inTypesSorted, sizeof(ComponentTypeInArchetype) * count);

                var hasCleanup = false;
                var removedTypes = 0;
                for (var t = 0; t < srcArchetype->TypesCount; ++t)
                {
                    var type = srcArchetype->Types[t];

                    hasCleanup |= type.TypeIndex == m_CleanupEntityType;

                    var skip = type.IsSystemStateComponent || type.TypeIndex == m_PrefabType;
                    if (skip)
                        ++removedTypes;
                    else
                        types[t - removedTypes] = srcArchetype->Types[t];
                }

                // Entity has already been destroyed, so it shouldn't be instantiated anymore
                if (hasCleanup)
                {
                    srcArchetype->InstantiableArchetype = null;
                }
                else if (removedTypes > 0)
                {
                    var instantiableArchetype = GetOrCreateArchetype(types, count - removedTypes);

                    srcArchetype->InstantiableArchetype = instantiableArchetype;
                    Assert.IsTrue(instantiableArchetype->InstantiableArchetype == instantiableArchetype);
                    Assert.IsTrue(instantiableArchetype->SystemStateResidueArchetype == null);
                }
                else
                {
                    srcArchetype->InstantiableArchetype = srcArchetype;
                }
            }


            // Setup System state cleanup archetype
            if (srcArchetype->SystemStateCleanupNeeded)
            {
                var cleanupEntityType = new ComponentTypeInArchetype(ComponentType.FromTypeIndex(m_CleanupEntityType));
                bool cleanupAdded = false;

                types[0] = inTypesSorted[0];
                var newTypeCount = 1;

                for (var t = 1; t < srcArchetype->TypesCount; ++t)
                {
                    var type = srcArchetype->Types[t];

                    if (type.IsSystemStateComponent)
                    {
                        if (!cleanupAdded && (cleanupEntityType < srcArchetype->Types[t]))
                        {
                            types[newTypeCount++] = cleanupEntityType;
                            cleanupAdded = true;
                        }

                        types[newTypeCount++] = srcArchetype->Types[t];
                    }
                }

                if (!cleanupAdded)
                {
                    types[newTypeCount++] = cleanupEntityType;
                }

                var systemStateResidueArchetype = GetOrCreateArchetype(types, newTypeCount);
                srcArchetype->SystemStateResidueArchetype = systemStateResidueArchetype;

                Assert.IsTrue(systemStateResidueArchetype->SystemStateResidueArchetype == systemStateResidueArchetype);
                Assert.IsTrue(systemStateResidueArchetype->InstantiableArchetype == null);
            }

            // Setup meta chunk archetype
            if (count > 1)
            {
                types[0] = new ComponentTypeInArchetype(m_EntityComponentType);
                int metaArchetypeTypeCount = 1;
                for (int i = 1; i < count; ++i)
                {
                    var t = inTypesSorted[i];
                    ComponentType typeToInsert;
                    if (inTypesSorted[i].IsChunkComponent)
                    {
                        typeToInsert = new ComponentType
                        {
                            TypeIndex = ChunkComponentToNormalTypeIndex(t.TypeIndex)
                        };
                        SortingUtilities.InsertSorted(types, metaArchetypeTypeCount++, typeToInsert);
                    }
                }

                if (metaArchetypeTypeCount > 1)
                {
                    SortingUtilities.InsertSorted(types, metaArchetypeTypeCount++, m_ChunkHeaderComponentType);
                    srcArchetype->MetaChunkArchetype = GetOrCreateArchetype(types, metaArchetypeTypeCount);
                }
            }

            return srcArchetype;
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        struct ArchetypeChunkFilter
        {
            public Archetype* Archetype;
            public fixed int SharedComponentValues[kMaxSharedComponentCount];

            public ArchetypeChunkFilter(Archetype* archetype, int* sharedComponentValues)
            {
                Archetype = archetype;
                for (int i = 0; i < archetype->NumSharedComponents; i++)
                    SharedComponentValues[i] = sharedComponentValues[i];
            }
            
            public ArchetypeChunkFilter(Archetype* archetype, SharedComponentValues sharedComponentValues)
            {
                Archetype = archetype;
                for (int i = 0; i < archetype->NumSharedComponents; i++)
                    SharedComponentValues[i] = sharedComponentValues[i];
            }
        }

        Chunk* GetChunkWithEmptySlotsWithAddedComponent(Entity entity, ComponentType componentType)
        {
            if (!Exists(entity))
                return null;
            
            return GetChunkWithEmptySlotsWithAddedComponent(GetChunk(entity), componentType);
        }
        
        Chunk* GetChunkWithEmptySlotsWithAddedComponent(Chunk* srcChunk, ComponentType componentType, int sharedComponentIndex=0)
        {
            var archetypeChunkFilter = GetArchetypeChunkFilterWithAddedComponent(srcChunk, componentType, sharedComponentIndex);
            if (archetypeChunkFilter.Archetype == null)
                return null;
            
            return GetChunkWithEmptySlots(ref archetypeChunkFilter);
        }
        
        Chunk* GetChunkWithEmptySlotsWithRemovedComponent(Entity entity, ComponentType componentType)
        {
            if (!Exists(entity))
                return null;
            
            return GetChunkWithEmptySlotsWithRemovedComponent(GetChunk(entity), componentType);
        }
        
        Chunk* GetChunkWithEmptySlotsWithRemovedComponent(Chunk* srcChunk, ComponentType componentType)
        {
            var archetypeChunkFilter = GetArchetypeChunkFilterWithRemovedComponent(srcChunk, componentType);
            if (archetypeChunkFilter.Archetype == null)
                return null;
            
            return GetChunkWithEmptySlots(ref archetypeChunkFilter);
        }

        Chunk* GetChunkWithEmptySlots(ref ArchetypeChunkFilter archetypeChunkFilter)
        {
            var archetype = archetypeChunkFilter.Archetype;
            fixed (int* sharedComponentValues = archetypeChunkFilter.SharedComponentValues)
            {
                var chunk = archetype->GetExistingChunkWithEmptySlots(sharedComponentValues);
                if (chunk == null)
                    chunk = GetCleanChunk(archetype, sharedComponentValues);
                return chunk;
            }
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithChangedArchetype(Chunk* srcChunk, Archetype* dstArchetype)
        {
            var srcArchetype = srcChunk->Archetype;

            var archetypeChunkFilter = new ArchetypeChunkFilter();
            archetypeChunkFilter.Archetype = dstArchetype;
            BuildSharedComponentIndicesWithChangedArchetype(srcArchetype, dstArchetype, srcChunk->SharedComponentValues, archetypeChunkFilter.SharedComponentValues);
            return archetypeChunkFilter;
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithChangedSharedComponent(Chunk* srcChunk, ComponentType componentType, int dstSharedComponentIndex)
        {
            var typeIndex = componentType.TypeIndex;
            var srcArchetype = srcChunk->Archetype;
            var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(srcArchetype, typeIndex);
            
            var srcSharedComponentValueArray = srcChunk->SharedComponentValues;
            var sharedComponentOffset = indexInTypeArray - srcArchetype->FirstSharedComponent;
            var srcSharedComponentIndex = srcSharedComponentValueArray[sharedComponentOffset];

            if (dstSharedComponentIndex == srcSharedComponentIndex)
                return default;

            var archetypeChunkFilter = new ArchetypeChunkFilter();
            archetypeChunkFilter.Archetype = srcArchetype;
            srcSharedComponentValueArray.CopyTo(archetypeChunkFilter.SharedComponentValues, 0, srcArchetype->NumSharedComponents);
            archetypeChunkFilter.SharedComponentValues[sharedComponentOffset] = dstSharedComponentIndex;

            return archetypeChunkFilter;
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithAddedComponent(Chunk* srcChunk, Archetype* dstArchetype, int indexInTypeArray, ComponentType componentType, int sharedComponentIndex)
        {
            var srcArchetype = srcChunk->Archetype;
            var archetypeChunkFilter = new ArchetypeChunkFilter();
            archetypeChunkFilter.Archetype = dstArchetype;
            if (componentType.IsSharedComponent)
            {
                int indexOfNewSharedComponent = indexInTypeArray - dstArchetype->FirstSharedComponent;
                BuildSharedComponentIndicesWithAddedComponent(indexOfNewSharedComponent, sharedComponentIndex, dstArchetype->NumSharedComponents, srcChunk->SharedComponentValues, archetypeChunkFilter.SharedComponentValues);
            }
            else
            {
                for (int i = 0; i < srcArchetype->NumSharedComponents; i++)
                    archetypeChunkFilter.SharedComponentValues[i] = srcChunk->SharedComponentValues[i];
            }

            return archetypeChunkFilter;
        }
        
        ArchetypeChunkFilter GetArchetypeChunkFilterWithAddedComponent(Chunk* srcChunk, ComponentType componentType, int sharedComponentIndex)
        {
            var srcArchetype = srcChunk->Archetype;
            int indexInTypeArray = 0;
            var dstArchetype = GetArchetypeWithAddedComponent(srcArchetype, componentType, &indexInTypeArray);
            if (dstArchetype == null)
            {
                Assert.IsTrue(sharedComponentIndex == 0);
                return default;
            }

            Assert.IsTrue(dstArchetype->NumSharedComponents <= kMaxSharedComponentCount);

            return GetArchetypeChunkFilterWithAddedComponent(srcChunk, dstArchetype, indexInTypeArray, componentType, sharedComponentIndex);
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithAddedComponents(Chunk* srcChunk, ComponentTypes componentTypes)
        {
            var srcArchetype = srcChunk->Archetype;
            var srcTypes = srcArchetype->Types;
            var dstTypesCount = srcArchetype->TypesCount + componentTypes.Length;
            
            ComponentTypeInArchetype* dstTypes = stackalloc ComponentTypeInArchetype[dstTypesCount];

            var indexOfNewTypeInNewArchetype = stackalloc int[componentTypes.Length];

            // zipper the two sorted arrays "type" and "componentTypeInArchetype" into "componentTypeInArchetype"
            // because this is done in-place, it must be done backwards so as not to disturb the existing contents.

            var unusedIndices = 0;
            {
                var oldThings = srcArchetype->TypesCount;
                var newThings = componentTypes.Length;
                var mixedThings = oldThings + newThings;
                while (oldThings > 0 && newThings > 0) // while both are still zippering,
                {
                    var oldThing = srcTypes[oldThings - 1];
                    var newThing = componentTypes.GetComponentType(newThings - 1);
                    if (oldThing.TypeIndex > newThing.TypeIndex) // put whichever is bigger at the end of the array
                    {
                        dstTypes[--mixedThings] = oldThing;
                        --oldThings;
                    }
                    else
                    {
                        if (oldThing.TypeIndex == newThing.TypeIndex)
                            --oldThings;

                        var componentTypeInArchetype = new ComponentTypeInArchetype(newThing);
                        dstTypes[--mixedThings] = componentTypeInArchetype;
                        --newThings;
                        indexOfNewTypeInNewArchetype[newThings] = mixedThings; // "this new thing ended up HERE"
                    }
                }

                Assert.AreEqual(0, newThings); // must not be any new things to copy remaining, oldThings contain entity

                while (oldThings > 0) // if there are remaining old things, copy them here
                {
                    dstTypes[--mixedThings] = srcTypes[--oldThings];
                }

                unusedIndices = mixedThings; // In case we ignored duplicated types, this will be > 0
            }

            var dstArchetype = GetOrCreateArchetype(dstTypes + unusedIndices, dstTypesCount);
            var archetypeChunkFilter = new ArchetypeChunkFilter();
            archetypeChunkFilter.Archetype = dstArchetype;
            
            if (dstArchetype->NumSharedComponents > srcArchetype->NumSharedComponents)
            {
                BuildSharedComponentIndicesWithAddedComponents(srcArchetype, dstArchetype, srcChunk->SharedComponentValues, archetypeChunkFilter.SharedComponentValues);
            }
            else
            {
                for (int i = 0; i < srcArchetype->NumSharedComponents; i++)
                    archetypeChunkFilter.SharedComponentValues[i] = srcChunk->SharedComponentValues[i];
            }

            return archetypeChunkFilter;
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithRemovedComponent(Chunk* srcChunk, Archetype* dstArchetype, int indexInTypeArray, ComponentType componentType)
        {
            var srcArchetype = srcChunk->Archetype;
            var archetypeChunkFilter = new ArchetypeChunkFilter();
            archetypeChunkFilter.Archetype = dstArchetype;
            if (componentType.IsSharedComponent)
            {
                int indexOfRemovedSharedComponent = indexInTypeArray - srcArchetype->FirstSharedComponent;
                BuildSharedComponentIndicesWithRemovedComponent(indexOfRemovedSharedComponent, dstArchetype->NumSharedComponents, srcChunk->SharedComponentValues, archetypeChunkFilter.SharedComponentValues);
            }
            else
            {
                for (int i = 0; i < srcArchetype->NumSharedComponents; i++)
                    archetypeChunkFilter.SharedComponentValues[i] = srcChunk->SharedComponentValues[i];
            }

            return archetypeChunkFilter;
        }

        ArchetypeChunkFilter GetArchetypeChunkFilterWithRemovedComponent(Chunk* srcChunk, ComponentType componentType)
        {
            var srcArchetype = srcChunk->Archetype;
            int indexInTypeArray = 0;
            var dstArchetype = GetArchetypeWithRemovedComponent(srcArchetype, componentType, &indexInTypeArray);
            if (dstArchetype == srcArchetype)
                return default;

            return GetArchetypeChunkFilterWithRemovedComponent(srcChunk, dstArchetype, indexInTypeArray, componentType);
        }
        
        static void BuildSharedComponentIndicesWithAddedComponent(int indexOfNewSharedComponent, int value,
            int newCount, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            Assert.IsTrue(newCount <= kMaxSharedComponentCount);
            
            srcSharedComponentValues.CopyTo(dstSharedComponentValues, 0, indexOfNewSharedComponent);
            dstSharedComponentValues[indexOfNewSharedComponent] = value;
            srcSharedComponentValues.CopyTo(dstSharedComponentValues + indexOfNewSharedComponent + 1,
                indexOfNewSharedComponent, newCount - indexOfNewSharedComponent - 1);
        }

        static void BuildSharedComponentIndicesWithRemovedComponent(int indexOfRemovedSharedComponent,
            int newCount, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            srcSharedComponentValues.CopyTo(dstSharedComponentValues, 0, indexOfRemovedSharedComponent);
            srcSharedComponentValues.CopyTo(dstSharedComponentValues + indexOfRemovedSharedComponent,
                indexOfRemovedSharedComponent + 1, newCount - indexOfRemovedSharedComponent);
        }

        static void BuildSharedComponentIndicesWithAddedComponents(Archetype* srcArchetype,
            Archetype* dstArchetype, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            int oldFirstShared = srcArchetype->FirstSharedComponent;
            int newFirstShared = dstArchetype->FirstSharedComponent;
            int oldCount = srcArchetype->NumSharedComponents;
            int newCount = dstArchetype->NumSharedComponents;

            for (int oldIndex = oldCount - 1, newIndex = newCount - 1; newIndex >= 0; --newIndex)
            {
                // oldIndex might become -1 which is ok since oldFirstShared is always at least 1. The comparison will then always be false
                if (dstArchetype->Types[newIndex + newFirstShared] == srcArchetype->Types[oldIndex + oldFirstShared])
                    dstSharedComponentValues[newIndex] = srcSharedComponentValues[oldIndex--];
                else
                    dstSharedComponentValues[newIndex] = 0;
            }
        }

        static void BuildSharedComponentIndicesWithChangedArchetype(Archetype* srcArchetype,
            Archetype* dstArchetype, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            Assert.IsTrue(dstArchetype->NumSharedComponents <= kMaxSharedComponentCount);

            int oldFirstShared = srcArchetype->FirstSharedComponent;
            int newFirstShared = dstArchetype->FirstSharedComponent;
            int oldCount = srcArchetype->NumSharedComponents;
            int newCount = dstArchetype->NumSharedComponents;

            int o = 0;
            int n = 0;
            
            for (; n < newCount && o < oldCount;)
            {
                int srcType = srcArchetype->Types[o + oldFirstShared].TypeIndex;
                int dstType = dstArchetype->Types[n + newFirstShared].TypeIndex;
                if (srcType == dstType)
                    dstSharedComponentValues[n++] = srcSharedComponentValues[o++];
                else if (dstType > srcType)
                    o++;
                else
                    dstSharedComponentValues[n++] = 0;
            }
            
            for (;n < newCount;n++)
                dstSharedComponentValues[n] = 0;
        }

        Archetype* GetArchetypeWithAddedComponent(Archetype* archetype, ComponentType addedComponentType, int* indexInTypeArray = null)
        {
            var componentType = new ComponentTypeInArchetype(addedComponentType);
            ComponentTypeInArchetype* newTypes = stackalloc ComponentTypeInArchetype[archetype->TypesCount + 1];

            var t = 0;
            while (t < archetype->TypesCount && archetype->Types[t] < componentType)
            {
                newTypes[t] = archetype->Types[t];
                ++t;
            }

            if (indexInTypeArray != null)
                *indexInTypeArray = t;

            if (archetype->Types[t] == componentType)
            {
                // Tag component type is already there, no new archetype required.
                return null;
            }

            newTypes[t] = componentType;
            while (t < archetype->TypesCount)
            {
                newTypes[t + 1] = archetype->Types[t];
                ++t;
            }

            return GetOrCreateArchetype(newTypes, archetype->TypesCount + 1);
        }

        Archetype* GetArchetypeWithRemovedComponent(Archetype* archetype, ComponentType addedComponentType, int* indexInOldTypeArray = null)
        {
            var componentType = new ComponentTypeInArchetype(addedComponentType);
            ComponentTypeInArchetype* newTypes = stackalloc ComponentTypeInArchetype[archetype->TypesCount];

            var removedTypes = 0;
            for (var t = 0; t < archetype->TypesCount; ++t)
                if (archetype->Types[t].TypeIndex == componentType.TypeIndex)
                {
                    if (indexInOldTypeArray != null)
                        *indexInOldTypeArray = t;
                    ++removedTypes;
                }
                else
                    newTypes[t - removedTypes] = archetype->Types[t];

            return GetOrCreateArchetype(newTypes, archetype->TypesCount - removedTypes);
        }
    }
}