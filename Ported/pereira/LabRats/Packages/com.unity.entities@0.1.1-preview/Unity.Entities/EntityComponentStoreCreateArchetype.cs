using System;
using Unity.Assertions;
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

        Archetype* GetArchetypeWithAddedComponentType(Archetype* archetype, ComponentType addedComponentType,
            int* indexInTypeArray = null)
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
                Assert.IsTrue(addedComponentType.IgnoreDuplicateAdd,
                    $"{addedComponentType} is already part of the archetype.");
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

        Archetype* GetArchetypeWithRemovedComponentType(Archetype* archetype, ComponentType addedComponentType,
            int* indexInOldTypeArray = null)
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