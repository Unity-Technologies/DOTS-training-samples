using System;

namespace Unity.Entities
{
    /// <summary>
    /// An interface for implementing general-purpose components.
    /// </summary>
    /// <remarks>
    /// An IComponentData implementation must be a struct and can only contain unmanaged, blittable types, including:
    ///
    /// * C#-defined [blittable types](https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types)
    /// * bool
    /// * char
    /// * <see cref="NativeString"/> (a fixed-sized character buffer)
    /// * <see cref="BlobAssetReference{T}"/> (a reference to a Blob data structure)
    /// * [fixed arrays](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/fixed-statement) (in
    ///   an [unsafe](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/unsafe) context)
    /// * structs containing these unmanaged, blittable fields
    ///
    /// Note that you can also use a separate, <see cref="IBufferElementData"/> component in a
    /// <see cref="DynamicBuffer{T}"/> as an array-like data structure.
    ///
    /// A single IComponentData implementation should only contain fields for data that is always, or almost always,
    /// accessed at the same time. In general, using a greater number of smaller component types is more
    /// efficient than using fewer, larger component types.
    ///
    /// Add, set, and remove the components of an entity using the <see cref="EntityManager"/> or an
    /// <see cref="EntityCommandBuffer"/>. (You can also update the fields of an IComponentData struct normally when you
    /// have a reference to it.)
    ///
    /// IComponentData objects are stored in chunks (<see cref="ArchetypeChunk"/>), indexed by <see cref="Entity"/>. You
    /// can implement systems (<see cref="ComponentSystemBase"/>) to select and iterate over a set of entities having
    /// specific components. Use <see cref="EntityQueryBuilder"/> with <see cref="ComponentSystem"/> for non-Job based
    /// systems. Use <see cref="EntityQuery"/> with <see cref="JobComponentSystem"/> for <see cref="IJobForEach{T0}"/>
    /// and <see cref="IJobChunk"/> based systems. All the components of an entity must fit into a single chunk and
    /// thus cannot exceed 16 KB. (Some components, such as <see cref="DynamicBuffer{T}"/> and
    /// <see cref="BlobArray{T}"/> can store data outside the chunk, so may not fully count against that limit.)
    ///
    /// While, most of the components that you add to entities implement IComponentData, ECS also provides several,
    /// specialized component types. These specialized types include:
    ///
    /// * <see cref="IBufferElementData"/> -- for use in a <see cref="DynamicBuffer{T}"/>
    /// * <see cref="ISharedComponentData"/> -- a component whose value is shared by all entities in the same chunk
    /// * <see cref="ISystemStateComponentData"/> -- a component for storing internal system state associated with an entity.
    /// * <see cref="ISystemStateSharedComponentData"/> -- the system state version of the shared component interface.
    /// * <see cref="ISystemStateBufferElementData"/> -- the system state version of the buffer element interface.
    ///
    /// *Note:* Chunk components, which you can use to store data associated with a chunk
    /// (see <see cref="EntityManager.AddChunkComponentData{T}(Entity)"/>) and singleton components, which are
    /// components for which only one instance of a type is allowed (see
    /// <see cref="ComponentSystemBase.SetSingleton{T}"/>), use the IComponentData interface.
    ///
    /// See [General-purpose components](xref:ecs-component-data) for additional information.
    /// </remarks>
    public interface IComponentData
    {
    }

    /// <summary>
    /// An interface for creating structs that can be stored in a <see cref="DynamicBuffer{T}"/>.
    /// </summary>
    /// <remarks>IBufferElementData implementations are subject to the same constraints as
    /// <see cref="IComponentData"/>.
    ///
    /// Create a <see cref="DynamicBuffer{T}"/> containing a given type `T` by adding that IBufferElementData type to
    /// an entity. The DynamicBuffer container is created automatically. You can specify the maximum number of elements a buffer
    /// stores inside a chunk by placing an <see cref="InternalBufferCapacityAttribute"/> on the IBufferElementData
    /// declaration. When the number of elements exceeds the internal capacity, the entire is moved outside the chunk
    /// into heap memory. (In either case, you access an element the same way through the dynamic buffer API.)
    ///
    /// To remove a buffer from an entity, remove that entity's IBufferElementData component. (To remove an individual
    /// element from a buffer, call <see cref="DynamicBuffer{T}.RemoveAt(Int32)"/>.)
    ///
    /// You can find entities with a particular type of buffer using either <see cref="EntityQuery"/> or
    /// <see cref="EntityQueryBuilder"/> in the same way you select entities with specific types of <see cref="IComponentData"/>.
    /// Use the IBufferElementData type in the query (not DynamicBuffer).
    ///
    /// To access the buffer of an entity in a <see cref="ComponentSystem"/>, use <see cref="EntityManager.GetBuffer{T}(Entity)"/>,
    /// where `T` is the IBufferElementData subtype.
    ///
    /// To access the buffer of an entity in a <see cref="JobComponentSystem"/>, define a field of type,
    /// <see cref="JobComponentSystem.GetBufferFromEntity{T}"/>, as part of the Job struct. Set the field value when you
    /// schedule the Job with <see cref="EntityManager.GetBufferFromEntity{T}"/>.
    ///
    /// The DynamicBuffer interface provides array-like access to buffer contents. You can treat a buffer like a
    /// [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html). You can also use
    /// <see cref="DynamicBuffer{T}.Reinterpret{T}"/> to treat the buffer as a container of the underlying type, rather
    /// than a container of IBufferElementData.
    ///
    /// See [Dynamic Buffers](xref:ecs-dynamic-buffers) for additional information.
    /// </remarks>
    public interface IBufferElementData
    {
    }

    /// <summary>
    /// Specifies the maximum number of elements to store inside a chunk.
    /// </summary>
    /// <remarks>
    /// Use this attribute on the declaration of your IBufferElementData subtype:
    ///
    /// <code>
    /// [InternalBufferCapacity(10)]
    /// public struct FloatBufferElement : IBufferElementData
    /// {
    ///     public float Value;
    /// }
    /// </code>
    ///
    /// All <see cref="DynamicBuffer{T}"/> with this type of element store the specified number of elements inside the
    /// chunk along with other component types in the same archetype. When the number of elements in the buffer exceeds
    /// this limit, the entire buffer is moved outside the chunk.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Struct)]
    public class InternalBufferCapacityAttribute : Attribute
    {
        /// <summary>
        /// The number of elements stored inside the chunk.
        /// </summary>
        public readonly int Capacity;

        /// <summary>
        /// The number of elements stored inside the chunk.
        /// </summary>
        /// <param name="capacity"></param>
        public InternalBufferCapacityAttribute(int capacity)
        {
            Capacity = capacity;
        }
    }
    
    /// <summary>
    /// Specifies the maximum number of components of a type that can be stored in the same chunk.
    /// </summary>
    /// <remarks>Place this attribute on the declaration of a component, such as <see cref="IComponentData"/>, to
    /// limit the number of entities with that component which can be stored in a single chunk. Note that the actual
    /// limit on the number of entities in a chunk can be smaller, based on the actual size of all the components in the
    /// same <see cref="EntityArchetype"/> as the component defining this limit.
    ///
    /// If an archetype contains more than one component type specifying a chunk capacity limit, then the lowest limit
    /// is used.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MaximumChunkCapacityAttribute : Attribute
    {
        /// <summary>
        /// The maximum number of entities having this component type in an <see cref="ArchetypeChunk"/>.
        /// </summary>
        public readonly int Capacity;

        /// <summary>
        /// The maximum number of entities having this component type in an <see cref="ArchetypeChunk"/>.
        /// </summary>
        /// <param name="capacity"></param>
        public MaximumChunkCapacityAttribute(int capacity)
        {
            Capacity = capacity;
        }
        
    }

    /// <summary>
    /// States that a component type is serializable.
    /// </summary>
    /// <remarks>
    /// By default, ECS does not support storing pointer types in chunks. Apply this attribute to a component declaration
    /// to allow the use of pointers as fields in the component.
    ///
    /// Note that ECS does not perform any pre- or post-serialization processing to maintain pointer validity. When
    /// using this attribute, your code assumes responsibility for handling pointer serialization and deserialization.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ChunkSerializableAttribute : Attribute
    {
    }

    // [TODO: Document shared components with Jobs...]
    /// <summary>
    /// An interface for a component type whose value is shared by all entities in the same chunk.
    /// </summary>
    /// <remarks>ISharedComponentData implementations are subject to the same constraints as
    /// <see cref="IComponentData"/>.
    ///
    /// ISharedComponent implementations must implement <see cref="IEquatable{T}"/> and <see cref="Object.GetHashCode"/>.
    ///
    /// *Note:* Currently, the ISharedComponentData interface allows fields having reference types. However, we plan to
    /// restrict ISharedComponentData to unmanaged, blittable types only in a future version of the Entities package.
    ///
    /// When you add a shared component to an <see cref="EntityArchetype"/>, ECS stores entities assigned the same
    /// values of that shared component in the same chunks. Thus, shared components further categorize entities within
    /// the same archetype. Use shared components when many entities share the same data values and it is more efficient
    /// to process all the entities of a given value together. For example, the `RenderMesh` shared component (in the
    /// Hybrid.Rendering package) defines a set of fields whose values can be shared by many 3D objects. Since all the
    /// entities with the same values for the RenderMesh fields are stored in the same chunks, the renderer can
    /// efficiently batch the draw calls for those entities based on the shared values.
    ///
    /// You must set the value of a shared component on the main thread using either the <see cref="EntityManager"/>
    /// or an <see cref="EntityCommandBuffer"/>. When you change a shared component value, the affected entity is
    /// moved to a different chunk. If a chunk already exists with the same values, and has enough room, the entity is moved
    /// to that chunk. Otherwise, a new chunk is allocated. Changing a shared component value is a structural change that
    /// potentially creates a sync-point in your application.
    ///
    /// You can find entities with a particular type of shared component using either <see cref="EntityQuery"/> or
    /// <see cref="EntityQueryBuilder"/> in the same way you select entities with specific types of <see cref="IComponentData"/>.
    /// You can also filter an entity query to select only entities with a specific shared component value using
    /// <see cref="EntityQuery.SetFilter{SharedComponent1}"/>. You can filter based on two different shared components.
    /// (EntityQueryBuilder does not support filtering queries by shared component value.)
    ///
    /// Avoid too many shared components and values on the same archetype. Since each combination of values, whether in the
    /// same component type or in different shared components, is stored in different chunks, too many combinations can
    /// lead to poor chunk utilization. Use the Entity Debugger window in the Unity Editor
    /// (menu: *Window* > *Analysis* > *Entity Debugger*) to monitor chunk utilization.
    ///
    /// See [Shared Component Data](xref:ecs-shared-component-data) for additional information.
    /// </remarks>
    public interface ISharedComponentData
    {
    }

    /// <summary>
    /// An interface for a component type that stores system-specific data.
    /// </summary>
    /// <remarks>
    /// ISystemStateComponentData implementations are subject to the same constraints as
    /// <see cref="IComponentData"/>: they can only contain
    /// [blittable](https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types) data
    /// types.
    ///
    /// System state components are specialized components designed to allow systems to store their own stateful data on
    /// an entity. The functional difference between a general-purpose component and a system state component is that the
    /// presence of a system state component delays entity destruction until the system explicitly removes the component.
    /// This delay allows a system to cleanup any state or persistent resources it has created and associated with an entity.
    ///
    /// The typical pattern for using a system state component is for the system to find new entities by querying for
    /// entities with specific archetype, that do not have the component. The system can add a system state component to
    /// the entity and then set state values or create resources for the new entity. A system can then detect entity
    /// destruction by querying for entities that have the system state component, but not the other components in the
    /// original archetype. The system must then cleanup any state or resources and then remove the system state
    /// component. The ECS code only fully deletes the entity after the system removes the system state component.
    ///
    /// See [System State Components](xref:ecs-system-state-components) for additional information.
    /// </remarks>
    public interface ISystemStateComponentData : IComponentData
    {
    }

    /// <summary>
    /// An interface for a component type that stores system-specific data in a buffer.
    /// </summary>
    /// <seealso cref="ISystemStateComponentData"/>
    /// <seealso cref="IBufferElementData"/>
    public interface ISystemStateBufferElementData : IBufferElementData
    {
    }

    /// <summary>
    /// An interface for a component type that stores shared system-specific data.
    /// </summary>
    /// <seealso cref="ISystemStateComponentData"/>
    /// <seealso cref="ISharedComponentData"/>
    public interface ISystemStateSharedComponentData : ISharedComponentData
    {
    }

    /// <summary>
    /// Disables the entity.
    /// </summary>
    /// <remarks> By default, an <see cref="EntityQuery"/> ignores all entities that have a Disabled component. You
    /// can override this default behavior by setting the <see cref="EntityQueryOptions.IncludeDisabled"/> flag of the
    /// <see cref="EntityQueryDesc"/> object used to create the query. When using the EntityQueryBuilder class
    /// in a ComponentSystem, set this flag by calling the <see cref="EntityQueryBuilder.With(EntityQueryOptions)"/>
    /// function.</remarks>
    public struct Disabled : IComponentData
    {
    }
    
    /// <summary>
    /// Marks the entity as a prefab, which implicitly disables the entity.
    /// </summary>
    /// <remarks> By default, an <see cref="EntityQuery"/> ignores all entities that have a Prefab component. You
    /// can override this default behavior by setting the <see cref="EntityQueryOptions.IncludePrefab"/> flag of the
    /// <see cref="EntityQueryDesc"/> object used to create the query. When using the EntityQueryBuilder class
    /// in a ComponentSystem, set this flag by calling the <see cref="EntityQueryBuilder.With(EntityQueryOptions)"/>
    /// function.</remarks>
    public struct Prefab : IComponentData
    {
    }

    /// <summary>
    /// Marks the entity as an asset, which is used for the Export phase of GameObject conversion.
    /// </summary>
    public struct Asset : IComponentData
    {
    }

    /// <summary>
    /// The LinkedEntityGroup buffer makes the entity be the root of a set of connected entities.
    /// </summary>
    /// <remarks>
    /// Referenced Prefabs automatically add a LinkedEntityGroup with the complete child hierarchy.
    /// EntityManager.Instantiate uses LinkedEntityGroup to instantiate the whole set of entities automatically.
    /// EntityManager.SetEnabled uses LinkedEntityGroup to enable the whole set of entities. 
    /// </remarks>
    public struct LinkedEntityGroup : IBufferElementData
    {
        /// <summary>
        /// A child entity.
        /// </summary>
        public Entity Value;
        
        /// <summary>
        /// Provides implicit conversion of an <see cref="Entity"/> to a LinkedEntityGroup element.
        /// </summary>
        /// <param name="e">The entity to convert</param>
        /// <returns>A new buffer element.</returns>
        public static implicit operator LinkedEntityGroup(Entity e)
        {
            return new LinkedEntityGroup {Value = e};
        }
    }
    
    /// <summary>
    /// A Unity-defined shared component assigned to all entities in the same subscene.
    /// </summary>
    [Serializable]
    public struct SceneTag : ISharedComponentData, IEquatable<SceneTag>
    {
        /// <summary>
        /// The root entity of the subscene.
        /// </summary>
        public Entity  SceneEntity;

        /// <summary>
        /// A unique hash code for comparison.
        /// </summary>
        /// <returns>The scene entity has code.</returns>
        public override int GetHashCode()
        {
            return SceneEntity.GetHashCode();
        }

        /// <summary>
        /// Two SceneTags are equal if they have the same root subscene entity.
        /// </summary>
        /// <param name="other">The other SceneTag.</param>
        /// <returns>True if both SceneTags refer to the same Subscene. False, otherwise.</returns>
        public bool Equals(SceneTag other)
        {
            return SceneEntity == other.SceneEntity;
        }

        /// <summary>
        /// A string for logging.
        /// </summary>
        /// <returns>A string identifying the root subscene entity.</returns>
        public override string ToString()
        {
            return $"SubSceneTag: {SceneEntity}";
        }
    }
}

