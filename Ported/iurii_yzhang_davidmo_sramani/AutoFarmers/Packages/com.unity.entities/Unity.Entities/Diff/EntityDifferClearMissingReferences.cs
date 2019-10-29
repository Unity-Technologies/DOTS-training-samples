using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    static unsafe partial class EntityDiffer
    {
        [BurstCompile]
        struct ClearMissingReferencesJob : IJobParallelFor
        {
            [NativeDisableUnsafePtrRestriction] public TypeManager.TypeInfo* TypeInfo;
            [NativeDisableUnsafePtrRestriction] public TypeManager.EntityOffsetInfo* EntityOffsetInfo;
            [ReadOnly] public uint GlobalSystemVersion;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly, NativeDisableUnsafePtrRestriction] public EntityComponentStore* EntityComponentStore;

            public void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var archetype = chunk->Archetype;
                var typesCount = archetype->TypesCount;
                var entityCount = Chunks[index].Count;
                
                for (var typeIndexInArchetype = 1; typeIndexInArchetype < typesCount; typeIndexInArchetype++)
                {
                    var componentTypeInArchetype = archetype->Types[typeIndexInArchetype];
                    
                    if (!componentTypeInArchetype.HasEntityReferences || componentTypeInArchetype.IsSharedComponent || componentTypeInArchetype.IsZeroSized)
                    {
                        continue;
                    }
                    
                    var typeInfo = TypeInfo[componentTypeInArchetype.TypeIndex & TypeManager.ClearFlagsMask];
                    var typeInChunkPtr = GetChunkBuffer(chunk) + archetype->Offsets[typeIndexInArchetype];
                    var typeSizeOf = archetype->SizeOfs[typeIndexInArchetype];

                    var changed = false;

                    if (componentTypeInArchetype.IsBuffer)
                    {
                        for (var entityIndexInChunk = 0; entityIndexInChunk < entityCount; entityIndexInChunk++)
                        {
                            var componentDataPtr = typeInChunkPtr + typeSizeOf * entityIndexInChunk;
                            var bufferHeader = (BufferHeader*) componentDataPtr;
                            var bufferLength = bufferHeader->Length;
                            var bufferPtr = BufferHeader.GetElementPointer(bufferHeader);
                            changed |= ClearEntityReferences(typeInfo, bufferPtr, bufferLength);
                        }
                    }
                    else
                    {
                        for (var entityIndexInChunk = 0; entityIndexInChunk < entityCount; entityIndexInChunk++)
                        {
                            var componentDataPtr = typeInChunkPtr + typeSizeOf * entityIndexInChunk;
                            changed |= ClearEntityReferences(typeInfo, componentDataPtr, 1);
                        }
                    }

                    if (changed)
                    {
                        chunk->SetChangeVersion(typeIndexInArchetype, GlobalSystemVersion);
                    }
                }
            }

            bool ClearEntityReferences(TypeManager.TypeInfo typeInfo, byte* address, int elementCount)
            {
                var changed = false;

                var offsets = EntityOffsetInfo + typeInfo.EntityOffsetStartIndex;

                for (var elementIndex = 0; elementIndex < elementCount; elementIndex++)
                {
                    var elementPtr = address + typeInfo.ElementSize * elementIndex;

                    for (var offsetIndex = 0; offsetIndex < typeInfo.EntityOffsetCount; offsetIndex++)
                    {
                        var offset = offsets[offsetIndex].Offset;
                        
                        if (EntityComponentStore->Exists(*(Entity*) (elementPtr + offset)))
                            continue;

                        *(Entity*) (elementPtr + offset) = Entity.Null;
                        changed = true;
                    }
                }

                return changed;
            }
        }        
        
        static void ClearMissingReferences(EntityManager entityManager, NativeArray<ArchetypeChunk> chunks, out JobHandle jobHandle, JobHandle dependsOn)
        {
            jobHandle = new ClearMissingReferencesJob
            {
                TypeInfo = TypeManager.GetTypeInfoPointer(),
                EntityOffsetInfo = TypeManager.GetEntityOffsetsPointer(),
                GlobalSystemVersion = entityManager.GlobalSystemVersion,
                Chunks = chunks,
                EntityComponentStore = entityManager.EntityComponentStore,
            }.Schedule(chunks.Length, 64, dependsOn);
        }
    }
}