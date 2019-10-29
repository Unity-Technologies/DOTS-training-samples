---
uid: ecs-chunk-component-data
---

# Chunk component data

Use chunk components to associate data with a specific [chunk](xref:Unity.Entities.ArchetypeChunk).

Chunk components contain data that applies to all entities in a specific chunk. For example, if you had chunks of entities representing 3D objects that are organized by proximity, you could store a collective bounding box for the entities in a chunk using a chunk component. Chunk components use the interface type [IComponentData](xref:Unity.Entities.IComponentData). 

Add and set the values of a chunk component using 
Although chunk components can have values unique to an individual chunk, they are still part of the archetype of the entities in the chunk. Thus if you remove a chunk component from an entity, that entity is moved to a different chunk (possibly a new one). Likewise, if you add a chunk component to an entity, that entity moves to a different chunk since its archetype changes; the addition of the chunk component does not affect the remaining entities in the original chunk. 

If you change the value of a chunk component using an entity in that chunk, it changes the value of the chunk component common to all the entities in that chunk. If you change the archetype of an entity such that it moves into a new chunk that happens to have the same type of chunk component, then the existing value in the destination chunk is unaffected. (If the entity is moved to a newly created chunk, then a new chunk component for that chunk is also created and assigned its default value.)

The main differences between working with chunk components and general-purpose components is that you use different functions to add, set, and remove them. Chunk components also have their own [ComponentType](xref:Unity.Entities.ComponentType.ChunkComponentType) functions for use in defining entity archetypes and queries. 

 
**Relevant APIs**

| **Purpose** | **Function** |
| :---------------  | :---------------- |
| Declaration | [IComponentData](xref:Unity.Entities.IComponentData) |
| &nbsp; | **[ArchetypeChunk methods](xref:Unity.Entities.ArchetypeChunk)** |
| Read | [GetChunkComponentData<T>(ArchetypeChunkComponentType<T>)](xref:Unity.Entities.ArchetypeChunk.GetChunkComponentData*) |
| Check | [HasChunkComponent<T>(ArchetypeChunkComponentType<T>)](xref:Unity.Entities.ArchetypeChunk.HasChunkComponent*) |
| Write | [SetChunkComponentData<T>(ArchetypeChunkComponentType<T>, T)](xref:Unity.Entities.ArchetypeChunk.SetChunkComponentData*) |
| &nbsp; | **[EntityManager methods](xref:Unity.Entities.EntityManager)** |
| Create | [AddChunkComponentData<T>(Entity)](xref:Unity.Entities.EntityManager.AddChunkComponentData``1(Unity.Entities.Entity)) |
| Create | [AddChunkComponentData<T>(EntityQuery, T)](xref:Unity.Entities.EntityManager.AddChunkComponentData``1(Unity.Entities.EntityQuery,``0)) |
| Create | [AddComponents(Entity,ComponentTypes)](xref:Unity.Entities.EntityManager.AddComponents(Unity.Entities.Entity,Unity.Entities.ComponentTypes)) |
| Get type info | [GetArchetypeChunkComponentType<T>(Boolean)](xref:Unity.Entities.EntityManager.GetArchetypeChunkComponentType*) |
| Read | [GetChunkComponentData<T>(ArchetypeChunk)](xref:Unity.Entities.EntityManager.GetChunkComponentData``1(Unity.Entities.ArchetypeChunk)) |
| Read | [GetChunkComponentData<T>(Entity)](xref:Unity.Entities.EntityManager.GetChunkComponentData``1(Unity.Entities.Entity)) |
| Check | [HasChunkComponent<T>(Entity)](xref:Unity.Entities.EntityManager.HasChunkComponent*) |
| Delete | [RemoveChunkComponent<T>(Entity)](xref:Unity.Entities.EntityManager.RemoveChunkComponent``1(Unity.Entities.Entity)) |
| Delete | [RemoveChunkComponentData<T>(EntityQuery)](xref:Unity.Entities.EntityManager.RemoveChunkComponentData*) |
| Write | [EntityManager.SetChunkComponentData<T>(ArchetypeChunk, T)](xref:Unity.Entities.EntityManager.SetChunkComponentData*) |

<a name="declare"></a>
## Declaring a chunk component

Chunk components use the interface type [IComponentData](xref:Unity.Entities.IComponentData).

```
public struct ChunkComponentA : IComponentData
{
    public float Value;
}
```

<a name="create"></a>
## Creating a chunk component 

You can add a chunk component directly, using an entity in the target chunk or using an entity query that selects a group of target chunks. Chunk components cannot be added inside a Job, nor can they be added with an EntityCommandBuffer.
 
 You can also include chunk components as part of  the [EntityArchetype](xref:Unity.Entities.EntityArchetype) or list of [ComponentType](xref:Unity.Entities.ComponentType) objects used to create entities and the chunk components are created for each chunk storing entities with that archetype. Use [ComponentType.ChunkComponent&lt;T&gt;](xref:Unity.Entities.ComponentType``1) or [ComponentType.ChunkComponentReadOnly&lt;T&gt;](xref:Unity.Entities.ComponentTypeReadOnly``1) with these methods. Otherwise, the component is treated as a general-purpose component instead of a chunk component.
 
**With an entity in a chunk**

Given an entity in the target chunk, you can add a chunk component to the chunk using the [EntityManager.AddChunkComponentData&lt;T&gt;()](xref:Unity.Entities.EntityManager.AddChunkComponentData``1) function:

```
EntityManager.AddChunkComponentData<ChunkComponentA>(entity);
```

Using this method, you cannot immediately set a value for the chunk component.

**With an [EntityQuery](xref:Unity.Entities.EntityQuery)**

Given an entity query that selects all the chunks to which you want to add a chunk component, you can add and set the component using the [EntityManager.AddChunkComponentData&lt;T&gt;()](xref:Unity.Entities.EntityManager.AddChunkComponentData``1(Unity.Entities.EntityQuery,``0)) function:

```
EntityQueryDesc ChunksWithoutComponentADesc = new EntityQueryDesc()
{
    None = new ComponentType[] {ComponentType.ChunkComponent<ChunkComponentA>()}
};
ChunksWithoutChunkComponentA = GetEntityQuery(ChunksWithoutComponentADesc);

EntityManager.AddChunkComponentData<ChunkComponentA>(ChunksWithoutChunkComponentA,
        new ChunkComponentA() {Value = 4});
```

Using this method, you can set the same initial value for all the new chunk components.

**With an [EntityArchetype](xref:Unity.Entities.EntityArchetype)**

When creating entities with an archetype or list of component types, include the chunk component types in the archetype:

```
ArchetypeWithChunkComponent = EntityManager.CreateArchetype(
    ComponentType.ChunkComponent(typeof(ChunkComponentA)),
    ComponentType.ReadWrite<GeneralPurposeComponentA>());
var entity = EntityManager.CreateEntity(ArchetypeWithChunkComponent);

```

or list of component types:

```
ComponentType[] compTypes = {ComponentType.ChunkComponent<ChunkComponentA>(),
                             ComponentType.ReadOnly<GeneralPurposeComponentA>()};
var entity = EntityManager.CreateEntity(compTypes);
```

Using these methods, the chunk components for new chunks created as part of entity construction receive the default struct value. Chunk components in existing chunks are not changed. See [Updating a chunk component](#update) for how to set the chunk component value given a reference to an entity.

<a name="read"></a>
## Reading a chunk component 

You can read a chunk component using the [ArchetypeChunk](xref:Unity.Entities.ArchetypeChunk) object representing the chunk or using an entity in the target chunk.

**With the ArchetypeChunk instance**

Given a chunk, you can read its chunk component using the [EntityManager.GetChunkComponentData&lt;T&gt;](xref:Unity.Entities.EntityManager.GetChunkComponentData``1(Unity.Entities.ArchetypeChunk)) function. The following code iterates over all the chunks matching a query and accesses a chunk component of type ChunkComponentA:

```
var chunks = ChunksWithChunkComponentA.CreateArchetypeChunkArray(Allocator.TempJob);
foreach (var chunk in chunks)
{
    var compValue = EntityManager.GetChunkComponentData<ChunkComponentA>(chunk);
    //..
}
chunks.Dispose();
```

**With an entity in a chunk**

Given an entity, you can access a chunk component in the chunk containing that entity with [EntityManager.GetChunkComponentData&lt;T&gt;](xref:Unity.Entities.EntityManager.GetChunkComponentData``1(Unity.Entities.Entity)):

```
if(EntityManager.HasChunkComponent<ChunkComponentA>(entity))
    chunkComponentValue = EntityManager.GetChunkComponentData<ChunkComponentA>(entity);
```

You can also set up a [fluent query](xref:ecs-fluent-query) to select only entities that have a chunk component:

```
Entities.WithAll(ComponentType.ChunkComponent<ChunkComponentA>()).ForEach(
    (Entity entity) =>
{
    var compValue = EntityManager.GetChunkComponentData<ChunkComponentA>(entity);
    //...
});
```

Note that you cannot pass a chunk component to the for-each portion of the query. Instead, you must pass the [Entity](xref:Unity.Entities.Entity) object and use the [EntityManager](xref:Unity.Entities.Manager) to access the chunk component.

<a name="update"></a>
## Updating a chunk component 

You can update a chunk component given a reference to the [chunk](xref:Unity.Entities.ArchetypeChunk) it belongs to. In an IJobChunk Job, you can call [ArchetypeChunk.SetChunkComponentData](xref:Unity.Entities.ArchetypeChunk.SetChunkComponentData*). On the main thread, you can use the EntityManager version, [EntityManager.SetChunkComponentData](xref:Unity.Entities.EntityManager.SetChunkComponentData*). Note that you cannot access chunk components in an IJobForEach Job because you do not have access to the ArchetypeChunk object or the EntityManager.

**With the ArchetypeChunk instance**

To update a chunk component in a Job, see [Reading and writing in a JobComponentSystem](#read-and-write-jcs).

To update a chunk component on the main thread, use the EntityManager:

```
EntityManager.SetChunkComponentData<ChunkComponentA>(chunk, 
                    new ChunkComponentA(){Value = 7});
```

**With an Entity instance**

If you have an entity in the chunk rather than the chunk reference itself, you can also use the EntityManger to get the chunk containing the entity:

```
var entityChunk = EntityManager.GetChunk(entity);
EntityManager.SetChunkComponentData<ChunkComponentA>(entityChunk, 
                    new ChunkComponentA(){Value = 8});

``` 

**Detecting changes in data**

Use component change versions to detect when a chunk component needs to be updated for a given chunk. ECS updates the component versions for a chunk whenever the data in a component is accessed as writable or when an entity is added or removed from the chunk.

For example, if the chunk component contains the center point of the entities in a chunk, calculated from the LocalToWorld component of those entities, you can check the version of the LocalToWorld components to determine whether the chunk component should be updated. If your chunk component is derived from more than one component, you should check the versions of all of the components to see if any of them changed.

See [Skipping chunks with unchanged entities](chunk_iteration_job.md#filtering) for additional information.

<a name="read-and-write-jcs"></a>
## Reading and writing in a JobComponentSystem

In an [IJobChunk](xref:Unity.Entities.IJobChunk) inside a JobComponentSystem, you can access chunk components using the [chunk](xref:Unity.Entities.ArchetypeChunk) parameter passed to the IJobChunk Execute function. As with any component data in an IJobChunk Job, you must pass a [ArchetypeChunkComponentType&lt;T&gt;](xref:Unity.Entities.EntityManager.GetArchetypeChunkComponentType*) object to the Job using a field struct in order to access the component.

The following system defines a query that selects all entities and chunks with a chunk component of type, _ChunkComponentA_. It then runs an IJobChunk Job using the query to iterate over the selected chunks and access the individual chunk components. The Job uses the [ArchetypeChunk](xref:Unity.Entities.ArchetypeChunk) GetChunkComponentData and SetChunkComponentData functions to read and write the chunk component data.

```
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

public class ChunkComponentChecker : JobComponentSystem
{
  private EntityQuery ChunksWithChunkComponentA;
  protected override void OnCreate()
  {
      EntityQueryDesc ChunksWithComponentADesc = new EntityQueryDesc()
      {
        All = new ComponentType[]{ComponentType.ChunkComponent<ChunkComponentA>()}
      };
      ChunksWithChunkComponentA = GetEntityQuery(ChunksWithComponentADesc);
  }

  [BurstCompile]
  struct ChunkComponentCheckerJob : IJobChunk
  {
      public ArchetypeChunkComponentType<ChunkComponentA> ChunkComponentATypeInfo;
      public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
      {
          var compValue = chunk.GetChunkComponentData(ChunkComponentATypeInfo);
          //...
          var squared = compValue.Value * compValue.Value;
          chunk.SetChunkComponentData(ChunkComponentATypeInfo, 
                      new ChunkComponentA(){Value= squared});
      }
  }
  
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    var job = new ChunkComponentCheckerJob()
    {
      ChunkComponentATypeInfo = GetArchetypeChunkComponentType<ChunkComponentA>()
    };
    return job.Schedule(ChunksWithChunkComponentA, inputDependencies);
  }
}
```

Note that if you are only reading a chunk component and not writing, you should use [ComponentType.ChunkComponentReadOnly](xref:Unity.Entities.ComponentType.ChunkComponentReadOnly*) when defining your entity query to avoid creating unnecessary Job scheduling constraints.

<a name="delete"></a>
## Deleting a chunk component 

Use the [EntityManager.RemoveChunkComponent](xref:Unity.Entities.EntityManager.RemoveChunkComponent*) functions to delete a chunk component. You can remove a chunk component given an entity in the target chunk or you can remove all the chunk components of a given type from all chunks selected by an entity query. If you remove a chunk component from an individual entity, that entity moves to a different chunk because the archetype of the entity changes; the chunk itself retains the unchanged chunk component as long as there are other entities remaining in the chunk. 

<a name="in-query"></a>
## Using a chunk component in a query

To use a chunk component in an entity query, you must specify the type using either the [ComponentType.ChunkComponent&lt;T&gt;](xref:Unity.Entities.ComponentType``1) or [ComponentType.ChunkComponentReadOnly&lt;T&gt;](xref:Unity.Entities.ComponentTypeReadOnly``1) functions. Otherwise, the component is treated as a general-purpose component instead of a chunk component.

**In an [EntityQueryDesc](Unity.Entities.EntityQueryDesc)**

The following query description can be used to create an entity query that selects all chunks -- and entities in those chunks -- that have a chunk component of type, _ChunkComponentA_:

```
EntityQueryDesc ChunksWithChunkComponentADesc = new EntityQueryDesc()
{
    All = new ComponentType[]{ComponentType.ChunkComponent<ChunkComponentA>()}
};
```

**In an [EntityQueryBuilder](Unity.Entities.EntityQueryBulder) lambda function**

The following fluent query iterates over all entities in chunks that have a chunk component of type, _ChunkComponentA_:

```
Entities.WithAll(ComponentType.ChunkComponentReadOnly<ChunkCompA>())
        .ForEach((Entity ent) =>
{
    var chunkComponentA = EntityManager.GetChunkComponentData<ChunkCompA>(ent);
});
```

**Note:** you cannot pass a chunk component to the lambda function itself. To read or write the value of a chunk component in a fluent query, use the [ComponentSystem.EntityManager](xref:Unity.Entities.ComponentSystemBase.EntityManager) property. Modifying a chunk component effectively modifies the value for all entities in the same chunk; it does not move the current entity to a different chunk.