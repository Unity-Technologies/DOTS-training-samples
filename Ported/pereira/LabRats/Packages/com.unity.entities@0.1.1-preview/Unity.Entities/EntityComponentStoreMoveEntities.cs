namespace Unity.Entities
{
    internal unsafe partial struct EntityComponentStore
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------
        
        public void AddExistingChunk(Chunk* chunk, int* sharedComponentIndices)
        {
            var archetype = chunk->Archetype;
            archetype->AddToChunkList(chunk, sharedComponentIndices, GlobalSystemVersion);
            archetype->EntityCount += chunk->Count;

            for (var i = 0; i < archetype->NumSharedComponents; ++i)
                ManagedChangesTracker.AddReference(sharedComponentIndices[i]);

            if (chunk->Count < chunk->Capacity)
                archetype->EmptySlotTrackingAddChunk(chunk);

            AddExistingEntitiesInChunk(chunk);
        }
        
    }
}