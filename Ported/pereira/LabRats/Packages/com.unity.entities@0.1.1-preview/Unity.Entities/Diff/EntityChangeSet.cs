using System;
using Unity.Collections;

namespace Unity.Entities
{
    public readonly struct EntityChanges : IDisposable
    {
        private readonly EntityChangeSet m_ForwardChangeSet;
        private readonly EntityChangeSet m_ReverseChangeSet;

        public EntityChanges(EntityChangeSet forwardChangeSet, EntityChangeSet reverseChangeSet)
        {
            m_ForwardChangeSet = forwardChangeSet;
            m_ReverseChangeSet = reverseChangeSet;
        }

        public bool AnyChanges => HasForwardChangeSet || HasReverseChangeSet;
        public bool HasForwardChangeSet => m_ForwardChangeSet.IsCreated && m_ForwardChangeSet.HasChanges;
        public bool HasReverseChangeSet => m_ReverseChangeSet.IsCreated && m_ReverseChangeSet.HasChanges;

        public EntityChangeSet ForwardChangeSet => m_ForwardChangeSet;
        public EntityChangeSet ReverseChangeSet => m_ReverseChangeSet;

        public void Dispose()
        {
            if (m_ForwardChangeSet.IsCreated)
            {
                m_ForwardChangeSet.Dispose();
            }

            if (m_ReverseChangeSet.IsCreated)
            {
                m_ReverseChangeSet.Dispose();
            }
        }
    }
    
    /// <summary>
    /// Represents a packed component within an <see cref="EntityChangeSet"/>
    /// </summary>
    public struct PackedComponent
    {
        /// <summary>
        /// Entity index in the packed entities array. <see cref="EntityChangeSet.Entities"/>
        /// </summary>
        public int PackedEntityIndex;

        /// <summary>
        /// Type index in the packed stableTypeHash array. <see cref="EntityChangeSet.TypeHashes"/>
        /// </summary>
        public int PackedTypeIndex;
    }
    
    /// <summary>
    /// Represents a packed component data change within a <see cref="EntityChangeSet"/>
    /// </summary>
    public struct PackedComponentDataChange
    {
        /// <summary>
        /// The entity and component this change is targeted.
        /// </summary>
        public PackedComponent Component;

        /// <summary>
        /// The start offset for this data change.
        /// </summary>
        /// <remarks>
        /// This is the field offset and NOT the payload offset.
        /// </remarks>
        public int Offset;
        
        /// <summary>
        /// The size of this data change. This is be the size in <see cref="EntityChangeSet.Payload"/> for this entry.
        /// </summary>
        public int Size;
    }
    
    /// <summary>
    /// Represents an entity reference that was changed within a <see cref="EntityChangeSet"/>
    ///
    /// This structure references the entity by it's unique <see cref="EntityGuid"/>.
    /// </summary>
    /// <remarks>
    /// Multiple patches could exist for the same component with different offsets.
    /// </remarks>
    public struct EntityReferenceChange
    {
        /// <summary>
        /// The entity and component this patched is targeted at.
        /// </summary>
        public PackedComponent Component;
        
        /// <summary>
        /// The entity that the field should reference. Identified by the unique <see cref="EntityGuid"/>.
        /// </summary>
        public EntityGuid Value;
        
        /// <summary>
        /// The field offset for the <see cref="Entity"/> field. 
        /// </summary>
        public int Offset;
    }
    
    public struct PackedSharedComponentDataChange
    {
        public PackedComponent Component;
        public object BoxedSharedValue;
    }
    
    public struct LinkedEntityGroupChange
    {
        public EntityGuid RootEntityGuid;
        public EntityGuid ChildEntityGuid;
    }

    [Flags]
    public enum ComponentTypeFlags
    {
        None = 0,
        ChunkComponent = 1 << 0
    }

    public struct ComponentTypeHash : IEquatable<ComponentTypeHash>
    {
        public ulong StableTypeHash;
        public ComponentTypeFlags Flags;

        public bool Equals(ComponentTypeHash other)
        {
            return StableTypeHash == other.StableTypeHash && Flags == other.Flags;
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentTypeHash other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StableTypeHash.GetHashCode() * 397) ^ (int) Flags;
            }
        }

        public static bool operator ==(ComponentTypeHash left, ComponentTypeHash right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ComponentTypeHash left, ComponentTypeHash right)
        {
            return !left.Equals(right);
        }
    }
    
    /// <summary>
    /// An atomic package of changes to entity and component data.
    /// </summary>
    public readonly struct EntityChangeSet : IDisposable
    {
        /// <summary>
        /// Number of entities from the start of <see cref="Entities"/> that should be considered as created.
        /// </summary>
        public readonly int CreatedEntityCount;
        
        /// <summary>
        /// Number of entities from the end of <see cref="Entities"/> that should be considered as destroyed.
        /// </summary>
        public readonly int DestroyedEntityCount;
        
        /// <summary>
        /// A packed array of all entities in this change-set.
        /// </summary>
        public readonly NativeArray<EntityGuid> Entities;
        
        /// <summary>
        /// A packed array of all types in this change-set.
        /// </summary>
        public readonly NativeArray<ComponentTypeHash> TypeHashes;
        
        /// <summary>
        /// Names for each entity in this change-set.
        /// </summary>
        public readonly NativeArray<NativeString64> Names;
        
        /// <summary>
        /// A set of all component additions in this change-set.
        /// </summary>
        public readonly NativeArray<PackedComponent> AddComponents;
        
        /// <summary>
        /// A set of all component removals in this change-set.
        /// </summary>
        public readonly NativeArray<PackedComponent> RemoveComponents;
        
        /// <summary>
        /// A set of all component data modifications in this change-set.
        /// </summary>
        public readonly NativeArray<PackedComponentDataChange> SetComponents;
        
        /// <summary>
        /// Data payload for all component changes specified in <see cref="SetComponents"/>
        /// </summary>
        /// <remarks>
        /// Data changes are tightly packed. Use the <see cref="PackedComponentDataChange.Size"/> to read back.
        /// </remarks>
        public readonly NativeArray<byte> Payload;
        
        /// <summary>
        /// A packed set of all entity references to patch.
        /// </summary>
        public readonly NativeArray<EntityReferenceChange> EntityPatches;

        /// <summary>
        /// A set of all shared component data changes.
        /// </summary>
        public readonly PackedSharedComponentDataChange[] SetSharedComponents;
        
        /// <summary>
        /// A set of all linked entity group additions.
        /// </summary>
        public readonly NativeArray<LinkedEntityGroupChange> LinkedEntityGroupAdditions;
        
        /// <summary>
        /// A set of all linked entity group removals.
        /// </summary>
        public readonly NativeArray<LinkedEntityGroupChange> LinkedEntityGroupRemovals;

        public EntityChangeSet(
            int createdEntityCount, 
            int destroyedEntityCount, 
            NativeArray<EntityGuid> entities, 
            NativeArray<ComponentTypeHash> typeHashes, 
            NativeArray<NativeString64> names,
            NativeArray<PackedComponent> addComponents, 
            NativeArray<PackedComponent> removeComponents, 
            NativeArray<PackedComponentDataChange> setComponents, 
            NativeArray<byte> payload, 
            NativeArray<EntityReferenceChange> entityPatches, 
            PackedSharedComponentDataChange[] setSharedComponents, 
            NativeArray<LinkedEntityGroupChange> linkedEntityGroupAdditions, 
            NativeArray<LinkedEntityGroupChange> linkedEntityGroupRemovals)
        {
            CreatedEntityCount = createdEntityCount;
            DestroyedEntityCount = destroyedEntityCount;
            Entities = entities;
            TypeHashes = typeHashes;
            Names = names;
            AddComponents = addComponents;
            RemoveComponents = removeComponents;
            SetComponents = setComponents;
            Payload = payload;
            EntityPatches = entityPatches;
            SetSharedComponents = setSharedComponents;
            LinkedEntityGroupAdditions = linkedEntityGroupAdditions;
            LinkedEntityGroupRemovals = linkedEntityGroupRemovals;
            IsCreated = true;
        }

        /// <summary>
        /// Returns true if this object is allocated.
        /// </summary>
        public bool IsCreated { get; }
        
        public bool HasChanges => 
            CreatedEntityCount != 0 || 
            DestroyedEntityCount != 0 || 
            AddComponents.Length != 0 || 
            RemoveComponents.Length != 0 ||
            SetComponents.Length != 0 || 
            EntityPatches.Length != 0 || 
            SetSharedComponents.Length != 0 || 
            LinkedEntityGroupAdditions.Length != 0 ||
            LinkedEntityGroupRemovals.Length != 0;
        
        public void Dispose()
        {
            if (!IsCreated)
            {
                return;
            }
            
            Entities.Dispose();
            TypeHashes.Dispose();
            Names.Dispose();
            AddComponents.Dispose();
            RemoveComponents.Dispose();
            SetComponents.Dispose();
            Payload.Dispose();
            EntityPatches.Dispose();
            LinkedEntityGroupAdditions.Dispose();
            LinkedEntityGroupRemovals.Dispose();
        }

        public EntityChangeSet Clone(Allocator allocator)
        {
            return new EntityChangeSet
            (
                CreatedEntityCount,
                DestroyedEntityCount,
                new NativeArray<EntityGuid>(Entities, allocator),
                new NativeArray<ComponentTypeHash>(TypeHashes, allocator),
                new NativeArray<NativeString64>(Names, allocator),
                new NativeArray<PackedComponent>(AddComponents, allocator),
                new NativeArray<PackedComponent>(RemoveComponents, allocator),
                new NativeArray<PackedComponentDataChange>(SetComponents, allocator),
                new NativeArray<byte>(Payload, allocator),
                new NativeArray<EntityReferenceChange>(EntityPatches, allocator),
                SetSharedComponents,
                new NativeArray<LinkedEntityGroupChange>(LinkedEntityGroupAdditions, allocator),
                new NativeArray<LinkedEntityGroupChange>(LinkedEntityGroupRemovals, allocator)
            );
        }
    }
}