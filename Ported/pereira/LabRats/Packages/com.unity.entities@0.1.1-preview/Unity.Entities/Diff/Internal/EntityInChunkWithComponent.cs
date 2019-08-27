using System;
using System.Collections.Generic;

namespace Unity.Entities
{
    /// <summary>
    /// Handle to an entity within a chunk along with a component.
    /// </summary>
    internal struct EntityInChunkWithComponent<TComponent>
        where TComponent : unmanaged, IComponentData
    {
        public EntityInChunk EntityInChunk;
        public TComponent Component;
        public ChunkChangeFlags ChunkChangeFlags;
    }
    
    internal struct EntityInChunkWithComponentComparer<TComponent> : IComparer<EntityInChunkWithComponent<TComponent>> 
        where TComponent : unmanaged, IComponentData, IComparable<TComponent>
    {
        public int Compare(EntityInChunkWithComponent<TComponent> x, EntityInChunkWithComponent<TComponent> y)
        {
            return x.Component.CompareTo(y.Component);
        }
    }
}