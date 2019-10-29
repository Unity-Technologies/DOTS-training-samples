---
uid: ecs-dynamic-buffers
---

# Dynamic Buffers

Use dynamic buffers to associate array-like data with an entity. Dynamic buffers are ECS components that can hold a variable number of elements, automatically resizing as necessary. 

To create a dynamic buffer, first declare a struct that implements [IBufferElementData](xref:Unity.Entities.IBufferElementData) and which defines the elements stored in the buffer. For example, you could use the following struct for a buffer component that stores integers:

[!code-cs[declare-element](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#declare-element)]

To associate a dynamic buffer with an entity, add an IBufferElementData component directly to the entity rather than adding the [dynamic buffer container](xref:Unity.Entities.DynamicBuffer`1) itself. 

ECS manages the container; for most purposes, you can treat a dynamic buffer the same as any other ECS component by using your declared IBufferElementData type. For example, you can use the IBufferElementData type in [entity queries](xref:Unity.Entities.EntityQuery) as well as when adding or removing the buffer component. However, you use different functions to access a buffer component and those functions provide the [DynamicBuffer](xref:Unity.Entities.DynamicBuffer`1) instance, which provides an array-like interface to the buffer data.

You can specify an “internal capacity" for a dynamic buffer component using the [InternalBufferCapacity attribute](xref:Unity.Entities.InternalBufferCapacityAttribute). The internal capacity defines the number of elements the dynamic buffer stores in the [ArchetypeChunk](xref:Unity.Entities.ArchetypeChunk) along with the other components of an entity. If you increase the size of a buffer beyond the internal capacity, the buffer allocates a heap memory block outside the current chunk (and moves all existing elements). ECS manages this external buffer memory automatically, freeing the memory when the buffer component is removed. 

Note that if the data in a buffer is not dynamic, you can use a [blob asset](xref:Unity.Entities.BlobBuilder) instead of a dynamic buffer. Blob assets can store structured data, including arrays and can be shared by multiple entities.
 
## Declaring Buffer Element Types

To declare a buffer, declare a struct defining the type of element that you are
putting into the buffer. The struct must implement [IBufferElementData](xref:Unity.Entities.IBufferElementData):

[!code-cs[declare-element-full](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#declare-element-full)]


## Adding Buffer Types To Entities

To add a buffer to an entity, add the IBufferElementData struct that defines the data type of the buffer element, and add that type directly to an entity or to an [archetype](xref:Unity.Entities.EntityArchetype):

### Using [EntityManager.AddBuffer()](xref:Unity.Enities.EntityManager.AddBuffer`1(Unity.Entities.Entity))

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#add-with-manager)]

### Using an archetype

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#add-with-archetype)]

### Using an [EntityCommandBuffer](xref:Unity.Entities.EntityCommandBuffer)

You can add or set a buffer component when adding commands to an entity command buffer. Use [AddBuffer](xref:Unity.Entities.EntityCommandBuffer.AddBuffer``1(Unity.Entities.Entity)) to create a new buffer for the entity, changing the entity's archetype. Use [SetBuffer](xref:Unity.Entities.EntityCommandBuffer.SetBuffer``1(Unity.Entities.Entity)) to wipe out the existing buffer (which must exist) and create a new, empty buffer in its place. Both functions return a [DynamicBuffer](xref:Unity.Entities.DynamicBuffer`1) instance that you can use to populate the new buffer. You can add elements to the buffer immediately, but they are not otherwise accessible until the buffer is actually added to the entity when the command buffer is executed.

The following job creates a new entity using a command buffer and then adds a dynamic buffer component using [EntityCommandBuffer.AddBuffer](xref:Unity.Entities.EntityCommandBuffer.AddBuffer``1(Unity.Entities.Entity)). The job also adds a number of elements to the dynamic buffer. 

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#add-in-job)]

**Note:** You are not required to add data to the dynamic buffer immediately. However, you won't have access to the buffer again until after the entity command buffer you are using is executed.

## Accessing Buffers

You can access the [DynamicBuffer](xref:Unity.Entities.DynamicBuffer`1) instance using [EntityManager](xref:Unity.Entities.EntityManager), [systems](ecs_systems.md), and jobs, in much the same way as you access other component types of your entities. 

### EntityManager

You can use an instance of the [EntityManager](xref:Unity.Entities.EntityManager) to access a dynamic buffer:

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#access-manager)]

### Component System Entities.ForEach

You can also access dynamic buffers in a [component system](xref:Unity.Entities.ComponentSystem):

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#access-buffer-system)]

### Looking up buffers of another entity

When you need to look up the buffer data belonging to another entity in a job, you can pass a [BufferFromEntity](xref:Unity.Entities.BufferFromEntity`1) 
You can also look up buffers on a per-entity basis from a JobComponentSystem:

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#lookup-snippet)]

### IJobForEach

You can access dynamic buffers associated with the entities you process with an IJobForEach job. 

In the IJobforEach or IJobForEachWithEntity declaration, use the IBufferElementData type contained in the buffer as one of the generic parameters. For example:

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#declare-ijfe-with-buffer)]

However, in the `Execute()` method of the job struct, use the DynamicBuffer&lt;T&gt; as the parameter type. For example:

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#declare-ijfe-execute-method)]
 
 The following example adds up the contents of all dynamic buffers containing elements of type, `MyBufferElement`. Because IJobForEach jobs process entities in parallel, the example first stores the intermediate sum for each buffer in a native array and uses a second job to calculate the final sum.
 
[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#access-ijfe)]

### IJobChunk

To access an individual buffer in an IJobChunk job, pass the buffer data type to the job and use that to get a [BufferAccessor](xref:Unity.Entities.BufferAccessor`1). A buffer accessor is an array-like structure that provides access to all of the dynamic buffers in the current chunk. 

 Like the previous, IJobForEach example, the following example adds up the contents of all dynamic buffers containing elements of type, `MyBufferElement`. IJobChunk jobs can also run in parallel on each chunk so the example first stores the intermediate sum for each buffer in a native array and uses a second job to calculate the final sum. In this case, the intermediate array holds one result for each chunk, rather than one result for each entity.

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#access-chunk-job)]

## Reinterpreting Buffers

Buffers can be reinterpreted as a type of the same size. The intention is to
allow controlled type-punning and to get rid of the wrapper element types when
they get in the way. To reinterpret, call [Reinterpret&lt;T&gt;](xref:Unity.Entities.DynamicBuffer`1.Reinterpret*):

[!code-cs[declare](../package/DocCodeSamples.Tests/DynamicBufferExamples.cs#reinterpret-snippet)]

The reinterpreted buffer instance retains the safety handle of the original
buffer, and is safe to use. Reinterpreted buffers reference to original data, so
modifications to one reinterpreted buffer are immediately reflected in
others.

Note that the reinterpret function only enforces that the types involved have the same length; you could alias a `uint` and `float` buffer without raising an error since both types are 32-bits long. It is your responsibility to make sure that the reinterpretation makes sense logically.


